namespace BookingApi.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}