namespace SIMS.Models
{
    public class Programme
    {
        public int ProgrammeID { get; set; }
        public string ProgrammeCode { get; set; } = "";
        public string ProgrammeName { get; set; } = "";
        public int DurationYears { get; set; }
    }
}