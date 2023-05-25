using JobApplicationTrackerForStudents_Server.Data;
using JobApplicationTrackerForStudents_Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JobApplicationTrackerForStudents_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly JobApplicationsTrackerContext _context;
        private readonly UserManager<ApplicationsUser> _userManager;

        public ApplicationsController(JobApplicationsTrackerContext context, UserManager<ApplicationsUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Applications
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
        {
            List<Application> applications = await _context.Applications.ToListAsync();
            return applications;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Application>> GetApplication(int id)
        {
            Application application = await _context.Applications.FirstOrDefaultAsync(app => app.JobId == id);
            if (application == null)
            {
                return NotFound();
            }
            return application;
        }

        // POST: api/Applications
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateApplication([FromBody] ApplicationsCsv applicationsCsv)
        {
            // Check if studentcsv field is provided
            if (applicationsCsv?.studentcsv == null)
            {
                return BadRequest(new { message = "The studentcsv field is required." });
            }

            Console.WriteLine("1");
            // Get the currently logged-in user
            ApplicationsUser currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                Console.WriteLine("2");
                return Unauthorized(new { message = "User not authenticated" });
            }

            // Create a new Student object using properties from the ApplicationsUser
            Student student = new Student
            {
                StudentId = applicationsCsv.studentcsv.studentId,
                Email = applicationsCsv.studentcsv.email,
                Phone = applicationsCsv.studentcsv.phoneNumber,
                Url = applicationsCsv.studentcsv.url
            };

            // Create a new Application object
            Application application = new Application
            {
                Position = applicationsCsv.position,
                Company = applicationsCsv.company,
                Date = applicationsCsv.date,
                StatusId = applicationsCsv.statusId,
                StudentId = int.Parse(currentUser.Id), // Assign the StudentId
                Student = student // Assign the Student object to the Student property
            };
            Console.WriteLine(application);
            // Save the new application to the database
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();


            // Create a new ApplicationsCsv object
            //ApplicationsCsv createdApplicationCsv = new ApplicationsCsv
            //{
            //    jobId = application.JobId,
            //    position = applicationsCsv.position,
            //    company = applicationsCsv.company,
            //    date = applicationsCsv.date,
            //    statusId = applicationsCsv.statusId,
            //    studentcsv = student
            //};

            return Ok(application);
        }

        // DELETE: api/Applications/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            Application application = await _context.Applications.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApplicationExists(int id)
        {
            return _context.Applications.Any(e => e.JobId == id);
        }
    }
}
