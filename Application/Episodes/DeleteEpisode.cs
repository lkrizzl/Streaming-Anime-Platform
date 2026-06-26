using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Episodes;

public record DeleteEpisodeCommand(Guid Id) : IRequest;

public class DeleteEpisodeCommandValidator : AbstractValidator<DeleteEpisodeCommand>
{
    public DeleteEpisodeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Episode ID is required.");
    }
}

public class DeleteEpisodeHandler(
    IEpisodeRepository episodeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteEpisodeCommand>
{
    public async Task Handle(DeleteEpisodeCommand request, CancellationToken ct)
    {
        var episode = await episodeRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(EpisodeErrors.EpisodeNotFound(request.Id));

        await episodeRepository.DeleteAsync(episode, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
