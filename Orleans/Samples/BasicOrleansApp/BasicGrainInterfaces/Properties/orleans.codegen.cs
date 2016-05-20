#if !EXCLUDE_CODEGEN
#pragma warning disable 162
#pragma warning disable 219
#pragma warning disable 414
#pragma warning disable 649
#pragma warning disable 693
#pragma warning disable 1591
#pragma warning disable 1998
[assembly: global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0")]
[assembly: global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute("BasicGrainInterfaces, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
namespace BasicOrleansApp
{
    using global::Orleans.Async;
    using global::Orleans;

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::System.SerializableAttribute, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.GrainReferenceAttribute(typeof (global::BasicOrleansApp.IClient))]
    internal class OrleansCodeGenClientReference : global::Orleans.Runtime.GrainReference, global::BasicOrleansApp.IClient
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
                return 906781924;
            }
        }

        public override global::System.String InterfaceName
        {
            get
            {
                return "global::BasicOrleansApp.IClient";
            }
        }

        public override global::System.Boolean @IsCompatible(global::System.Int32 @interfaceId)
        {
            return @interfaceId == 906781924;
        }

        protected override global::System.String @GetMethodName(global::System.Int32 @interfaceId, global::System.Int32 @methodId)
        {
            switch (@interfaceId)
            {
                case 906781924:
                    switch (@methodId)
                    {
                        case -312323863:
                            return "Initialize";
                        case -640371947:
                            return "Ping";
                        case -1249175558:
                            return "Pong";
                        default:
                            throw new global::System.NotImplementedException("interfaceId=" + 906781924 + ",methodId=" + @methodId);
                    }

                default:
                    throw new global::System.NotImplementedException("interfaceId=" + @interfaceId);
            }
        }

        public global::System.Threading.Tasks.Task<global::System.String> @Initialize(global::BasicOrleansApp.IServer @server)
        {
            return base.@InvokeMethodAsync<global::System.String>(-312323863, new global::System.Object[]{@server is global::Orleans.Grain ? @server.@AsReference<global::BasicOrleansApp.IServer>() : @server});
        }

        public global::System.Threading.Tasks.Task<global::System.Int32> @Ping()
        {
            return base.@InvokeMethodAsync<global::System.Int32>(-640371947, null);
        }

        public global::System.Threading.Tasks.Task<global::System.Int32> @Pong(global::System.Int32 @counter)
        {
            return base.@InvokeMethodAsync<global::System.Int32>(-1249175558, new global::System.Object[]{@counter});
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::Orleans.CodeGeneration.MethodInvokerAttribute("global::BasicOrleansApp.IClient", 906781924, typeof (global::BasicOrleansApp.IClient)), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
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
                    case 906781924:
                        switch (methodId)
                        {
                            case -312323863:
                                return ((global::BasicOrleansApp.IClient)@grain).@Initialize((global::BasicOrleansApp.IServer)arguments[0]).@Box();
                            case -640371947:
                                return ((global::BasicOrleansApp.IClient)@grain).@Ping().@Box();
                            case -1249175558:
                                return ((global::BasicOrleansApp.IClient)@grain).@Pong((global::System.Int32)arguments[0]).@Box();
                            default:
                                throw new global::System.NotImplementedException("interfaceId=" + 906781924 + ",methodId=" + methodId);
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
                return 906781924;
            }
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::System.SerializableAttribute, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.GrainReferenceAttribute(typeof (global::BasicOrleansApp.IServer))]
    internal class OrleansCodeGenServerReference : global::Orleans.Runtime.GrainReference, global::BasicOrleansApp.IServer
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
                return 142989943;
            }
        }

        public override global::System.String InterfaceName
        {
            get
            {
                return "global::BasicOrleansApp.IServer";
            }
        }

        public override global::System.Boolean @IsCompatible(global::System.Int32 @interfaceId)
        {
            return @interfaceId == 142989943;
        }

        protected override global::System.String @GetMethodName(global::System.Int32 @interfaceId, global::System.Int32 @methodId)
        {
            switch (@interfaceId)
            {
                case 142989943:
                    switch (@methodId)
                    {
                        case -1593989268:
                            return "Ping";
                        default:
                            throw new global::System.NotImplementedException("interfaceId=" + 142989943 + ",methodId=" + @methodId);
                    }

                default:
                    throw new global::System.NotImplementedException("interfaceId=" + @interfaceId);
            }
        }

        public global::System.Threading.Tasks.Task<global::System.Int32> @Ping(global::BasicOrleansApp.IClient @client, global::System.Int32 @counter)
        {
            return base.@InvokeMethodAsync<global::System.Int32>(-1593989268, new global::System.Object[]{@client is global::Orleans.Grain ? @client.@AsReference<global::BasicOrleansApp.IClient>() : @client, @counter});
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::Orleans.CodeGeneration.MethodInvokerAttribute("global::BasicOrleansApp.IServer", 142989943, typeof (global::BasicOrleansApp.IServer)), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
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
                    case 142989943:
                        switch (methodId)
                        {
                            case -1593989268:
                                return ((global::BasicOrleansApp.IServer)@grain).@Ping((global::BasicOrleansApp.IClient)arguments[0], (global::System.Int32)arguments[1]).@Box();
                            default:
                                throw new global::System.NotImplementedException("interfaceId=" + 142989943 + ",methodId=" + methodId);
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
                return 142989943;
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
