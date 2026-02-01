using System.Collections.Generic;

namespace Application.ViewModels
{
    public class PetrolPriceBuildupListVM
    {
        public string role { get; set; }
        public List<TempMenuHeadingsVM> ListMenu { get; set; }
        public List<TempPhotoGalleryVM> Tempphotogallery { get; set; }
        public List<TempVideoVM> TempvdoVM { get; set; }
        public List<TempNewsVM> NewsList { get; set; }
        public List<TempOtherLinkHeadingVM> ListProduct { get; set; }

        //Added By Ankit 11.11.2022
        public List<TempBannerVM> TempbannersVM { get; set; }

        //Added By Ankit 22.11.2022
        public List<TempMediaVM> tempmedia { get; set; }
        public List<TempWhatsNewVM> TempWhatsNewList { get; set; }

    }
}