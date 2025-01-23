using RabbitMQ.Client;
using System.Text;

namespace CustomerOrders.Application.RabbitMQ.Interfaces
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly string _hostName = "localhost";  // RabbitMQ sunucu adresi
        private readonly string _queueName = "orderQueue";  // Kullanılacak kuyruk adı

        public void PublishOrderMessage(string orderMessage)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = _hostName };
                factory.Uri = new Uri("amqps://inggfgpc:AZ1qC8UyCg5up82NsNeALVy-Fl0GokBp@whale.rmq.cloudamqp.com/inggfgpc");

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var body = Encoding.UTF8.GetBytes(orderMessage);

                    channel.BasicPublish(exchange: "",
                                         routingKey: _queueName,
                                         basicProperties: null,
                                         body: body);

                    Console.WriteLine(" [x] Sent order message: {0}", orderMessage);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
