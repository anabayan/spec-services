using BuildingBlocks.Abstractions.Dapr.Models;
using Dapr.Client;

namespace BuildingBlocks.Abstractions.Dapr;

public interface IStateStore
{
    public Task<TResult> GetStateAsync<TResult>(
        string storeName,
        string key,
        ConsistencyMode consistencyMode = ConsistencyMode.Eventual,
        IReadOnlyDictionary<string, string> metadata = default,
        CancellationToken cancellationToken = default);

    public Task SaveStateAsync<TValue>(
        string storeName,
        string key,
        TValue value,
        StateStoreOptions stateOptions = default,
        IReadOnlyDictionary<string, string> metadata = default,
        CancellationToken cancellationToken = default);
}
