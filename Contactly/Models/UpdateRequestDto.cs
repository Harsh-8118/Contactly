using System.ComponentModel.DataAnnotations;

namespace Contactly.Models
{
    public class UpdateRequestDto
    {
        [Required]
        public required string Name { get; set; }
        public string? Email { get; set; }
        public required string? Phone { get; set; }
        public bool Favorite { get; set; }

    }

}
