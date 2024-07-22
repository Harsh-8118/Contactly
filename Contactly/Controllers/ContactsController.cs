using Contactly.Data;
using Contactly.Models;
using Contactly.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Contactly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ContactlyDbContext dbContext;

        public ContactsController(ContactlyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllContacts()
        {
            var contacts = dbContext.Contacts.ToList();
            return Ok(contacts);
        }

        [HttpGet("/userData")]
        public IActionResult GetContacts([FromQuery] int userId)
        {
            var contacts = dbContext.Contacts.Where(c => c.UserId == userId).ToList();
            return Ok(contacts);
        }

        [HttpPost]
        public IActionResult AddContact(AddRequestDto request)
        {
            var domainModelContact = new Contact
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Favorite = request.Favorite,
                UserId = request.UserId
            };

            dbContext.Contacts.Add(domainModelContact);
            dbContext.SaveChanges();

            return Ok(domainModelContact);
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateContact(Guid id, UpdateRequestDto request)
        {
            var existingContact = dbContext.Contacts.Find(id);

            if (existingContact == null)
            {
                return NotFound();
            }

            // Update the properties of existing contact based on the request
            existingContact.Name = request.Name;
            existingContact.Email = request.Email;
            existingContact.Phone = request.Phone;
            existingContact.Favorite = request.Favorite;

            dbContext.SaveChanges();

            return Ok(existingContact);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteContact(Guid id)
        {
            var contact = dbContext.Contacts.Find(id);

            if (contact is not null)
            {
                dbContext.Contacts.Remove(contact);
                dbContext.SaveChanges();
            }

            return Ok(contact);
        }
    }
}
