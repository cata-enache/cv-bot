using CSharpFunctionalExtensions;
using MediatR;

namespace CVBot.Application.CvExploration;

public record AnswerCvQuery(string UserQuery) : IRequest<Result<string>>
{
    public Result Validate() => Result.FailureIf(string.IsNullOrWhiteSpace(UserQuery), "Query cannot be empty!");
};