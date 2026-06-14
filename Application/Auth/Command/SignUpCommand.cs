using Application.Abstractions;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.ValueObjects;
using MediatR;

namespace Application.Auth;

public record SignUpCommand(
    string Email,
    string Username,
    string Password) : IRequest<SignUpResponse>;

public record SignUpResponse(
    Guid Id,
    string Email,
    string Username,
    string SecurityStamp);

public class SignUp(
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    IUserIdentityService userIdentityService,
    IUnitOfWork unitOfWork) : IRequestHandler<SignUpCommand, SignUpResponse>
{
    public async Task<SignUpResponse> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var username = Username.Create(request.Username);
        var email = Email.Create(request.Email);

        if (await userIdentityService.ExistsByEmailOrUsernameAsync(email, username, cancellationToken))
        {
            throw new BadRequestException(UserErrors.UserAlreadyExists);
        }

        var userIdentityId = Guid.NewGuid();

        var user = new User(userIdentityId, username, email);

        var userIdentity = new UserIdentity(
            userIdentityId,
            user.Id,
            username,
            email,
            Password.Create(request.Password),
            passwordHasher);

        await userRepository.AddAsync(user, cancellationToken);
        await userIdentityService.AddAsync(userIdentity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SignUpResponse(user.Id, userIdentity.Email, userIdentity.Username, userIdentity.SecurityStamp);
    }
}
