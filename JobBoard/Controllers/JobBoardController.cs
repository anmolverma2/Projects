using Microsoft.AspNetCore.Mvc;
using JobBoard.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;


namespace JobBoard.Controllers
{
    public class JobBoardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public JobBoardController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }
        string SQLConnectionString = "Data Source=DESKTOP-FIK5SPH;Initial Catalog=Portal;User ID=anmol;Password=********;Trust Server Certificate=True";

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

        public IActionResult ApplyJob(string JobTitle,string Company)
        {
            JobModel model = new JobModel();
            model.JobTitle = JobTitle;
            model.CompanyName = Company;
            return View(model);
        }


        [HttpPost]
        public JsonResult ApplyJob(ApplicantModel model)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);
            SqlCommand cmd = new SqlCommand("[WebApplication_SP].[InsertApplicantDetails]", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@JobTitle", model.JobTitle);
            cmd.Parameters.AddWithValue("@FullName", model.FullName);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Phone", model.Phone);
            cmd.Parameters.AddWithValue("@Age", model.Age);
            cmd.Parameters.AddWithValue("@Gender", model.Gender);
            cmd.Parameters.AddWithValue("@ResumeFilePath", model.ResumeFilePath);
            cmd.Parameters.AddWithValue("@Qualification", model.Qualification);
            cmd.Parameters.AddWithValue("@QualificationType", model.QualificationType);
            cmd.Parameters.AddWithValue("@Experience", model.Experience);
            cmd.Parameters.AddWithValue("@Address", model.Address);
            cmd.Parameters.AddWithValue("@OTP", model.OTP);

            cmd.Parameters.AddWithValue("@OperationType", "INSERT");


            conn.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);


            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model.ApplicantID = dr["ApplicantID"].ToString();
                    model.Email = dr["Email"].ToString();
                    model.ApplicationDate = dr["ApplicationDate"].ToString();
                    model.OTP = dr["OTP"].ToString();

                }
            }
            MailModel mail = new MailModel();
            mail.RecieverEmail = model.Email;
            mail.Subject = "Verification OTP";
            mail.Body = "<p>Dear "+model.FullName+",</p>"+
                "<p>Your OTP to verify the job application is <b>"+model.OTP+"</b></p>";
         
            long res = SendMail(mail);


            conn.Close();


            return Json(model);
        }


        [HttpPost]
        public JsonResult VerifyOTP(ApplicantModel model)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);
            SqlCommand cmd = new SqlCommand("[WebApplication_SP].[InsertApplicantDetails]", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@JobTitle", model.JobTitle);
            cmd.Parameters.AddWithValue("@OTP", model.OTP);
            cmd.Parameters.AddWithValue("@ApplicantID", model.ApplicantID);            

            cmd.Parameters.AddWithValue("@OperationType", "Verify");

            conn.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            int result = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    result = Convert.ToInt32(dr["result"].ToString());

                }
            }


            MailModel mail = new MailModel();
            mail.RecieverEmail = model.Email;
            mail.Subject = "Interview Schedule";
            mail.Body = "<p>Dear <b>" + model.FullName + "</b>,</p>" +
                "<p>Your interview for the <b>"+ model.JobTitle +"</b> is scheduled on <b>"+ model.ApplicationDate +"</b> </p>" +
                "<p>Please reach at "+model.CompanyName+" before 3 pm on the selected date.<p>"+
                "<p>Best of luck for the interview</p>";

            long res = SendMail(mail);


            conn.Close();


            return Json(result);
        }


        public long SendMail(MailModel model)
        {
            long IsSuccess = 0;
            try
            {
                string to = model.RecieverEmail;
                string from = model.MyMail;
                string password = model.MyAppPassword;

                using (MailMessage mail = new MailMessage(from, to))
                {
                    mail.Subject = model.Subject;
                    mail.Body = model.Body;
                    mail.IsBodyHtml = true;
                    using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.EnableSsl = true;
                        smtpClient.Credentials = new NetworkCredential(from, password);
                        smtpClient.Send(mail);
                    }

                    IsSuccess = 1;
                }
            }
            catch (Exception ex)
            {
                IsSuccess = -1;
            }

            return IsSuccess;
        }


        [HttpPost]
        public IActionResult SaveResume(IFormFile resume)
        {
            try
            {
                // Check if the uploaded file is not null
                if (resume != null)
                {
                    // Get the path to the 'usersupload' folder
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "UsersUpload");

                    // Create the 'usersupload' folder if it doesn't exist
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // Get the file name
                    string fileName = Path.GetFileName(resume.FileName);

                    // Combine the path to the upload folder with the file name
                    string filePath = Path.Combine(uploadFolder, fileName);

                    // Save the uploaded file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        resume.CopyTo(stream);
                    }

                    // Return a success response
                    return Ok(new { success = true });
                }
                else
                {
                    // Return a bad request response if the uploaded file is null
                    return BadRequest("No file uploaded.");
                }
            }
            catch (Exception ex)
            {
                // Return an error response if an exception occurs during file saving
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        public IActionResult About()
        {
            return View();
        }

    }
}
