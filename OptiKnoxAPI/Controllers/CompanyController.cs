using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using OptiKnoxAPI.Models;

namespace OptiKnoxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        Company objCompanyResult = new Company();
        #region Create Company
        [HttpPost]
        [Route("Company-Creation")]
        public IActionResult CompanyCreation(Company obj)
        {
            var dtCompanyDetails = obj.AddCompany(obj);
            return Ok(JsonConvert.SerializeObject(dtCompanyDetails, new JsonSerializerSettings()));
        }

        #endregion

        #region Get Organization  Details

        [HttpGet]
        [Route("get-Organization-Details")]
        public IActionResult GetOrganizationDeatils(string OrgId)
        {
            var dtMasterCompany = objCompanyResult.MasterCompanyDetails(OrgId);
            return Ok(JsonConvert.SerializeObject(dtMasterCompany, new JsonSerializerSettings()));
        }

        #endregion
    }
}
