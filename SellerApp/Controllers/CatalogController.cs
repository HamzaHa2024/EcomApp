using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using SellerApp.Interfaces;

namespace SellerApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogController(IOrderCloudClient orderCloudClient, IMapper mapper, ILogger<CatalogController> logger, ICatalogService catalogService) : ControllerBase
{
    private readonly ILogger<CatalogController> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IOrderCloudClient _orderCloudClient = orderCloudClient;
    private readonly ICatalogService _catalogService = catalogService;


    /// <summary>
    /// GET a list of Catalogs.
    /// </summary>
    //[OrderCloudUserAuth(ApiRole.CatalogAdmin, ApiRole.CatalogReader)]
    [HttpGet]
    public async Task<ActionResult<ListPage<Catalog>>> GetCatalogs()
    {
        var catalogs = await _orderCloudClient.Catalogs.ListAsync();
        return Ok(catalogs);
    }

    /// <summary>
    /// GET a Catalog by Id.
    /// </summary>
    // GET: api/Catalog/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Catalog>> GetCatalog(string id)
    {
        var catalog = await _orderCloudClient.Catalogs.GetAsync(id);
        return Ok(catalog);
    }

    // POST: api/Catalog
    [HttpPost]
    public async Task<ActionResult<Catalog>> CreateCatalog([FromBody] Catalog catalog)
    {

        var createdCatalog = await _orderCloudClient.Catalogs.CreateAsync(catalog);

        return CreatedAtAction(nameof(GetCatalog), new { id = createdCatalog.ID }, createdCatalog);
    }

    // PUT: api/Catalog/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<Catalog>> UpdateCatalog(string id, [FromBody] Catalog catalog)
    {
        var updatedCatalog = await _orderCloudClient.Catalogs.SaveAsync(id, catalog);
        return Ok(updatedCatalog);
    }

    // DELETE: api/Catalog/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCatalog(string id)
    {
        // Delete Catalog assignement (Products, Owners)

        await _orderCloudClient.Catalogs.DeleteAsync(id);
        return NoContent();
    }


    /// <summary>
    /// Upload a CSV file to create or update Catalog items.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost("Upload")]
    public async Task<IActionResult> UploadCatalogs(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file selected for upload.");
        }

        try
        {
            await _catalogService.CreateOrUpdate(_orderCloudClient, mapper, file);
            return Ok("Catalog items uploaded successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while uploading the catalog: {ex.Message}");
        }
    }

}
