using System.Text;
using RabbitMQ.Client;

namespace Producer
{
    internal class AsyncRabbitMQPublisher
    {
        public static async Task PublishMessageAsync(string queueName, string message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            try
            {
                await channel.QueueDeclareAsync(queue: queueName,
                                                durable: false,     // Queue will survive broker restart
                                                exclusive: false,  // Queue can be accessed by other connections
                                                autoDelete: false, // Queue won't be deleted when connection closes
                                                arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                
                await channel.BasicPublishAsync(exchange: "",
                                                routingKey: queueName,
                                                body: body);

                Console.WriteLine($"Sent message: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing message: {ex.Message}");
                throw;
            }
        }        
    }
}
