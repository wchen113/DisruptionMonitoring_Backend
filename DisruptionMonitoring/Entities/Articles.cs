namespace DisruptionMonitoring.Entities
{
    public class Articles
    {
        public int Id { get; set; }

        public DateTime? Created_At { get; set; }

        public string? Title { get; set; }

        public string? Text { get; set; }

        public string? Location { get; set; }

        public decimal Lat { get; set; }

        public decimal Lng { get; set; }


        public string? DisruptionType { get; set; }

        public string? Severity { get; set; }

        public string? SourceName { get; set; }

        public DateOnly? PublishedDate { get; set; }

        public string? Url { get; set; }

        public string? ImageUrl { get; set; }

        public long? Radius { get; set; }

        public bool IsDeleted { get; set; }

    }
}
