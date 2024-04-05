namespace DatabaseLibrary.Interfaces
{
    public interface IAuthorMetadata
    {
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }
    }
}
