#if !EXCLUDE_CODEGEN
#pragma warning disable 162
#pragma warning disable 219
#pragma warning disable 414
#pragma warning disable 649
#pragma warning disable 693
#pragma warning disable 1591
#pragma warning disable 1998
[assembly: global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0")]
[assembly: global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute("OrleansBuggyExample.Interfaces, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
namespace OrleansBuggyExample
{
    using global::Orleans.Async;
    using global::Orleans;

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::System.SerializableAttribute, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.GrainReferenceAttribute(typeof (global::OrleansBuggyExample.IReceiver))]
    internal class OrleansCodeGenReceiverReference : global::Orleans.Runtime.GrainReference, global::OrleansBuggyExample.IReceiver
    {
        protected @OrleansCodeGenReceiverReference(global::Orleans.Runtime.GrainReference @other): base (@other)
        {
        }

        protected @OrleansCodeGenReceiverReference(global::System.Runtime.Serialization.SerializationInfo @info, global::System.Runtime.Serialization.StreamingContext @context): base (@info, @context)
        {
        }

        protected override global::System.Int32 InterfaceId
        {
            get
            {
                return 1134536864;
            }
        }

        public override global::System.String InterfaceName
        {
            get
            {
                return "global::OrleansBuggyExample.IReceiver";
            }
        }

        public override global::System.Boolean @IsCompatible(global::System.Int32 @interfaceId)
        {
            return @interfaceId == 1134536864;
        }

        protected override global::System.String @GetMethodName(global::System.Int32 @interfaceId, global::System.Int32 @methodId)
        {
            switch (@interfaceId)
            {
                case 1134536864:
                    switch (@methodId)
                    {
                        case 1092224019:
                            return "StartTransaction";
                        case -716877348:
                            return "TransmitData";
                        case 1381309745:
                            return "GetCurrentCount";
                        default:
                            throw new global::System.NotImplementedException("interfaceId=" + 1134536864 + ",methodId=" + @methodId);
                    }

                default:
                    throw new global::System.NotImplementedException("interfaceId=" + @interfaceId);
            }
        }

        public global::System.Threading.Tasks.Task @StartTransaction()
        {
            return base.@InvokeMethodAsync<global::System.Object>(1092224019, null);
        }

        public global::System.Threading.Tasks.Task @TransmitData(global::System.String @item)
        {
            return base.@InvokeMethodAsync<global::System.Object>(-716877348, new global::System.Object[]{@item});
        }

        public global::System.Threading.Tasks.Task<global::System.Int32> @GetCurrentCount()
        {
            return base.@InvokeMethodAsync<global::System.Int32>(1381309745, null);
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::Orleans.CodeGeneration.MethodInvokerAttribute("global::OrleansBuggyExample.IReceiver", 1134536864, typeof (global::OrleansBuggyExample.IReceiver)), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    internal class OrleansCodeGenReceiverMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
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
                    case 1134536864:
                        switch (methodId)
                        {
                            case 1092224019:
                                return ((global::OrleansBuggyExample.IReceiver)@grain).@StartTransaction().@Box();
                            case -716877348:
                                return ((global::OrleansBuggyExample.IReceiver)@grain).@TransmitData((global::System.String)arguments[0]).@Box();
                            case 1381309745:
                                return ((global::OrleansBuggyExample.IReceiver)@grain).@GetCurrentCount().@Box();
                            default:
                                throw new global::System.NotImplementedException("interfaceId=" + 1134536864 + ",methodId=" + methodId);
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
                return 1134536864;
            }
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::System.SerializableAttribute, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.GrainReferenceAttribute(typeof (global::OrleansBuggyExample.ISender))]
    internal class OrleansCodeGenSenderReference : global::Orleans.Runtime.GrainReference, global::OrleansBuggyExample.ISender
    {
        protected @OrleansCodeGenSenderReference(global::Orleans.Runtime.GrainReference @other): base (@other)
        {
        }

        protected @OrleansCodeGenSenderReference(global::System.Runtime.Serialization.SerializationInfo @info, global::System.Runtime.Serialization.StreamingContext @context): base (@info, @context)
        {
        }

        protected override global::System.Int32 InterfaceId
        {
            get
            {
                return -638123477;
            }
        }

        public override global::System.String InterfaceName
        {
            get
            {
                return "global::OrleansBuggyExample.ISender";
            }
        }

        public override global::System.Boolean @IsCompatible(global::System.Int32 @interfaceId)
        {
            return @interfaceId == -638123477;
        }

        protected override global::System.String @GetMethodName(global::System.Int32 @interfaceId, global::System.Int32 @methodId)
        {
            switch (@interfaceId)
            {
                case -638123477:
                    switch (@methodId)
                    {
                        case 1847227850:
                            return "DoSomething";
                        default:
                            throw new global::System.NotImplementedException("interfaceId=" + -638123477 + ",methodId=" + @methodId);
                    }

                default:
                    throw new global::System.NotImplementedException("interfaceId=" + @interfaceId);
            }
        }

        public global::System.Threading.Tasks.Task @DoSomething(global::System.Int32 @numberOfItems)
        {
            return base.@InvokeMethodAsync<global::System.Object>(1847227850, new global::System.Object[]{@numberOfItems});
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::Orleans.CodeGeneration.MethodInvokerAttribute("global::OrleansBuggyExample.ISender", -638123477, typeof (global::OrleansBuggyExample.ISender)), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    internal class OrleansCodeGenSenderMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
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
                    case -638123477:
                        switch (methodId)
                        {
                            case 1847227850:
                                return ((global::OrleansBuggyExample.ISender)@grain).@DoSomething((global::System.Int32)arguments[0]).@Box();
                            default:
                                throw new global::System.NotImplementedException("interfaceId=" + -638123477 + ",methodId=" + methodId);
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
                return -638123477;
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
