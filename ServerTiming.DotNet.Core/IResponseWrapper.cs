namespace ServerTiming.DotNet.Core
{
    public interface IResponseWrapper
    {
        void AddHeader(string key, string value);
        bool HeadersWritten { get; }
    }
}