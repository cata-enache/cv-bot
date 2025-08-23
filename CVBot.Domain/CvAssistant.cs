using CSharpFunctionalExtensions;
using CVBot.Domain.CvContext;
using CVBot.Domain.Intelligence;

namespace CVBot.Domain;

public class CvAssistant(ICvContextRetriever contextRetriever, IReasoningModel reasoningModel) : ICvAssistant
{
    private const string FailureToReplyMessage = "Something went wrong while trying to reply. Please try again later.";

    private const string EmptyQueryMessage =
        "Something went wrong. The query I received was empty. Please try again later.";

    private const string NoContextAvailableMessage =
        "I'm sorry but I do not have enough information to answer this query.";
    
    private const string BasePrompt =
        "You are a spokesman on behalf of someone's CV. Rely only on the available context. Do not make any statements that cannot be backed by the provided context.";

    public async Task<Result<CvAssistantAnswer>> AnswerCvQueryAsync(string query)
    {
        if (string.IsNullOrEmpty(query))
            return Result.Failure<CvAssistantAnswer>(EmptyQueryMessage);

        var chunks = await contextRetriever.GetRelevantContextAsync(query);
        if (chunks.Count == 0)
            return Result.Failure<CvAssistantAnswer>(NoContextAvailableMessage);


        var contextualSystemPrompt = BasePrompt + Environment.NewLine + ConvertContextToPromptPatch(chunks);
        var response = await reasoningModel.Answer(query, contextualSystemPrompt);

        return response.IsFailure
            ? Result.Failure<CvAssistantAnswer>(FailureToReplyMessage)
            : new CvAssistantAnswer(response.Value, chunks);
    }

    private static string ConvertContextToPromptPatch(IReadOnlyCollection<CvContextChunk> chunks)
    {
        if (chunks.Count == 0)
            return "No relevant context was found!";

        var promptContextualPatch =
            $"The following parts of the cv you represent were found relevant in some capacity to user's query:{Environment.NewLine}";
        
        var stringifiedChunkList = string.Join(Environment.NewLine, chunks.Select((c, i) => $"{i}. {c.Value}"));

        return promptContextualPatch + stringifiedChunkList;
    }
}