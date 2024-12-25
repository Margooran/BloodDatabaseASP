namespace BloodDatabaseASP.Models
{
    public class BloodTypeEntity
    {
        public int BloodTypeID { get; set; }
        public string BloodTypeName { get; set; } = string.Empty;
        public string ABO { get; set; } = string.Empty;
        public bool Resus { get; set; }
    }
}
