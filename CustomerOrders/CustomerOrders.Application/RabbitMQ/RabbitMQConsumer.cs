using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CustomerOrders.Application.RabbitMQ.Interfaces
{
    public class RabbitMQConsumer
    {
        private readonly string _queueName = "orderQueue";
        private readonly string _hostName = "localhost";

        public void Consume()
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = _hostName };

                // RabbitMQ bağlantısını oluşturuyoruz
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    // Kuyruğu oluşturuyoruz
                    channel.QueueDeclare(queue: _queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    // EventingBasicConsumer ile mesaj alıcıyı oluşturuyoruz
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (sender, ea) =>
                    {
                        var body = ea.Body.ToArray();  // Mesajı al
                        var message = Encoding.UTF8.GetString(body);  // Mesajı string'e dönüştür
                        Console.WriteLine(" [x] Received order message: {0}", message);

                        // Burada gelen mesajı işleyebilirsiniz
                    };

                    // Kuyruktan mesajları alıyoruz
                    channel.BasicConsume(queue: _queueName,
                                         autoAck: true,
                                         consumer: consumer);

                    Console.WriteLine(" [*] Waiting for messages.");
                    Console.ReadLine();  // Sonsuz döngüde mesajları dinliyoruz
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
            }
        }
    }
}
