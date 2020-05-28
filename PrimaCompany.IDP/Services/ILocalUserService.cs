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
		Task<bool> ValdiateClearTextCredentialsAsync(string username, string password);
		//Task<bool> ValidateCredentialsAsyc(string username, string password);
		Task<IEnumerable<Entities.UserClaim>> GetUserClaimsBySubjectAsyc(string subject);
		Task<User> GetUserByUserNameAsync(string username);
		Task<User> GetUserBySubjectAsync(string subject);
		void AddUser(User userToAdd);
		//void AddUser(User userToAdd, string password);
		Task<bool> IsUserActive(string subject);
		//Task<bool> ActivateUser(string securityCode);
		Task<bool> SaveChangesAsync();
		Task<string> InitiatePasswordResetRequest(string email);
		Task<bool> SetPassword(string securityCode, string password);
	}
}
