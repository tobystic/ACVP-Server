﻿using NIST.CVP.ACVTS.Libraries.Generation.Core;
using NIST.CVP.ACVTS.Libraries.Math.Domain;

namespace NIST.CVP.ACVTS.Libraries.Generation.AES_GCM_SIV.v1_0
{
    public class Parameters : IParameters
    {
        public int VectorSetId { get; set; }
        public string Algorithm { get; set; }
        public string Mode { get; set; }
        public bool IsSample { get; set; }
        public string Revision { get; set; }

        public string[] Conformances { get; set; } = { };
        public string[] Direction { get; set; }
        public int[] KeyLen { get; set; }
        public MathDomain PayloadLen { get; set; }
        public MathDomain AadLen { get; set; }
    }
}
