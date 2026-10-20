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
        return ResponseHelpers.BuildFromResult(this, reqResult, _logger);
    }

    [HttpPost("xmlparser")]
    public async Task<IActionResult> XmlParser([FromQuery] string project)
    {
        var reqResult = await _processRequestActionLogic.ProcessRequestMessageAsync(ParserType.XML, HttpContext.Request.Body, project);
        return ResponseHelpers.BuildFromResult(this, reqResult, _logger);
    }
}


public static class ResponseHelpers
{
    public static IActionResult BuildFromResult(ControllerBase controller, RequestResult reqResult, ILogger logger)
    {
        reqResult.HandleResponseHeaders(controller.Response.Headers);

        switch(reqResult.ResultStatus)
        {
            case ReturnResultStatus.Result200:
                return controller.Ok(reqResult.ResponseStr);

            case ReturnResultStatus.Result400:
                return controller.BadRequest(reqResult.ResponseStr);

            case ReturnResultStatus.Result500:
                if (!string.IsNullOrWhiteSpace(reqResult.LoggingError))
                {
                    logger.LogError(reqResult.LoggingError);
                }
                return controller.StatusCode(500, reqResult.ResponseStr);

            default:
                string err = $"{reqResult.ResultStatus} is not a supported ReturnResultStatus value";
                logger.LogError(err);
                return controller.StatusCode(500, err);
        }
    }
}
