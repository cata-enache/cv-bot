using CSharpFunctionalExtensions;

namespace CVBot.Application.CvIngestion;

public interface ICvIngestor
{
    public Task<Result> IngestAsync(string cvText);
}