﻿using NIST.CVP.ACVTS.Libraries.Common;
using NIST.CVP.ACVTS.Libraries.Generation.DSA.v1_0.SigGen;
using NIST.CVP.ACVTS.Libraries.Generation.Tests;
using NIST.CVP.ACVTS.Libraries.Math;
using NIST.CVP.ACVTS.Tests.Core.TestCategoryAttributes;
using NUnit.Framework;

namespace NIST.CVP.ACVTS.Libraries.Generation.DSA.FFC.SigGen.IntegrationTests
{
    [TestFixture, LongRunningIntegrationTest]
    public class GenValTests : GenValTestsSingleRunnerBase
    {
        public override string Algorithm { get; } = "DSA";
        public override string Mode { get; } = "sigGen";

        public override AlgoMode AlgoMode => AlgoMode.DSA_SigGen_v1_0;


        public override IRegisterInjections RegistrationsGenVal => new RegisterInjections();

        protected override void ModifyTestCaseToFail(dynamic testCase)
        {
            var rand = new Random800_90();

            // If TC has a result, change it
            if (testCase.r != null)
            {
                testCase.r = rand.GetDifferentBitStringOfSameSize(new BitString((string)testCase.r)).ToHex();
            }

            if (testCase.s != null)
            {
                testCase.s = rand.GetDifferentBitStringOfSameSize(new BitString((string)testCase.s)).ToHex();
            }
        }

        protected override string GetTestFileFewTestCases(string targetFolder)
        {
            var caps = new Capability[2];

            caps[0] = new Capability
            {
                L = 2048,
                N = 224,
                HashAlg = new[] { "SHA2-256" }
            };

            caps[1] = new Capability
            {
                L = 2048,
                N = 256,
                HashAlg = new[] { "SHA2-224", "SHA2-512" }
            };

            var p = new Parameters
            {
                Algorithm = Algorithm,
                Mode = Mode,
                Revision = Revision,
                IsSample = true,
                Capabilities = caps,
                Conformances = new[] { "SP800-106" }
            };

            return CreateRegistration(targetFolder, p);
        }

        protected override string GetTestFileLotsOfTestCases(string targetFolder)
        {
            var caps = new Capability[3];

            caps[0] = new Capability
            {
                L = 2048,
                N = 224,
                HashAlg = ParameterValidator.VALID_HASH_ALGS
            };

            caps[1] = new Capability
            {
                L = 2048,
                N = 256,
                HashAlg = ParameterValidator.VALID_HASH_ALGS
            };

            caps[2] = new Capability
            {
                L = 3072,
                N = 256,
                HashAlg = ParameterValidator.VALID_HASH_ALGS
            };

            var p = new Parameters
            {
                Algorithm = Algorithm,
                Mode = Mode,
                Revision = Revision,
                IsSample = true,
                Capabilities = caps,
                Conformances = new[] { "SP800-106" }
            };

            return CreateRegistration(targetFolder, p);
        }
    }
}
