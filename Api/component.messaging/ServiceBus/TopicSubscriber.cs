using component.logger;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace component.messaging.ServiceBus
{
    public class TopicSubscriber : ITopicSubscriber
    {
        private readonly ISubscriptionClient _subscriptionClient;
        private readonly ILogger _logger;

        public TopicSubscriber(string serviceBusEndpoint, string serviceBusTopic, string subscriptionName, ILogger logger)
        {
            _logger = logger;
            _subscriptionClient = new SubscriptionClient(serviceBusEndpoint, serviceBusTopic, subscriptionName);
        }

        public void ReceiveMessage(Func<Message, CancellationToken, Task> callBackFunc)
        {
            try
            {
                var messageHandlerOptions = new MessageHandlerOptions((args) => Task.CompletedTask)
                {
                    MaxConcurrentCalls = 50,
                    AutoComplete = true
                };
                _subscriptionClient.RegisterMessageHandler(callBackFunc, messageHandlerOptions);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in service bus subscription : {ex.Message}, StackTrace : {ex.StackTrace}");
            }
        }

        public T DecodeMessage<T>(Message message)
        {
            var stringMessage = ExtractJsonObject(Encoding.UTF8.GetString(message.Body)).ToString();
            var data = JsonConvert.DeserializeObject<T>(stringMessage);
            return data;
        }

        private object ExtractJsonObject(string mixedString)
        {
            for (var i = mixedString.IndexOf('{'); i > -1; i = mixedString.IndexOf('{', i + 1))
            {
                for (var j = mixedString.LastIndexOf('}'); j > -1; j = mixedString.LastIndexOf("}", j - 1))
                {
                    var jsonProbe = mixedString.Substring(i, j - i + 1);
                    return JsonConvert.DeserializeObject(jsonProbe);
                }
            }
            return null;
        }
    }
}
