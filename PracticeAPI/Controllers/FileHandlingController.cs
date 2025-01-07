using ClosedXML.Excel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using PracticeAPI.Common;
using PracticeAPI.Model;
using PracticeAPI.Services;
using System.Data;

namespace PracticeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileHandlingController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly APIResponse _response;
        private readonly ICommonRepository<CollegeStudent> commonRepository;
        private readonly IEmailService _email;
        public FileHandlingController(IEmailService email,IWebHostEnvironment webHostEnvironment,ICommonRepository<CollegeStudent> common)
        {
            _webHostEnvironment = webHostEnvironment;
            _response = new();
            commonRepository = common;
            _email = email;
        }
        [NonAction]
        public string GetFilePath(string username)
        {
            return Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "Users", username);
        }
        [HttpPut("UploadImage")]
        public async Task<ActionResult> UploadImage(IFormFile formFile,string username)
        {
            try
            {
                string folderpath = GetFilePath(username);
                folderpath = folderpath.Replace("\\", "/");
                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                }
                var allowedExtension = new[] { ".jpg", ".png", ".jpeg", ".gif" };
                string extention = Path.GetExtension(formFile.FileName).ToLower();

                if (!allowedExtension.Contains(extention))
                {
                    return BadRequest("Invalid file upload!");
                }

                string imagePath = Path.Combine(folderpath, username + "_" + formFile.FileName);
                imagePath = imagePath.Replace("\\", "/");
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using (FileStream stream = System.IO.File.Create(imagePath))
                {
                    await formFile.CopyToAsync(stream);

                }
                byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);

                string base64Image = Convert.ToBase64String(imageBytes);

                //UserImage userImage = new UserImage();
                //userImage.UserName = username;
                //userImage.ImagePath = base64Image;
                //await commonRepository.Create(userImage);

                _response.StatusCode = System.Net.HttpStatusCode.OK;
                _response.Success = true;
                _response.Data = imagePath;
            }
            catch (Exception ex) {

                _response.Error = ex.Message.ToString();

            }

            return Ok(_response);
        }
        [HttpPut("UploadMultipleImage")]  
        public async Task<ActionResult> UploadMultiple(IFormFileCollection collection,string username)
        {
            int successCount = 0, errorCount = 0;
            string[] pathOfImages = new string[collection.Count];
            string[] localpath = new string[collection.Count];
            try
            {
                string folderpath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "Multi", username);
                folderpath = folderpath.Replace("\\", "/");

                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                }

                var allowedExtension = new[] { ".jpg", ".png", ".jpeg", ".gif" };
                foreach (var file in collection)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtension.Contains(extension))
                    {
                        return BadRequest("One of the file uploaded is invalid");
                    }
                    string imagePath = Path.Combine(folderpath,username + "_" +file.FileName);
                    imagePath = imagePath.Replace("\\", "/");
                    if(System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    using(FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                    localpath[successCount] = GetHostingPath(username, file.FileName);
                    pathOfImages[successCount] = imagePath;
                    successCount++;
                }
            }
            catch (Exception ex) {
                errorCount++;
                _response.Error = ex.Message.ToString();
            }
            _response.Data = localpath;
            _response.Success = successCount > 0;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            return Ok(_response);

        }

        [HttpGet("DownloadImage")]
        public async Task<ActionResult> DownloadImage(string username,string filename)
        {
            try
            {
                string folderpath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads/Multi/", username,username + "_" + filename);
                folderpath = folderpath.Replace("\\", "/");
                
                string image = GetHostingPath(username,filename);
                if(System.IO.File.Exists(folderpath))
                {
                    MemoryStream stream = new MemoryStream();
                    using(FileStream fileStream = new FileStream(folderpath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    string contentType = "application/octet-stream";
                    return File(stream, contentType, filename);
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex) {
                return BadRequest(ex.Message.ToString());
            }
        }
        
        [NonAction]
        public string GetHostingPath(string username,string fileName)
        {

            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            string imageName = username + "_" + fileName;
            string folderName = "Uploads/Multi";
            var path = Path.Combine(hosturl,folderName, username ,imageName);
            path = path.Replace("\\","/");
            return path;
        }
        [HttpGet("GetExcelData")]
        public async Task<ActionResult> GetExcel()
        {
            var student = await GetData();
            var fileName = "StudentSheet";
            var uniquefile = fileName + "_" + DateTime.Now.ToString() + ".xlsx";
            uniquefile = uniquefile.Replace(" ","_");
            var content = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            using (XLWorkbook wb = new XLWorkbook())
            {
                var sheet = wb.AddWorksheet(student,fileName);
                sheet.Columns().AdjustToContents();
                sheet.Row(1).Style.Fill.BackgroundColor = XLColor.Almond;
                sheet.Row(1).Style.Font.FontColor = XLColor.Red;
                sheet.Column(7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
             
                using(MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    return File(stream.ToArray(),content, uniquefile);
                }
            }

        }

        [NonAction]
        public async Task<DataTable> GetData()
        {
            DataTable dt = new DataTable();
            dt.TableName = "StudentData";
            dt.Columns.Add("Id",typeof(int));
            dt.Columns.Add("Name",typeof (string));
            dt.Columns.Add("Age",typeof (int));
            dt.Columns.Add("Email",typeof (string));
            dt.Columns.Add("Mobile",typeof (string));
            dt.Columns.Add("Address",typeof (string));
            dt.Columns.Add("DOB",typeof (DateTime));
            dt.Columns.Add("UniqueCode",typeof (string));

            var list = await commonRepository.GetAllAsync();

            list.ForEach(x =>
            {
                dt.Rows.Add(x.Id,x.Name,x.Age,x.Email,x.Mobile,x.Address, x.DOB.ToString(), Guid.NewGuid());
            });

            return dt;
        }

        [HttpPost("SendEmail")]
        public async Task<ActionResult> SendEmail()
        {
            try
            {
                Random random = new Random();
                int otp = random.Next(100000,999999);
                MailRequest mail = new MailRequest();
                mail.ToEmail = "test@gmail.com";
                mail.Subject = "Testing Email";
                mail.Body = $"This is my email body having otp {otp}";
                await _email.SendEmailAsync(mail);
                return Ok(mail);
            }
            catch(Exception ex) { 
            
                return BadRequest(ex.Message);  
            }
        }
    }
}
