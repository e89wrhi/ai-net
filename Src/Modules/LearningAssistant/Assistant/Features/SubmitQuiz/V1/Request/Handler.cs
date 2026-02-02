using LearningAssistant.Data;
using LearningAssistant.Exceptions;
using LearningAssistant.ValueObjects;
using MediatR;

namespace LearningAssistant.Features.SubmitQuiz.V1;


internal class SubmitQuizHandler : IRequestHandler<SubmitQuizCommand, SubmitQuizCommandResponse>
{
    private readonly LearningDbContext _dbContext;

    public SubmitQuizHandler(LearningDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubmitQuizCommandResponse> Handle(SubmitQuizCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var lesson = await _dbContext.Lessons
            .FirstOrDefaultAsync(x => x.Id == LearningId.Of(request.LessonId), cancellationToken);

        if (lesson == null)
        {
            throw new LessonNotFoundException(request.LessonId);
        }

        var profile = await _dbContext.Profiles
            .Include(x => x.Lessons)
                .ThenInclude(l => l.Quizzes)
            .FirstOrDefaultAsync(x => x.Id == lesson.ProfileId, cancellationToken);

        if (profile == null)
        {
            throw new ProfileNotFoundException(lesson.ProfileId);
        }

        profile.SubmitQuiz(LearningId.Of(request.LessonId), QuizId.Of(request.QuizId), request.Score);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SubmitQuizCommandResponse(request.QuizId);
    }
}
