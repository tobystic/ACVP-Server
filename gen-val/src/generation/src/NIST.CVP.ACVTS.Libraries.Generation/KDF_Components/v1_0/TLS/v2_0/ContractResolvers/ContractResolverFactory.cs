﻿using System;
using NIST.CVP.ACVTS.Libraries.Generation.Core.ContractResolvers;
using NIST.CVP.ACVTS.Libraries.Generation.Core.Enums;
using NIST.CVP.ACVTS.Libraries.Generation.KDF_Components.v1_0.TLS.v1_0;
using NIST.CVP.ACVTS.Libraries.Generation.KDF_Components.v1_0.TLS.v1_0.ContractResolvers;

namespace NIST.CVP.ACVTS.Libraries.Generation.KDF_Components.v1_0.TLS.v2_0.ContractResolvers
{
    public class ContractResolverFactory : IContractResolverFactory<TestGroup, TestCase>
    {
        public ProjectionContractResolverBase<TestGroup, TestCase> GetContractResolver(Projection projection)
        {
            switch (projection)
            {
                case Projection.Server:
                    return new ServerProjectionContractResolver<TestGroup, TestCase>();
                case Projection.Prompt:
                    return new PromptProjectionContractResolver();
                case Projection.Result:
                    return new ResultProjectionContractResolver();
                default:
                    throw new ArgumentException($"Invalid {nameof(projection)} ({projection})");
            }
        }
    }
}
