using JobApplicationTrackerForStudents_Server.Data;
using JobApplicationTrackerForStudents_Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;

namespace JobApplicationTrackerForStudents_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly JobApplicationsTrackerContext _context;
        public ApplicationsController(JobApplicationsTrackerContext context)
        {
            _context = context;
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
            Application? application = _context.Applications.FirstOrDefault(application => application.JobId == id);
            if (application == null)
            {
                return NotFound();
            }
            return application;
        }

        // POST: api/Applications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostApplication([FromBody] ApplicationsCsv applicationCsv)
        {
            Console.WriteLine("add appl");
            int loggedInStudentId;
            if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out loggedInStudentId))
            {
                Console.WriteLine("Invalid student ID");
                return BadRequest(new { message = "Invalid student ID" });
            }

            Student studentResult = await _context.Students.FindAsync(loggedInStudentId);

            if (studentResult == null)
            { 
                Console.WriteLine("No student found with the given ID");
                return BadRequest(new { message = "No student found with the given ID" });
            }

            var existingUser = await _context.Students.FindAsync(loggedInStudentId);
            var application = new Application
            {
                Position = applicationCsv.position,
                Company = applicationCsv.company,
                Date = applicationCsv.date,
                StatusId = applicationCsv.statusId,
                Student = studentResult
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            StudentCsv studentCsv = new StudentCsv
            {
                studentId = applicationCsv.studentcsv.studentId,
                email = applicationCsv.studentcsv.email,
                phoneNumber = applicationCsv.studentcsv.phoneNumber,
                url= applicationCsv.studentcsv.url

            };

            ApplicationsCsv createdApplicationCsv = new ApplicationsCsv
            {
                jobId = application.JobId,
                position= applicationCsv.position,
                company = applicationCsv.company,
                date = applicationCsv.date,
                statusId = applicationCsv.statusId,
                studentcsv = studentCsv
            };
            return Ok(createdApplicationCsv);

        }

        //// PUT: api/Applications/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //[Authorize]
        //public async Task<IActionResult> PutApplication(int id, Application application)
        //{
        //    if (id != application.JobId)
        //    {
        //        return base.BadRequest();
        //    }

        //    _context.Entry(application).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ApplicationExists(id))
        //        {
        //            return NotFound();
        //        }
        //        throw;
        //    }

        //    return NoContent();
        //}

        // DELETE: api/Applications/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            //Application? application = _context.Applications.FirstOrDefault(application => application.JobId == id);
            Application? application = await _context.Applications.FindAsync(id);
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
            return (_context.Applications?.Any(e => e.JobId == id)).GetValueOrDefault();
        }
    }
}