using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.DynamicData;
using System.Web.Http.Filters;

namespace SampleSnippets.AuthFilters
{
    public class BasicAuthFilterAttribute : Attribute, IAuthenticationFilter
    {
        /// <summary>
        /// Set to the Authorization header Scheme value that this filter is intended to support
        /// </summary>
        public const string SupportedTokenScheme = "Basic";

        // TODO: Decide if your filter should allow multiple instances per controller or
        //       per-method; set AllowMultiple to true if so
        public bool AllowMultiple { get { return false; } }

        /// <summary>
        /// True if the filter supports WWW-Authenticate challenge headers,
        /// defaults to false.
        /// </summary>
        public bool SendChallenge { get; set; }

        public BasicAuthFilterAttribute()
        {
            SendChallenge = true;
        }

        /// <summary>
        /// Logic to authenticate the credentials. Must do one of:
        ///  -- exit out, doing nothing, if it cannot understand the token scheme presented,
        ///  -- set context.ErrorResult to an IHttpActionResult holding reason for invalid authentication.
        ///  -- set context.Principal to an IPrincipal if authenticated,
        /// https://www.base64encode.org/
        /// Authorization Basic username:password in base 64
        /// </summary>
        public async Task AuthenticateAsync(HttpAuthenticationContext context,
            CancellationToken cancellationToken)
        {
            // STEP 1: Look for credentials in the request.
            var authHeader = context.Request.Headers.Authorization;
            // STEP 2: If there are no credentials, do nothing.
            if (authHeader == null)
                return;

            // STEP 3: If there are credentials but the filter does not recognize the 
            //         authentication scheme, do nothing.
            var tokenType = authHeader.Scheme;
            if (!tokenType.Equals(SupportedTokenScheme))
                return;

            // STEP 4: If there are credentials that the filter understands, try to validate them.
            var credentials = authHeader.Parameter;
            if (String.IsNullOrEmpty(credentials))
            {
                // no credentials sent with the scheme, abort out of the pipeline with an error result
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", context.Request);
                return;
            }

            // STEP 5: If the credentials are bad, set the error result, else set the IPrincipal 
            //         on the context.
            IPrincipal principal = await ValidateCredentialsAsync(credentials, cancellationToken);
            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", context.Request);
            }
            else
            {
                // We have a valid, authenticated user; save off the IPrincipal instance
                context.Principal = principal;
            }
        }

        /// <summary>
        /// Extra logic associated with Basic/Digest authentication scheme
        /// </summary>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            // if this filter wants to support WWW-Authenticate header challenges, add one to the
            // result
            if (SendChallenge)
            {
                context.Result = new AddChallengeOnUnauthorizedResult(
                    new AuthenticationHeaderValue(SupportedTokenScheme),
                    context.Result);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Internal method to validate the credentials included in the request,
        /// returning an IPrincipal for the resulting authenticated entity.
        /// </summary>
        private async Task<IPrincipal> ValidateCredentialsAsync(string credentials, CancellationToken cancellationToken)
        {
            var subject = ParseBasicAuthCredential(credentials);

            // we would pbbly do an async database lookup here...
            if (String.IsNullOrEmpty(subject.Item2) || subject.Item2 != "Test123")
                return null;

            IList<Claim> claimCollection = new List<Claim>
            {
                new Claim(ClaimTypes.Name, subject.Item1),
                new Claim(ClaimTypes.AuthenticationInstant, DateTime.UtcNow.ToString("o")),
            };

            if (subject.Item1 == "SuperUser")
                claimCollection.Add(new Claim(ClaimTypes.Role, "Admin"));

            if (subject.Item1 == "CustomUser")
                claimCollection.Add(new Claim("CustomUserClaim", "my special value"));

            var identity = new ClaimsIdentity(claimCollection, SupportedTokenScheme);
            var principal = new ClaimsPrincipal(identity);
            return await Task.FromResult(principal);
        }

        private Tuple<string, string> ParseBasicAuthCredential(string credential)
        {
            string password = null;
            string userName = null;
            var subject = (Encoding.GetEncoding("iso-8859-1").GetString(Convert.FromBase64String(credential)));
            if (String.IsNullOrEmpty(subject))
                return new Tuple<string, string>(null, null);

            if (subject.Contains(":"))
            {
                var index = subject.IndexOf(':');
                password = subject.Substring(index + 1);
                userName = subject.Substring(0, index);
            }

            return new Tuple<string, string>(userName, password);
        }
    }
}