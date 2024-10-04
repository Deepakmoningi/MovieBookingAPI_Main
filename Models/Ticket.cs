using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MovieBookingAPI.Models
{
    [BsonIgnoreExtraElements]
    public class Ticket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("LoginId")]
        public string LoginId { get; set; } = string.Empty;

        [BsonElement("moviename")]
        public string MovieName { get; set; } = string.Empty;


        [BsonElement("theatrename")]
        public string TheatreName { get; set; } = string.Empty;

        [BsonElement("numberoftickets")]
        [BsonRepresentation(BsonType.Int32)]
        public int NumberOfTickets { get; set; }

        [BsonElement("seatnumbers")]
        [BsonRepresentation(BsonType.Int32,AllowOverflow =true)]
        public List<int> SeatNumbers { get; set; } = new List<int>();
    }
}
