using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using SellerApp.Interfaces;

namespace SellerApp.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ProductController(IOrderCloudClient orderCloudClient, ILogger<ProductController> logger, IMapper mapper, IProductService productService) : ControllerBase
{
    private readonly ILogger<ProductController> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IOrderCloudClient _orderCloudClient = orderCloudClient;
    private readonly IProductService _productService = productService;

    // GET: api/products/{productID}
    [HttpGet("{productID}")]
    public async Task<ActionResult<Product>> GetProduct(string productID)
    {
        var product = await _orderCloudClient.Products.GetAsync(productID);
        return Ok(product);
    }


    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<List<Product>>> ListProducts()
    {
        var products = await _orderCloudClient.Products.ListAsync();
        return Ok(products);
    }

    // POST: api/product
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        // check product if valid

        var newProduct = await _orderCloudClient.Products.CreateAsync(product);
        return CreatedAtRoute("GetProduct", new { productID = newProduct.ID }, newProduct);
    }

    // POST: api/products
    [HttpPost("Upload")]
    public async Task<IActionResult> CreateOrUpdateProductsFromCSV(string filepath)
    {
        try
        {
            var productList = _productService.GetProductsFromCSV(filepath);

            foreach (var product in productList)
            {
                await _productService.CreateOrUpdateAsync(orderCloudClient, product, mapper);
            }

            return Ok("Products uploaded successfully.");

        }
        catch (Exception ex)
        {

            return StatusCode(500, $"An error occurred while uploading the Product: {ex.Message}");

        }
    }

    // PUT: api/products/{productID}
    [HttpPut]
    public async Task<ActionResult<Product>> UpdateProduct(string productID, Product product)
    {
        product.ID = productID;
        var updatedProduct = await _orderCloudClient.Products.SaveAsync(product.ID, product);
        return Ok(updatedProduct);
    }

    // DELETE: api/products/{productID}
    [HttpDelete]
    public async Task<ActionResult> DeleteProduct(string productID)
    {
        /// Delete Product assignement 
        await _orderCloudClient.Products.DeleteAsync(productID);
        return Ok();
    }
}

