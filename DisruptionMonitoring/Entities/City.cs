namespace DisruptionMonitoring.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string? Location { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Continent { get; set; }
        public bool IsDeleted { get; set; }
    }
}
