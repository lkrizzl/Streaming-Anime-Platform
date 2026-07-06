using Application.Abstractions;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.Users;

public record UpdateProfileCommand(
    string? Username,
    string? Bio,
    string? AvatarUrl) : IRequest;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        When(x => x.Username is not null, () =>
        {
            RuleFor(x => x.Username!)
                .MinimumLength(Username.MinLength)
                .MaximumLength(Username.MaxLength);
        });

        When(x => x.Bio is not null, () =>
        {
            RuleFor(x => x.Bio!)
                .MaximumLength(1000);
        });

        When(x => x.AvatarUrl is not null, () =>
        {
            RuleFor(x => x.AvatarUrl!)
                .MaximumLength(2048);
        });
    }
}

public class UpdateProfileHandler(
    ICurrentUser currentUser,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProfileCommand>
{
    public async Task Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var user = await userRepository.GetUserByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        if (request.Username is not null)
            user.UpdateUsername(Username.Create(request.Username));

        if (request.Bio is not null)
            user.UpdateBio(request.Bio);

        if (request.AvatarUrl is not null)
            user.UpdateAvatar(request.AvatarUrl);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
