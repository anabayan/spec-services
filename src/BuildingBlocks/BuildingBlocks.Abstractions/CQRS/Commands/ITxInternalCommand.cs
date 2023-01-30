namespace BuildingBlocks.Abstractions.CQRS.Commands;

public interface ITxInternalCommand : IInternalCommand, ITxRequest
{
}
