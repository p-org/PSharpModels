//-----------------------------------------------------------------------
// <copyright file="IActorProxy.cs">
//      Copyright (c) Microsoft Corporation. All rights reserved.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//      IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//      CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//      TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//      SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Client
{
    //
    // Summary:
    //     Provides the interface for implementation of proxy access for actor service.
    public interface IActorProxy
    {
        //
        // Summary:
        //     Gets Microsoft.ServiceFabric.Actors.ActorId associated with the proxy object.
        ActorId ActorId { get; }
        //
        // Summary:
        //     Gets Microsoft.ServiceFabric.Actors.Client.IActorServicePartitionClient that
        //     this proxy is using to communicate with the actor.
        //IActorServicePartitionClient ActorServicePartitionClient { get; }
    }
}
