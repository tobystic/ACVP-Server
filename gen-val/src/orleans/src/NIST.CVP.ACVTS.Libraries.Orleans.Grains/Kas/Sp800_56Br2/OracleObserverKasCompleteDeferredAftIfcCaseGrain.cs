﻿using System;
using System.Linq;
using System.Threading.Tasks;
using NIST.CVP.ACVTS.Libraries.Common;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.Builders;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.Enums;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.FixedInfo;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.Helpers;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.KC;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.KDA;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.Scheme;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KES;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KTS;
using NIST.CVP.ACVTS.Libraries.Crypto.KTS;
using NIST.CVP.ACVTS.Libraries.Math.Entropy;
using NIST.CVP.ACVTS.Libraries.Oracle.Abstractions.ParameterTypes.Kas.Sp800_56Br2;
using NIST.CVP.ACVTS.Libraries.Oracle.Abstractions.ResultTypes.Kas.Sp800_56Br2;
using NIST.CVP.ACVTS.Libraries.Orleans.Grains.Interfaces.Kas.Sp800_56Br2;

namespace NIST.CVP.ACVTS.Libraries.Orleans.Grains.Kas.Sp800_56Br2
{
    public class OracleObserverKasCompleteDeferredAftIfcCaseGrain : ObservableOracleGrainBase<KasAftDeferredResult>,
        IOracleObserverKasCompleteDeferredAftIfcCaseGrain
    {
        private readonly IKasIfcBuilder _kasBuilder;
        private readonly ISchemeIfcBuilder _schemeBuilder;
        private readonly IIfcSecretKeyingMaterialBuilder _serverSecretKeyingMaterialBuilder;
        private readonly IIfcSecretKeyingMaterialBuilder _iutSecretKeyingMaterialBuilder;
        private readonly IKdfFactory _kdfFactory;
        private readonly IKtsFactory _ktsFactory;
        private readonly IKeyConfirmationFactory _keyConfirmationFactory;
        private readonly IFixedInfoFactory _fixedInfoFactory;
        private readonly IEntropyProvider _entropyProvider;
        private readonly IRsaSve _rsaSve;

        private KasAftDeferredParametersIfc _param;

        public OracleObserverKasCompleteDeferredAftIfcCaseGrain(
            LimitedConcurrencyLevelTaskScheduler nonOrleansScheduler,
            IKasIfcBuilder kasBuilder,
            ISchemeIfcBuilder schemeBuilder,
            IIfcSecretKeyingMaterialBuilder serverSecretKeyingMaterialBuilder,
            IIfcSecretKeyingMaterialBuilder iutSecretKeyingMaterialBuilder,
            IKdfFactory kdfFactory,
            IKtsFactory ktsFactory,
            IKeyConfirmationFactory keyConfirmationFactory,
            IFixedInfoFactory fixedInfoFactory,
            IEntropyProvider entropyProvider,
            IRsaSve rsaSve
        )
            : base(nonOrleansScheduler)
        {
            _kasBuilder = kasBuilder;
            _schemeBuilder = schemeBuilder;
            _serverSecretKeyingMaterialBuilder = serverSecretKeyingMaterialBuilder;
            _iutSecretKeyingMaterialBuilder = iutSecretKeyingMaterialBuilder;
            _kdfFactory = kdfFactory;
            _ktsFactory = ktsFactory;
            _keyConfirmationFactory = keyConfirmationFactory;
            _fixedInfoFactory = fixedInfoFactory;
            _entropyProvider = entropyProvider;
            _rsaSve = rsaSve;
        }

        public async Task<bool> BeginWorkAsync(KasAftDeferredParametersIfc param)
        {
            _param = param;

            await BeginGrainWorkAsync();
            return await Task.FromResult(true);
        }

        protected override async Task DoWorkAsync()
        {
            try
            {
                var isServerPartyU = _param.ServerKeyAgreementRole == KeyAgreementRole.InitiatorPartyU;
                var isServerPartyV = !isServerPartyU;

                _serverSecretKeyingMaterialBuilder
                    .WithPartyId(_param.ServerPartyId)
                    .WithKey(_param.ServerKey)
                    .WithC(_param.ServerC)
                    .WithZ(_param.ServerZ)
                    .WithK(_param.ServerK)
                    .WithDkmNonce(_param.ServerNonce);

                var serverSecretKeyingMaterial = _serverSecretKeyingMaterialBuilder
                    .Build(
                        _param.Scheme,
                        _param.KasMode,
                        _param.ServerKeyAgreementRole,
                        _param.ServerKeyConfirmationRole,
                        _param.KeyConfirmationDirection);

                var iutSecretKeyingMaterial = _iutSecretKeyingMaterialBuilder
                    .WithPartyId(_param.IutPartyId)
                    .WithKey(_param.IutKey)
                    .WithC(_param.IutC)
                    .WithZ(_param.IutZ)
                    .WithDkmNonce(_param.IutNonce)
                    .Build(
                        _param.Scheme,
                        _param.KasMode,
                        _param.IutKeyAgreementRole,
                        _param.IutKeyConfirmationRole,
                        _param.KeyConfirmationDirection);

                var fixedInfoParameter = new FixedInfoParameter()
                {
                    L = _param.L,
                };

                // KDF fixed info construction
                if (KeyGenerationRequirementsHelper.IfcKdfSchemes.Contains(_param.Scheme))
                {
                    fixedInfoParameter.Encoding = _param.KdfParameter.FixedInputEncoding;
                    fixedInfoParameter.FixedInfoPattern = _param.KdfParameter.FixedInfoPattern;
                    fixedInfoParameter.Salt = _param.KdfParameter.Salt;
                    fixedInfoParameter.Iv = _param.KdfParameter.Iv;
                    fixedInfoParameter.Label = _param.KdfParameter.Label;
                    fixedInfoParameter.Context = _param.KdfParameter.Context;
                    fixedInfoParameter.AlgorithmId = _param.KdfParameter.AlgorithmId;
                    fixedInfoParameter.L = _param.L;
                    fixedInfoParameter.T = _param.KdfParameter.T;
                    fixedInfoParameter.EntropyBits = _param.KdfParameter.EntropyBits;
                }

                // KTS fixed info construction
                if (KeyGenerationRequirementsHelper.IfcKtsSchemes.Contains(_param.Scheme))
                {
                    fixedInfoParameter.Encoding = _param.KtsParameter.Encoding;
                    fixedInfoParameter.FixedInfoPattern = _param.KtsParameter.AssociatedDataPattern;
                    fixedInfoParameter.Label = _param.KtsParameter.Label;
                    fixedInfoParameter.Context = _param.KtsParameter.Context;
                    fixedInfoParameter.AlgorithmId = _param.KtsParameter.AlgorithmId;
                    fixedInfoParameter.L = _param.L;
                }

                MacParameters macParam = null;
                IKeyConfirmationFactory kcFactory = null;
                if (KeyGenerationRequirementsHelper.IfcKcSchemes.Contains(_param.Scheme))
                {
                    macParam = _param.MacParameter;
                    kcFactory = _keyConfirmationFactory;
                }

                _schemeBuilder
                    .WithSchemeParameters(
                        new SchemeParametersIfc(
                            new KasAlgoAttributesIfc(_param.Scheme, _param.Modulo, _param.L),
                            _param.ServerKeyAgreementRole,
                            _param.KasMode,
                            _param.ServerKeyConfirmationRole,
                            _param.KeyConfirmationDirection,
                            KasAssurance.None,
                            _param.ServerPartyId))
                    .WithThisPartyKeyingMaterialBuilder(_serverSecretKeyingMaterialBuilder)
                    .WithThisPartyKeyingMaterial(serverSecretKeyingMaterial)
                    .WithFixedInfo(_fixedInfoFactory, fixedInfoParameter)
                    .WithKdf(_kdfFactory, _param.KdfParameter)
                    .WithKts(_ktsFactory, _param.KtsParameter)
                    .WithRsaSve(_rsaSve)
                    .WithEntropyProvider(_entropyProvider)
                    .WithKeyConfirmation(kcFactory, macParam);

                var serverKas = _kasBuilder.WithSchemeBuilder(_schemeBuilder).Build();

                var result = serverKas.ComputeResult(iutSecretKeyingMaterial);
                var returnResult = new KasAftDeferredResult()
                {
                    ServerKeyingMaterial = isServerPartyU ? result.KeyingMaterialPartyU : result.KeyingMaterialPartyV,
                    IutKeyingMaterial = isServerPartyV ? result.KeyingMaterialPartyU : result.KeyingMaterialPartyV,
                    Result = new KasResult(result.Dkm, result.MacKey, result.MacData, result.Tag)
                };

                await Notify(returnResult);
            }
            catch (DecryptionFailedException e)
            {
                await Notify(new KasAftDeferredResult()
                {
                    Result = new KasResult(e.Message)
                });
            }
            catch (Exception e)
            {
                await Throw(e);
            }
        }
    }
}
