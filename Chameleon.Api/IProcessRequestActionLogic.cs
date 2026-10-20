using Chameleon.CommonApiLib;

namespace Chameleon.Api
{
    public interface IProcessRequestActionLogic
    {
        public Task<RequestResult> ProcessRequestMessageAsync(ParserType parserType, Stream messageStream, string project);
    }
}
