using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;
using System.Drawing;
using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    public static class PhotoUtilsLogic
    {

        public static ImageList ListOfPhotosGeneratorFromAlbums(FacebookObjectCollection<Album> i_Albums)
        {
            ImageList images = new ImageList();
            images.ImageSize = new Size(150, 150);
            foreach (Album album in i_Albums)
            {
                foreach (Photo photo in album.Photos)
                {
                    images.Images.Add(photo.ImageNormal);
                }

            }
            return images;
        }
        public static ImageList ListOfPhotosGenerator(FacebookObjectCollection<Photo> i_PhotoCollection)
        {
            ImageList images = new ImageList();
            images.ImageSize = new Size(70, 70);
           
            foreach (Photo photo in i_PhotoCollection)
            {
                images.Images.Add(photo.ImageNormal);
            }

            return images;
        }
    }
}
