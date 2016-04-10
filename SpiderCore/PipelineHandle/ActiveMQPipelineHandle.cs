using System;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using log4net;
using Newtonsoft.Json;

namespace SpiderCore.PipelineHandle
{
    public class ActiveMQPipelineHandle : IPipelineHandle
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ActiveMQPipelineHandle));
        private const string ActiveMQConnectionString = "tcp://127.0.0.1:61616";
        private const string ActiveMQHtmlQueue = "com.ljja.spider.html";
        private IConnection _connection;
        private ISession _session;
        private IDestination _destination;
        private IMessageProducer _messageProducer;

        public string FilterPath
        {
            get
            {
                return "";
            }
        }

        public ActiveMQPipelineHandle()
        {
            Init();
        }

        public void Extract(MessageContext message)
        {
            try
            {
                //发送到MQ队列

                if (message == null) return;
                
                var jsonContent = JsonConvert.SerializeObject(message);

                if (string.IsNullOrEmpty(jsonContent))
                {
                    return;
                }

                IMessage activemqMessage = _messageProducer.CreateTextMessage(jsonContent);

                _messageProducer.Send(activemqMessage);
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
            }

        }

        private void Init()
        {
            try
            {
                IConnectionFactory factory = new ConnectionFactory(ActiveMQConnectionString);
                _connection = factory.CreateConnection();
                _connection.Start();
                _session = _connection.CreateSession();
                _destination = _session.GetQueue(ActiveMQHtmlQueue);
                _messageProducer = _session.CreateProducer(_destination);
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
            }
        }

        private void Close()
        {
            if (_messageProducer != null)
            {
                _messageProducer.Close();
                _messageProducer.Dispose();
                _messageProducer = null;
            }

            if (_session != null)
            {
                _session.Close();
                _session.Dispose();
                _session = null;
            }

            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        ~ActiveMQPipelineHandle()
        {
            this.Close();
        }
    }
}
