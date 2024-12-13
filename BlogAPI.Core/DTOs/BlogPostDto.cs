using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAPI.Core.DTOs
{
    public class BlogPostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }


    public class CreateBlogPostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class UpdateBlogPostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }


}
