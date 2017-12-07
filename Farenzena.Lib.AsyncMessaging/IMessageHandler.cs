using Farenzena.Lib.AsyncMessaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.AsyncMessaging
{
    public interface IMessageHandler
    {
        Task HandleMessageAsync(BasicMessage message);
        Task HandleBooleanPromptMessageAsync(BooleanPromptMessage message);
        Task HandleSingleAnswerPromptMessageAsync<T>(SingleAnswerPromptMessage<T> message);
        Task HandleMultipleAnswerPromptMessageAsync<T>(MultipleAnswerPromptMessage<T> message);
    }
}
