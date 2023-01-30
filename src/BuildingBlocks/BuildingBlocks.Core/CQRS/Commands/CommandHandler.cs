using BuildingBlocks.Abstractions.CQRS.Commands;
using MediatR;

namespace BuildingBlocks.Core.CQRS.Commands;

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public Task<Unit> Handle(TCommand request, CancellationToken cancellationToken)
    {
        return HandleCommandAsync(request, cancellationToken);
    }

    protected abstract Task<Unit> HandleCommandAsync(TCommand command, CancellationToken cancellationToken);
}

public abstract class CommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
    public Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken)
    {
        return HandleCommandAsync(request, cancellationToken);
    }

    protected abstract Task<TResponse> HandleCommandAsync(
        TCommand command,
        CancellationToken cancellationToken = default);
}
