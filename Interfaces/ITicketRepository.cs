using MovieBookingAPI.Models;

namespace MovieBookingAPI.Interfaces
{
    public interface ITicketRepository
    {
        public Task<int> BookTickets(Ticket ticket);

        public Task<string> UpdateTicketStatus(string moviename,string theatrename);

        public Task<BookingInformation> GetBookingInfo(string moviename, string theatrename);

        public Task<List<Ticket>> getTicketDataByUserId(string userId);
    }
}
