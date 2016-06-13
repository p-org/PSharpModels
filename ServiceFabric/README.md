# Testing Actor services of Azure Service Fabric
We provide a model (or a *mock*) of Azure Service Fabric written using [P#](https://github.com/p-org/PSharp). The model exposes the same APIs as Service Fabric. To use it, one must redirect the application to build against the model DLL instead of the actual Fabric DLLs. The model currently only covers the [Actor programming model](https://azure.microsoft.com/en-in/documentation/articles/service-fabric-reliable-actors-introduction/).

## How to build
To build ServiceFabricModel do the following:

```
cd ${PSHARP_MODELS_DIR}
cd ServiceFabric
cd ServiceFabricModel
```
Open `ServiceFabricModel.sln` in Visual Studio and build it.

## How to build and run the buggy SmartHome application on Service Fabric
```
cd ${PSHARP_MODELS_DIR}\ServiceFabric\Samples\FabricSmartHome
FabricSmartHome.sln
```

Compile with Visual Studio, and run as you would a normal Azure Service Fabric application.

There is a bug in this application, but its hard to manifest on the actual Service Fabric runtime.

## How to build and test SmartHome using P# #
First you need to build the example using the P# compiler. The example uses ServiceFabricModel, instead of the original Azure Service Fabric dlls. This allows P# to take control of nondeterminism during testing.

```
cd ${PSHARP_MODELS_DIR}
cd PSharp
cd Binaries
.\PSharpCompiler.exe /s:{PSHARP_MODELS_DIR}\ServiceFabric\Samples\SmartHome\SmartHome.sln /p:SmartHome.Client /t:test
```

Running the P# compiler will output `SmartHome.Client.dll`. You can then systematically test this dll using P#:

```
.\PSharpTester.exe /test:{PATH_TO_BUGGY_EXAMPLE_DLL}\SmartHome.Client.dll /i:1
```

Where `/i:N` is the number of scheduling tests. You can use `/v:2` to raise the verbosity of the output. When a bug is found, two traces will be generated: a human-readable one, and a reproducible one.

For more testing flags, see the P# [guidelines](https://github.com/p-org/PSharp).
