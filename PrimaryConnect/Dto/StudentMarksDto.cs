namespace PrimaryConnect.Dto
{
    public class StudentMarksDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string studentId { get; set; }
        public TermMarksDto term1 { get; set; }
        public TermMarksDto term2 { get; set; }
        public TermMarksDto term3 { get; set; }
        public double finalAverage { get; set; }
        public string remarks { get; set; }
    }
}
