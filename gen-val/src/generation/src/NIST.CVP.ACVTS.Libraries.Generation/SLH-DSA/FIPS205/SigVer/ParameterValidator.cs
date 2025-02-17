using System.Collections.Generic;
using System.Linq;
using NIST.CVP.ACVTS.Libraries.Common;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.PQC.SLH_DSA.Enums;
using NIST.CVP.ACVTS.Libraries.Generation.Core;
using NIST.CVP.ACVTS.Libraries.Generation.Core.PqcHelpers;

namespace NIST.CVP.ACVTS.Libraries.Generation.SLH_DSA.FIPS205.SigVer;

public class ParameterValidator : PqcParameterValidator, IParameterValidator<Parameters>
{
    public static readonly SlhdsaParameterSet[] FastSigningParameterSets =  
    { 
        SlhdsaParameterSet.SLH_DSA_SHA2_128f, SlhdsaParameterSet.SLH_DSA_SHA2_192f, SlhdsaParameterSet.SLH_DSA_SHA2_256f, 
        SlhdsaParameterSet.SLH_DSA_SHAKE_128f, SlhdsaParameterSet.SLH_DSA_SHAKE_192f, SlhdsaParameterSet.SLH_DSA_SHAKE_256f
    };
    
    public static readonly SlhdsaParameterSet[] SmallSignatureParameterSets =  
    { 
        SlhdsaParameterSet.SLH_DSA_SHA2_128s, SlhdsaParameterSet.SLH_DSA_SHA2_192s, SlhdsaParameterSet.SLH_DSA_SHA2_256s, 
        SlhdsaParameterSet.SLH_DSA_SHAKE_128s, SlhdsaParameterSet.SLH_DSA_SHAKE_192s, SlhdsaParameterSet.SLH_DSA_SHAKE_256s
    };
    
    public ParameterValidateResponse Validate(Parameters parameters)
    {
        var errors = new List<string>();

        ValidateAlgoMode(parameters, new[] { AlgoMode.SLH_DSA_SigVer_FIPS205 }, errors);
        ValidateSignatureInterfacesAndPreHash(parameters, errors);
        ValidateCapabilities(parameters, errors);

        return errors.Any() ? new ParameterValidateResponse(errors) : new ParameterValidateResponse();
    }

    private void ValidateCapabilities(Parameters parameters, List<string> errors)
    {
        // 1) was Capabilities included, but empty?
        if (!parameters.Capabilities.Distinct().Any())
        {
            errors.Add($"Expected {nameof(parameters.Capabilities)} to not be empty");
            return;
        }
        
        // 2) examine each Capability that was provided
        foreach (var capability in parameters.Capabilities)
        {
            // i) is ParameterSets non-empty?
            if (!capability.ParameterSets.Distinct().Any())
            {
                errors.Add($"Expected {nameof(capability.ParameterSets)} to contain at least one valid ML-DSA parameter set");
                return;
            }

            // ii) check no duplicates are provided
            if (capability.ParameterSets.Length != capability.ParameterSets.Distinct().Count())
            {
                errors.Add($"{nameof(capability.ParameterSets)} must not contain the same ML-DSA parameter set more than once");
            }
            
            // iii) run the base validator on each capability
            ValidateCapability(capability, parameters, errors);
        }
    }
}
