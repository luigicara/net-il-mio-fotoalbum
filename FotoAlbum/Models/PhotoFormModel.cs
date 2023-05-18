using Microsoft.AspNetCore.Mvc.Rendering;

namespace FotoAlbum.Models
{
    public class PhotoFormModel
    {
        public Photo Photo { get; set; }

        public List<string>? SelectedCategories { get; set; }

        public List<SelectListItem>? Categories { get; set; }
    }
}
