namespace JobBoard.Models
{
    public class MailModel
    {
        public string RecieverEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }

        //Private Credentials
        public string MyMail = "anmol2099v@gmail.com";
        public string MyAppPassword = "***********";

    }
}
