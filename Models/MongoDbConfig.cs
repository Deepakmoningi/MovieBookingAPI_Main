using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Models
{
    public class MongoDbConfig : IMongoDbConfig
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string MovieCollectionName { get; set; } = string.Empty;
        public string TicketCollectionName { get; set; } = string.Empty;

    }
}
