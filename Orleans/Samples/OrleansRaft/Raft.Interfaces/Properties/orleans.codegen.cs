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

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::System.SerializableAttribute, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.GrainReferenceAttribute(typeof (global::Raft.Interfaces.ISafetyMonitor))]
    internal class OrleansCodeGenSafetyMonitorReference : global::Orleans.Runtime.GrainReference, global::Raft.Interfaces.ISafetyMonitor
    {
        protected @OrleansCodeGenSafetyMonitorReference(global::Orleans.Runtime.GrainReference @other): base (@other)
        {
        }

        protected @OrleansCodeGenSafetyMonitorReference(global::System.Runtime.Serialization.SerializationInfo @info, global::System.Runtime.Serialization.StreamingContext @context): base (@info, @context)
        {
        }

        protected override global::System.Int32 InterfaceId
        {
            get
            {
                return -30188181;
            }
        }

        public override global::System.String InterfaceName
        {
            get
            {
                return "global::Raft.Interfaces.ISafetyMonitor";
            }
        }

        public override global::System.Boolean @IsCompatible(global::System.Int32 @interfaceId)
        {
            return @interfaceId == -30188181;
        }

        protected override global::System.String @GetMethodName(global::System.Int32 @interfaceId, global::System.Int32 @methodId)
        {
            switch (@interfaceId)
            {
                case -30188181:
                    switch (@methodId)
                    {
                        case 1518790450:
                            return "NotifyLeaderElected";
                        default:
                            throw new global::System.NotImplementedException("interfaceId=" + -30188181 + ",methodId=" + @methodId);
                    }

                default:
                    throw new global::System.NotImplementedException("interfaceId=" + @interfaceId);
            }
        }

        public global::System.Threading.Tasks.Task @NotifyLeaderElected(global::System.Int32 @term)
        {
            return base.@InvokeMethodAsync<global::System.Object>(1518790450, new global::System.Object[]{@term});
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::Orleans.CodeGeneration.MethodInvokerAttribute("global::Raft.Interfaces.ISafetyMonitor", -30188181, typeof (global::Raft.Interfaces.ISafetyMonitor)), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    internal class OrleansCodeGenSafetyMonitorMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
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
                    case -30188181:
                        switch (methodId)
                        {
                            case 1518790450:
                                return ((global::Raft.Interfaces.ISafetyMonitor)@grain).@NotifyLeaderElected((global::System.Int32)arguments[0]).@Box();
                            default:
                                throw new global::System.NotImplementedException("interfaceId=" + -30188181 + ",methodId=" + methodId);
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
                return -30188181;
            }
        }
    }

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
                        case -1339241695:
                            return "ProcessResponse";
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

        public global::System.Threading.Tasks.Task @ProcessResponse()
        {
            return base.@InvokeMethodAsync<global::System.Object>(-1339241695, null);
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
                            case -1339241695:
                                return ((global::Raft.Interfaces.IClient)@grain).@ProcessResponse().@Box();
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
                        case -1686842035:
                            return "RelayClientRequest";
                        case -1360605994:
                            return "RedirectClientRequest";
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

        public global::System.Threading.Tasks.Task @RelayClientRequest(global::System.Int32 @clientId, global::System.Int32 @command)
        {
            return base.@InvokeMethodAsync<global::System.Object>(-1686842035, new global::System.Object[]{@clientId, @command});
        }

        public global::System.Threading.Tasks.Task @RedirectClientRequest(global::System.Int32 @clientId, global::System.Int32 @command)
        {
            return base.@InvokeMethodAsync<global::System.Object>(-1360605994, new global::System.Object[]{@clientId, @command});
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
                            case -1686842035:
                                return ((global::Raft.Interfaces.IClusterManager)@grain).@RelayClientRequest((global::System.Int32)arguments[0], (global::System.Int32)arguments[1]).@Box();
                            case -1360605994:
                                return ((global::Raft.Interfaces.IClusterManager)@grain).@RedirectClientRequest((global::System.Int32)arguments[0], (global::System.Int32)arguments[1]).@Box();
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
                        case -1478130170:
                            return "AppendEntriesRequest";
                        case 1694759622:
                            return "AppendEntriesResponse";
                        case -1360605994:
                            return "RedirectClientRequest";
                        case 127647396:
                            return "ProcessClientRequest";
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

        public global::System.Threading.Tasks.Task @AppendEntriesRequest(global::System.Int32 @term, global::System.Int32 @leaderId, global::System.Int32 @prevLogIndex, global::System.Int32 @prevLogTerm, global::System.Collections.Generic.List<global::Raft.Interfaces.Log> @entries, global::System.Int32 @leaderCommit, global::System.Int32 @clientId)
        {
            return base.@InvokeMethodAsync<global::System.Object>(-1478130170, new global::System.Object[]{@term, @leaderId, @prevLogIndex, @prevLogTerm, @entries, @leaderCommit, @clientId});
        }

        public global::System.Threading.Tasks.Task @AppendEntriesResponse(global::System.Int32 @term, global::System.Boolean @success, global::System.Int32 @serverId, global::System.Int32 @clientId)
        {
            return base.@InvokeMethodAsync<global::System.Object>(1694759622, new global::System.Object[]{@term, @success, @serverId, @clientId});
        }

        public global::System.Threading.Tasks.Task @RedirectClientRequest(global::System.Int32 @clientId, global::System.Int32 @command)
        {
            return base.@InvokeMethodAsync<global::System.Object>(-1360605994, new global::System.Object[]{@clientId, @command});
        }

        public global::System.Threading.Tasks.Task @ProcessClientRequest(global::System.Int32 @clientId, global::System.Int32 @command)
        {
            return base.@InvokeMethodAsync<global::System.Object>(127647396, new global::System.Object[]{@clientId, @command});
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
                            case -1478130170:
                                return ((global::Raft.Interfaces.IServer)@grain).@AppendEntriesRequest((global::System.Int32)arguments[0], (global::System.Int32)arguments[1], (global::System.Int32)arguments[2], (global::System.Int32)arguments[3], (global::System.Collections.Generic.List<global::Raft.Interfaces.Log>)arguments[4], (global::System.Int32)arguments[5], (global::System.Int32)arguments[6]).@Box();
                            case 1694759622:
                                return ((global::Raft.Interfaces.IServer)@grain).@AppendEntriesResponse((global::System.Int32)arguments[0], (global::System.Boolean)arguments[1], (global::System.Int32)arguments[2], (global::System.Int32)arguments[3]).@Box();
                            case -1360605994:
                                return ((global::Raft.Interfaces.IServer)@grain).@RedirectClientRequest((global::System.Int32)arguments[0], (global::System.Int32)arguments[1]).@Box();
                            case 127647396:
                                return ((global::Raft.Interfaces.IServer)@grain).@ProcessClientRequest((global::System.Int32)arguments[0], (global::System.Int32)arguments[1]).@Box();
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

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.SerializerAttribute(typeof (global::Raft.Interfaces.Log)), global::Orleans.CodeGeneration.RegisterSerializerAttribute]
    internal class OrleansCodeGenRaft_Interfaces_LogSerializer
    {
        private static readonly global::System.Reflection.FieldInfo field1 = typeof (global::Raft.Interfaces.Log).@GetField("Command", (System.@Reflection.@BindingFlags.@Instance | System.@Reflection.@BindingFlags.@NonPublic | System.@Reflection.@BindingFlags.@Public));
        private static readonly global::System.Func<global::Raft.Interfaces.Log, global::System.Int32> getField1 = (global::System.Func<global::Raft.Interfaces.Log, global::System.Int32>)global::Orleans.Serialization.SerializationManager.@GetGetter(field1);
        private static readonly global::System.Action<global::Raft.Interfaces.Log, global::System.Int32> setField1 = (global::System.Action<global::Raft.Interfaces.Log, global::System.Int32>)global::Orleans.Serialization.SerializationManager.@GetReferenceSetter(field1);
        private static readonly global::System.Reflection.FieldInfo field0 = typeof (global::Raft.Interfaces.Log).@GetField("Term", (System.@Reflection.@BindingFlags.@Instance | System.@Reflection.@BindingFlags.@NonPublic | System.@Reflection.@BindingFlags.@Public));
        private static readonly global::System.Func<global::Raft.Interfaces.Log, global::System.Int32> getField0 = (global::System.Func<global::Raft.Interfaces.Log, global::System.Int32>)global::Orleans.Serialization.SerializationManager.@GetGetter(field0);
        private static readonly global::System.Action<global::Raft.Interfaces.Log, global::System.Int32> setField0 = (global::System.Action<global::Raft.Interfaces.Log, global::System.Int32>)global::Orleans.Serialization.SerializationManager.@GetReferenceSetter(field0);
        [global::Orleans.CodeGeneration.CopierMethodAttribute]
        public static global::System.Object DeepCopier(global::System.Object original)
        {
            global::Raft.Interfaces.Log input = ((global::Raft.Interfaces.Log)original);
            global::Raft.Interfaces.Log result = new global::Raft.Interfaces.Log();
            setField1(result, getField1(input));
            setField0(result, getField0(input));
            global::Orleans.@Serialization.@SerializationContext.@Current.@RecordObject(original, result);
            return result;
        }

        [global::Orleans.CodeGeneration.SerializerMethodAttribute]
        public static void Serializer(global::System.Object untypedInput, global::Orleans.Serialization.BinaryTokenStreamWriter stream, global::System.Type expected)
        {
            global::Raft.Interfaces.Log input = (global::Raft.Interfaces.Log)untypedInput;
            global::Orleans.Serialization.SerializationManager.@SerializeInner(getField1(input), stream, typeof (global::System.Int32));
            global::Orleans.Serialization.SerializationManager.@SerializeInner(getField0(input), stream, typeof (global::System.Int32));
        }

        [global::Orleans.CodeGeneration.DeserializerMethodAttribute]
        public static global::System.Object Deserializer(global::System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
        {
            global::Raft.Interfaces.Log result = new global::Raft.Interfaces.Log();
            global::Orleans.@Serialization.@DeserializationContext.@Current.@RecordObject(result);
            setField1(result, (global::System.Int32)global::Orleans.Serialization.SerializationManager.@DeserializeInner(typeof (global::System.Int32), stream));
            setField0(result, (global::System.Int32)global::Orleans.Serialization.SerializationManager.@DeserializeInner(typeof (global::System.Int32), stream));
            return (global::Raft.Interfaces.Log)result;
        }

        public static void Register()
        {
            global::Orleans.Serialization.SerializationManager.@Register(typeof (global::Raft.Interfaces.Log), DeepCopier, Serializer, Deserializer);
        }

        static OrleansCodeGenRaft_Interfaces_LogSerializer()
        {
            Register();
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
