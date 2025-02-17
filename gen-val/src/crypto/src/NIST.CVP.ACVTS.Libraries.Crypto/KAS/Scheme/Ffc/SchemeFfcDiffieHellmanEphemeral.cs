﻿using System;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.Asymmetric.DSA.FFC;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.Enums;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.FixedInfo;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.KC;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.KDA;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.KDA.KdfOneStep;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.NoKC;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.Scheme;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KES;
using NIST.CVP.ACVTS.Libraries.Math;
using NIST.CVP.ACVTS.Libraries.Math.Entropy;

namespace NIST.CVP.ACVTS.Libraries.Crypto.KAS.Scheme.Ffc
{
    public class SchemeFfcDiffieHellmanEphemeral : SchemeBaseFfc
    {
        private readonly IDiffieHellman<FfcDomainParameters, FfcKeyPair> _diffieHellman;

        public SchemeFfcDiffieHellmanEphemeral(
            IDsaFfc dsa,
            IKdfOneStepFactory kdfFactory,
            IKeyConfirmationFactory keyConfirmationFactory,
            INoKeyConfirmationFactory noKeyConfirmationFactory,
            IOtherInfoFactory otherInfoFactory,
            IEntropyProvider entropyProvider,
            SchemeParametersBase<KasDsaAlgoAttributesFfc> schemeParameters,
            KdfParameters kdfParameters,
            MacParameters macParameters,
            IDiffieHellman<FfcDomainParameters, FfcKeyPair> dh
        )
            : base(dsa, kdfFactory, keyConfirmationFactory, noKeyConfirmationFactory, otherInfoFactory, entropyProvider, schemeParameters, kdfParameters, macParameters)
        {
            _diffieHellman = dh;

            if (SchemeParameters.KasAlgoAttributes.Scheme != FfcScheme.DhEphem)
            {
                throw new ArgumentException(nameof(SchemeParameters.KasAlgoAttributes.Scheme));
            }

            if (SchemeParameters.KasMode == KasMode.KdfKc)
            {
                throw new ArgumentException($"{SchemeParameters.KasMode} not possible with {SchemeParameters.KasAlgoAttributes.Scheme}");
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Generate the domain parameters if null, 
        /// set generate a ephemeral key pair, 
        /// generate a no key confirmation nonce when the u/initiator party
        /// </summary>
        protected override void GenerateKasKeyNonceInformation()
        {
            if (DomainParameters == null)
            {
                GenerateDomainParameters();
            }

            EphemeralKeyPair = Dsa.GenerateKeyPair(DomainParameters).KeyPair;

            // when party U and KdfNoKc, a NoKeyConfirmationNonce is needed.
            if (SchemeParameters.KeyAgreementRole == KeyAgreementRole.InitiatorPartyU
                && SchemeParameters.KasMode == KasMode.KdfNoKc)
            {
                NoKeyConfirmationNonce = EntropyProvider.GetEntropy(128);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Generates the shared secret.  
        /// Shared secret Z is made up of this party's private key along with the other parties public key, 
        /// run through the DiffieHellman primitive.
        /// </summary>
        /// <param name="otherPartyInformation"></param>
        /// <returns></returns>
        protected override BitString ComputeSharedSecret(OtherPartySharedInformation<FfcDomainParameters, FfcKeyPair> otherPartyInformation)
        {
            return _diffieHellman.GenerateSharedSecretZ(
                DomainParameters,
                EphemeralKeyPair,
                otherPartyInformation.EphemeralPublicKey
            ).SharedSecretZ;
        }
    }
}
