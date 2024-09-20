namespace DisruptionMonitoring.Entities
{
    public class UserProfileRequest
    {
        public UserProfiles Profile { get; set; }
        public List<int> CategoryKeywordIds { get; set; }
        public List<int> SupplierIds { get; set; }
        public List<int> LocationIds { get; set; }
    }


}
