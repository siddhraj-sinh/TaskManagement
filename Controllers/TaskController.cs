using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    //[Route("[controller]")]
    //[ApiController]
    public class TaskController : Controller
    {
        private readonly TaskContext _taskContext;
        public TaskController(TaskContext taskContext)
        {
            _taskContext = taskContext;
        }
        // GET: /task
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var tasks = await _taskContext.Tasks.ToListAsync();
            return View(tasks);
            //return Ok(tasks);
        }
        [HttpGet]
        // GET: Task/Create
        public IActionResult Create()
        {
            // Create a dummy list of users
            //        var users = new List<User>
            //{
            //    new User { Id = 1, FullName = "User 1", Email = "user1@example.com", Password = "password1" },
            //    new User { Id = 2, FullName = "User 2", Email = "user2@example.com", Password = "password2" },
            //    new User { Id = 3, FullName = "User 3", Email = "user3@example.com", Password = "password3" }
            //};

            //        // Convert the dummy list to SelectListItem objects
            //        var selectListItems = users.Select(u => new SelectListItem
            //        {
            //            Value = u.Id.ToString(),
            //            Text = u.FullName
            //        });

            //        // Create a SelectList from the SelectListItem objects
            //        var usersSelectList = new SelectList(selectListItems, "Value", "Text");

            //        // Assign the SelectList to the ViewBag.Users
            //        ViewBag.Users = usersSelectList;
            PopulateUsers();
            return View();
        }


        private void PopulateUsers()
        {
            ViewBag.Users = GetUsers();
        }

        private IEnumerable<SelectListItem> GetUsers()
        {
            // Fetch and return the users from the database or any other source
            var users = _taskContext.Users.ToList();

            // Convert the users to SelectListItem objects
            var selectListItems = users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.FullName
            });

            return selectListItems;
        }
        //POST: Task/Create
        [HttpPost]
        public async Task<IActionResult> Create(TaskModel task)
        {
            task.User = await _taskContext.Users.FindAsync(task.UserId);
            await _taskContext.Tasks.AddAsync(task);
            await _taskContext.SaveChangesAsync();

            var userDetails = _taskContext.Users.Find(task.UserId);

            string body = "New Task has been added. Please Check Index board";
            Email(userDetails.Email, body);
            return RedirectToAction(nameof(Index));

        }

        //GET: Task/Edit/{id}
        [HttpGet("Edit/{id}")]
        public async Task<ActionResult> Edit(int id) {
            var task = await _taskContext.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Get the list of users for the dropdown
            var users = _taskContext.Users.ToList();
            ViewBag.Users = new SelectList(users, "Id", "FullName", task.UserId); // Set the selected value

            return View(task);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, TaskModel task)
        {
            if(id != task.Id)
            {
                return BadRequest();
            }
            _taskContext.Entry(task).State = EntityState.Modified;

            try
            {
                await _taskContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var userDetails = _taskContext.Users.Find(task.UserId);

            string body = "Your task has beed updated. Please Check Index board";

            Email(userDetails.Email, body);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Comment/{id}")]

        public async Task<ActionResult> Comment(int id)
        {
            try
            {
                var task = await _taskContext.Tasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                return View(task);
            }
            catch (Exception ex)
            {
                // Log the exception or perform any necessary error handling
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPost("Comment/{id}")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Comment([FromForm] Comment comment,  int id)
        {
            try
            {
                var NewComment = new Comment
                {
                   TaskId = id,
                    Content = comment.Content,
                    CreatedAt = DateTime.Now,
                };
                await _taskContext.Comments.AddAsync(NewComment);
                await _taskContext.SaveChangesAsync();

             return RedirectToAction("Index", "Task");
            }
            catch (Exception ex)
            {
                // Log the exception or perform any necessary error handling

                // Redirect to the Index action with an error message
                return RedirectToAction("Index", new { errorMessage = "An error occurred while processing the comment." });
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var task = await _taskContext.Tasks.FindAsync(id);
            if(task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskContext.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            _taskContext.Remove(task);
            _taskContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<ActionResult> Search(string searchTerm)
        {
                if(string.IsNullOrWhiteSpace(searchTerm))
                {
                    return View();
                }
                var task1 = _taskContext.Tasks.Where(item => item.Title.Contains(searchTerm)).ToList();
                return View(task1);
        }

        [HttpGet]
        public IActionResult Email(string email, string Body)
        {
            string recipient = email;
            string subject = "Task";
            string body = Body;

            SendEmail(recipient, subject, body);

            return Ok();
        }

        public void SendEmail(string recipient, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("chiraglabha05@gmail.com", "xmbmrinlzcavyppt")
            };

            var message = new MailMessage
            {
                From = new MailAddress("chiraglabha05@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(recipient);

            // Send the email
            smtpClient.Send(message);
        }
        private bool TaskExists(int id)
        {
            return _taskContext.Tasks.Any(t => t.Id == id);
        }
    }
}