//-----------------------------------------------------------------------
// <copyright file="ActorExtensions.cs">
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

using Microsoft.ServiceFabric.Actors.Runtime;

namespace Microsoft.ServiceFabric.Actors
{
    //
    // Summary:
    //     Class containing extension methods for Actors.
    public static class ActorExtensions
    {
        //
        // Summary:
        //     Gets Microsoft.ServiceFabric.Actors.ActorId for the actor./>
        //
        // Parameters:
        //   actor:
        //     Actor object to get ActorId for.
        //
        // Type parameters:
        //   TIActor:
        //     Actor interface type.
        //
        // Returns:
        //     Microsoft.ServiceFabric.Actors.ActorId for the actor.
        public static ActorId GetActorId<TIActor>(this TIActor actor) where TIActor : IActor
        {
            var act = actor as Actor;
            return act.Id;
        }
        //
        // Summary:
        //     Gets Microsoft.ServiceFabric.Actors.ActorReference for the actor.
        //
        // Parameters:
        //   actor:
        //     Actor object to get ActorReference for.
        //
        // Returns:
        //     Microsoft.ServiceFabric.Actors.ActorReference for the actor.
        /*
        public static ActorReference GetActorReference(this IActor actor)
        {
            throw new NotImplementedException();
        }
        */
    }
}
