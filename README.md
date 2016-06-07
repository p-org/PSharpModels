# Robust Unit Testing of Distributed Services
This project describes a methodology for robust testing of applications build using [Azure Service Fabric](https://azure.microsoft.com/en-in/services/service-fabric/) or [Orleans](https://github.com/dotnet/orleans). Both these platform expose an API for programming distributed services and offer state management and fault tolerance for free. However, the resulting applications are still highly asynchronous and may be difficult to "unit test". We use the [P# framework](https://github.com/p-org/PSharp) for setting up reliable unit tests of (largely unmodified) Actor services written on either Service Fabric or on Orleans. The P# testing engine systematically covers many interleavings of the service that are possible in actual production environments (such as message duplication, out-of-order delivery, timer non-determinism, etc.). P# also offers mechanisms for writing detailed specifications, including liveness properties (i.e., asserting that something good happens eventually).

## How to build P\# #
First you need to download and build P#, which is provided as a git submodule:

```
cd ${PSHARP_MODELS_DIR}
git submodule init
git submodule update
cd PSharp
```

Open `PSharp.sln` in Visual Studio and build it.

## How to build and use OrleansModel
Follow the instructions [here](https://github.com/p-org/PSharpModels/tree/master/Orleans).


## How to build and use ServiceFabricModel
Follow the instructions [here](https://github.com/p-org/PSharpModels/tree/master/ServiceFabric).
