using Data.Repos.Abs;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize (Roles = "Administrator")]
    [Route ("api/[controller]")]
    [ApiController]
    public class ProductsController: ControllerBase
    {
        private readonly IModelRepository <Product> _productsRepository;

        public ProductsController (IModelRepository<Product> productsRepository)
        {
            _productsRepository = productsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<TaskResult<List<Product>>>> GetProducts ()
        {
            try
            {
                var taskResult = await _productsRepository.GetAll();
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet ("{productId}")]
        public async Task<ActionResult<TaskResult<Product>>> GetProduct (string productId)
        {
            try
            {
                var taskResult = await _productsRepository.Get(productId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPost]
        public async Task<ActionResult<TaskResult<Product>>> PostProduct (Product model)
        {
            try
            {
                var taskResult = await _productsRepository.Create(model);
                return CreatedAtAction (nameof (GetProduct), new { productId = model.ProductId }, taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPut ("{productId}")]
        public async Task<ActionResult<TaskResult<Product>>> PutProduct (string productId, Product model)
        {
            try
            {
                if (productId != model.ProductId)
                    return BadRequest ("ProductId mismatch");

                var taskResult = await _productsRepository.Update(model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpDelete ("{productId}")]
        public async Task<ActionResult<TaskResult<Product>>> DeleteProduct (string productId)
        {
            try
            {
                var taskResult = await _productsRepository.Delete(productId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
