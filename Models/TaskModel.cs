using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Models
{
    public class TaskModel
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }

        public int UserId { get; set; }  // Foreign key

        [ForeignKey("UserId")]  // Specify the foreign key relationship
        public User User { get; set; }  // Navigation property

        public Comment Comment { get; set; }
    }
}
