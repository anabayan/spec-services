namespace BuildingBlocks.Dapr;

public class DaprOptions
{
    public string AppId { get; set; } = default!;
    public BindingComponent Binding { get; set; } = default!;
    public PubSubComponent PubSub { get; set; } = default!;
}

public class BindingComponent
{
    public string Name { get; set; } = default!;

    // TODO: Add other binding related properties
}

public class PubSubComponent
{
    public string Name { get; set; } = default!;
    // TODO: Add PubSub related properties
}
