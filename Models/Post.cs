using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace HtmxBlog.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [SwaggerParameter(Description = "Property description", Required = false)]
        public string? Content { get; set; }

        [DefaultValue("poat.jfif")]
        public string? postImage { get; set; }

        [JsonIgnore]
        public string Src => $"~/assests/img/uploads/{postImage}";
    }
}
