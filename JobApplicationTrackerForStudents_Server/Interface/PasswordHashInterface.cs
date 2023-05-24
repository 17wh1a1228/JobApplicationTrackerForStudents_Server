using Microsoft.AspNetCore.Identity;

namespace JobApplicationTrackerForStudents_Server.Interface
{
    public interface IPasswordHasherWrapper
    {
        string HashPassword(ApplicationsUser user, string password);
        PasswordVerificationResult VerifyHashedPassword(ApplicationsUser user, string hashedPassword, string providedPassword);
    }

    public class PasswordHasherWrapper : IPasswordHasherWrapper
    {
        private readonly PasswordHasher<ApplicationsUser> _passwordHasher;

        public PasswordHasherWrapper(PasswordHasher<ApplicationsUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string HashPassword(ApplicationsUser user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public PasswordVerificationResult VerifyHashedPassword(ApplicationsUser user, string hashedPassword, string providedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }
    }
}