using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PrimaCompany.IDP.DbContexts;
using PrimaCompany.IDP.Entities;
using PrimaCompany.IDP.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PrimaCompany.IDP.Services
{
	public class LocalUserService : ILocalUserService
	{
		private readonly IdentityDbContext _context;
		private readonly IPasswordHasher<User> _passwordHasher;

		public LocalUserService(
			IdentityDbContext context,
			IPasswordHasher<User> passwordHasher)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
		}

		public void AddUser(User userToAdd, string password)
		{
			if (userToAdd == null) throw new ArgumentNullException(nameof(userToAdd));
			if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

			if (_context.Users.Any(u => u.Username == userToAdd.Username)) throw new UsernameIsNotUniqueException();
			if (_context.Users.Any(u => u.Email == userToAdd.Email)) throw new EmailIsNotUniqueException();
			if (!ValidateEmail(userToAdd.Email)) throw new EmailIsInvalidException();

			userToAdd.SecurityCode = GenerateSecurityCode();
			userToAdd.SecurityCodeExpirationDate = DateTime.UtcNow;
			userToAdd.Password = _passwordHasher.HashPassword(userToAdd, password);

			_context.Users.Add(userToAdd);
		}
			

		public async Task<User> GetUserBySubjectAsync(string subject)
		{
			if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException(nameof(subject));
			return await _context.Users.FirstOrDefaultAsync(u => u.Subject == subject);
		}

		public async Task<User> GetUserByUserNameAsync(string username)
		{
			if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));

			return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
		}

		public async Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectAsyc(string subject)
		{
			if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException(nameof(subject));

			return await _context.UserClaims.Where(u => u.User.Subject == subject).ToListAsync();
		}

		public async Task<string> InitiatePasswordResetRequest(string email)
		{
			if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

			if (user == null) throw new Exception($"Use with email address '{email}' can't be found.");

			user.SecurityCode = GenerateSecurityCode();
			user.SecurityCodeExpirationDate = DateTime.UtcNow.AddHours(1);
			
			return user.SecurityCode;
		}

		public async Task<bool> IsUserActive(string subject)
		{
			if (string.IsNullOrWhiteSpace(subject)) return false;

			var user = await GetUserBySubjectAsync(subject);

			if (user == null) return false;

			return user.Active;
		}

		public async Task<bool> SaveChangesAsync()
		{
			return (await _context.SaveChangesAsync() > 0);
		}

		public async Task<bool> SetPassword(string securityCode, string password)
		{
			if (string.IsNullOrWhiteSpace(securityCode)) throw new ArgumentNullException(nameof(securityCode));
			if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

			var user = await _context.Users.FirstOrDefaultAsync(u => u.SecurityCode == securityCode && u.SecurityCodeExpirationDate >= DateTime.UtcNow);

			if(user == null)
			{
				return false;
			}

			user.SecurityCode = null;

			// salt and hash the new password\
			user.Password = _passwordHasher.HashPassword(user, password);
			return true;
		}

		public async Task<bool> ValidateCredentialsAsync(string username, string password)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return false;

			var user = await GetUserByUserNameAsync(username);

			if (user == null) return false;
			if (!user.Active) return false;

			// Validate credentials
			var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
			return (verificationResult == PasswordVerificationResult.Success);
		}

		private bool ValidateEmail(string email)
		{
			var emailComponents = email.Split("@");
			
			if (emailComponents.Length == 2)
			{
				if (emailComponents[1].ToLower() == "prima.com") return true;
				if (emailComponents[1].ToLower() == "gerawan.com") return true;
				if (emailComponents[1].ToLower() == "wawonapacking.com") return true;
			}

			return false;
		}
		
		private string GenerateSecurityCode()
		{
			using var randomNumberGenerator = new RNGCryptoServiceProvider();
			var securityCodeData = new byte[128];
			randomNumberGenerator.GetBytes(securityCodeData);
			return Convert.ToBase64String(securityCodeData);
		}
	}
}
