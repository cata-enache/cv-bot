using CSharpFunctionalExtensions;
using CVBot.Domain.Intelligence;
using CVBot.Infrastructure.AiConfigurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CVBot.Infrastructure.Intelligence;

public class ReasoningModel([FromKeyedServices(nameof(AiConfig.ReasoningAiModel))] IChatCompletionService completionService) : IReasoningModel
{
    private const string FailedToAnswerMessage = "Something went wrong while trying to answer.";
    public async Task<Result<string>> AnswerAsync(string prompt, string systemPrompt)
    {
        var zeroShotHistory = new ChatHistory(systemMessage: systemPrompt);
        zeroShotHistory.AddUserMessage(prompt);
        var response = await completionService.GetChatMessageContentsAsync(zeroShotHistory);
        return response.FirstOrDefault()?.Content ?? Result.Failure<string>(FailedToAnswerMessage);
    }
}