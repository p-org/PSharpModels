# PSharpModels
Models of distributed systems using P#

## How to build P#
First you need to download and build P# (provided as a git submodule). To do this run the following:

```
cd PSharpModels
git submodule init
git submodule update
cd PSharp
```

Open `PSharp.sln` and build.

## How to build Service Fabric Model
To build the ServiceFabricModel run the following:

```
cd PSharpModels
cd ServiceFabric
cd ServiceFabricModel
```
Open `ServiceFabricModel.sln` and build.

## How to build and run the ServiceFabric buggy example
First you need to build the example using the P# compiler. The example uses the ServiceFabricModel library, instead of the original ServiceFabric dlls. Do the following:

```
cd PSharpModels
cd PSharp
cd Binaries
.\PSharpCompiler.exe /s:{PATH_TO_PSharpModels_ROOT}\ServiceFabric\Samples\BuggyExample\BuggyExam
ple.sln /p:BuggyExample.Client /t:test
```

The above command should dump the `BuggyExample.dll`. You can then systematically test this compiled buggy example using P#:

```
.\PSharpTester.exe /test:{PATH_TO_BUGGY_EXAMPLE_DLL}\BuggyExample.dll /i:1 /v:2
```

For more testing options, see the P# [guidelines](https://github.com/p-org/PSharp)