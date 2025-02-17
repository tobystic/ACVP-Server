﻿using NIST.CVP.ACVTS.Libraries.Crypto.Common.Hash.ShaWrapper.Enums;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.Enums;
using NIST.CVP.ACVTS.Libraries.Generation.Core;
using NIST.CVP.ACVTS.Libraries.Math.Domain;

namespace NIST.CVP.ACVTS.Libraries.Generation.KDA.Sp800_56Cr1.Hkdf
{
    public class Parameters : IParameters
    {
        public int VectorSetId { get; set; }
        public string Algorithm { get; set; }
        public string Mode { get; set; }
        public string Revision { get; set; }
        public bool IsSample { get; set; }
        public string[] Conformances { get; set; }

        /// <summary>
        /// The pattern used for FixedInputConstruction.
        /// </summary>
        public string FixedInfoPattern { get; set; }
        /// <summary>
        /// The encoding type of the fixedInput.
        /// </summary>
        public FixedInfoEncoding[] Encoding { get; set; }
        /// <summary>
        /// Hash/Hmac functions to test with.
        /// </summary>
        public HashFunctions[] HmacAlg { get; set; }
        /// <summary>
        /// The salting methods used by the implementation. 
        /// </summary>
        public MacSaltMethod[] MacSaltMethods { get; set; }
        /// <summary>
        /// The length of the keying material to derive.
        /// </summary>
        public int L { get; set; }
        /// <summary>
        /// The supported lengths of the shared secret.
        /// </summary>
        public MathDomain Z { get; set; }
    }
}
