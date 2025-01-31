using Contracts.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Worker
{
    public class OrderCreatedNotificationConsumer : IConsumer<OrderCreated>
    {
        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            await Task.Delay(1000);
            Console.WriteLine($"Notifications.Worker - OrderCreatedNotificationConsumer- {context.Message.OrderId}");
            
        }
    }
}
