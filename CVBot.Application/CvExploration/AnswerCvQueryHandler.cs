using CSharpFunctionalExtensions;
using CVBot.Domain;
using MediatR;

namespace CVBot.Application.CvExploration;

public class AnswerCvQueryHandler(ICvAssistant assistant): IRequestHandler<AnswerCvQuery, Result<string>>
{
    public async Task<Result<string>> Handle(AnswerCvQuery request, CancellationToken cancellationToken)
    {
        var validationResult = request.Validate();
        if(validationResult.IsFailure)
            return Result.Failure<string>(validationResult.Error);

        var result = await assistant.AnswerCvQueryAsync(request.UserQuery);
        
        if(result.IsFailure)
            return Result.Failure<string>(result.Error);

        return result.Value.ToString();
    }
}