using System;
using System.Threading;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using log4net;
using Newtonsoft.Json;
using SpiderCore.Extendsions;

namespace SpiderCore.Scheduler
{
    public class ActiveMQScheduler : IScheduler
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ActiveMQScheduler));
        private const string ActiveMQConnectionString = "tcp://127.0.0.1:61616";
        private const string ActiveMQUrlQueue = "com.ljja.spider.url";
        private IConnection _connection;
        private ISession _session;
        private IDestination _destination;
        private IMessageConsumer _messageConsumer;
        private IMessageProducer _messageProducer;
        private const int TimeOut = 2000;

        public ActiveMQScheduler()
        {
            this.Init();
        }

        public bool Push(Request request)
        {
            try
            {
                if (request == null) return false;

                request.UrlHash = request.Url.ToMd5();

                var jsonContent = JsonConvert.SerializeObject(request);

                if (string.IsNullOrEmpty(jsonContent))
                {
                    return false;
                }

                //推送到队列
                IMessage message = _messageProducer.CreateTextMessage(jsonContent);

                _messageProducer.Send(message);

                return true;
            }
            catch (BrokerException brokerException)
            {
                _logger.Error(brokerException);

                Close();

                Init();

                return false;
            }
            catch (Exception exception)
            {
                _logger.Error(exception);

                return false;
            }
        }

        public Request Pop()
        {
            try
            {
                var message = _messageConsumer.Receive(TimeSpan.FromMilliseconds(TimeOut)) as ActiveMQTextMessage;

                if (message == null)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<Request>(message.Text);
            }
            catch (NMSException nmsException)
            {
                _messageConsumer.Close();
                _messageConsumer.Dispose();
                _messageConsumer = null;

                Thread.Sleep(5000);

                _messageConsumer = _session.CreateConsumer(_destination);

                _logger.Error(nmsException);
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
            }

            return null;
        }

        public void Switch()
        {

        }

        private void Init()
        {
            try
            {
                IConnectionFactory factory = new ConnectionFactory(ActiveMQConnectionString);
                _connection = factory.CreateConnection();
                _connection.Start();
                _session = _connection.CreateSession();
                _destination = _session.GetQueue(ActiveMQUrlQueue);
                _messageConsumer = _session.CreateConsumer(_destination);
                _messageProducer = _session.CreateProducer(_destination);
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
            }
        }

        private void Close()
        {
            if (_messageConsumer != null)
            {
                _messageConsumer.Close();
                _messageConsumer.Dispose();
                _messageConsumer = null;
            }

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

        ~ActiveMQScheduler()
        {
            Close();
        }

    }
}
