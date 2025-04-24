using PrimaryConnect.Data;
using PrimaryConnect.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimaryConnect.Dto
{
    public class RequestDocumentDto
    {
       
        public string title { get; set; }
        
        public string Description { get; set; }
       
        public UsefulFunctions.DeliveryMethode DeliveryMethode { get; set; }

        public RequestDocument ToDocument()
        {
            RequestDocument student = new RequestDocument();
            student.IsApproved = null;
            student.title = title;
            student.Dsecription = Description;
            student.DeliveryMethode = DeliveryMethode;
            return student;
        }
    }
}
