using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FotoAlbum.Models
{
    public class Message
    {
        [Key] 
        public int Id { get; set; }

        [Required] 
        public string Email { get; set; }

        [Required] 
        public string Text { get; set; }

    }
}