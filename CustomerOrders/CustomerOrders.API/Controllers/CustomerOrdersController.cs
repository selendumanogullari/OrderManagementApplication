using CustomerOrders.API.Controllers;
using CustomerOrders.Application.CustomerOrder.Commands;
using CustomerOrders.Application.CustomerOrder.Responses;
using CustomerOrders.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Redis.Interfaces;

namespace Customer.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    /// <summary>
    /// API controller for managing customer orders. Allows retrieving, updating, and managing customer orders.
    /// </summary>

    [Authorize]
    public class CustomerOrdersController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly DatabaseContext _context;
        private readonly ICacheService _cacheService;
        public CustomerOrdersController(IMediator mediator, DatabaseContext context, ICacheService cacheService)
        {
            _mediator = mediator;
            _context = context;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Retrieves all orders for a specific customer by their name. The result is first checked in the cache (Redis), and if not found, it fetches the data from the database and caches it for future requests.
        /// </summary>
        /// <param name="customerName">The name of the customer whose orders are to be retrieved.</param>
        /// <returns>A list of customer orders, or a 404 status if no orders are found for the customer.</returns>
        /// <response code="200">Returns the list of orders for the customer.</response>
        /// <response code="500">If an error occurs during the retrieval of the data.</response>
        [HttpGet("GetAllCustomersOrders")]

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CustomerOrders.Core.Entities.CustomerOrders>))]

        public async Task<ActionResult<IEnumerable<CustomerOrders.Core.Entities.CustomerOrders>>> GetAllCustomersOrders(string customerName)
        {
            try
            {
                var cacheKey = $"customerOrders_{customerName}";
                string serializedOrders;
                List<CustomerOrders.Core.Entities.CustomerOrders> customerOrders;

                var redisOrders = await _cacheService.GetCacheAsync(cacheKey);

                if (!string.IsNullOrEmpty(redisOrders))
                {
                    customerOrders = JsonConvert.DeserializeObject<List<CustomerOrders.Core.Entities.CustomerOrders>>(redisOrders);
                }
                else
                {
                    customerOrders = await _context.CustomerOrders
                        .Include(co => co.Customer)
                        .Include(co => co.OrderItems)
                        .Where(co => co.Customer.Name == customerName)
                        .ToListAsync();

                    var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                    serializedOrders = JsonConvert.SerializeObject(customerOrders, settings);

                    await _cacheService.SetCacheAsync(cacheKey, serializedOrders, TimeSpan.FromMinutes(5));
                }
                return Ok(customerOrders);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Creates a new customer order.
        /// </summary>
        /// <param name="command">The customer order to create.</param>
        /// <returns>The created customer order.</returns>
        [HttpPost("CreateCustomerOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateCustomerOrdersResponse>> CreateCustomerOrders([FromBody] CreateCustomerOrdersCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Updates the details of a customer's order based on the provided command.
        /// If the order is not found, a 404 Not Found response is returned.
        /// </summary>
        /// <param name="command">The command containing the updated order information.</param>
        /// <returns>A response with the updated order details or a 404 status if the order is not found.</returns>
        /// <response code="200">Returns the updated order details.</response>
        /// <response code="404">If the order was not found for the provided information.</response>
        [HttpPost("UpdateCustomerOrders")]
        public async Task<ActionResult<UpdateCustomerOrdersResponse>> UpdateCustomerOrders([FromBody] UpdateCustomerOrdersCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                if (result == null)
                {
                    return NotFound("Order not found.");
                }

                return Ok(result);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Deletes an existing customer order by ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer order to delete.</param>
        /// <param name="customerOrderId">The ID of the customers order to delete.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("DeleteCustomerOrder")]
        public async Task<ActionResult<bool>> DeleteCustomerOrder(int customerId, int customerOrderId)
        {
            try
            {
                var customerOrder = await _context.CustomerOrders
                 .Include(co => co.Customer)
                 .Include(co => co.OrderItems)
                 .Where(co => co.Customer.Id == customerId && co.OrderItems.Any(m => m.CustomerOrderId == customerOrderId))
                 .FirstOrDefaultAsync();

                if (customerOrder == null)
                {
                    return NotFound("Order not found.");
                }

                _context.CustomerOrders.Remove(customerOrder);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}