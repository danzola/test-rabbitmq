using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    internal static class AsyncRabbitMqSubscriber
    {
        public static async Task ConsumeMessageAsync(string queueName)
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

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += Consumer_ReceivedAsync;

                await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing message: {ex.Message}");
                throw;
            }
        }

        private static async Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
        {
            var body = @event.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received message: {message}");
            await Task.CompletedTask;
        }
    }
}
