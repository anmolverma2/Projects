using Hospital_Management_System.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Hospital_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        string SQLConnectionString = "Data Source=********;Initial Catalog=master;Integrated Security=True;Trust Server Certificate=True;";
        [HttpGet]
        public IActionResult CreateUpdatePatient()
        {
            return View();
        }
        [HttpPost]
        public JsonResult CreatePatient(PatientModel patient)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);
            SqlCommand cmd = new SqlCommand("PatientDetail", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PatientName", patient.PatientName);
            cmd.Parameters.AddWithValue("@PatientAddress", patient.PatientAddress);
            cmd.Parameters.AddWithValue("@Gender", patient.Gender);
            cmd.Parameters.AddWithValue("@PatientAge", patient.PatientAge);
            cmd.Parameters.AddWithValue("@PhoneNumber", patient.PhoneNumber);
            cmd.Parameters.AddWithValue("@Email", patient.Email);
            cmd.Parameters.AddWithValue("@EmergencyNumber", patient.EmergentContactNumber);
            cmd.Parameters.AddWithValue("@EmergencyPerson", patient.EmergentContactName);
            cmd.Parameters.AddWithValue("@PatientBloodGroup", patient.PatientBloodGroup);
            cmd.Parameters.AddWithValue("@AppointedDoctor", patient.AppointedDoctor);
            cmd.Parameters.AddWithValue("@OperationType", "INSERT");


            conn.Open();
            int res = cmd.ExecuteNonQuery();
            conn.Close();

            string result = "Data inserted Successfully!!";

            return Json(result);
        }
        [HttpGet]
        public IActionResult PatientList()
        {

            return View();
        }
        public JsonResult Search(string keyword)
        {
            if (keyword == null)
            {
                keyword = "";
            }
            List<PatientModel> SearchedPatient = new List<PatientModel>();
            SqlConnection conn = new SqlConnection(SQLConnectionString);

            SqlCommand cmd = new SqlCommand("select * from dbo.[Hospital_PatientDetail_Sheet] where PatientName like '%" + keyword + "%' and IsDeleted = 0", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PatientModel cut = new PatientModel();
                    cut.PatientId = dr["PatientId"].ToString();
                    cut.PatientName = dr["PatientName"].ToString();
                    cut.PatientAddress = dr["PatientAddress"].ToString();
                    cut.PatientAge = dr["PatientAge"].ToString();
                    cut.PatientBloodGroup = dr["PatientBloodGroup"].ToString();
                    cut.Illness = dr["Illness"].ToString();
                    cut.AppointedDoctor = dr["AppointedDoctor"].ToString();
                    cut.DateOfAdmission = dr["DateOfAdmission"].ToString();
                    cut.DateofBirth = dr["DateOfAdmission"].ToString();
                    cut.DateofDischarge = dr["DateOfAdmission"].ToString();
                    cut.Gender = dr["Gender"].ToString();
                    cut.PhoneNumber = dr["PhoneNumber"].ToString();
                    cut.Email = dr["Email"].ToString();
                    cut.EmergentContactNumber = dr["EmergencyContactNumber"].ToString();
                    cut.EmergentContactName = dr["EmergencyContactName"].ToString();

                    SearchedPatient.Add(cut);
                }
            }
            return Json(SearchedPatient);
        }
        public JsonResult BillDetails(String PatientId)
        {
            List<PatientModel> SearchedPatient = new List<PatientModel>();
            SqlConnection conn = new SqlConnection(SQLConnectionString);

            SqlCommand cmd = new SqlCommand("select * from dbo.[Hospital_PatientDetail_Sheet] where PatientId = "+PatientId+"", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PatientModel cut = new PatientModel();
                    cut.PatientId = dr["PatientId"].ToString();
                    cut.PatientName = dr["PatientName"].ToString();
                    cut.PatientAddress = dr["PatientAddress"].ToString();
                    cut.PatientAge = dr["PatientAge"].ToString();
                    cut.PatientBloodGroup = dr["PatientBloodGroup"].ToString();
                    cut.Illness = dr["Illness"].ToString();
                    cut.AppointedDoctor = dr["AppointedDoctor"].ToString();
                    cut.DateOfAdmission = dr["DateOfAdmission"].ToString();
                    cut.DateofBirth = dr["DateOfAdmission"].ToString();
                    cut.DateofDischarge = dr["DateOfAdmission"].ToString();
                    cut.Gender = dr["Gender"].ToString();
                    cut.PhoneNumber = dr["PhoneNumber"].ToString();
                    cut.Email = dr["Email"].ToString();
                    cut.EmergentContactNumber = dr["EmergencyContactNumber"].ToString();
                    cut.EmergentContactName = dr["EmergencyContactName"].ToString();

                    SearchedPatient.Add(cut);
                }
            }
            return Json(SearchedPatient);
        }
        public JsonResult ShowPatientDetailById(string PatientId)
        {
            List<PatientModel> SearchedPatient = new List<PatientModel>();
            SqlConnection conn = new SqlConnection(SQLConnectionString);

            SqlCommand cmd = new SqlCommand("PatientDetail_ShowById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientId", PatientId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PatientModel cut = new PatientModel();
                    cut.PatientId = dr["PatientId"].ToString();
                    cut.PatientName = dr["PatientName"].ToString();
                    cut.PatientAddress = dr["PatientAddress"].ToString();
                    cut.PatientAge = dr["PatientAge"].ToString();
                    cut.PatientBloodGroup = dr["PatientBloodGroup"].ToString();
                    cut.Illness = dr["Illness"].ToString();
                    cut.AppointedDoctor = dr["AppointedDoctor"].ToString();
                    cut.DateOfAdmission = dr["DateOfAdmission"].ToString();
                    cut.DateofBirth = dr["DateOfAdmission"].ToString();
                    cut.DateofDischarge = dr["DateOfAdmission"].ToString();
                    cut.Gender = dr["Gender"].ToString();
                    cut.PhoneNumber = dr["PhoneNumber"].ToString();
                    cut.Email = dr["Email"].ToString();
                    cut.EmergentContactNumber = dr["EmergencyContactNumber"].ToString();
                    cut.EmergentContactName = dr["EmergencyContactName"].ToString();
                    cut.AppointmentTime = dr["Timing"].ToString();

                    SearchedPatient.Add(cut);
                }
            }
            return Json(SearchedPatient);
        }
        [HttpPost]
        public JsonResult UpdatePatient(PatientModel patient)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);
            SqlCommand cmd = new SqlCommand("PatientDetail_Update", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientId", patient.PatientId);
            cmd.Parameters.AddWithValue("@PatientName", patient.PatientName);
            cmd.Parameters.AddWithValue("@PatientAddress", patient.PatientAddress);
            cmd.Parameters.AddWithValue("@Gender", patient.Gender);
            cmd.Parameters.AddWithValue("@PatientAge", patient.PatientAge);
            cmd.Parameters.AddWithValue("@PhoneNumber", patient.PhoneNumber);
            cmd.Parameters.AddWithValue("@Email", patient.Email);
            cmd.Parameters.AddWithValue("@EmergencyNumber", patient.EmergentContactNumber);
            cmd.Parameters.AddWithValue("@EmergencyPerson", patient.EmergentContactName);
            cmd.Parameters.AddWithValue("@PatientBloodGroup", patient.PatientBloodGroup);
            cmd.Parameters.AddWithValue("@AppointedDoctor", patient.AppointedDoctor);

            conn.Open();
            int res = cmd.ExecuteNonQuery();
            conn.Close();

            string result = "Patient Updated Successfully!!";

            return Json(result);
        }
        [HttpPost]
        public JsonResult UpdateAppointment(PatientModel patient)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);
            SqlCommand cmd = new SqlCommand("dbo.[Appointments_SP]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientId", patient.PatientId);
            cmd.Parameters.AddWithValue("@AppointmentTime", patient.AppointmentTime);

            conn.Open();
            int res = cmd.ExecuteNonQuery();
            conn.Close();

            string result = "Appointment Details Saved Successfully!!";

            return Json(result);
        }
        [HttpPost]
        public JsonResult DeleteAppointment(PatientModel patient)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);
            SqlCommand cmd = new SqlCommand("dbo.[Appointments_SP_Delete]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientId", patient.PatientId);

            conn.Open();
            int res = cmd.ExecuteNonQuery();
            conn.Close();

            string result = "Appointment Details Deleted Successfully!!";

            return Json(result);
        }
        [HttpPost]
        public JsonResult DeletePatient(int PatientId)
        {
            SqlConnection conn = new SqlConnection(SQLConnectionString);
            SqlCommand cmd = new SqlCommand("Delete_SP", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientId", PatientId);
            conn.Open();
            int res = cmd.ExecuteNonQuery();
            conn.Close();


            string result = "Data deleted  Successfully!!";

            return Json(result);

        }
        [HttpGet]
        public IActionResult BillingView()
        {
            return View();
        }
        public IActionResult AppointmentView()
        {
            return View();
        }
        public JsonResult AppointmentList(string keyword)
        {
            if (keyword == null)
            {
                keyword = "";
            }
            List<PatientModel> SearchedPatient = new List<PatientModel>();
            SqlConnection conn = new SqlConnection(SQLConnectionString);

            SqlCommand cmd = new SqlCommand("select * from dbo.[Hospital_PatientDetail_Sheet] where PatientName like '%" + keyword + "%' and IsDeleted = 0 and Timing is not null", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PatientModel cut = new PatientModel();
                    cut.PatientId = dr["PatientId"].ToString();
                    cut.PatientName = dr["PatientName"].ToString();
                    cut.PatientAddress = dr["PatientAddress"].ToString();
                    cut.PatientAge = dr["PatientAge"].ToString();
                    cut.PatientBloodGroup = dr["PatientBloodGroup"].ToString();
                    cut.Illness = dr["Illness"].ToString();
                    cut.AppointedDoctor = dr["AppointedDoctor"].ToString();
                    cut.DateOfAdmission = dr["DateOfAdmission"].ToString();
                    cut.DateofBirth = dr["DateOfAdmission"].ToString();
                    cut.DateofDischarge = dr["DateOfAdmission"].ToString();
                    cut.Gender = dr["Gender"].ToString();
                    cut.PhoneNumber = dr["PhoneNumber"].ToString();
                    cut.Email = dr["Email"].ToString();
                    cut.EmergentContactNumber = dr["EmergencyContactNumber"].ToString();
                    cut.EmergentContactName = dr["EmergencyContactName"].ToString();
                    cut.AppointmentTime = dr["Timing"].ToString();

                    SearchedPatient.Add(cut);
                }
            }
            return Json(SearchedPatient);
        }

        [HttpGet]
        public IActionResult NonAppointmentView()
        {
            return View();
        }
        public IActionResult GenerateBill(int PatientId,int amount)
        {
            
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();
            string gdtm = DateTime.Now.ToString();
            PatientModel patientModel = new PatientModel();
            SqlConnection conn = new SqlConnection(SQLConnectionString);

            SqlCommand cmd = new SqlCommand("select * from dbo.[Hospital_PatientDetail_Sheet] where PatientId = " + PatientId + " and IsDeleted = 0", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            string name1="";
            string age1 = "";
            string phone1 = "";
            string email1 = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PatientModel cut = new PatientModel();
                     cut.PatientId = dr["PatientId"].ToString();
                    name1 = dr["PatientName"].ToString();
                    cut.PatientAddress = dr["PatientAddress"].ToString();
                    age1 = dr["PatientAge"].ToString();
                    cut.PatientBloodGroup = dr["PatientBloodGroup"].ToString();
                    cut.Illness = dr["Illness"].ToString();
                    cut.AppointedDoctor = dr["AppointedDoctor"].ToString();
                    cut.DateOfAdmission = dr["DateOfAdmission"].ToString();
                    cut.DateofBirth = dr["DateOfAdmission"].ToString();
                    cut.DateofDischarge = dr["DateOfAdmission"].ToString();
                    cut.Gender = dr["Gender"].ToString();
                    phone1 = dr["PhoneNumber"].ToString();
                   email1= dr["Email"].ToString();
                    cut.EmergentContactNumber = dr["EmergencyContactNumber"].ToString();
                    cut.EmergentContactName = dr["EmergencyContactName"].ToString();

                    cut = patientModel;
                }
            }
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            // Add the invoice header
            Paragraph header1 = new Paragraph("\n\nGeneral Hospital\n\n");
            Paragraph header = new Paragraph("Invoice -" + gdtm + "\n\n");
            header.Alignment = Element.ALIGN_LEFT;
            header1.Alignment = Element.ALIGN_CENTER;
            header.Font = boldFont;
            header1.Font = boldFont;
            document.Add(header1);
            document.Add(header);

            // Add the invoice items
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.AddCell("Name");
            table.AddCell(name1);

            table.AddCell("Age");
            table.AddCell(age1);

            table.AddCell("Phone Number");
            table.AddCell(phone1);


            table.AddCell("Email");
            table.AddCell(email1);



            document.Add(table);

            // Add the invoice footer
            Paragraph footer = new Paragraph("\nTotal: Rs." + amount+"/-\n\n");
            Paragraph footer1 = new Paragraph("\n\n\n\nThis is a computer generate invoice and does not need any signature or stamp.\n\n");
            footer.Alignment = Element.ALIGN_RIGHT;
            footer1.Alignment = Element.ALIGN_CENTER;
            document.Add(footer);
            document.Add(footer1);

            // Close the document and write it to a PDF file
            document.Close();
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            return new FileContentResult(bytes, "application/pdf");
        }
        public JsonResult NonAppointmentList(string keyword)
        {
            if (keyword == null)
            {
                keyword = "";
            }
            List<PatientModel> SearchedPatient = new List<PatientModel>();
            SqlConnection conn = new SqlConnection(SQLConnectionString);

            SqlCommand cmd = new SqlCommand("select * from dbo.[Hospital_PatientDetail_Sheet] where PatientName like '%" + keyword + "%' and IsDeleted = 0 and Timing is null", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PatientModel cut = new PatientModel();
                    cut.PatientId = dr["PatientId"].ToString();
                    cut.PatientName = dr["PatientName"].ToString();
                    cut.PatientAddress = dr["PatientAddress"].ToString();
                    cut.PatientAge = dr["PatientAge"].ToString();
                    cut.PatientBloodGroup = dr["PatientBloodGroup"].ToString();
                    cut.Illness = dr["Illness"].ToString();
                    cut.AppointedDoctor = dr["AppointedDoctor"].ToString();
                    cut.DateOfAdmission = dr["DateOfAdmission"].ToString();
                    cut.DateofBirth = dr["DateOfAdmission"].ToString();
                    cut.DateofDischarge = dr["DateOfAdmission"].ToString();
                    cut.Gender = dr["Gender"].ToString();
                    cut.PhoneNumber = dr["PhoneNumber"].ToString();
                    cut.Email = dr["Email"].ToString();
                    cut.EmergentContactNumber = dr["EmergencyContactNumber"].ToString();
                    cut.EmergentContactName = dr["EmergencyContactName"].ToString();
                    cut.AppointmentTime = dr["Timing"].ToString();

                    SearchedPatient.Add(cut);
                }
            }
            return Json(SearchedPatient);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }  
}
