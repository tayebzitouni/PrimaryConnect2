namespace PrimaryConnect.Dto
{
    public class MarkDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StudentId { get; set; }
        public TermMarksDto Term1 { get; set; }
        public TermMarksDto Term2 { get; set; }
        public TermMarksDto Term3 { get; set; }
        public double FinalAverage { get; set; }
        public string Remarks { get; set; }
    }
}
