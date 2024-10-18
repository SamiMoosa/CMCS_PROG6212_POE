

namespace CMCS_PROG6212_POE.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AcademicId { get; set; }
        public string Role { get; set; } // Either "Program Coordinator" or "Academic Manager"
    }
}