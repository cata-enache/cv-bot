namespace CVBot.Domain.CvContext;

public interface ICvContextRetriever
{
    public Task<IReadOnlyCollection<CvContextChunk>> GetRelevantContext(string query);
}