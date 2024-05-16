using System.Data;
using System.Net;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;


namespace OptiKnoxAPI.Models
{
    public class Company
    {

        private readonly IConfiguration _configuration;
        string connectionstring = "";

        public string Organizationid { get; set; }
        public string TenantId { get; set; }
        public string Organizationname { get; set; }
        public string RegdOfficeAddress { get; set; }
        public string ContactPerson { get; set; }

        public string Contactno { get; set; }

        public string Website { get; set; }
        public string OrganizationLogo { get; set; }
        public string StatusId { get; set; }


     //   @Organizationid INT,
     //   @Website Varchar(100),
	    //@Address Varchar(150),
	    //@LogoFileName Varchar(150)





        public Company()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory) // Set the base path of your application
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
           .Build();

            // Get the connection string
            connectionstring = configuration.GetConnectionString("DefaultConnection");
        }

        public DataTable MasterCompanyDetails(string OrgId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@Organizationid",Convert.ToInt64(OrgId));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_get_master_OrganizationProfile", connectionstring);
            return dt;

        }

        public DataTable AddCompany(Company obj)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@Organizationid",Convert.ToInt64(obj.Organizationid));
            cmd.Parameters.AddWithValue("@Website", obj.Website);
            cmd.Parameters.AddWithValue("@Address", obj.RegdOfficeAddress);
            cmd.Parameters.AddWithValue("@LogoFileName", obj.OrganizationLogo);

          //  cmd.Parameters.AddWithValue("", obj.ImageName);
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_upd_master_Organization", connectionstring);
            return dt;
        }

    }
}
