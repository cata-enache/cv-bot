using CVBot.Domain.CvContext;

namespace CVBot.Domain;

public record CvAssistantAnswer(string Answer, IReadOnlyCollection<CvContextChunk> Sources)
{
    public override string ToString() => Answer
                                         + Environment.NewLine
                                         + "Response based on the following sources: "
                                         + Environment.NewLine
                                         + string.Join(Environment.NewLine, Sources);
};