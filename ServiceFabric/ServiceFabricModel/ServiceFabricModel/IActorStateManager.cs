using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    //
    // Summary:
    //     Represents the interface that state manager for Microsoft.ServiceFabric.Actors.Runtime.Actor
    //     implements.
    public interface IActorStateManager
    {
        //
        // Summary:
        //     Adds an actor state with given state name, if it does not already exist or updates
        //     the state with specified state name, if it exists.
        //
        // Parameters:
        //   stateName:
        //     Name of the actor state to add or update.
        //
        //   addValue:
        //     Value of the actor state to add if it does not exist.
        //
        //   updateValueFactory:
        //     Factory function to generate value of actor state to update if it exists.
        //
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Type parameters:
        //   T:
        //     Type of value associated with given state name.
        //
        // Returns:
        //     A task that represents the asynchronous add/update operation. The value of TResult
        //     parameter contains value of actor state that was added/updated.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The specified state name is null.
        //
        //   T:System.OperationCanceledException:
        //     The operation was canceled.
        //
        // Remarks:
        //     The type of state value T must be https://msdn.microsoft.com/library/ms731923.aspx
        //     serializable.
        Task<T> AddOrUpdateStateAsync<T>(string stateName, T addValue, Func<string, T, T> updateValueFactory, CancellationToken cancellationToken = default(CancellationToken));
        //
        // Summary:
        //     Adds an actor state with given state name.
        //
        // Parameters:
        //   stateName:
        //     Name of the actor state to add.
        //
        //   value:
        //     Value of the actor state to add.
        //
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Type parameters:
        //   T:
        //     Type of value associated with given state name.
        //
        // Returns:
        //     A task that represents the asynchronous add operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     An actor state with given state name already exists.
        //
        //   T:System.ArgumentNullException:
        //     The specified state name is null.
        //
        //   T:System.OperationCanceledException:
        //     The operation was canceled.
        //
        // Remarks:
        //     The type of state value T must be https://msdn.microsoft.com/library/ms731923.aspx
        //     serializable.
        Task AddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken));
        //
        // Summary:
        //     Checks if an actor state with specified name exists.
        //
        // Parameters:
        //   stateName:
        //     Name of the actor state.
        //
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Returns:
        //     A task that represents the asynchronous check operation. The value of TResult
        //     parameter is true if state with specified name exists otherwise false.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The specified state name is null.
        //
        //   T:System.OperationCanceledException:
        //     The operation was canceled.
        Task<bool> ContainsStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken));
        //
        // Summary:
        //     Gets an actor state with given state name, if it exists or adds the state with
        //     specified state name and value, if it does not exist.
        //
        // Parameters:
        //   stateName:
        //     Name of the actor state to get or add.
        //
        //   value:
        //     Value of the actor state to add if it does not exist.
        //
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Type parameters:
        //   T:
        //     Type of value associated with given state name.
        //
        // Returns:
        //     A task that represents the asynchronous get or add operation. The value of TResult
        //     parameter contains value of actor state with given state name.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The specified state name is null.
        //
        //   T:System.OperationCanceledException:
        //     The operation was canceled.
        //
        // Remarks:
        //     The type of state value T must be https://msdn.microsoft.com/library/ms731923.aspx
        //     serializable.
        Task<T> GetOrAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken));
        //
        // Summary:
        //     Gets an actor state with specified state name.
        //
        // Parameters:
        //   stateName:
        //     Name of the actor state to get.
        //
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Type parameters:
        //   T:
        //     Type of value associated with given state name.
        //
        // Returns:
        //     A task that represents the asynchronous get operation. The value of TResult parameter
        //     contains value of actor state with given state name.
        //
        // Exceptions:
        //   T:System.Collections.Generic.KeyNotFoundException:
        //     An actor state with given state name does not exist.
        //
        //   T:System.ArgumentNullException:
        //     The specified state name is null.
        //
        //   T:System.OperationCanceledException:
        //     The operation was canceled.
        //
        // Remarks:
        //     The type of state value T must be https://msdn.microsoft.com/library/ms731923.aspx
        //     serializable.
        Task<T> GetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken));
        //
        // Summary:
        //     Creates an enumerable of all actor state names for current actor.
        //
        // Parameters:
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Returns:
        //     A task that represents the asynchronous enumeration operation. The value of TResult
        //     parameter is an enumerable of all actor state names.
        //
        // Exceptions:
        //   T:System.OperationCanceledException:
        //     The operation was canceled.
        Task<IEnumerable<string>> GetStateNamesAsync(CancellationToken cancellationToken = default(CancellationToken));
        //
        // Summary:
        //     Removes an actor state with specified state name.
        //
        // Parameters:
        //   stateName:
        //     Name of the actor state to remove.
        //
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Returns:
        //     A task that represents the asynchronous remove operation.
        //
        // Exceptions:
        //   T:System.Collections.Generic.KeyNotFoundException:
        //     An actor state with given state name does not exist.
        //
        //   T:System.ArgumentNullException:
        //     The specified state name is null.
        //
        //   T:System.OperationCanceledException:
        //     The operation was canceled.
        Task RemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken));
        //
        // Summary:
        //     Sets an actor state with given state name to specified value. If an actor state
        //     with specified name does not exist, it is added.
        //
        // Parameters:
        //   stateName:
        //     Name of the actor state to set.
        //
        //   value:
        //     Value of the actor state.
        //
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Type parameters:
        //   T:
        //     Type of value associated with given state name.
        //
        // Returns:
        //     A task that represents the asynchronous set operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The specified state name is null.
        //
        //   T:System.OperationCanceledException:
        //     The operation was canceled.
        //
        // Remarks:
        //     The type of state value T must be https://msdn.microsoft.com/library/ms731923.aspx
        //     serializable.
        Task SetStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken));
        //
        // Summary:
        //     Attempts to add an actor state with given state name.
        //
        // Parameters:
        //   stateName:
        //     Name of the actor state to add.
        //
        //   value:
        //     Value of the actor state to add.
        //
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Type parameters:
        //   T:
        //     Type of value associated with given state name.
        //
        // Returns:
        //     A task that represents the asynchronous add operation. The value of TResult parameter
        //     indicates if the state was successfully added.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The specified state name is null.
        //
        //   T:System.OperationCanceledException:
        //     The operation was canceled.
        //
        // Remarks:
        //     The type of state value T must be https://msdn.microsoft.com/library/ms731923.aspx
        //     serializable.
        Task<bool> TryAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken));
        //
        // Summary:
        //     Attempts to get an actor state with specified state name.
        //
        // Parameters:
        //   stateName:
        //     Name of the actor state to get.
        //
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Type parameters:
        //   T:
        //     Type of value associated with given state name.
        //
        // Returns:
        //     A task that represents the asynchronous get operation. The value of TResult parameter
        //     contains Microsoft.ServiceFabric.Data.ConditionalValue`1 indicating whether the
        //     actor state is present and the value of actor state if it is present.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The specified state name is null.
        //
        //   T:System.OperationCanceledException:
        //     The operation was canceled.
        //
        // Remarks:
        //     The type of state value T must be https://msdn.microsoft.com/library/ms731923.aspx
        //     serializable.
        Task<ConditionalValue<T>> TryGetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken));
        //
        // Summary:
        //     Attempts to remove an actor state with specified state name.
        //
        // Parameters:
        //   stateName:
        //     Name of the actor state to remove.
        //
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Returns:
        //     A task that represents the asynchronous remove operation. The value of TResult
        //     parameter indicates if the state was successfully removed.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The specified state name is null.
        //
        //   T:System.OperationCanceledException:
        //     The operation was canceled.
        Task<bool> TryRemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken));
    }
}