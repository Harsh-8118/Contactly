namespace Contactly.Models
{
    public class AddUserInfo
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
