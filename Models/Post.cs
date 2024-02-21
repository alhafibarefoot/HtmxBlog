using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HtmxBlog.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [JsonIgnore]
        public string? Title { get; set; }

        [JsonIgnore]
        public string? Content { get; set; }


        public byte[]? Image { get; set; }
    }
}
