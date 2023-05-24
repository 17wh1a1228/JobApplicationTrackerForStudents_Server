using CsvHelper.Configuration;
using CsvReader = CsvHelper.CsvReader;
using JobApplicationTrackerForStudents_Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using JobApplicationTrackerForStudents_Server.Models;

namespace JobApplicationTrackerForStudents_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly UserManager<ApplicationsUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly JobApplicationsTrackerContext _context;
        private readonly string _pathName;

        public SeedController(UserManager<ApplicationsUser> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, JobApplicationsTrackerContext context, IHostEnvironment environment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _pathName = Path.Combine(environment.ContentRootPath, "Data/applications.csv");
        }

        // POST: api/Seed
        [HttpPost("Applications")]
        public async Task<IActionResult> ImportApplications()
        {
            Dictionary<int, Application> applicationById = _context.Applications
                .AsNoTracking().ToDictionary(x => x.JobId);


            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };

            using StreamReader reader = new(_pathName);
            using CsvReader csv = new(reader, config);

            IEnumerable<ApplicationsCsv>? records = csv.GetRecords<ApplicationsCsv>();
            foreach (ApplicationsCsv record in records)
            {
                if (applicationById.ContainsKey(record.jobId))
                {
                    continue;
                }

                Application application = new Application();
                if(record.jobId != null)
                {
                    application.JobId = record.jobId;
                }
                if(!string.IsNullOrEmpty(record.position)) { 
                    application.Position = record.position;
                }
                if (!string.IsNullOrEmpty(record.company))
                {
                    application.Company = record.position;
                }
                if(record.date != null)
                {
                    application.Date = record.date;
                }
                if(record.statusId != null)
                {
                    application.StatusId = record.statusId;
                }

                await _context.SaveChangesAsync();
                return new JsonResult(applicationById.Count);
            }

            await _context.SaveChangesAsync();

            return new JsonResult(applicationById.Count);
        }

        [HttpPost("Students")]
        public async Task<IActionResult> ImportStudents()
        {
            const string roleStudent = "RegisteredStudent";
            const string roleAdmin = "Administrator";

            if (await _roleManager.FindByNameAsync(roleStudent) is null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleStudent));
            }
            if (await _roleManager.FindByNameAsync(roleAdmin) is null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleAdmin));
            }

            List<ApplicationsUser> addedUserList = new();
            (string name, string email) = ("admin", "admin@email.com");

            if (await _userManager.FindByNameAsync(name) is null)
            {
                ApplicationsUser userAdmin = new()
                {
                    UserName = name,
                    Email = email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                await _userManager.CreateAsync(userAdmin, _configuration["DefaultPasswords:Administrator"]
                    ?? throw new InvalidOperationException());
                await _userManager.AddToRolesAsync(userAdmin, new[] { roleStudent, roleAdmin });
                userAdmin.EmailConfirmed = true;
                userAdmin.LockoutEnabled = false;
                addedUserList.Add(userAdmin);
            }

            (string name, string email) registered = ("student", "student@email.com");

            if (await _userManager.FindByNameAsync(registered.name) is null)
            {
                ApplicationsUser user = new()
                {
                    UserName = registered.name,
                    Email = registered.email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                await _userManager.CreateAsync(user, _configuration["DefaultPasswords:RegisteredUser"]
                    ?? throw new InvalidOperationException());
                await _userManager.AddToRoleAsync(user, roleStudent);
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                addedUserList.Add(user);
            }

            if (addedUserList.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            return new JsonResult(new
            {
                addedUserList.Count,
                Users = addedUserList
            });

        }
    }
}
