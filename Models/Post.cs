using System.ComponentModel;
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

        [DefaultValue("poat.jfif")]
        public string? postImage { get; set; }

        [JsonIgnore]
        public string Src =>
            // $"~/assets/img/{Id}/uploads/{postImage}";
            $"~/assests/img/uploads/{postImage}";
    }
}
