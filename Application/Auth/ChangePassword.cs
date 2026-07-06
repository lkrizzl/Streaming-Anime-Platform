using Application.Abstractions;
using Domain.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.Auth;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(Password.MinLength)
            .MaximumLength(Password.MaxLength);
    }
}

public class ChangePasswordHandler(
    ICurrentUser currentUser,
    IUserIdentityService userIdentityService,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var userIdentity = await userIdentityService.FindByUserIdAsync(userId, ct)
            ?? throw new NotFoundException("User identity not found.");

        if (!passwordHasher.VerifyPassword(request.CurrentPassword, userIdentity.PasswordHash))
            throw new BadRequestException(UserErrors.InvalidCredentials);

        userIdentity.UpdatePassword(Password.Create(request.NewPassword), passwordHasher);
        userIdentity.UpdateSecurityStamp();

        await unitOfWork.SaveChangesAsync(ct);
    }
}
