namespace JobBoard.Models
{
    public class JobModel
    {

        public string JobId { get; set; }

        public string JobTitle { get; set; }

        public string JobDescription { get; set; }

        public string Location { get; set; }

        public string Salary { get; set; }

        public string ExpiryDate { get; set; }

        public string PostedDate { get; set; }

        public string CompanyName { get; set; }

        public string ContactEmail { get; set; }

        public string IsActive { get; set; }

        public string IsDeleted { get; set; }

        public string NoOfApplied { get; set; }
    }
}
