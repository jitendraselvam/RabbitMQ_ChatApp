using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace user1
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { DispatchConsumersAsync = true, HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //Queue for posting messages
                    channel.QueueDeclare(queue: "queue1",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    //Queue for receiving messages
                    channel.QueueDeclare(queue: "queue2",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;
                    channel.BasicConsume(queue: "queue2",
                        autoAck: true,
                        consumer: consumer);

                    while (true)
                    {
                        Console.WriteLine("Message sent: ");
                        string message = Console.ReadLine();

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                            routingKey: "queue1",
                            basicProperties: null,
                            body: body);
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronous method for consuming the messages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        private static async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            var message = Encoding.UTF8.GetString(@event.Body);

            Console.WriteLine($"Message obtained: {message}");

            await Task.Delay(250);

        }
    }
}