using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class Document
    {
      public int Id { get; set; }
      public string title { get; set; }
      public string Date { get; set; }
      public string Dsecription { get; set; }
      public bool ?IsApproved { get; set; }
            [NotMapped]
      public IFormFile File { get; set; }
      public string UploaderRole { get; set; }
      public string FileName { get; set; }
      public string Type { get; set; }
      public int personid { get; set; }
      public Person Person { get; set; }
    }
}
