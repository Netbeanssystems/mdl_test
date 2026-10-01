namespace Application.Dtos
{
    public class DeletionDTO
    {
        public int Id { get; set; }
        public bool Hard { get; set; }
        public string Message { get; set; } = "Are you sure you want to delete this item? This can not be undone.";
    }
}
