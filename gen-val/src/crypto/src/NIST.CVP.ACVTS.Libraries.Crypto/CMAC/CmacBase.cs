﻿using System;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.MAC;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.MAC.CMAC;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.Symmetric;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.Symmetric.BlockModes;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.Symmetric.Engines;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.Symmetric.Enums;
using NIST.CVP.ACVTS.Libraries.Math;
using NIST.CVP.ACVTS.Libraries.Math.Helpers;
using NLog;

namespace NIST.CVP.ACVTS.Libraries.Crypto.CMAC
{
    public abstract class CmacBase : ICmac
    {
        protected abstract BitString RbConstant { get; }
        protected abstract IModeBlockCipher<SymmetricCipherResult> AlgoMode { get; }
        protected abstract IBlockCipherEngine Engine { get; }

        public int OutputLength => Engine.BlockSizeBits;

        public MacResult Generate(BitString keyBits, BitString message, int macLength = 0)
        {
            // https://nvlpubs.nist.gov/nistpubs/specialpublications/nist.sp.800-38b.pdf

            //6.2 MAC Generation

            //Prerequisites:
            //    block cipher CIPH with block size b;
            //    key K;
            //    MAC length parameter Tlen

            //Input:
            //    message M of bit length Mlen.

            //Output:
            //    MAC T of bit length Tlen.

            //Suggested Notation:
            //    CMAC(K, M, Tlen) or, if Tlen is understood from the context, CMAC(K, M).

            //Steps:
            //    1. Apply the subkey generation process in Sec. 6.1 to K to produce K1 and K2.
            var subKeys = ComputeSubKey(keyBits);
            var K1 = subKeys.k1;
            var K2 = subKeys.k2;

            //    2. If Mlen = 0, let n = 1; else, let n = ceiling(Mlen / b).
            //var n = message.BitLength == 0 ? 1 : System.Math.Ceiling(
            //    message.BitLength / (double)Engine.BlockSizeBits
            //);
            var n = message.BitLength == 0 ? 1 : message.BitLength.CeilingDivide(Engine.BlockSizeBits);

            //    3. Let M1, M2, ... , Mn - 1, Mn* denote the unique sequence of bit strings 
            //       such that M = M1 || M2 || ... || Mn - 1 || Mn*,
            //       where M1, M2,..., Mn-1 are complete blocks.2

            //    4. If Mn* is a complete block, let Mn = K1 XOR Mn*; 


            //var numOfBlocks = (int)System.Math.Ceiling(message.BitLength / (double)_engine.BlockSizeBits);
            var numOfBlocks = message.BitLength.CeilingDivide(Engine.BlockSizeBits);

            var s1 = message.BitLength > Engine.BlockSizeBits ?
                message.MSBSubstring(0, (numOfBlocks - 1) * Engine.BlockSizeBits) :
                new BitString(0);

            var lastBlock = message.BitLength != 0
                ? message.MSBSubstring(s1.BitLength, message.BitLength - s1.BitLength) :
                new BitString(0);

            if (message.BitLength % Engine.BlockSizeBits == 0 && message.BitLength != 0)
            {
                lastBlock = lastBlock.XOR(K1);
            }
            //       else, let Mn = K2 XOR (Mn* || 10^j), where j = nb - Mlen - 1.
            else
            {
                var padding = new BitString(Engine.BlockSizeBits - lastBlock.BitLength);
                padding.Set(padding.BitLength - 1, true);
                lastBlock = K2.XOR(lastBlock.ConcatenateBits(padding));
            }
            message = s1.ConcatenateBits(lastBlock).GetDeepCopy();
            //if this was an empty message, it would have been padded with another block
            if (message.BitLength % Engine.BlockSizeBits != 0)
            {
                throw new Exception("Message isn't composed of same sized blocks.");
            }
            numOfBlocks = message.BitLength / Engine.BlockSizeBits;
            BitString prevC = new BitString(Engine.BlockSizeBits);
            BitString currC = new BitString(Engine.BlockSizeBits);
            for (var i = 0; i < numOfBlocks; i++)
            {
                var block = message.MSBSubstring(i * Engine.BlockSizeBits, Engine.BlockSizeBits);
                var param2 = new ModeBlockCipherParameters(
                    BlockCipherDirections.Encrypt, keyBits, prevC.XOR(block)
                );
                currC = AlgoMode.ProcessPayload(param2).Result;
                prevC = currC.GetDeepCopy();
            }

            //    5. Let C0 = 0^b.
            //    6. For i = 1 to n, let Ci = CIPHK(Ci - 1 XOR Mi).
            //    7. Let T = MSBTlen(Cn).
            //    8. Return T.

            BitString mac;
            if (macLength != 0)
            {
                mac = currC.GetMostSignificantBits(macLength);
            }
            else
            {
                mac = currC.GetDeepCopy();
            }

            return new MacResult(mac);
        }

        public MacResult Verify(BitString keyBits, BitString message, BitString macToVerify)
        {
            try
            {
                var mac = Generate(keyBits, message, macToVerify.BitLength);

                if (!mac.Success)
                {
                    return new MacResult(mac.ErrorMessage);
                }

                if (mac.Mac.Equals(macToVerify))
                {
                    return new MacResult(mac.Mac);
                }

                return new MacResult("CMAC did not match.");
            }
            catch (Exception ex)
            {
                ThisLogger.Debug($"keyLen:{keyBits.BitLength}; dataLen:{message.BitLength}");
                ThisLogger.Error(ex);
                return new MacResult(ex.Message);
            }
        }

        private (BitString k1, BitString k2) ComputeSubKey(BitString key)
        {
            //6.1 SUBKEY GENERATION

            //Prerequisites:
            //    block cipher CIPH with block size b;
            //    key K.

            //Output:
            //subkeys K1, K2.

            //Suggested Notation:
            //    SUBK(K).

            //Steps:
            //    1.Let L = CIPHK(0^b).
            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Encrypt, key, new BitString(Engine.BlockSizeBits)
            );
            var L = AlgoMode.ProcessPayload(param).Result;

            BitString K1, K2;
            //    2.If MSB1(L) = 0, then K1 = L << 1;
            K1 = L.LSBShift();
            //Else K1 = (L << 1) XOR Rb; see Sec. 5.3 for the definition of Rb.
            if (L.GetMostSignificantBits(1).Bits[0])
            {
                K1 = K1.XOR(RbConstant);
            }

            //    3.If MSB1(K1) = 0, then K2 = K1 << 1;
            K2 = K1.LSBShift();
            //    Else K2 = (K1 << 1) XOR Rb.
            if (K1.GetMostSignificantBits(1).Bits[0])
            {
                K2 = K2.XOR(RbConstant);
            }

            //    4.Return K1, K2.
            return (K1, K2);
        }

        private Logger ThisLogger => LogManager.GetCurrentClassLogger();
    }
}
