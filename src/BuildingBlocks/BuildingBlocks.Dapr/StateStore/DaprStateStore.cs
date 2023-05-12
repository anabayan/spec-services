using BuildingBlocks.Abstractions.Dapr;
using BuildingBlocks.Abstractions.Dapr.Models;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Dapr.StateStore;

public class DaprStateStore : IStateStore, IDaprService
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<DaprStateStore> _logger;

    // TODO: https://andrewlock.net/exploring-dotnet-6-part-8-improving-logging-performance-with-source-generators/


    public DaprStateStore(ILogger<DaprStateStore> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task<TResult> GetStateAsync<TResult>(
        string storeName,
        string key,
        ConsistencyMode consistencyMode = ConsistencyMode.Eventual,
        IReadOnlyDictionary<string, string> metadata = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting state for key {Key} from store {StoreName}", key, storeName);

            return await _daprClient.GetStateAsync<TResult>(
                storeName,
                key,
                consistencyMode,
                metadata,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Dapr State Store method GetStateAsync");
            throw;
        }
    }

    public async Task SaveStateAsync<TValue>(
        string storeName,
        string key,
        TValue value,
        StateStoreOptions stateOptions = default,
        IReadOnlyDictionary<string, string> metadata = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Saving state for key {Key} from store {StoreName}", key, storeName);

            await _daprClient.SaveStateAsync(
                storeName,
                key,
                value,
                stateOptions,
                metadata,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Dapr State Store method SaveStateAsync");
            throw;
        }
    }
}
