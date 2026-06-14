using Application.Abstractions;
using Domain.Exceptions;
using MediatR;

namespace Application.Auth;

public record SignOutAllCommand() : IRequest;

public class SignOutAll(
    ICurrentUser currentUser,
    IUserIdentityService userIdentityService,
    IUnitOfWork unitOfWork) : IRequestHandler<SignOutAllCommand>
{
    public async Task Handle(SignOutAllCommand request, CancellationToken cancellationToken)
    {
        var userCredential = await userIdentityService.FindByUserIdAsync(
            currentUser.UserId!.Value,
            cancellationToken) ?? throw new ForbiddenException();

        userCredential.UpdateSecurityStamp();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
