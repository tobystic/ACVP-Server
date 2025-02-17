﻿using NIST.CVP.ACVTS.Libraries.Crypto.Common.Symmetric.BlockModes;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.Symmetric.Enums;
using NIST.CVP.ACVTS.Libraries.Crypto.Symmetric.BlockModes;
using NIST.CVP.ACVTS.Libraries.Crypto.Symmetric.BlockModes.ShiftRegister;
using NIST.CVP.ACVTS.Libraries.Crypto.Symmetric.Engines;
using NIST.CVP.ACVTS.Libraries.Math;
using NIST.CVP.ACVTS.Libraries.Math.Helpers;
using NIST.CVP.ACVTS.Tests.Core.TestCategoryAttributes;
using NUnit.Framework;

namespace NIST.CVP.ACVTS.Libraries.Crypto.AES_CFB1.Tests
{
    [TestFixture, FastCryptoTest]
    public class MMTs
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var engine = new AesEngine();
            _subject = new CfbBlockCipher(engine, new ShiftRegisterStrategyBit(engine));
        }

        private CfbBlockCipher _subject;

        [Test]
        public void ShouldMMTEncrypt1Byte128BitKey()
        {
            BitString key = new BitString("2695b44439192d099f0a31b89f24dc0f");
            BitString iv = new BitString("5086c0ed84593ed006919af33d4c8902");
            BitString plainText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("11"));
            BitString expectedCipherText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("11"));

            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Encrypt,
                iv,
                key,
                plainText
            );
            var result = _subject.ProcessPayload(param);

            Assert.That(result.Result, Is.EqualTo(expectedCipherText));
        }

        [Test]
        public void ShouldMMTEncryptMultipleBytes128BitKey()
        {
            BitString key = new BitString("7df9fff1c14de3e251aecd3a4328a907");
            BitString iv = new BitString("3903d581031b6b60c0162185bc223fa9");
            BitString plainText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("0100100001"));
            BitString expectedCipherText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("0100011111"));

            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Encrypt,
                iv,
                key,
                plainText
            );
            var result = _subject.ProcessPayload(param);

            Assert.That(result.Result, Is.EqualTo(expectedCipherText));
        }

        [Test]
        public void ShouldMMTEncrypt1Byte192BitKey()
        {
            BitString key = new BitString("d077cf91d6961cea53cea9e4bad61e1e854b74381f03fdd0");
            BitString iv = new BitString("9a3c967ae97b977e04f2168d7024614f");
            BitString plainText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("01"));
            BitString expectedCipherText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("10"));

            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Encrypt,
                iv,
                key,
                plainText
            );
            var result = _subject.ProcessPayload(param);

            Assert.That(result.Result, Is.EqualTo(expectedCipherText));
        }

        [Test]
        public void ShouldMMTEncryptMultipleBytes192BitKey()
        {
            BitString key = new BitString("5ca48a8c95bc37e4b526d5b86da5a7922db2d44b827c6029");
            BitString iv = new BitString("460564e43d949ebe559fc3c230919d5e");
            BitString plainText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("1101100100"));
            BitString expectedCipherText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("0011101110"));

            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Encrypt,
                iv,
                key,
                plainText
            );
            var result = _subject.ProcessPayload(param);

            Assert.That(result.Result, Is.EqualTo(expectedCipherText));
        }

        [Test]
        public void ShouldMMTEncrypt1Byte256BitKey()
        {
            BitString key = new BitString("654ba9cf4cfab4a913e85d8ff8aeed7ba2a56730584b61c071e813500b71ae1f");
            BitString iv = new BitString("fc0ed6b5e59f7fe7094697b78d20174e");
            BitString plainText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("11"));
            BitString expectedCipherText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("00"));

            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Encrypt,
                iv,
                key,
                plainText
            );
            var result = _subject.ProcessPayload(param);

            Assert.That(result.Result, Is.EqualTo(expectedCipherText));
        }

        [Test]
        public void ShouldMMTEncryptMultipleBytes256BitKey()
        {
            BitString key = new BitString("ec1272869c97608876ac778400d00a11cc59e45b9f0c8d60d1ebc960f74e628e");
            BitString iv = new BitString("61b17b1f9aa2145054d596b4a88e8bbc");
            BitString plainText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("1110110000"));
            BitString expectedCipherText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("1000011110"));

            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Encrypt,
                iv,
                key,
                plainText
            );
            var result = _subject.ProcessPayload(param);

            Assert.That(result.Result, Is.EqualTo(expectedCipherText));
        }

        [Test]
        public void ShouldMMTDecrypt1Byte128BitKey()
        {
            BitString key = new BitString("83185129c67311b6fc765639b7bb63c0");
            BitString iv = new BitString("84324d79ab6adc655d295ee9f8263725");
            BitString cipherText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("10"));
            BitString expectedPlainText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("01"));

            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Decrypt,
                iv,
                key,
                cipherText
            );
            var result = _subject.ProcessPayload(param);

            Assert.That(result.Result, Is.EqualTo(expectedPlainText));
        }

        [Test]
        public void ShouldMMTDecryptMultipleBytes128BitKey()
        {
            BitString key = new BitString("2a5e39f2de044223a0fcde0327331602");
            BitString iv = new BitString("5e343033f92a3c82efdf9a35ac5e3657");
            BitString cipherText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("1001 1100 00"));
            BitString expectedPlainText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("0000 0110 11"));

            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Decrypt,
                iv,
                key,
                cipherText
            );
            var result = _subject.ProcessPayload(param);

            Assert.That(result.Result, Is.EqualTo(expectedPlainText));
        }

        [Test]
        public void ShouldMMTDecrypt1Byte192BitKey()
        {
            BitString key = new BitString("6ec4710e35f8794eb50109d8a4c63fe8a672691c28c71171");
            BitString iv = new BitString("6e63d7b3e89e6d5800827a492c40ba5b");
            BitString cipherText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("10"));
            BitString expectedPlainText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("10"));

            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Decrypt,
                iv,
                key,
                cipherText
            );
            var result = _subject.ProcessPayload(param);

            Assert.That(result.Result, Is.EqualTo(expectedPlainText));
        }

        [Test]
        public void ShouldMMTDecryptMultipleBytes192BitKey()
        {
            BitString key = new BitString("677bb41a620fae519723937ebfe1f7b430970056505d76db");
            BitString iv = new BitString("be91c031133b08c35ca153ad7cecb110");
            BitString cipherText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("1000110010"));
            BitString expectedPlainText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("1100100011"));

            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Decrypt,
                iv,
                key,
                cipherText
            );
            var result = _subject.ProcessPayload(param);

            Assert.That(result.Result, Is.EqualTo(expectedPlainText));
        }

        [Test]
        public void ShouldMMTDecrypt1Byte256BitKey()
        {
            BitString key = new BitString("9992afb14c86eaf1f2a2aea3276193c8796abad31d6f18b0e1551104629549b7");
            BitString iv = new BitString("8a8f6521669c93d3d5fea9237b929f70");
            BitString cipherText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("00"));
            BitString expectedPlainText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("00"));

            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Decrypt,
                iv,
                key,
                cipherText
            );
            var result = _subject.ProcessPayload(param);

            Assert.That(result.Result, Is.EqualTo(expectedPlainText));
        }

        [Test]
        public void ShouldMMTDecryptMultipleBytes256BitKey()
        {
            BitString key = new BitString("274e1639d0b7ecc24c3ea8d968092be8b2fe0f313c7b8d1a9c479dc737c95eee");
            BitString iv = new BitString("bd8831473e62ce2f92873d8ac4677b77");
            BitString cipherText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("1111101010"));
            BitString expectedPlainText = new BitString(MsbLsbConversionHelpers.GetBitArrayFromStringOf1sAnd0sReversed("0101100011"));

            var param = new ModeBlockCipherParameters(
                BlockCipherDirections.Decrypt,
                iv,
                key,
                cipherText
            );
            var result = _subject.ProcessPayload(param);

            Assert.That(result.Result, Is.EqualTo(expectedPlainText));
        }
    }
}
