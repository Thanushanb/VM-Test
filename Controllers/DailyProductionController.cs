using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using DailyProduction.Models;

namespace IbasAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DailyProductionController : ControllerBase
    {
        private readonly TableClient _tableClient;

        public DailyProductionController(IConfiguration config)
        {
            var connString = config["AzureStorage:ConnectionString"];
            Console.WriteLine(connString);
            _tableClient = new TableClient(connString, "Bikes"); // tabellens navn
            _tableClient.CreateIfNotExists();
        }

        [HttpGet]
        public IEnumerable<DailyProductionDTO> Get()
        {
            var results = new List<DailyProductionDTO>();

            // Hent alle rows
            Pageable<TableEntity> entities = _tableClient.Query<TableEntity>();

            foreach (var entity in entities)
            {
                var dto = new DailyProductionDTO
                {
                    Date = DateTime.Parse(entity.RowKey), // RowKey gemmer dato
                    Model = Enum.Parse<BikeModel>(entity.PartitionKey),   // PartitionKey gemmer BikeModel
                    ItemsProduced = entity.GetInt32("itemsProduced") ?? 0 // Felt i tabellen
                };
                Console.WriteLine($"{dto.Date}, {dto.Model}, {dto.ItemsProduced}");
                results.Add(dto);
            }
            
            return results;
        }
    }
}

