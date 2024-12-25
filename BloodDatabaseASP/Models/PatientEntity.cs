namespace BloodDatabaseASP.Models
{
    public class PatientEntity
    {
        public int PatientID { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientGender { get; set; } = string.Empty;
        public int BloodTypeID { get; set; }
    }
}
