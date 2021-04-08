using System;

namespace BigbossScraping.Contracts.Interfaces
{
    public interface IParsedResponse
    {
        event EventHandler<ResponsePayload> ParsingCompleted;

        void RaiseCompleted(ResponsePayload data);
    }
}