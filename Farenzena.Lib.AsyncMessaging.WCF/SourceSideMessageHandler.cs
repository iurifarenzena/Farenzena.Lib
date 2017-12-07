using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Farenzena.Lib.AsyncMessaging.Messages;

namespace Farenzena.Lib.AsyncMessaging.WCFPushingQueue
{
    // Vai conectar com o WCF service no consumidor pra enviar as mensagens
    public class SourceSideMessageHandler : IMessageHandler
    {        

        public Task HandleBooleanPromptMessageAsync(BooleanPromptMessage message)
        {
            throw new NotImplementedException();
        }

        public Task HandleMessageAsync(BasicMessage message)
        {
            SendMessage(new MessageDTO(message));
            return Task.CompletedTask;
        }

        public Task HandleMultipleAnswerPromptMessageAsync<T>(MultipleAnswerPromptMessage<T> message)
        {
            throw new NotImplementedException();
        }

        public Task HandleSingleAnswerPromptMessageAsync<T>(SingleAnswerPromptMessage<T> message)
        {
            throw new NotImplementedException();
        }

        private void SendMessage(MessageDTO messageDTO)
        {
            try
            {
                var client = SourceConfigurationHelper.GetMessageServerClient();
                client.Open();
                client.SendMessage(messageDTO);
                client.Close();
            }
            catch
            {
                throw new MessageHandlerUnavailableException();
            }
        }
    }
}
