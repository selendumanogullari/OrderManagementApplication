using CustomerOrders.Application.Product.Commands;
using CustomerOrders.Application.Product.Responses;
using CustomerOrders.Core.Entities;
using CustomerOrders.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Redis.Interfaces;

namespace CustomerOrders.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    /// <summary>
    /// API controller for managing  products. Allows retrieving, updating, and managing products.
    /// </summary>

    //[Authorize]
    public class ProductsController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly DatabaseContext _context;
        private readonly ICacheService _cacheService;
        public ProductsController(IMediator mediator, DatabaseContext context, ICacheService cacheService)
        {
            _mediator = mediator;
            _context = context;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Retrieves all products from the system.
        /// </summary>
        /// <returns>A list of all available products.</returns>
        /// <response code="200">Products retrieved successfully.</response>
        /// <response code="500">An internal server error occurred.</response>

        [HttpGet("GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Core.Entities.Product>))]
        public async Task<ActionResult<IEnumerable<Core.Entities.Product>>> GetAllProducts()
        {
            try
            {
                var cacheKey = $"products";
                string serializedOrders;
                List<Core.Entities.Product> products;

                var redisOrders = await _cacheService.GetCacheAsync(cacheKey);

                if (!string.IsNullOrEmpty(redisOrders))
                {
                    products = JsonConvert.DeserializeObject<List<Core.Entities.Product>>(redisOrders);
                }
                else
                {
                    products = await _context.Products.ToListAsync();
                    var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                    serializedOrders = JsonConvert.SerializeObject(products, settings);

                    await _cacheService.SetCacheAsync(cacheKey, serializedOrders, TimeSpan.FromMinutes(5));
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Creates a new product in the system.
        /// </summary>
        /// <param name="command">The command containing the details of the product to be created.</param>
        /// <returns>The response containing details of the created product.</returns>
        /// <response code="200">Product created successfully.</response>
        /// <response code="400">Invalid request or missing product details.</response>
        /// <response code="500">An internal server error occurred.</response>

        [HttpPost("CreateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        /// <summary>
        /// Updates the details of an existing product in the system.
        /// </summary>
        /// <param name="command">The command containing the updated details of the product.</param>
        /// <returns>The response containing details of the updated product.</returns>
        /// <response code="200">Product updated successfully.</response>
        /// <response code="404">Product not found.</response>
        /// <response code="400">Invalid request or missing product details.</response>
        /// <response code="500">An internal server error occurred.</response>

        [HttpPost("UpdateProduct")]
        public async Task<ActionResult<UpdateProductResponse>> UpdateProducts([FromBody] UpdateProductCommand command)
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
        /// Deletes a product from the system by its ID.
        /// </summary>
        /// <param name="productId">The unique identifier of the product to be deleted.</param>
        /// <returns>A boolean indicating whether the product was successfully deleted or not.</returns>
        /// <response code="200">Product deleted successfully.</response>
        /// <response code="404">Product not found.</response>
        /// <response code="500">An internal server error occurred.</response>

        [HttpDelete("DeleteProduct")]
        public async Task<ActionResult<bool>> DeleteProduct(int productId)
        {
            try
            {
                var product = await _context.Products
                 .Where(co => co.Id == productId)
                 .FirstOrDefaultAsync();

                if (product == null)
                {
                    return NotFound("Product not found.");
                }

                _context.Products.Remove(product);
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
