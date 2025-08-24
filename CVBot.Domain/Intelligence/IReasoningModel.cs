using CSharpFunctionalExtensions;

namespace CVBot.Domain.Intelligence;

public interface IReasoningModel
{
    public Task<Result<string>> AnswerAsync(string prompt, string systemPrompt);
}