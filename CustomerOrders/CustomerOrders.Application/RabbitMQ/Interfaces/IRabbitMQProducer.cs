namespace CustomerOrders.Application.RabbitMQ.Interfaces
{
    public interface IRabbitMQProducer
    {
        void PublishOrderMessage(string orderMessage);
    }
}