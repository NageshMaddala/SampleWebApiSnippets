using System.Collections.Generic;
using System.Web.Http;
using System.Web.UI;
using SampleSnippets.AuthorizationFilters;

namespace SampleSnippets.Controllers
{
    [RoutePrefix("api/Accounts")]
    public class AccountsController : ApiController
    {
        // GET: api/Accounts
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { User.Identity.Name, User.Identity.AuthenticationType };
        }

        [Authorize(Roles = "Admin")]
        public IEnumerable<string> Delete(int id)
        {
            return new string[] { User.Identity.Name, User.Identity.AuthenticationType, $"Successfully deleted: {id}" };
        }

        [Authorize(Roles = "User")]
        public IEnumerable<string> Delete(string id)
        {
            return new string[] { User.Identity.Name, User.Identity.AuthenticationType, $"Successfully deleted: {id}" };
        }

        [HttpGet, Route("CheckClaim")]
        [RequireClaim("CustomUserClaim", IncludeMissingInResponse = true)]
        public IEnumerable<string> CheckClaim()
        {
            return new string[] { User.Identity.Name, User.Identity.AuthenticationType, $"CustomClaim" };
        }

        // GET: api/Accounts/5
        [AllowAnonymous]
        public IEnumerable<string> Get(int id)
        {
            return new string[] { User.Identity.Name, User.Identity.AuthenticationType, $"Value: {id}" };
        }
    }
}