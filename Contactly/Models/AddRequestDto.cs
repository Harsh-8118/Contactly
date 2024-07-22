namespace Contactly.Models
{
    public class AddRequestDto
    {
        public required string Name { get; set; }
        public string? Email { get; set; }
        public required string? Phone { get; set; }
        public bool Favorite { get; set; }
        public int UserId { get; set; } // Adjust the data type as needed

    }
}
