using Microsoft.AspNetCore.Identity;

namespace FotoAlbum.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Photo>? Photos { get; set; }
    }
}