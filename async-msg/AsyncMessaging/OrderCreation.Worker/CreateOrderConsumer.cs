using API.Models;
using AutoMapper;
using Contracts.Events;
using MassTransit;
using Orders.Domain.Entities;
using Orders.Service;
using System;
using System.Threading.Tasks;

namespace OrderCreation
{
    public class CreateOrderConsumer : IConsumer<OrderModel>
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public CreateOrderConsumer(IOrderService orderService, IMapper mapper)
        {
            this.mapper = mapper;
            this.orderService = orderService;
        }


        public async Task Consume(ConsumeContext<OrderModel> context)
        {
            
            Console.WriteLine($"OrderCreationWorker. Got a command to create an order:{context.Message.DeliveryInstructions}");
            //mapping from Message to an order object
            var orderToAdd = mapper.Map<Order>(context.Message);

            /// Implement the logic to create an order
            var savedOrder = await orderService.AddOrderAsync(orderToAdd);
            /// send a notification to Admin

            var notifyOrderCreated = context.Publish(new OrderCreated()
            {
                CreatedAt = savedOrder.OrderDate,
                OrderId = savedOrder.Id
            });


            await Task.CompletedTask;
        }
    }
}
