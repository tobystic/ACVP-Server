﻿using NIST.CVP.ACVTS.Libraries.Math;

namespace NIST.CVP.ACVTS.Libraries.Crypto.Common.KDF.Components.AnsiX963
{
    public class KdfResult
    {
        public BitString DerivedKey { get; }
        public string ErrorMessage { get; }

        public bool Success => string.IsNullOrEmpty(ErrorMessage);

        public KdfResult(BitString key)
        {
            DerivedKey = key;
        }

        public KdfResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
