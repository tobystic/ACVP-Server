﻿using System.Linq;
using NIST.CVP.ACVTS.Libraries.Generation.AES_GCM.v1_0;
using NIST.CVP.ACVTS.Libraries.Math.Domain;
using NIST.CVP.ACVTS.Tests.Core.TestCategoryAttributes;
using NUnit.Framework;

namespace NIST.CVP.ACVTS.Libraries.Generation.Tests.AES.GCM
{
    [TestFixture, UnitTest]
    public class ParameterValidatorTests
    {
        [Test]
        public void ShouldReturnNoErrorsWithValidParameters()
        {
            ParameterValidator subject = new ParameterValidator();
            var result = subject.Validate(new ParameterBuilder().Build());

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void ShouldReturnErrorWithNullTagSupplied()
        {
            ParameterValidator subject = new ParameterValidator();
            var result = subject.Validate(
                new ParameterBuilder()
                    .WithTagLen(null)
                    .Build()
            );

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void ShouldReturnErrorWithNullPtLengthSupplied()
        {
            ParameterValidator subject = new ParameterValidator();
            var result = subject.Validate(
                new ParameterBuilder()
                    .WithPtLen(null)
                    .Build()
            );

            Assert.That(result.Success, Is.False);
        }

        [Test]
        [TestCase("1", null, 0)]
        [TestCase("2", new int[] { }, 0)]
        [TestCase("3", new int[] { -1 }, 0)]
        [TestCase("4", new int[] { 128, -1 }, 0)]
        [TestCase("5", new int[] { 128, -1, -2 }, 1)]
        [TestCase("6", new int[] { 128, -1, -2, -3 }, 2)]
        public void ShouldReturnErrorWithInvalidKeyLength(string label, int[] keyLengths, int errorsExpected)
        {
            ParameterValidator subject = new ParameterValidator();
            var result = subject.Validate(
                new ParameterBuilder()
                    .WithKeyLen(keyLengths)
                    .Build()
            );

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage.Count(c => c == ','), Is.EqualTo(errorsExpected));
        }

        static object[] directionTestCases = new object[]
        {
            new object[] { "null", null },
            new object[] { "empty", new string[] { } },
            new object[] { "Invalid value", new string[] { "notValid" } },
            new object[] { "Partially valid", new string[] { "encrypt", "notValid" } },
            new object[] { "Partially valid w/ null", new string[] { "encrypt", null } }
        };
        [Test]
        [TestCaseSource(nameof(directionTestCases))]
        public void ShouldReturnErrorWithInvalidDirection(string testCaseLabel, string[] direction)
        {
            ParameterValidator subject = new ParameterValidator();
            var result = subject.Validate(
                new ParameterBuilder()
                    .WithMode(direction)
                    .Build()
            );

            Assert.That(result.Success, Is.False, testCaseLabel);
        }

        [Test]
        [TestCase("1", new int[] { -1 }, 0)]
        [TestCase("2", new int[] { 128, -1 }, 0)]
        [TestCase("3", new int[] { 128, -1, -2 }, 1)]
        [TestCase("4", new int[] { 128, -1, -2, -3 }, 2)]
        public void ShouldReturnErrorWithInvalidTagLength(string label, int[] tagLengths, int errorsExpected)
        {
            ParameterValidator subject = new ParameterValidator();
            var result = subject.Validate(
                new ParameterBuilder()
                    .WithTagLen(tagLengths)
                    .Build()
            );

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage.Count(c => c == ','), Is.EqualTo(errorsExpected));
        }

        [Test]
        // invalid range
        [TestCase("1", new int[] { -128 })]
        [TestCase("2", new int[] { 128, -128 })]
        [TestCase("3", new int[] { 128, -128, -256 })]
        [TestCase("4", new int[] { 128, -128, -256, -384 })]
        // invalid multiple
        [TestCase("5", new int[] { 128, 1, 2 })]
        [TestCase("6", new int[] { 128, 1, 2, 3 })]
        public void ShouldReturnErrorWithInvalidPtLength(string label, int[] ptLengths)
        {
            MathDomain md = new MathDomain();
            foreach (var value in ptLengths)
            {
                md.AddSegment(new ValueDomainSegment(value));
            }

            ParameterValidator subject = new ParameterValidator();
            var result = subject.Validate(
                new ParameterBuilder()
                    .WithPtLen(md)
                    .Build()
            );

            Assert.That(result.Success, Is.False);
        }

        [Test]
        // invalid range
        [TestCase("1", new int[] { -128 })]
        [TestCase("2", new int[] { 128, -128 })]
        [TestCase("3", new int[] { 128, -128, -256 })]
        [TestCase("4", new int[] { 128, -128, -256, -384 })]
        // invalid,  multiple
        [TestCase("5", new int[] { 128, 1, 2 })]
        [TestCase("6", new int[] { 128, 1, 2, 3 })]
        public void ShouldReturnErrorWithInvalidAadLength(string label, int[] aadLengths)
        {
            MathDomain md = new MathDomain();
            foreach (var value in aadLengths)
            {
                md.AddSegment(new ValueDomainSegment(value));
            }

            ParameterValidator subject = new ParameterValidator();
            var result = subject.Validate(
                new ParameterBuilder()
                    .WithAadLen(md)
                    .Build()
            );

            Assert.That(result.Success, Is.False);
        }

