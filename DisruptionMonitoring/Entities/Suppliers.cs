namespace DisruptionMonitoring.Entities
{
    public class Suppliers
    {
        public int Id { get; set; }
        public string? BP_Code { get; set; }
        public string? BP_Name { get; set; }
        public string? Address_1 { get; set; }
        public string? Address_2 { get; set; }
        public string? Address_3 { get; set; }
        public string? Address_4 { get; set; }
        public DateOnly? Creation_Date { get; set; }
        public string? Contact_Person { get; set; }
        public string? Telephone { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
        public bool IsDeleted { get; set; }
    }
}
