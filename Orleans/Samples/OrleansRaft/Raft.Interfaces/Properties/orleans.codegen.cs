#if !EXCLUDE_CODEGEN
#pragma warning disable 162
#pragma warning disable 219
#pragma warning disable 414
#pragma warning disable 649
#pragma warning disable 693
#pragma warning disable 1591
#pragma warning disable 1998
[assembly: global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0")]
[assembly: global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute("Raft.Interfaces, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
namespace Raft.Interfaces
{
    using global::Orleans.Async;
    using global::Orleans;

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::System.SerializableAttribute, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.GrainReferenceAttribute(typeof (global::Raft.Interfaces.IClient))]
    internal class OrleansCodeGenClientReference : global::Orleans.Runtime.GrainReference, global::Raft.Interfaces.IClient
    {
        protected @OrleansCodeGenClientReference(global::Orleans.Runtime.GrainReference @other): base (@other)
        {
        }

        protected @OrleansCodeGenClientReference(global::System.Runtime.Serialization.SerializationInfo @info, global::System.Runtime.Serialization.StreamingContext @context): base (@info, @context)
        {
        }

        protected override global::System.Int32 InterfaceId
        {
            get
            {
                return 108865850;
            }
        }

        public override global::System.String InterfaceName
        {
            get
            {
                return "global::Raft.Interfaces.IClient";
            }
        }

        public override global::System.Boolean @IsCompatible(global::System.Int32 @interfaceId)
        {
            return @interfaceId == 108865850;
        }

        protected override global::System.String @GetMethodName(global::System.Int32 @interfaceId, global::System.Int32 @methodId)
        {
            switch (@interfaceId)
            {
                case 108865850:
                    switch (@methodId)
                    {
                        case -1605520623:
                            return "Configure";
                        default:
                            throw new global::System.NotImplementedException("interfaceId=" + 108865850 + ",methodId=" + @methodId);
                    }

                default:
                    throw new global::System.NotImplementedException("interfaceId=" + @interfaceId);
            }
        }

        public global::System.Threading.Tasks.Task @Configure(global::System.Int32 @clusterId)
        {
            return base.@InvokeMethodAsync<global::System.Object>(-1605520623, new global::System.Object[]{@clusterId});
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::Orleans.CodeGeneration.MethodInvokerAttribute("global::Raft.Interfaces.IClient", 108865850, typeof (global::Raft.Interfaces.IClient)), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    internal class OrleansCodeGenClientMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
    {
        public global::System.Threading.Tasks.Task<global::System.Object> @Invoke(global::Orleans.Runtime.IAddressable @grain, global::Orleans.CodeGeneration.InvokeMethodRequest @request)
        {
            global::System.Int32 interfaceId = @request.@InterfaceId;
            global::System.Int32 methodId = @request.@MethodId;
            global::System.Object[] arguments = @request.@Arguments;
            try
            {
                if (@grain == null)
                    throw new global::System.ArgumentNullException("grain");
                switch (interfaceId)
                {
                    case 108865850:
                        switch (methodId)
                        {
                            case -1605520623:
                                return ((global::Raft.Interfaces.IClient)@grain).@Configure((global::System.Int32)arguments[0]).@Box();
                            default:
                                throw new global::System.NotImplementedException("interfaceId=" + 108865850 + ",methodId=" + methodId);
                        }

                    default:
                        throw new global::System.NotImplementedException("interfaceId=" + interfaceId);
                }
            }
            catch (global::System.Exception exception)
            {
                return global::Orleans.Async.TaskUtility.@Faulted(exception);
            }
        }

        public global::System.Int32 InterfaceId
        {
            get
            {
                return 108865850;
            }
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::System.SerializableAttribute, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.GrainReferenceAttribute(typeof (global::Raft.Interfaces.IClusterManager))]
    internal class OrleansCodeGenClusterManagerReference : global::Orleans.Runtime.GrainReference, global::Raft.Interfaces.IClusterManager
    {
        protected @OrleansCodeGenClusterManagerReference(global::Orleans.Runtime.GrainReference @other): base (@other)
        {
        }

        protected @OrleansCodeGenClusterManagerReference(global::System.Runtime.Serialization.SerializationInfo @info, global::System.Runtime.Serialization.StreamingContext @context): base (@info, @context)
        {
        }

        protected override global::System.Int32 InterfaceId
        {
            get
            {
                return 2139488188;
            }
        }

        public override global::System.String InterfaceName
        {
            get
            {
                return "global::Raft.Interfaces.IClusterManager";
            }
        }

        public override global::System.Boolean @IsCompatible(global::System.Int32 @interfaceId)
        {
            return @interfaceId == 2139488188;
        }

        protected override global::System.String @GetMethodName(global::System.Int32 @interfaceId, global::System.Int32 @methodId)
        {
            switch (@interfaceId)
            {
                case 2139488188:
                    switch (@methodId)
                    {
                        case -227017028:
                            return "Configure";
                        case 1647346573:
                            return "NotifyLeaderUpdate";
                        default:
                            throw new global::System.NotImplementedException("interfaceId=" + 2139488188 + ",methodId=" + @methodId);
                    }

                default:
                    throw new global::System.NotImplementedException("interfaceId=" + @interfaceId);
            }
        }

        public global::System.Threading.Tasks.Task @Configure()
        {
            return base.@InvokeMethodAsync<global::System.Object>(-227017028, null);
        }

        public global::System.Threading.Tasks.Task @NotifyLeaderUpdate(global::System.Int32 @leaderId, global::System.Int32 @term)
        {
            return base.@InvokeMethodAsync<global::System.Object>(1647346573, new global::System.Object[]{@leaderId, @term});
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::Orleans.CodeGeneration.MethodInvokerAttribute("global::Raft.Interfaces.IClusterManager", 2139488188, typeof (global::Raft.Interfaces.IClusterManager)), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    internal class OrleansCodeGenClusterManagerMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
    {
        public global::System.Threading.Tasks.Task<global::System.Object> @Invoke(global::Orleans.Runtime.IAddressable @grain, global::Orleans.CodeGeneration.InvokeMethodRequest @request)
        {
            global::System.Int32 interfaceId = @request.@InterfaceId;
            global::System.Int32 methodId = @request.@MethodId;
            global::System.Object[] arguments = @request.@Arguments;
            try
            {
                if (@grain == null)
                    throw new global::System.ArgumentNullException("grain");
                switch (interfaceId)
                {
                    case 2139488188:
                        switch (methodId)
                        {
                            case -227017028:
                                return ((global::Raft.Interfaces.IClusterManager)@grain).@Configure().@Box();
                            case 1647346573:
                                return ((global::Raft.Interfaces.IClusterManager)@grain).@NotifyLeaderUpdate((global::System.Int32)arguments[0], (global::System.Int32)arguments[1]).@Box();
                            default:
                                throw new global::System.NotImplementedException("interfaceId=" + 2139488188 + ",methodId=" + methodId);
                        }

                    default:
                        throw new global::System.NotImplementedException("interfaceId=" + interfaceId);
                }
            }
            catch (global::System.Exception exception)
            {
                return global::Orleans.Async.TaskUtility.@Faulted(exception);
            }
        }

        public global::System.Int32 InterfaceId
        {
            get
            {
                return 2139488188;
            }
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::System.SerializableAttribute, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.GrainReferenceAttribute(typeof (global::Raft.Interfaces.IServer))]
    internal class OrleansCodeGenServerReference : global::Orleans.Runtime.GrainReference, global::Raft.Interfaces.IServer
    {
        protected @OrleansCodeGenServerReference(global::Orleans.Runtime.GrainReference @other): base (@other)
        {
        }

        protected @OrleansCodeGenServerReference(global::System.Runtime.Serialization.SerializationInfo @info, global::System.Runtime.Serialization.StreamingContext @context): base (@info, @context)
        {
        }

        protected override global::System.Int32 InterfaceId
        {
            get
            {
                return 2115164851;
            }
        }

        public override global::System.String InterfaceName
        {
            get
            {
                return "global::Raft.Interfaces.IServer";
            }
        }

        public override global::System.Boolean @IsCompatible(global::System.Int32 @interfaceId)
        {
            return @interfaceId == 2115164851;
        }

        protected override global::System.String @GetMethodName(global::System.Int32 @interfaceId, global::System.Int32 @methodId)
        {
            switch (@interfaceId)
            {
                case 2115164851:
                    switch (@methodId)
                    {
                        case 1472034337:
                            return "Configure";
                        case -84135934:
                            return "VoteRequest";
                        case -1645665559:
                            return "VoteResponse";
                        default:
                            throw new global::System.NotImplementedException("interfaceId=" + 2115164851 + ",methodId=" + @methodId);
                    }

                default:
                    throw new global::System.NotImplementedException("interfaceId=" + @interfaceId);
            }
        }

        public global::System.Threading.Tasks.Task @Configure(global::System.Int32 @id, global::System.Collections.Generic.List<global::System.Int32> @serverIds, global::System.Int32 @clusterId)
        {
            return base.@InvokeMethodAsync<global::System.Object>(1472034337, new global::System.Object[]{@id, @serverIds, @clusterId});
        }

        public global::System.Threading.Tasks.Task @VoteRequest(global::System.Int32 @term, global::System.Int32 @candidateId, global::System.Int32 @lastLogIndex, global::System.Int32 @lastLogTerm)
        {
            return base.@InvokeMethodAsync<global::System.Object>(-84135934, new global::System.Object[]{@term, @candidateId, @lastLogIndex, @lastLogTerm});
        }

        public global::System.Threading.Tasks.Task @VoteResponse(global::System.Int32 @term, global::System.Boolean @voteGranted)
        {
            return base.@InvokeMethodAsync<global::System.Object>(-1645665559, new global::System.Object[]{@term, @voteGranted});
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::Orleans.CodeGeneration.MethodInvokerAttribute("global::Raft.Interfaces.IServer", 2115164851, typeof (global::Raft.Interfaces.IServer)), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    internal class OrleansCodeGenServerMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
    {
        public global::System.Threading.Tasks.Task<global::System.Object> @Invoke(global::Orleans.Runtime.IAddressable @grain, global::Orleans.CodeGeneration.InvokeMethodRequest @request)
        {
            global::System.Int32 interfaceId = @request.@InterfaceId;
            global::System.Int32 methodId = @request.@MethodId;
            global::System.Object[] arguments = @request.@Arguments;
            try
            {
                if (@grain == null)
                    throw new global::System.ArgumentNullException("grain");
                switch (interfaceId)
                {
                    case 2115164851:
                        switch (methodId)
                        {
                            case 1472034337:
                                return ((global::Raft.Interfaces.IServer)@grain).@Configure((global::System.Int32)arguments[0], (global::System.Collections.Generic.List<global::System.Int32>)arguments[1], (global::System.Int32)arguments[2]).@Box();
                            case -84135934:
                                return ((global::Raft.Interfaces.IServer)@grain).@VoteRequest((global::System.Int32)arguments[0], (global::System.Int32)arguments[1], (global::System.Int32)arguments[2], (global::System.Int32)arguments[3]).@Box();
                            case -1645665559:
                                return ((global::Raft.Interfaces.IServer)@grain).@VoteResponse((global::System.Int32)arguments[0], (global::System.Boolean)arguments[1]).@Box();
                            default:
                                throw new global::System.NotImplementedException("interfaceId=" + 2115164851 + ",methodId=" + methodId);
                        }

                    default:
                        throw new global::System.NotImplementedException("interfaceId=" + interfaceId);
                }
            }
            catch (global::System.Exception exception)
            {
                return global::Orleans.Async.TaskUtility.@Faulted(exception);
            }
        }

        public global::System.Int32 InterfaceId
        {
            get
            {
                return 2115164851;
            }
        }
    }
}
#pragma warning restore 162
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 649
#pragma warning restore 693
#pragma warning restore 1591
#pragma warning restore 1998
#endif
