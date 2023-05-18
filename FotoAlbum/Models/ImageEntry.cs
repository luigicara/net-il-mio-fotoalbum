using System.ComponentModel.DataAnnotations;

namespace FotoAlbum.Models
{
    public class ImageEntry
    {
        [Key] 
        public int Id { get; set; }

        public byte[] Data { get; set; }

        public List<Photo>? Photos { get; set; }
    }
}
