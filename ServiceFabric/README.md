# ServiceFabricModel
Model of Azure Service Fabric in [P#](https://github.com/p-org/PSharp).

## How to build
To build ServiceFabricModel do the following:

```
cd ${PSHARP_MODELS_DIR}
cd ServiceFabric
cd ServiceFabricModel
```
Open `ServiceFabricModel.sln` in Visual Studio and build it.

## How to build and run the ServiceFabric buggy example
First you need to build the example using the P# compiler. The example uses ServiceFabricModel, instead of the original Azure Service Fabric dlls. This allows P# to take control of nondeterminism during testing.

```
cd ${PSHARP_MODELS_DIR}
cd PSharp
cd Binaries
.\PSharpCompiler.exe /s:{PSHARP_MODELS_DIR}\ServiceFabric\Samples\BuggyExample\BuggyExam
ple.sln /p:BuggyExample.Client /t:test
```

Running the P# compiler will output `BuggyExample.dll`. You can then systematically test this dll using P#:

```
.\PSharpTester.exe /test:{PATH_TO_BUGGY_EXAMPLE_DLL}\BuggyExample.dll /i:1
```

Where `/i:N` is the number of scheduling tests. You can use `/v:2` to raise the verbosity of the output. When a bug is found, a readable trace is generated.

For more testing flags, see the P# [guidelines](https://github.com/p-org/PSharp).
