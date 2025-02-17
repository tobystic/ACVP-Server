﻿using System.Threading.Tasks;
using NIST.CVP.ACVTS.Libraries.Generation.Core;
using NIST.CVP.ACVTS.Libraries.Generation.Core.Async;

namespace NIST.CVP.ACVTS.Libraries.Generation.KeyWrap.v1_0
{
    public class TestCaseGeneratorNull<TTestGroup, TTestCase> : ITestCaseGeneratorAsync<TTestGroup, TTestCase>
        where TTestGroup : TestGroupBase<TTestGroup, TTestCase>
        where TTestCase : TestCaseBase<TTestGroup, TTestCase>, new()
    {
        public int NumberOfTestCasesToGenerate => 1;

        public Task<TestCaseGenerateResponse<TTestGroup, TTestCase>> GenerateAsync(TTestGroup group, bool isSample, int caseNo = 0)
        {
            return Task.FromResult(new TestCaseGenerateResponse<TTestGroup, TTestCase>("Null generator"));
        }
    }
}
