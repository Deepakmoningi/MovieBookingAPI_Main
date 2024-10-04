using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ITicketRepository ticketRepository, ILogger<TicketsController> logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }


        [Authorize(Roles = "Admin, Member")]
        [HttpPost]
        [Route("booktickets")]
        public async Task<ActionResult> BookTickets([FromBody] Ticket ticket)
        {
            if (ticket.SeatNumbers.Count == ticket.NumberOfTickets)
            {
                _logger.LogInformation($"Booking {ticket.NumberOfTickets} for this {ticket.MovieName} movie");
                var response = await _ticketRepository.BookTickets(ticket);
                if (response == -1)
                {
                    //return BadRequest("Booking failed as there is no requested number of seats");
                    return BadRequest(new
                    {
                        message = "Booking failed as there is no requested number of seats"
                    });
                }
                else if (response == 0)
                {
                    //return BadRequest("You are trying to book already booked seat");
                    return BadRequest(new
                    {
                        message = "You are trying to book already booked seat"
                    });
                }
                return Ok(new 
                { 
                    message= "Tickets Booked Successfully" 
                });
            }
            else
            {
                return BadRequest(new 
                { 
                    message = "Number of tickets is not matching with number of seats" 
                });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("{moviename}/update/{theatrename}")]
        public async Task<ActionResult> UpdateStatus(string moviename,string theatrename)
        {
            _logger.LogInformation($"Updating the status of a {moviename} movie");
            var status = await _ticketRepository.UpdateTicketStatus(moviename, theatrename);
            return Ok(status);
        }

        [Authorize(Roles = "Admin,Member")]
        [HttpGet]
        [Route("{moviename}/getBookingInfo/{theatrename}")]
        public async Task<ActionResult<BookingInformation>> GetBookingInfo(string moviename, string theatrename)
        {
            _logger.LogInformation($"Getting the Booking information for {moviename} - movie in {theatrename} - theatre");
            var response = await _ticketRepository.GetBookingInfo(moviename, theatrename);
            if (response != null)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest($"{moviename} movie name or {theatrename} theatre name doesn't exists");
            }
        }

        [Authorize(Roles = "Admin,Member")]
        [HttpGet]
        [Route("getticketsbyuser/{userId}")]
        public async Task<ActionResult<List<Ticket>>> getTicketDataByUserId(string userId)
        {
            _logger.LogInformation($"Getting the TIckets Booked information by {userId} user");
            var response = await _ticketRepository.getTicketDataByUserId(userId);
            if (response.Count > 0)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(new
                {
                    message = $"{userId} user data not found!"
                });
            }
        }

    }
}
