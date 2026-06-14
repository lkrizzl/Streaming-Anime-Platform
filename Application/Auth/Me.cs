using Application.Abstractions;
using MediatR;

namespace Application.Auth;

public record MeCommand() : IRequest<MeUserResponse>;

public record MeUserResponse(Guid Id, string Username);

public class Me(ICurrentUser currentUser) : IRequestHandler<MeCommand, MeUserResponse>
{
    public Task<MeUserResponse> Handle(MeCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new MeUserResponse(currentUser.UserId!.Value, currentUser.Name!));
    }
}
