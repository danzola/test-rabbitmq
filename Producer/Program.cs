using Producer;

await AsyncRabbitMQPublisher.PublishMessageAsync("BasicTest", $"Hello, RabbitMQ! {DateTime.Now}");