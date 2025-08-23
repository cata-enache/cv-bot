using CSharpFunctionalExtensions;
using MediatR;

namespace CVBot.Application.CvIngestion;

public record IngestCvCommand(string CvContent) : IRequest<Result>
{
    public Result Validate() => Result.FailureIf(string.IsNullOrWhiteSpace(CvContent), "No CV content provided!");
};