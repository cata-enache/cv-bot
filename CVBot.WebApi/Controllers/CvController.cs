using CVBot.Application.CvExploration;
using CVBot.Application.CvIngestion;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CVBot.WebApi.Controllers;

[ApiController]
[Route("api/cv")]
public class CvController : ControllerBase
{
    private readonly IMediator _mediator;

    public CvController(IMediator mediator) => _mediator = mediator;

    [HttpPost("ingest")]
    public async Task<IActionResult> IngestCv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required and cannot be empty.");

        string cvContent = await ReadFileContentAsync(file);
        
        if (string.IsNullOrWhiteSpace(cvContent))
            return BadRequest("File content cannot be empty.");

        var command = new IngestCvCommand(cvContent);
        var result = await _mediator.Send(command);

        return result.IsSuccess 
            ? StatusCode(StatusCodes.Status201Created) 
            : BadRequest(result.Error);
    }

    [HttpPost("query")]
    public async Task<IActionResult> QueryCv([FromBody] CvQueryRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Query))
            return BadRequest("Query is required and cannot be empty.");

        var query = new AnswerCvQuery(request.Query);
        var result = await _mediator.Send(query);

        return result.IsSuccess 
            ? Ok(new CvQueryResponse(result.Value)) 
            : BadRequest(result.Error);
    }

    private async Task<string> ReadFileContentAsync(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        return await reader.ReadToEndAsync();
    }
}

public record CvQueryRequest(string Query);
public record CvQueryResponse(string Answer);