using CSharpFunctionalExtensions;
using MediatR;

namespace CVBot.Application.CvIngestion;

public class IngestCvCommandHandler(ICvIngestor cvIngestor): IRequestHandler<IngestCvCommand, Result>
{
    public async Task<Result> Handle(IngestCvCommand command, CancellationToken cancellationToken)
    {
       var validationResult = command.Validate();
       if(validationResult.IsFailure)
           return Result.Failure(validationResult.Error);

       await cvIngestor.IngestAsync(command.CvContent);
       
       return Result.Success();
    }
}