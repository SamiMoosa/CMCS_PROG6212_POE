using System.ComponentModel.DataAnnotations;
namespace CMCS_PROG6212_POE.Models  // Add this namespace declaration
{
    public class ClaimModel
    {
        [Key]  // Primary Key
        public int ClaimId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }

        public string Status { get; set; } = "Pending"; // Default status

        // Don't mark this as required
        public string FileName { get; set; }  // FileName will be set after the file is uploaded

        // New properties
        public string LecturerName => $"{FirstName} {LastName}";
        public decimal Amount => HoursWorked * HourlyRate;
    }
}
