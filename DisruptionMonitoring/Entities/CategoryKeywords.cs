namespace DisruptionMonitoring.Entities
{
    public class CategoryKeywords
    {
        public int Id { get; set; }
        public string? Category { get; set; }
        public string? Keyword { get; set; }
        public bool IsDeleted { get; set; }
    }
}
