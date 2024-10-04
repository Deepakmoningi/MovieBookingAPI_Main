namespace MovieBookingAPI.Interfaces
{
    public interface IMongoDbConfig
    {
        public string ConnectionString { get;set; }

        public string DatabaseName { get; set; }

        public string MovieCollectionName { get; set; } 

        public string TicketCollectionName { get; set; } 


    }
}