        [Test]
        // invalid range
        [TestCase("1", new int[] { -128 })]
        [TestCase("2", new int[] { 0 })]
        [TestCase("3", new int[] { 128, -128 })]
        [TestCase("4", new int[] { 128, -128, -256 })]
        [TestCase("5", new int[] { 128, -128, -256, -384 })]
        // invalid multiple
        [TestCase("6", new int[] { 128, 9, 10 })]
        [TestCase("7", new int[] { 128, 9, 10, 11 })]
        public void ShouldReturnErrorWithInvalidIvLength(string label, int[] ivLengths)
        {
            MathDomain md = new MathDomain();
            foreach (var value in ivLengths)
            {
                md.AddSegment(new ValueDomainSegment(value));
            }

            ParameterValidator subject = new ParameterValidator();
            var result = subject.Validate(
                new ParameterBuilder()
                    .WithIvLen(md)
                    .Build()
            );

            Assert.That(result.Success, Is.False);
        }

        [Test]
        [TestCase("internal", true)]
        [TestCase("external", true)]
        [TestCase("invalid", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void ShouldReturnErrorWithInvalidIvGen(string ivGen, bool isValid)
        {
            ParameterValidator subject = new ParameterValidator();
            var result = subject.Validate(
                new ParameterBuilder()
                    .WithIvGen(ivGen)
                    .Build()
            );

            Assert.That(result.Success, Is.EqualTo(isValid));
        }

        [Test]
        [TestCase("8.2.1", true)]
        [TestCase("8.2.2", true)]
        [TestCase("invalid", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void ShouldReturnErrorWithInvalidIvGenMode(string ivGenMode, bool isValid)
        {
            ParameterValidator subject = new ParameterValidator();
            var result = subject.Validate(
                new ParameterBuilder()
                    .WithIvGen("internal")
                    .WithIvGenMode(ivGenMode)
                    .Build()
            );

            Assert.That(result.Success, Is.EqualTo(isValid));
        }

        private class ParameterBuilder
        {
            private string _algorithm;
            private string[] _mode;
            private int[] _keyLen;
            private MathDomain _ptLen;
            private MathDomain _ivLen;
            private string _ivGen;
            private string _ivGenMode;
            private MathDomain _aadLen;
            private int[] _tagLen;

            public ParameterBuilder()
            {
                // Provides a valid (as of construction) set of parameters
                _algorithm = "ACVP-AES-GCM";
                _mode = ParameterValidator.VALID_DIRECTIONS;
                _keyLen = ParameterValidator.VALID_KEY_SIZES;
                _ptLen = new MathDomain().AddSegment(new ValueDomainSegment(128));
                _ivLen = new MathDomain().AddSegment(new ValueDomainSegment(96));
                _ivGen = ParameterValidator.VALID_IV_GEN[0];
                _ivGenMode = ParameterValidator.VALID_IV_GEN_MODE[0];
                _aadLen = new MathDomain().AddSegment(new ValueDomainSegment(128));
                _tagLen = new[] { 128 };
            }

            public ParameterBuilder WithAlgorithm(string value)
            {
                _algorithm = value;
                return this;
            }

            public ParameterBuilder WithMode(string[] value)
            {
                _mode = value;
                return this;
            }

            public ParameterBuilder WithKeyLen(int[] value)
            {
                _keyLen = value;
                return this;
            }

            public ParameterBuilder WithPtLen(MathDomain value)
            {
                _ptLen = value;
                return this;
            }

            public ParameterBuilder WithIvLen(MathDomain value)
            {
                _ivLen = value;
                return this;
            }

            public ParameterBuilder WithIvGen(string value)
            {
                _ivGen = value;
                return this;
            }

            public ParameterBuilder WithIvGenMode(string value)
            {
                _ivGenMode = value;
                return this;
            }

            public ParameterBuilder WithAadLen(MathDomain value)
            {
                _aadLen = value;
                return this;
            }

            public ParameterBuilder WithTagLen(int[] value)
            {
                _tagLen = value;
                return this;
            }

            public Parameters Build()
            {
                return new Parameters()
                {
                    AadLen = _aadLen,
                    Algorithm = _algorithm,
                    Revision = "1.0",
                    IvGen = _ivGen,
                    IvGenMode = _ivGenMode,
                    IvLen = _ivLen,
                    KeyLen = _keyLen,
                    Direction = _mode,
                    PayloadLen = _ptLen,
                    TagLen = _tagLen
                };
            }
        }
    }
}
