using CSharpFunctionalExtensions;

namespace CVBot.Domain.Intelligence;

public interface IReasoningModel
{
    public Task<Result<string>> Answer(string prompt, string systemPrompt);
}