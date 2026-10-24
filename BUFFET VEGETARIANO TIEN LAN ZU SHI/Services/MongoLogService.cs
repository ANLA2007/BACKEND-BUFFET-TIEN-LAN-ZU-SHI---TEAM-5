using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TIEN_LAN_ZU_SHI.Configurations;
using TIEN_LAN_ZU_SHI.Models;

namespace TIEN_LAN_ZU_SHI.Services;

public interface ILogService
{
    Task InsertLogAsync(SystemLog log);
}

public class MongoLogService : ILogService
{
    private readonly IMongoCollection<SystemLog> _collection;

    public MongoLogService(IOptions<MongoLogSettings> options)
    {
        var settings = options.Value;

        // Timeout corto: si Mongo está caído no nos quedamos esperando 30 s
        var mongoSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
        mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);

        var client = new MongoClient(mongoSettings);
        var database = client.GetDatabase(settings.DatabaseName);
        _collection = database.GetCollection<SystemLog>(settings.CollectionName);

        try
        {
            InicializarIndices(settings.RetentionDays);
        }
        catch (Exception ex)
        {
            // Si Mongo no está disponible al arrancar, la API NO debe caerse
            Console.WriteLine($"[WARNING] No se pudieron crear los índices de MongoDB: {ex.Message}");
        }
    }

    private void InicializarIndices(int retentionDays)
    {
        // 1. Índice TTL (Time-To-Live) para purga automática
        var ttlKeys = Builders<SystemLog>.IndexKeys.Ascending(x => x.Timestamp);
        var ttlOptions = new CreateIndexOptions
        {
            ExpireAfter = TimeSpan.FromDays(retentionDays)
        };

        // 2. Índices de consulta frecuente
        var traceKeys = Builders<SystemLog>.IndexKeys.Ascending(x => x.TraceId);
        var levelKeys = Builders<SystemLog>.IndexKeys.Ascending(x => x.Level);

        _collection.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<SystemLog>(ttlKeys, ttlOptions),
            new CreateIndexModel<SystemLog>(traceKeys),
            new CreateIndexModel<SystemLog>(levelKeys)
        });
    }

    public async Task InsertLogAsync(SystemLog log)
    {
        try
        {
            await _collection.InsertOneAsync(log);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRITICAL] Fallo de persistencia en MongoDB: {ex.Message}");
        }
    }
}