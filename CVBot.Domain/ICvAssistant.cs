using CSharpFunctionalExtensions;

namespace CVBot.Domain;

public interface ICvAssistant
{
    public Task<Result<CvAssistantAnswer>> AnswerCvQuery(string query);
}