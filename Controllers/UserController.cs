using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class UserController : Controller
    {
        private readonly TaskContext _context;

        public UserController(TaskContext context)
        {
            _context = context;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            // Render the registration form view
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest(new { Message = "Email already exists" });
            }

            if (user.Password.Length < 6)
            {
                return BadRequest(new { Message = "Password must be of length 6" });

            }

            var newUser = new User
            {
                Email = user.Email,
                FullName = user.FullName,
                Password = user.Password,
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            //return RedirectToAction("Login");
            return Redirect("/");
           // return Ok(newUser);
        }

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(User user)
        {
            var finduser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (finduser == null)
            {
               
                throw new KeyNotFoundException("Users not found. Try again");
            }


            if (user.Password != user.Password)
            {
                return Unauthorized(new { Message = "Invalid email or password" });
            }


            return RedirectToAction("Index","Task");
        }
 
    }
}
