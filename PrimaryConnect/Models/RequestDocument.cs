using PrimaryConnect.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Models
{
    public class RequestDocument
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string Date { get; set; }
        public string Dsecription { get; set; }
        public bool ?IsApproved { get; set; }
        public string ParentOrTeacher { get; set; }
        public UsefulFunctions.DeliveryMethode DeliveryMethode { get; set; }
        
        public int personid { get; set; }
        public Person Person { get; set; }
    }
}
