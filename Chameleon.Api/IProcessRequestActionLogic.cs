using Chameleon.CommonApiLib;
using Chameleon.CommonLib;

namespace Chameleon.Api
{
    public interface IProcessRequestActionLogic
    {
        public Task<RequestResult> ProcessRequestMessageAsync(ParserType parserType, Stream messageStream, string project);
        public Task<RequestResult> ProcessRequestMessageAsync(ParserType parserType, String messageStr, string project);
    }
}
