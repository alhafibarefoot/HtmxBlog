using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HtmxBlog.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]

        public string? Title { get; set; }


        public string? Content { get; set; }

        public string? postImage { get; set; }



    }
}
