namespace Hospital_Management_System.Models
{
    public class PatientModel
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientAge { get; set; }
        public string PatientBloodGroup { get; set;}
        public string PatientAddress { get; set;}
        public string Illness { get; set;}
        public string AppointedDoctor { get; set;}
        public string DateOfAdmission { get; set;}
        public string DateofBirth { get; set;}
        public string DateofDischarge { get; set;}
        public string Gender { get; set;}
        public string PhoneNumber { get; set;}
        public string Email { get; set;}
        public string EmergentContactName { get; set;}
        public string EmergentContactNumber { get; set;}
        public string Amount { get; set;}
        public string AppointmentDate { get; set;}
        public string AppointmentTime { get; set;}
    }    
}
