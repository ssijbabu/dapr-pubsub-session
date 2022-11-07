using System;
using Dapr.Client;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;

using var client = new DaprClientBuilder().Build();

int orders = 10;
int messagePerSession = 4;

var taskList = new List<Task>();

for (int orderId = 0; orderId < orders; orderId++)
{
    var orderNumber = $"OrderId-{orderId.ToString()}";
    
    taskList.Add(Task.Run(async() => {
        for (int m = 0; m < messagePerSession; m++)
        {
            var status = string.Empty;
            switch (m)
            {
                case 0:
                    status = "1 - Ordered";
                    break;
                case 1:
                    status = "2 - Picked";
                    break;
                case 2:
                    status = "3 - Packaged";
                    break;
                case 3:
                    status = "4 - Dispatched";
                    break;
            }
            var order = new Order(orderNumber, status);
            await client.PublishEventAsync("orderpubsub", "orders", order, new Dictionary<string, string> { { "SessionId", orderNumber } });
            Console.WriteLine("Published data: " + order);

            // Random random = new Random();
            // int randNumb = random.Next(1, 10);
            // await Task.Delay(TimeSpan.FromSeconds(randNumb));
        }        
    }));
}

Console.WriteLine("Sending all messages...");
await Task.WhenAll(taskList);
Console.WriteLine("All messages sent.");

public record Order([property: JsonPropertyName("orderId")] string OrderId, [property: JsonPropertyName("status")] string status);
