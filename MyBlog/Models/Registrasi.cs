using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models
{
    public class Registrasi
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Name { get; set; }
        [Key]
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
