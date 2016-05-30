# OrleansModel
Model of Orleans in [P#](https://github.com/p-org/PSharp).

## How to build
To build OrleansModel do the following:

```
cd ${PSHARP_MODELS_DIR}
cd Orleans
cd OrleansModel
```
Open `OrleansModel.sln` in Visual Studio and build it.

## How to build and run the Orleans buggy example
First you need to build the example using the P# compiler. The example uses OrleansModel, instead of the original Orleans dlls. This allows P# to take control of nondeterminism during testing.

```
cd ${PSHARP_MODELS_DIR}
cd PSharp
cd Binaries
.\PSharpCompiler.exe /s:{PSHARP_MODELS_DIR}\Orleans\Samples\BuggyOrleansApp\BuggyOrleansApp.sln /p:BuggyOrleansHost /t:test
```

Running the P# compiler will output `BuggyOrleansHost.dll`. You can then systematically test this dll using P#:

```
.\PSharpTester.exe /test:{PATH_TO_BUGGY_EXAMPLE_DLL}\BuggyOrleansHost.dll /i:1
```

Where `/i:N` is the number of scheduling tests. You can use `/v:2` to raise the verbosity of the output. When a bug is found, a readable trace is generated.

For more testing flags, see the P# [guidelines](https://github.com/p-org/PSharp).
