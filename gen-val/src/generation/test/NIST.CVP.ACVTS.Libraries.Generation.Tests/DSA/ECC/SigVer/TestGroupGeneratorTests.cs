﻿using System.Linq;
using System.Threading.Tasks;
using NIST.CVP.ACVTS.Libraries.Generation.ECDSA.v1_0.SigVer;
using NIST.CVP.ACVTS.Tests.Core.TestCategoryAttributes;
using NUnit.Framework;

namespace NIST.CVP.ACVTS.Libraries.Generation.Tests.DSA.ECC.SigVer
{
    [TestFixture, UnitTest]
    public class TestGroupGeneratorTests
    {
        private static object[] parameters =
        {
            new object[]
            {
                1,
                new ParameterBuilder()
                    .WithCapabilities(new []
                        {
                            ParameterBuilder.GetCapabilityWith(new[] { "p-224" }, new[] { "sha2-224" })
                        })
                    .Build()
            },
            new object[]
            {
                4,
                new ParameterBuilder()
                    .WithCapabilities(new []
                        {
                            ParameterBuilder.GetCapabilityWith(new[] { "b-233" }, new[] { "sha2-512", "sha2-512/224" }),
                            ParameterBuilder.GetCapabilityWith(new[] { "k-571" }, new[] { "sha2-384", "sha2-512/256" })
                        })
                    .Build()
            },
            new object[]
            {
                15 * 11,
                new ParameterBuilder()
                    .WithCapabilities(new []
                        {
                            ParameterBuilder.GetCapabilityWith(ParameterValidator.VALID_CURVES, ParameterValidator.VALID_HASH_ALGS),
                            ParameterBuilder.GetCapabilityWith(ParameterValidator.VALID_CURVES, ParameterValidator.VALID_HASH_ALGS),
                            ParameterBuilder.GetCapabilityWith(ParameterValidator.VALID_CURVES, ParameterValidator.VALID_HASH_ALGS),
                        })
                    .Build()
            },
            new object[]
            {
                2 + 2 + 2 + 1,
                new ParameterBuilder()
                    .WithCapabilities(new []
                        {
                            ParameterBuilder.GetCapabilityWith(new[] { "b-233" }, new[] { "sha2-512", "sha2-512/224" }),
                            ParameterBuilder.GetCapabilityWith(new[] { "k-571" }, new[] { "sha2-384", "sha2-512/256" }),
                            ParameterBuilder.GetCapabilityWith(new[] { "p-224", "b-233" }, new[] { "sha2-512", "sha2-224" })
                        })
                    .Build()
            }
        };

        [Test]
        [TestCaseSource(nameof(parameters))]
        public async Task ShouldCreate1TestGroupForEachCombinationOfCurveHashAlg(int expectedGroups, Parameters parameters)
        {
            var subject = new TestGroupGenerator(false);
            var result = await subject.BuildTestGroupsAsync(parameters);
            Assert.That(result.Count(), Is.EqualTo(expectedGroups));
        }
    }
}
