using MongoDB.Driver;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;

namespace MovieBookingAPI.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Movie> _movies;
        private readonly IMongoCollection<Ticket> _tickets;
        private readonly ILogger<TicketRepository> _logger;

        public TicketRepository(IMongoDbConfig config, IMongoClient mongoClient, ILogger<TicketRepository> logger)
        {
            var database = mongoClient.GetDatabase(config.DatabaseName);
            _movies = database.GetCollection<Movie>(config.MovieCollectionName);
            _tickets = database.GetCollection<Ticket>(config.TicketCollectionName);
            _logger = logger;
        }
        public async Task<int> BookTickets(Ticket ticket)
        {
            var allotedTickets = await getTotalTickets(ticket.MovieName,ticket.TheatreName);
            var bookedSeats = await getBookedSeats(ticket.MovieName, ticket.TheatreName);
            var totalAvailableTickets = allotedTickets - bookedSeats.Count;
            if (totalAvailableTickets < ticket.NumberOfTickets)
            {
                return -1;
            }
            else
            {
                foreach(int i in ticket.SeatNumbers)
                {
                    if(bookedSeats.Contains(i))
                    {
                        return 0;
                    }
                }
                _logger.LogInformation($"Booking ticket/tickets for this {ticket.MovieName} movie");
                await _tickets.InsertOneAsync(ticket);
                var userRequestedTickets = ticket.SeatNumbers.Count;
                var availableSeats = _movies.Find(m => m.MovieName.ToLower() == ticket.MovieName.ToLower() && m.TheatreName.ToLower() == ticket.TheatreName.ToLower()).SingleOrDefault().AvailableSeats;
                var updatedTickets = availableSeats - userRequestedTickets;

                var movie = _movies.Find(m => m.MovieName.ToLower() == ticket.MovieName.ToLower() && m.TheatreName.ToLower() == ticket.TheatreName.ToLower()).FirstOrDefault();

                //var movieObj = new Movie()
                //{
                //    Id= movie.Id,
                //    MovieName= movie.MovieName,
                //    TheatreName= movie.TheatreName,
                //    TotalTicketsAlloted=movie.TotalTicketsAlloted,
                //    AvailableSeats=updatedTickets
                     
                //};

                var filterDefinition = Builders<Movie>.Filter.Eq(m => m.Id, movie.Id);
                var updateDefinition = Builders<Movie>.Update.Set(m => m.AvailableSeats, updatedTickets);
                //_movies.ReplaceOne(m => m.MovieName.ToLower() == ticket.MovieName.ToLower() && m.TheatreName.ToLower() == ticket.TheatreName.ToLower(), movieObj);
                _logger.LogInformation($"updating the available seats for {ticket.MovieName} movie");
                await _movies.UpdateOneAsync(filterDefinition,updateDefinition);
                return 1;
            }
            
        }

        public async Task<string> UpdateTicketStatus(string moviename,string theatrename)
        {
            var bookedSeats = await getBookedSeats(moviename,theatrename);
            var totalTickets = await getTotalTickets(moviename, theatrename);
            _logger.LogInformation($"Updating booking status for this {moviename} movie");
            if(totalTickets-bookedSeats.Count> 0)
            {
                return "BOOK ASAP";
            }
            else
            {
                return "SOLD OUT";
            }
        }

        public async Task<int> getTotalTickets(string moviename,string theatrename)
        {
            _logger.LogInformation($"getting the total tickets allocated for this {moviename} movie");
            var movie = await _movies.FindAsync(m => m.MovieName.ToLower() == moviename.ToLower() && m.TheatreName.ToLower() == theatrename.ToLower());
            var totalTicketsAlloted = movie.FirstOrDefault().TotalTicketsAlloted;
            return totalTicketsAlloted;

        }
        public async Task<List<int>> getBookedSeats(string moviename, string theatrename)
        {
            _logger.LogInformation($"getting the tickets that are booked for this {moviename} movie");
            var tickets = await _tickets.Find(t => t.MovieName.ToLower() == moviename.ToLower() && t.TheatreName.ToLower() == theatrename.ToLower()).ToListAsync();

            var bookedSeats= new List<int>();
            foreach(var t in tickets)
            {
                bookedSeats.AddRange(t.SeatNumbers);
            }

            return bookedSeats;
        }

        public async Task<BookingInformation> GetBookingInfo(string moviename, string theatrename)
        {
            var result = new BookingInformation
            {
                totalTicketsAlloted = await getTotalTickets(moviename,theatrename),
                bookedTickets = await getBookedSeats(moviename,theatrename)
            };

            return result;
        }

        public async Task<List<Ticket>> getTicketDataByUserId(string userId)
        {
            return await _tickets.Find(ticket => ticket.LoginId== userId).ToListAsync();
        }
    }
}
