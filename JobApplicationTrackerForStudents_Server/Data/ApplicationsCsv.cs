using System.ComponentModel.DataAnnotations;

namespace JobApplicationTrackerForStudents_Server.Data
{
    public class ApplicationsCsv
    {
        public int jobId { get; set; }
        public string position { get; set; } = null!;
        public string company { get; set; } = null!;
        public DateTime date { get; set; }
        public int statusId { get; set; }
        public StudentCsv studentcsv { get; set; }        
    }
}
