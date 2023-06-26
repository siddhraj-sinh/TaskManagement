using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        public ICollection<TaskModel>? Tasks { get; set; }
       
    }
}
