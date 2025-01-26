using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using OrderCloud.SDK;
using SellerApp.Interfaces;
using SellerApp.Options;
using System.Globalization;

namespace SellerApp.Services
{
    public class CatalogService : ICatalogService
    {

        public async Task CreateOrUpdateAsync(IOrderCloudClient orderCloudClient, Catalog catalog, IMapper mapper)
        {
            Catalog? existingCatalog = null;


            // To new function 
            existingCatalog = await GetExistingCatalogAsync(orderCloudClient, catalog, existingCatalog);

            var partialCatalog = mapper.Map<PartialCatalog>(existingCatalog);

            if (existingCatalog != null)
            {
                await orderCloudClient.Catalogs.PatchAsync(existingCatalog.ID, partialCatalog);
            }
            else
            {
                try
                {
                    await orderCloudClient.Catalogs.CreateAsync(catalog);

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private static async Task<Catalog?> GetExistingCatalogAsync(IOrderCloudClient orderCloudClient, Catalog catalog, Catalog? existingCatalog)
        {
            try
            {
                existingCatalog = await orderCloudClient.Catalogs.GetAsync(catalog.ID);
            }
            catch (Exception)
            {
                return null;
            }

            return existingCatalog;
        }

        public List<Dictionary<string, string>> ConvertCsvToJson(string csvFilePath)
        {
            List<Dictionary<string, string>> jsonData = [];

            using (var reader = new StreamReader(csvFilePath))
            {
                string line;
                string[]? headers = reader.ReadLine()?.Split(',');

                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    Dictionary<string, string> rowData = [];

                    for (int i = 0; i < headers.Length && i < values.Length; i++)
                    {
                        rowData[headers[i]] = values[i];
                    }

                    jsonData.Add(rowData);
                }
            }

            return jsonData;
        }

        public async Task CreateOrUpdate(IOrderCloudClient orderCloudClient, IMapper mapper, IFormFile file)
        {
            var stream = file.OpenReadStream();
            var reader = new StreamReader(stream);
            var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            var catalogItems = csvReader.GetRecords<CatalogItemCsvModel>().ToList();

            //Check the OwnerID if Exist

            // Convert CSV to JSON to get the XP as Json Object
            // _catalogService.ConvertCsvToJson("C:\\Users\\hhajet\\Downloads\\convertcsv.csv");

            foreach (var item in catalogItems)
            {
                var partialCatalog = mapper.Map<Catalog>(item);
                await CreateOrUpdateAsync(orderCloudClient, partialCatalog, mapper);
            }
        }

    }
}
