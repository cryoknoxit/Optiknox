using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OptiKnoxAPI.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices.Marshalling;

namespace OptiKnoxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly EmailSettings _emailSettings;
        public AuthenticationController(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        #region Login Validation


       
        [HttpGet]
        [Route("Validate-Login")]
        public IActionResult ValidateLogin(string username, string password)
        {
            Authentication obj = new Authentication();
            obj.UserName = username;
            obj.Password = password;
            //obj.SendEmailAsync();

            var dtValidatelogin = obj.ValidateLogin();
            return Ok(JsonConvert.SerializeObject(dtValidatelogin, new JsonSerializerSettings()));
        }
        #endregion

        #region Get User Details
        [HttpGet]
        [Route("Get-User-Details")]
        public IActionResult GetUserDetails(string username)
        {
            Authentication obj = new Authentication();
            obj.UserName = username;
            var dtUserdetails = obj.GetUserDetails();
            return Ok(JsonConvert.SerializeObject(dtUserdetails, new JsonSerializerSettings()));
        }
        #endregion

        #region Check User
        [HttpGet]
        [Route("Get-Check-User")]
        public IActionResult CheckUser(string username)
        {
            Authentication obj = new Authentication();
            obj.UserName = username;
            var dtUserdetails = obj.CheckUser();
            return Ok(JsonConvert.SerializeObject(dtUserdetails, new JsonSerializerSettings()));
        }
        #endregion

        #region Reset Password
        [HttpGet]
        [Route("Reset-Password")]
        public IActionResult Generatepassword(string username, string NewPassword)
        {
            Authentication obj = new Authentication();
            obj.UserName = username;
            obj.NewPassword = NewPassword;
            var dtUserdetails = obj.Generatepassword();
            return Ok(JsonConvert.SerializeObject(dtUserdetails, new JsonSerializerSettings()));
        }
        #endregion

        [HttpGet]
        [Route("get-subscriptions")]
        public IActionResult GetSubscriptions(string OrgId)
        {
            Authentication obj = new Authentication();
           
            var dtUserdetails = obj.GetSubscriptions(OrgId);
            return Ok(JsonConvert.SerializeObject(dtUserdetails, new JsonSerializerSettings()));
        }

        [HttpGet]
        [Route("Recent-Logins")]
        public IActionResult GetRecentLogin(string username, string logincount)
        {
            Authentication obj = new Authentication();
            obj.UserName = username;
            obj.logincount = logincount;
            var dtLoginDetails = obj.GetRecentLogins();
            return Ok(JsonConvert.SerializeObject(dtLoginDetails, new JsonSerializerSettings()));
        }

        #region Check Organization
        [HttpGet]
        [Route("Get-Check-Organization")]
        public IActionResult CheckOrganization(string OrganizationName, string ContactName, string ContactNo, string UserName, string Email)
        {
            Authentication obj = new Authentication();
            var dtUserdetails = obj.CheckOrganition(OrganizationName, ContactName, ContactNo, UserName, Email);
            return Ok(JsonConvert.SerializeObject(dtUserdetails, new JsonSerializerSettings()));
        }
        #endregion


        #region  Organization Signup
        [HttpPost]
        [Route("Organization-Signup")]
        public IActionResult OrganizationSignup(Organzation obj)
        {
            var dtBranchDetails = obj.SignupOrganization(obj);
            return Ok(JsonConvert.SerializeObject(dtBranchDetails, new JsonSerializerSettings()));
        }
        #endregion

    }
}
