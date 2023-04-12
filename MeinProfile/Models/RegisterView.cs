using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MeinProfile.Models
{
    public class RegisterView
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public IFormFile ImagePath { get; set; }
    }
}
