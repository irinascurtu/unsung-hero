using API.Models;
using AutoMapper;
using Contracts.Response;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Orders.Domain.Entities;
using Orders.Service;
using OrdersApi.Service.Clients;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IProductStockServiceClient productStockService;
        private readonly IOrderService orderService;
        private readonly IMapper mapper;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly IRequestClient<VerifyOrder> requestClient;
        public OrdersController(IProductStockServiceClient productStockService,
            IOrderService orderService,
            ISendEndpointProvider sendEndpointProvider,
            IRequestClient<VerifyOrder> requestClient,
            IPublishEndpoint publishEndpoint,
            IMapper mapper)
        {
            this.productStockService = productStockService;
            this.orderService = orderService;
            this.sendEndpointProvider = sendEndpointProvider;
            this.requestClient = requestClient;
            this.publishEndpoint = publishEndpoint;
            this.mapper = mapper;
        }


        //// GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            // request/reply
            var order = await orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        /// GET: api/Orders/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetOrder(int id)
        //{
        //    var response = await requestClient.GetResponse<OrderResult, OrderNotFoundResult>(
        //        new Contracts.Response.VerifyOrder { Id = id });

        //    if (response.Is(out Response<OrderResult> incomingMessage))
        //    {
        //        return Ok(incomingMessage.Message);
        //    }

        //    if (response.Is(out Response<OrderNotFoundResult> notfound))
        //    {
        //        return NotFound(notfound.Message);
        //    }

        //    return BadRequest();
        //}


        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderModel model)
        {
            // Transform
            var orderToAdd = mapper.Map<Order>(model);

            //check stock
          //  var stock = await productStockService.GetStock(model.OrderItems.Select(x => x.ProductId).ToList());
            //save & Move on
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-order-command"));
            await sendEndpoint.Send(model);
            var createdOrder = await orderService.AddOrderAsync(orderToAdd);
            return CreatedAtAction("GetOrder", new { id = createdOrder.Id }, createdOrder);
        }

    }
}
