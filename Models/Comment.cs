namespace TaskManagement.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public int TaskId { get; set; }
        public TaskModel Task { get; set; }

        //public string UserId { get; set; }
        //public User User { get; set; }
    }
}
