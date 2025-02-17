﻿using NIST.CVP.ACVTS.Libraries.Generation.KAS.v1_0;

namespace NIST.CVP.ACVTS.Libraries.Generation.Tests.KAS.Builders
{
    public class KdfOptionsBuilder
    {
        public const string DefaultOiPattern = "uPartyInfo||vPartyInfo";

        private string _concatenation;
        private string _asn1;

        public KdfOptionsBuilder()
        {
            _concatenation = DefaultOiPattern;
            _asn1 = DefaultOiPattern;
        }

        public KdfOptionsBuilder WithConcatenation(string value)
        {
            _concatenation = value;
            return this;
        }

        public KdfOptionsBuilder WithAsn1(string value)
        {
            _asn1 = value;
            return this;
        }

        public KdfOptions BuildKdfOptions()
        {
            return new KdfOptions()
            {
                Asn1 = _asn1,
                Concatenation = _concatenation
            };
        }
    }
}
