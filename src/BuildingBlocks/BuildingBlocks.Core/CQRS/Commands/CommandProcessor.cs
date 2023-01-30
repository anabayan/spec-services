using BuildingBlocks.Abstractions.CQRS.Commands;
using MediatR;

namespace BuildingBlocks.Core.CQRS.Commands;

public class CommandProcessor : ICommandProcessor
{
    private readonly IMediator _mediator;

    public CommandProcessor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<TResult> SendAsync<TResult>(
        ICommand<TResult> command,
        CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        return _mediator.Send(command, cancellationToken);
    }
}
