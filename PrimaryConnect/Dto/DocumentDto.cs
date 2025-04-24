using PrimaryConnect.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Dto
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string Description { get; set; }
        public string ? DownloadUrl { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        public Document ToDocument()
        {
            Document student = new Document();
            student.title = title;
            student.Dsecription = Description;
            // student.ParentId = parentId;
            student.File = File;
            return student;
        }
    }
}
