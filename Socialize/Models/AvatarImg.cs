using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Socialize.Models
{
    /*
     * DB AvatarImg object
     */
    public class AvatarImg
    {
        // Id of the image
        [Key]
        public int Id { get; set; }
        // Url to the image in the server
        public string ImgUrl { get; set; }
    }
}