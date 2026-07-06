using Application.Abstractions;
using Domain.Exceptions;
using MediatR;

namespace Application.Auth;

public record MeCommand() : IRequest<MeUserResponse>;

public record MeUserResponse(
    Guid Id,
    string Username,
    string Email,
    string? AvatarUrl,
    string? Bio,
    string Role);

public class Me(
    ICurrentUser currentUser,
    IUserRepository userRepository) : IRequestHandler<MeCommand, MeUserResponse>
{
    public async Task<MeUserResponse> Handle(MeCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var user = await userRepository.GetUserByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        return new MeUserResponse(
            user.Id,
            user.Username,
            user.Email,
            user.AvatarUrl,
            user.Bio,
            user.Role);
    }
}
