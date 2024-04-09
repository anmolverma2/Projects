using JobBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Diagnostics;


namespace JobBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Admin()
        {
            return View();
        }

        public JsonResult VerifyAdmin(string username, string password)
        {
            if (username == "Anmol" && password == "anmol123")
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }


        string SQLConnectionString = "Data Source=DESKTOP-FIK5SPH;Initial Catalog=Portal;User ID=anmol;Password=*******;Trust Server Certificate=True";

        // Job List
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult Search(string keyword, string location)
        {
            if (keyword == null)
            {
                keyword = "";
            }
            if (location == null)
            {
                location = "";
            }
            List<JobModel> list = new List<JobModel>();
            SqlConnection conn = new SqlConnection(SQLConnectionString);

            SqlCommand cmd = new SqlCommand("WebApplication_SP.JobList", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@jobtitle", keyword);
            cmd.Parameters.AddWithValue("@location", location);
            cmd.Parameters.AddWithValue("@operationtype", "Select");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    JobModel cut = new JobModel();
                    cut.JobId = dr["JobId"].ToString();
                    cut.JobTitle = dr["JobTitle"].ToString();
                    cut.JobDescription = dr["JobDescription"].ToString();
                    cut.Location = dr["Location"].ToString();
                    cut.Salary = dr["Salary"].ToString();
                    cut.PostedDate = dr["PostedDate"].ToString();
                    cut.ExpiryDate = dr["ExpiryDate"].ToString();
                    cut.CompanyName = dr["CompanyName"].ToString();
                    cut.ContactEmail = dr["ContactEmail"].ToString();
                    cut.IsActive = dr["IsActive"].ToString();
                    cut.IsDeleted = dr["IsDeleted"].ToString();
                    cut.NoOfApplied = dr["NoOfApplied"].ToString();

                    list.Add(cut);
                }
            }
            return Json(list);
        }

        public IActionResult ShowJobsById(string JobId)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);

            SqlCommand cmd = new SqlCommand("WebApplication_SP.ViewData", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@JobId", JobId);
            cmd.Parameters.AddWithValue("@operationtype", "ViewJob");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            JobModel model = new JobModel();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model.JobId = dr["JobId"].ToString();
                    model.JobTitle = dr["JobTitle"].ToString();
                    model.JobDescription = dr["JobDescription"].ToString();
                    model.Location = dr["Location"].ToString();
                    model.Salary = dr["Salary"].ToString();
                    model.ContactEmail = dr["ContactEmail"].ToString();
                    model.CompanyName = dr["CompanyName"].ToString();
                    model.ExpiryDate = dr["ExpiryDate"].ToString();
                    model.PostedDate = dr["PostedDate"].ToString();
                    model.NoOfApplied = dr["NoOfApplied"].ToString();
                    model.IsActive = dr["IsActive"].ToString();


                }
            }
            return View(model);
        }

        // Post Job
        public IActionResult CreateJob()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateJob(JobModel model)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);
            SqlCommand cmd = new SqlCommand("WebApplication_SP.InsertJob", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@JobTitle", model.JobTitle);
            cmd.Parameters.AddWithValue("@JobDescription", model.JobDescription);
            cmd.Parameters.AddWithValue("@Location", model.Location);
            cmd.Parameters.AddWithValue("@Salary", model.Salary);
            cmd.Parameters.AddWithValue("@ExpiryDate", model.ExpiryDate);
            cmd.Parameters.AddWithValue("@CompanyName", model.CompanyName);
            cmd.Parameters.AddWithValue("@ContactEmail", model.ContactEmail);
            cmd.Parameters.AddWithValue("@OperationType", "INSERT");


            conn.Open();
            int res = cmd.ExecuteNonQuery();
            conn.Close();

            string result = "Job created Successfully!!";

            return Json(result);
        }

        [HttpPost]
        public JsonResult DeleteJob(JobModel model)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);
            SqlCommand cmd = new SqlCommand("WebApplication_SP.ViewData", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@JobId", model.JobId);
            cmd.Parameters.AddWithValue("@operationtype", "RemoveJob");

            conn.Open();
            int res = cmd.ExecuteNonQuery();
            conn.Close();

            string result = "Job Removed Successfully!!";

            return Json(result);
        }



        // Applicants List
        public IActionResult Applicants()
        {
            return View();
        }

        public JsonResult SearchApplicant(string Email, string Mobile)
        {
            if (Email == null)
            {
                Email = "";
            }
            if (Mobile == null)
            {
                Mobile = "";
            }
            List<ApplicantModel> list = new List<ApplicantModel>();
            SqlConnection conn = new SqlConnection(SQLConnectionString);

            SqlCommand cmd = new SqlCommand("WebApplication_SP.JobList", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@mobile", Mobile);
            cmd.Parameters.AddWithValue("@email", Email);
            cmd.Parameters.AddWithValue("@operationtype", "Aplicant");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ApplicantModel cut = new ApplicantModel();
                    cut.ApplicantID = dr["ApplicantID"].ToString();
                    cut.JobID = dr["JobID"].ToString();
                    cut.FullName = dr["FullName"].ToString();
                    cut.Email = dr["Email"].ToString();
                    cut.Phone = dr["Phone"].ToString();
                    cut.Age = dr["Age"].ToString();
                    cut.Gender = dr["Gender"].ToString();
                    cut.ResumeFilePath = dr["ResumeFilePath"].ToString();
                    cut.PhotoPath = dr["PhotoPath"].ToString();
                    cut.ApplicationDate = dr["ApplicationDate"].ToString();
                    cut.IsDeleted = dr["IsDeleted"].ToString();
                    cut.CreateDate = dr["CreateDate"].ToString();
                    cut.Qualification = dr["Qualification"].ToString();
                    cut.QualificationType = dr["QualificationType"].ToString();
                    cut.Experience = dr["QualificationType"].ToString();
                    cut.CompanyName = dr["CompanyName"].ToString();

                    list.Add(cut);
                }
            }
            return Json(list);
        }

        public IActionResult ShowApplicantsById(ApplicantModel applicant)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);

            SqlCommand cmd = new SqlCommand("WebApplication_SP.ViewApplicant", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ApplicantID", applicant.ApplicantID);
            cmd.Parameters.AddWithValue("@operationtype", "ViewApplicant");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ApplicantModel model = new ApplicantModel();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model.JobID = dr["JobID"].ToString();
                    model.ApplicantID = dr["ApplicantID"].ToString();
                    model.FullName = dr["FullName"].ToString();
                    model.Email = dr["Email"].ToString();
                    model.Phone = dr["Phone"].ToString();
                    model.Age = dr["Age"].ToString();
                    model.Gender = dr["Gender"].ToString();
                    model.Experience = dr["Experience"].ToString();
                    model.ApplicationDate = dr["ApplicationDate"].ToString();
                    model.ResumeFilePath = dr["ResumeFilePath"].ToString();
                    model.PhotoPath = dr["PhotoPath"].ToString();
                    model.Qualification = dr["Qualification"].ToString();
                    model.QualificationType = dr["QualificationType"].ToString();
                    model.IsDeleted = dr["IsDeleted"].ToString();
                    model.CreateDate = dr["CreateDate"].ToString();

                }
            }
            return View(model);
        }


        public JsonResult ShowResumeById(ApplicantModel applicant)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);

            SqlCommand cmd = new SqlCommand("WebApplication_SP.ViewApplicant", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ApplicantID", applicant.ApplicantID);
            cmd.Parameters.AddWithValue("@operationtype", "ViewApplicant");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ApplicantModel model = new ApplicantModel();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model.JobID = dr["JobID"].ToString();
                    model.ApplicantID = dr["ApplicantID"].ToString();
                    model.FullName = dr["FullName"].ToString();
                    model.Email = dr["Email"].ToString();
                    model.Phone = dr["Phone"].ToString();
                    model.Age = dr["Age"].ToString();
                    model.Gender = dr["Gender"].ToString();
                    model.Experience = dr["Experience"].ToString();
                    model.ApplicationDate = dr["ApplicationDate"].ToString();
                    model.ResumeFilePath = dr["ResumeFilePath"].ToString();
                    model.PhotoPath = dr["PhotoPath"].ToString();
                    model.Qualification = dr["Qualification"].ToString();
                    model.QualificationType = dr["QualificationType"].ToString();
                    model.IsDeleted = dr["IsDeleted"].ToString();
                    model.CreateDate = dr["CreateDate"].ToString();

                }
            }
            return Json(model);
        }



        [HttpPost]
        public JsonResult RejectApplicant(ApplicantModel model)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);
            SqlCommand cmd = new SqlCommand("WebApplication_SP.ViewApplicant", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ApplicantID", model.ApplicantID);
            cmd.Parameters.AddWithValue("@operationtype", "RejectApplicant");

            conn.Open();
            int res = cmd.ExecuteNonQuery();
            conn.Close();

            string result = "Applicant Rejected Successfully!!";

            return Json(result);
        }


        #region Export


        public IActionResult ExportApplicantsToExcel(string email, string mobile)
        {
            if (email == null)
            {
                email = "";

            }
            if (mobile == null)
            {
                mobile = "";
            }
            List<ApplicantModel> applicants = GetFilteredApplicants(email, mobile); // Implement this method to retrieve filtered applicants

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Applicants");
                worksheet.Cells.LoadFromCollection(applicants, true);

                for (int i = 1; i <= applicants.Count; i++)
                {
                    worksheet.Cells[i, 8].Style.Numberformat.Format = "yyyy-mm-dd";
                }

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var excelBytes = package.GetAsByteArray();

                // Generate unique filename with current date and time
                string fileName = $"Applicants_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }


        private List<ApplicantModel> GetFilteredApplicants(string email, string mobile)
        {
            List<ApplicantModel> applicants = new List<ApplicantModel>();
            SqlConnection conn = new SqlConnection(SQLConnectionString);

            SqlCommand cmd = new SqlCommand("WebApplication_SP.JobList", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@mobile", mobile);
            cmd.Parameters.AddWithValue("@operationtype", "Aplicant");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ApplicantModel model = new ApplicantModel();
                    model.JobID = dr["JobID"].ToString();
                    model.ApplicantID = dr["ApplicantID"].ToString();
                    model.FullName = dr["FullName"].ToString();
                    model.Email = dr["Email"].ToString();
                    model.Phone = dr["Phone"].ToString();
                    model.Age = dr["Age"].ToString();
                    model.Gender = dr["Gender"].ToString();
                    model.Experience = dr["Experience"].ToString();
                    model.ApplicationDate = dr["ApplicationDate"].ToString();
                    model.ResumeFilePath = dr["ResumeFilePath"].ToString();
                    model.PhotoPath = dr["PhotoPath"].ToString();
                    model.Qualification = dr["Qualification"].ToString();
                    model.QualificationType = dr["QualificationType"].ToString();
                    model.IsDeleted = dr["IsDeleted"].ToString();
                    model.CreateDate = dr["CreateDate"].ToString();
                    model.CompanyName = dr["CompanyName"].ToString();
                    model.JobTitle = dr["JobTitle"].ToString();

                    applicants.Add(model);
                }
            }
            return applicants;
        }


        #endregion

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
