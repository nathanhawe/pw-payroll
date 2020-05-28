using Microsoft.EntityFrameworkCore;
using PrimaCompany.IDP.DbContexts;
using PrimaCompany.IDP.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace PrimaCompany.IDP.Services
{
	public class LocalUserService : ILocalUserService
	{
		private readonly IdentityDbContext _context;

		public LocalUserService(IdentityDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}
		public async void AddUser(User userToAdd)
		{
			throw new NotImplementedException();
		}

		public async Task<User> GetUserBySubjectAsync(string subject)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		public async Task<bool> SetPassword(string securityCode, string password)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> ValdiateClearTextCredentialsAsync(string username, string password)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return false;

			var user = await GetUserByUserNameAsync(username);

			if (user == null) return false;
			if (!user.Active) return false;

			// Validate credentials
			return (user.Password == password);
		}
	}
}
