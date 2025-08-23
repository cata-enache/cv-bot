namespace CVBot.Domain.CvContext;

public interface ICvContextRetriever
{
    public Task<IReadOnlyCollection<CvContextChunk>> GetRelevantContextAsync(string query);
}