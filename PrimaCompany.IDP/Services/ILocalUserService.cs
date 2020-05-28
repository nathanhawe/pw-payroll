using IdentityServer4.EntityFramework.Entities;
using PrimaCompany.IDP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimaCompany.IDP.Services
{
	public interface ILocalUserService
	{
		void AddUser(User userToAdd, string password);
		Task<User> GetUserBySubjectAsync(string subject);
		Task<User> GetUserByUserNameAsync(string username);
		Task<IEnumerable<Entities.UserClaim>> GetUserClaimsBySubjectAsyc(string subject);
		Task<string> InitiatePasswordResetRequest(string email);
		Task<bool> IsUserActive(string subject);
		Task<bool> SaveChangesAsync();
		Task<bool> SetPassword(string securityCode, string password);
		Task<bool> ValidateCredentialsAsync(string username, string password);

	}
}
