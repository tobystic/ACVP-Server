﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NIST.CVP.ACVTS.Libraries.Generation.SHA2.v1_0;
using NIST.CVP.ACVTS.Tests.Core.TestCategoryAttributes;
using NUnit.Framework;

namespace NIST.CVP.ACVTS.Libraries.Generation.Tests.SHA2
{
    [TestFixture, UnitTest]
    public class TestGroupGeneratorMonteCarloTestTests
    {
        private static object[] parameters =
        {
            new object[]
            {
                0, // 1 * 0
                new ParameterValidatorTests.ParameterBuilder()
                    .WithDigestSizes(new List<string>() { }) // 0
                    .WithAlgorithm("SHA2")  // 1
                    .Build()
            },
            new object[]
            {
                2, // 1 * 2
                new ParameterValidatorTests.ParameterBuilder()
                    .WithDigestSizes(new List<string>() { "224", "256" }) // 2
                    .WithAlgorithm("SHA2")  // 1
                    .Build()
            },
            new object[]
            {
                3, // 1 * 3
                new ParameterValidatorTests.ParameterBuilder()
                    .WithDigestSizes(new List<string>() { "512", "512/224", "512/256" }) // 3
                    .WithAlgorithm("SHA2")  // 1
                    .Build()
            },
            new object[]
            {
                6, // 1 * 6
                new ParameterValidatorTests.ParameterBuilder()
                    .WithDigestSizes(new List<string>() { "224", "256", "384", "512", "512/224", "512/256" }) // 6
                    .WithAlgorithm("SHA2")  // 1
                    .Build()
            }
        };
        [Test]
        [TestCaseSource(nameof(parameters))]
        public async Task ShouldCreate1TestGroupForEachCombinationOfModeAndDigestSize(int expectedGroupsCreated, Parameters parameters)
        {
            var subject = new TestGroupGeneratorMonteCarloTest();
            var results = await subject.BuildTestGroupsAsync(parameters);
            Assert.That(results.Count(), Is.EqualTo(expectedGroupsCreated));
        }
    }
}
