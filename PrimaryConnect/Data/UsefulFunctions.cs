using ClosedXML.Excel;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ClosedXML.Excel;

namespace PrimaryConnect.Data
{
    public class UsefulFunctions
    {



        public enum DeliveryMethode
        {
            Email = 1, Pickup = 2
        }

        static public string GenerateandSendKey(string Email)
        {
            string fMail = "";

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(Email);
            mailMessage.Subject = "Confermation key";
            mailMessage.To.Add(Email);
            int num = Convert.ToInt32(RandomNumberGenerator.Create());
            mailMessage.Body = (num % 1000000).ToString();
            mailMessage.IsBodyHtml = false;
            string szzezz = "";
            SmtpClient smtpClient = new SmtpClient("satp-mail.outlook.com")
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fMail, szzezz)
                ,
                Port = 587

            };
            smtpClient.Send(mailMessage);
            return num.ToString();
        }




        //public static void CreateExcelWithDropdowns(string filePath)
        //{
        //        // Create workbook and worksheets


        //        var wb = new XLWorkbook();

        //        // 1. Create Options Sheet
        //        var wsOptions = wb.AddWorksheet("Options");
        //        wsOptions.Cell("A1").Value = "Parents";
        //        wsOptions.Cell("A2").Value = "John Doe";
        //        wsOptions.Cell("A3").Value = "Jane Smith";

        //        wsOptions.Cell("B1").Value = "Schools";
        //        wsOptions.Cell("B2").Value = "High School A";
        //        wsOptions.Cell("B3").Value = "Elementary B";

        //        wsOptions.Cell("C1").Value = "Classes";
        //        wsOptions.Cell("C2").Value = "Math";
        //        wsOptions.Cell("C3").Value = "Science";

        //        // 2. Create Named Ranges
        //        wb.NamedRanges.Add("ParentList", wsOptions.Range("A2:A3"));
        //        wb.NamedRanges.Add("SchoolList", wsOptions.Range("B2:B3"));
        //        wb.NamedRanges.Add("ClassList", wsOptions.Range("C2:C3"));

        //        // 3. Create Students Sheet
        //        var wsStudents = wb.AddWorksheet("Students");
        //        wsStudents.Cell("A1").Value = "Student";
        //        wsStudents.Cell("B1").Value = "Age";
        //        wsStudents.Cell("C1").Value = "Parent";
        //        wsStudents.Cell("D1").Value = "School";
        //        wsStudents.Cell("E1").Value = "Class";

        //        // 4. Apply Data Validation (corrected version)
        //        wsStudents.Column("C").SetDataValidation().List("ParentList");
        //        wsStudents.Column("D").SetDataValidation().List("SchoolList");
        //        wsStudents.Column("E").SetDataValidation().List("ClassList");

        //        // 5. Save
        //        wb.SaveAs("StudentTemplate_Fixed.xlsx");
        //    }
        public static MemoryStream CreateExcelWithDropdowns()
        {
            var stream = new MemoryStream();
            var wb = new XLWorkbook();

            // Options Sheet
            var wsOptions = wb.AddWorksheet("Options");
            wsOptions.Cell("A1").Value = "Parents";
            wsOptions.Cell("A2").Value = "John Doe";
            wsOptions.Cell("A3").Value = "Jane Smith";

            wsOptions.Cell("B1").Value = "Schools";
            wsOptions.Cell("B2").Value = "High School A";
            wsOptions.Cell("B3").Value = "Elementary B";

            wsOptions.Cell("C1").Value = "Classes";
            wsOptions.Cell("C2").Value = "Math";
            wsOptions.Cell("C3").Value = "Science";

            // Named Ranges
            wb.NamedRanges.Add("ParentList", wsOptions.Range("A2:A3"));
            wb.NamedRanges.Add("SchoolList", wsOptions.Range("B2:B3"));
            wb.NamedRanges.Add("ClassList", wsOptions.Range("C2:C3"));

            // Students Sheet
            var wsStudents = wb.AddWorksheet("Students");
            wsStudents.Cell("A1").Value = "Student";
            wsStudents.Cell("B1").Value = "Age";
            wsStudents.Cell("C1").Value = "Parent";
            wsStudents.Cell("D1").Value = "School";
            wsStudents.Cell("E1").Value = "Class";

            // Data Validation on columns
            wsStudents.Column("C").SetDataValidation().List("ParentList");
            wsStudents.Column("D").SetDataValidation().List("SchoolList");
            wsStudents.Column("E").SetDataValidation().List("ClassList");

            wb.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

    }
}

