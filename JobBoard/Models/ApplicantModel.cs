namespace JobBoard.Models
{
    public class ApplicantModel
    {
        public string ApplicantID { get; set; }
        public string JobID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Age { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string Gender { get; set; }
        public string ResumeFilePath { get; set; }
        public string PhotoPath { get; set; }
        public string Experience { get; set; }
        public string Address { get; set; }
        public string OTP { get; set; }
        public string ApplicationDate { get; set; }
        public string QualificationType { get; set; }
        public string Qualification { get; set; }
        public string CreateDate { get; set; }
        public string IsDeleted { get; set; }
    }
}
