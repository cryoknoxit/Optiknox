using System.Data;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using Microsoft.Data.SqlClient;
namespace OptiKnoxAPI.Models
{
    public class SuperAdmin
    {
        private readonly IConfiguration _configuration;
        string connectionstring = ""; string TenantName = "";



        public SuperAdmin()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory) // Set the base path of your application
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
           .Build();
            TenantName = clsSingleton.TenantName;
            if (TenantName == "localhost")
            {
                TenantName = "dbo";
            }
            // Get the connection string
            connectionstring = configuration.GetConnectionString("DefaultConnection");
        }


        #region Master Data
        public DataTable MasterDepartments()
        {
            SqlCommand cmd = new SqlCommand();
            
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_Departments", connectionstring);
            return dt;
        }
        public DataTable MasterRoles()
        {
            SqlCommand cmd = new SqlCommand();
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_get_master_roles", connectionstring);
            return dt;

        }
        public DataTable MasterBranch(string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@OrganizationId", Organizationid);
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_get_master_Branch", connectionstring);
            return dt;

        }
        #endregion

    }
    public class FileModel
    {
        public string Name { get; set; }
       
        public string OrdId { get; set; }
        //public IFormFile TileImage { get; set; }
        public List<IFormFile> TileImages { get; set; }
    }
    public class Branch
    {

        string connectionstring { get; set; }
        public string TenantName { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string BranchContactNo { get; set; }
        public string ContactPerson { get; set; }
        public string ContactMobileno { get; set; }
        public string BranchOperationDate { get; set; }
        public string Status { get; set; }
        public string OrganizationId { get; set; }



        public Branch()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory) // Set the base path of your application
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
           .Build();

            TenantName = clsSingleton.TenantName;
            if (TenantName == "localhost")
            {
                TenantName = "dbo";
            }
            // Get the connection string
            connectionstring = configuration.GetConnectionString("DefaultConnection");
        }
        public DataTable AddBranchDetails(Branch obj)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
            cmd.Parameters.AddWithValue("@Organizationid", Convert.ToInt64(obj.OrganizationId));
            cmd.Parameters.AddWithValue("@BranchName", obj.BranchName);
            cmd.Parameters.AddWithValue("@BranchAddress", obj.BranchAddress);
            cmd.Parameters.AddWithValue("@BranchContactNo", obj.BranchContactNo);
            cmd.Parameters.AddWithValue("@ContactPerson", obj.ContactPerson);
            cmd.Parameters.AddWithValue("@ContactMobileno", obj.ContactMobileno);
            
            DataTable dt = new DataTable();
            if (Convert.ToInt64(BranchId) > 0)
            {
                cmd.Parameters.AddWithValue("@BranchID", Convert.ToInt64(BranchId));
                cmd.Parameters.AddWithValue("@StatusId", Convert.ToInt64(Status));
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure,  "manage.sp_upd_master_Branch", connectionstring);
            }
            else
            {
                cmd.Parameters.AddWithValue("@BranchOperationDate", Convert.ToDateTime(DateTime.ParseExact(obj.BranchOperationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)));
                //cmd.Parameters.AddWithValue("@BranchOperationDate", Convert.ToDateTime(obj.BranchOperationDate));
                
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_ins_master_Branch", connectionstring);
            }
            return dt;
        }


    }

    public class User
    {

        string connectionstring { get; set; }
        public string TenantName { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string BranchId { get; set; }
        public string EmployeeID { get; set; }
        public string OrganizationId { get; set; }
        public string DepartmentId { get; set; }
        public string RoleId { get; set; }

        public string Status { get; set; }
        public User()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory) // Set the base path of your application
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
           .Build();

            TenantName = clsSingleton.TenantName;
            if (TenantName == "localhost")
            {
                TenantName = "dbo";
            }
            // Get the connection string
            connectionstring = configuration.GetConnectionString("DefaultConnection");
        }

        public DataTable GetActiveUsers(string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@OrganizationId", Organizationid);
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_get_master_User", connectionstring);
            return dt;
        }

      

        public DataTable AddUserDetails(User obj)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
            cmd.Parameters.AddWithValue("@Organizationid", Convert.ToInt64(obj.OrganizationId));
            cmd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
            cmd.Parameters.AddWithValue("@EmployeeID", Convert.ToInt64(obj.EmployeeID));
            cmd.Parameters.AddWithValue("@UserId", obj.UserId);
            cmd.Parameters.AddWithValue("@Password", obj.Password);
            cmd.Parameters.AddWithValue("@RoleId", Convert.ToInt64(obj.RoleId));
            
            //cmd.Parameters.AddWithValue("@OrganizationId", OrganizationId);
            DataTable dt = new DataTable();
            dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_ins_master_User", connectionstring);
            return dt;
        }
        public DataTable UpdateUserDetails(User obj)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
            cmd.Parameters.AddWithValue("@Organizationid", Convert.ToInt64(obj.OrganizationId));
            cmd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
            cmd.Parameters.AddWithValue("@EmployeeID", Convert.ToInt64(obj.EmployeeID));
            cmd.Parameters.AddWithValue("@UserId", obj.UserId);
            cmd.Parameters.AddWithValue("@RoleId", Convert.ToInt64(obj.RoleId));
            cmd.Parameters.AddWithValue("@StatusId", Convert.ToInt64(obj.Status));
            
            DataTable dt = new DataTable();
            dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_upd_master_User", connectionstring);
            return dt;
        }

        public DataTable UpdateRecordStatus(string BranchId, string Organizationid, string TableName, string StatusID, string ID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@Organizationid", Convert.ToInt64(Organizationid));
            cmd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(BranchId));
            cmd.Parameters.AddWithValue("@TableName", TableName);
            cmd.Parameters.AddWithValue("@StatusID", Convert.ToInt64(StatusID));
            cmd.Parameters.AddWithValue("@ID", Convert.ToInt64(ID));
           

            DataTable dt = new DataTable();
            dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_upd_record_Status", connectionstring);
            return dt;
        }

    }


    public class Employee
    {

        string connectionstring = "";
        public Employee()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory) // Set the base path of your application
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
           .Build();

            TenantName = clsSingleton.TenantName;
            if (TenantName == "localhost")
            {
                TenantName = "dbo";
            }
            // Get the connection string
            connectionstring = configuration.GetConnectionString("DefaultConnection");
        }
        public string EmployeeID { get; set; }
        public string TenantName { get; set; }
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string IDProof { get; set; }
        public string DesignationId { get; set; }
        public string StatusId { get; set; }


        public DataTable GetMasterEmployee(string BranchId,string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            cmd.Parameters.AddWithValue("@BranchId",Convert.ToInt64(BranchId));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_get_master_Employees", connectionstring);
            return dt;
        }
        public DataTable GetMasterEmployeebyBranch(string BranchId, string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            cmd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(BranchId));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_get_master_Employees_by Branch", connectionstring);
            return dt;
        }
        public DataTable GetEmployeeDesignation()
        {
            SqlCommand cmd = new SqlCommand();
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_get_master_Designation", connectionstring);
            return dt;
        }
        public DataTable AddEmployeeDetails(Employee obj)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);	
            cmd.Parameters.AddWithValue("@OrganizationId",Convert.ToInt64(obj.OrganizationId));
            cmd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
            cmd.Parameters.AddWithValue("@Name", obj.Name);
            cmd.Parameters.AddWithValue("@Gender", obj.Gender);
            cmd.Parameters.AddWithValue("@Age", Convert.ToInt64(obj.Age));
            cmd.Parameters.AddWithValue("@Address", obj.Address);
            cmd.Parameters.AddWithValue("@ContactNo", obj.ContactNo);
            cmd.Parameters.AddWithValue("@Email", obj.Email);
            cmd.Parameters.AddWithValue("@IDProof", obj.IDProof);
            cmd.Parameters.AddWithValue("@DesignationId", Convert.ToInt64(obj.DesignationId));
            DataTable dt = new DataTable();
            if (Convert.ToInt64(obj.EmployeeID) > 0)
            {
                cmd.Parameters.AddWithValue("@EmployeeID", Convert.ToInt64(obj.EmployeeID));
                cmd.Parameters.AddWithValue("@StatusId", Convert.ToInt64(obj.StatusId));
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_upd_master_Employees", connectionstring);
            }
            else
            {
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_ins_master_Employees", connectionstring);
            }
            return dt;
        }

    }
}
