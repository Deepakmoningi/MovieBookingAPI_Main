namespace MovieBookingAPI.Models
{
    public class BookingInformation
    {
        public int totalTicketsAlloted { get; set; }
        public List<int> bookedTickets { get; set; } = new List<int>();
    }
}
