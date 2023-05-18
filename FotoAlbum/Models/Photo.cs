using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FotoAlbum.Models
{
    public class Photo
    {
        [Key] 
        public int Id { get; set; }

        [Required] 
        public string Title { get; set; }

        [Required] 
        public string Description { get; set; }

        [NotMapped] 
        public IFormFile? Image { get; set; }
        [NotMapped] 
        public string ImageEntryBase64 => ImageEntry == null ? "" : "data:image/jpg;base64," + Convert.ToBase64String(ImageEntry.Data);

        public bool? Visible { get; set; }

        public List<Category>? Categories { get; set; }

        public int? ImageEntryId { get; set; }

        public ImageEntry? ImageEntry { get; set; }

        public string? ApplicationUserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

    }
}