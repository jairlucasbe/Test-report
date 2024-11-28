using MongoDB.Driver;

namespace seguimiento_expotec.Connection
{
    public class ConnectionDB
    {
        public IMongoDatabase Connect()
        {
            try
            {
                const string connectionUri = "mongodb+srv://grupoupgradeperu:CUhQCqGo3DG7z4sN@upgradedb.vcn6i.mongodb.net";
                var settings = MongoClientSettings.FromConnectionString(connectionUri);
                var client = new MongoClient(settings);
                var database = client.GetDatabase("seguimiento_expotec");
                return database;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
