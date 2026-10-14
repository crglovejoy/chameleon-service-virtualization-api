using Chameleon.CommonApiLib;
using Microsoft.AspNetCore.Mvc;

namespace Chameleon.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RequestController : ControllerBase
{
    private readonly ILogger<RequestController> _logger;
    private readonly IProcessRequestActionLogic _processRequestActionLogic;

    public RequestController(ILogger<RequestController> logger, IProcessRequestActionLogic processRequestActionLogic)
    {
        _logger = logger;
        _processRequestActionLogic = processRequestActionLogic;
    }
    
    [HttpPost("jsonparser")]
    public async Task<IActionResult> JsonParser([FromQuery] string project)
    {
        var reqResult = await _processRequestActionLogic.ProcessRequestMessageAsync(ParserType.JSON, HttpContext.Request.Body, project);
        reqResult.HandleResponseHeaders(Response.Headers);
        return ResponseHelpers.BuildFromResult(this, reqResult, _logger);
    }

    [HttpPost("xmlparser")]
    public async Task<IActionResult> XmlParser([FromQuery] string project)
    {
        var reqResult = await _processRequestActionLogic.ProcessRequestMessageAsync(ParserType.XML, HttpContext.Request.Body, project);
        reqResult.HandleResponseHeaders(Response.Headers);
        return ResponseHelpers.BuildFromResult(this, reqResult, _logger);
    }
}


public static class ResponseHelpers
{
    public static IActionResult BuildFromResult(ControllerBase controller, RequestResult reqResult, ILogger logger)
    {
        if (reqResult.Success)
        {
            return controller.Ok(reqResult.ResponseStr);
        }

        if (reqResult.ServerErr || (reqResult.LoggingError?.Length ?? 0) > 0)
        {
            foreach (string err in reqResult.LoggingError ?? [])
            {
                logger.LogError(err);
            }

            return controller.StatusCode(500, reqResult.ResponseStr);
        }

        return controller.BadRequest(reqResult.ResponseStr);
    }
}
