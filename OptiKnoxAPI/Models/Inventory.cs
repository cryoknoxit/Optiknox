using Microsoft.Data.SqlClient;
using OptiKnoxAPI.Models;
using System.Data;
using System.Globalization;
using System.Net;
using System.Reflection;

namespace OptiKnoxAPI.Models
{
    public class Inventory
    {
       
    }
    public class Customer
    {
        public string TenantName { get; set; }
        public string CustomerID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string DOB { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string BranchId { get; set; }
        public string OrganizationId { get; set; }
        public string StatusId { get; set; }

        private readonly IConfiguration _configuration;
        string connectionstring = "";

        public Customer()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory) // Set the base path of your application
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
           .Build();

            // Get the connection string
            connectionstring = configuration.GetConnectionString("DefaultConnection");
        }
        public DataTable GetCustomers(string OrganizationId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@OrganizationId", OrganizationId);
            cmd.Parameters.AddWithValue("@OrganizationId", OrganizationId);
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName+"."+"sp_get_master_Customers", connectionstring);
            return dt;
        }

        public DataTable CreateCustomers(Customer obj)
        {

            

            SqlCommand cmd = new SqlCommand();

            cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", obj.OrganizationId);
            cmd.Parameters.AddWithValue("@BranchId", obj.BranchId);
            cmd.Parameters.AddWithValue("@CustomerName", obj.Name);
            cmd.Parameters.AddWithValue("@Age", obj.Age);
            cmd.Parameters.AddWithValue("@Gender", obj.Gender);
            //cmd.Parameters.AddWithValue("@DOB", Convert.ToDateTime(DateTime.ParseExact(obj.DOB, "dd-MM-yyyy", CultureInfo.InvariantCulture)));
            cmd.Parameters.AddWithValue("@Address", obj.Address);
            cmd.Parameters.AddWithValue("@ContactNo", obj.ContactNo);
            cmd.Parameters.AddWithValue("@Email", obj.Email);
            DataTable dt = new DataTable();
            if (Convert.ToInt64(obj.CustomerID) > 0)
            {
                cmd.Parameters.AddWithValue("@CustomerId", obj.CustomerID);
                cmd.Parameters.AddWithValue("@StatusID",Convert.ToInt64(obj.StatusId));
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_upd_master_Customer", connectionstring);
            }
            else
            {
                 dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_master_Customer", connectionstring);
            }
           
            return dt;
        }
    }



    public class Vendar
    {

        public string VendorId { get; set; }
        public string TenantName { get; set; }
        public string OrganizationId { get; set; }
        public string VendorName { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactNo { get; set; }
        public string EmailId { get; set; }
        public string REGDNO { get; set; }
        public string GSTIN { get; set; }
        public string StatusId { get; set; }

        private readonly IConfiguration _configuration;
        string connectionstring = "";

        public Vendar()
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
        public DataTable GetVenders(string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_Vendor", connectionstring);
            return dt;
        }

        public DataTable CreateVendar(Vendar obj)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(obj.OrganizationId));
            cmd.Parameters.AddWithValue("@VendorName", obj.VendorName);
            cmd.Parameters.AddWithValue("@Address", obj.Address);
            cmd.Parameters.AddWithValue("@ContactName", obj.ContactName);
            cmd.Parameters.AddWithValue("@ContactNo", obj.ContactNo);
            cmd.Parameters.AddWithValue("@EmailId", obj.EmailId);
            cmd.Parameters.AddWithValue("@REGDNO", obj.REGDNO);
            cmd.Parameters.AddWithValue("@GSTIN", obj.GSTIN);


            DataTable dt = new DataTable();
            if (Convert.ToInt64(obj.VendorId) > 0)
            {
                cmd.Parameters.AddWithValue("@VendorID", obj.VendorId);
                cmd.Parameters.AddWithValue("@StatusID", Convert.ToInt64(obj.StatusId));
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_upd_master_Vendor", connectionstring);
            }
            else
            {
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_master_Vendor", connectionstring);
            }
            return dt;
        }
        public DataTable Vendors(string BranchId, string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            //cmd.Parameters.AddWithValue("@OrganizationId", OrganizationId);
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_Vendors", connectionstring);
            return dt;
        }

        public DataTable ProductBrand(string BranchId, string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_ProductBrand", connectionstring);
            return dt;
        }

        public DataTable ProductCategory(string BranchId, string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_ProductCategory", connectionstring);
            return dt;
        }

        public DataTable Customers(string Organizationid)
        {
         
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_Customers", connectionstring);
            return dt;
        }

        public DataTable ProductBrandByCategory(string CategoryID, string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@CategoryId", Convert.ToInt64(CategoryID));
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_ProductBrand_ByCategory", connectionstring);
            return dt;
        }

    }


    public class Product
    {
        string connectionstring = "";
        
        public string ProductId { get; set; }
        public string TenantName { get; set; }
        public string Organizationid { get; set; }
        public string CategoryId { get; set; }
        public string BrandID { get; set; }
        public string ProductName { get; set; }
        public string SIZE { get; set; }
        public string Colorcode { get; set; }
        public string ProductDescription { get; set; }
        public string StatusId { get; set; }

        

        public Product()
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
        public DataTable GetProducts(string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_Product", connectionstring);
            return dt;
        }

        public DataTable GetProductsByCategforyBrand(string Organizationid, string CategoryId, string BrandId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            cmd.Parameters.AddWithValue("@CategoryId", Convert.ToInt64(CategoryId));
            cmd.Parameters.AddWithValue("@BrandID", Convert.ToInt64(BrandId));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_Product_ByBrandCategory", connectionstring);
            return dt;
        }

        public DataTable GetProductsSize(string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_ProductSize", connectionstring);
            return dt;
        }

        public DataTable GetProductsColour(string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_ProductColor", connectionstring);
            return dt;
        }
        public DataTable GetPaymentTypes()
        {
            SqlCommand cmd = new SqlCommand();
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_get_master_Payments", connectionstring);
            return dt;
        }
        public string CreateProduct(Product obj)
        {
            string Message = "";
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
            cmd.Parameters.AddWithValue("@Organizationid",Convert.ToInt64(obj.Organizationid));
            cmd.Parameters.AddWithValue("@CategoryId", Convert.ToInt64(obj.CategoryId));
            cmd.Parameters.AddWithValue("@BrandID", Convert.ToInt64(obj.BrandID));
            cmd.Parameters.AddWithValue("@ProductName", obj.ProductName);
            cmd.Parameters.AddWithValue("@SIZE", obj.SIZE);
            cmd.Parameters.AddWithValue("@Colorcode", obj.Colorcode);
            cmd.Parameters.AddWithValue("@ProductDescription", obj.ProductDescription);

            DataTable dt = new DataTable();
            if (Convert.ToInt64(obj.ProductId) > 0)
            {
                cmd.Parameters.AddWithValue("@ProductId", obj.ProductId);
                cmd.Parameters.AddWithValue("@StatusID", Convert.ToInt64(obj.StatusId));
                var res = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_upd_master_Product", connectionstring);
                if (res.Rows.Count > 0)
                {
                    Message = res.Rows[0]["Message"].ToString();
                }
                //outputValue = Convert.ToInt64(cmd.Parameters["@OutputParameter"].Value);
            }
            else
            {
                SqlParameter outputParameter = new SqlParameter("@ProductID", SqlDbType.Int);
                outputParameter.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParameter);
                long outputValue = 0;
                var res = SQLHelper.ExecuteNonQuery(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_master_Product", connectionstring);
                if (Convert.ToInt64(res) > 0) {
                    outputValue = Convert.ToInt64(cmd.Parameters["@ProductID"].Value);
                    Message = outputValue.ToString();
                    if (outputValue > 0)
                    {
                        Message = "Product Created Successfully";
                    }
                    else
                    {
                        Message = "Failed to Save";
                    }
                } 
                else
                {
                    Message = "Failed to Save";
                }
                
            }
            return Message;
        }


    }
    public class Brand
    {

        public string TenantName { get; set; }
        public string BrandName { get; set; }
        public string ProductCategoryID { get; set; }
        public string BranchId { get; set; }
        public string Organizationid { get; set; }
        public string StatusID { get; set; }

        public string ProductBrandID { get; set; }


        private readonly IConfiguration _configuration;
        string connectionstring = "";
       

        public Brand()
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

        public DataTable CreateBrand(Brand obj)
        {
            SqlCommand cmd = new SqlCommand();
            //           @TenantName Varchar(20),
            //@OrganizationId INT,
            //   @CategoryID INT,
            //@ProductBrandID INT,
            //   @BrandName Varchar(100),
            //@StatusID INT
            cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
            cmd.Parameters.AddWithValue("@Organizationid", Convert.ToInt64(obj.Organizationid));
            cmd.Parameters.AddWithValue("@ProductCategoryID", Convert.ToInt64(obj.ProductCategoryID));
            cmd.Parameters.AddWithValue("@BrandName", obj.BrandName);
            
            DataTable dt = new DataTable();
            if (Convert.ToInt64(obj.ProductBrandID) > 0)
            {
              
                cmd.Parameters.AddWithValue("@ProductBrandID", Convert.ToInt64(obj.ProductBrandID));
                cmd.Parameters.AddWithValue("@StatusID", Convert.ToInt64(obj.StatusID));
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_upd_master_ProductBrand", connectionstring);
            }
            else
            {
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_master_ProductBrand", connectionstring);
            }

            return dt;
        }

    }
}

public class Category
    {

        public string TenantName { get; set; }
        public string ProductCategoryID { get; set; }
        public string  CategoryName { get; set; }
        public string  GSTPercentage { get; set; }
        public string  BranchId { get; set; }
        public string  Organizationid { get; set; }

        public string StatusId { get; set; }

        private readonly IConfiguration _configuration;
        string connectionstring = "";

        public Category()
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

        public DataTable CreateBrand(Category obj)
        {
            SqlCommand cmd = new SqlCommand();

            //@TenantName Varchar(20),
            //@OrganizationId INT,
            //@CategoryName Varchar(100),
            //@GSTPercentage decimal

            cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
            cmd.Parameters.AddWithValue("@Organizationid", Convert.ToInt64(obj.Organizationid));
            cmd.Parameters.AddWithValue("@CategoryName", obj.CategoryName);
            cmd.Parameters.AddWithValue("@GSTPercentage", obj.GSTPercentage);
           
            DataTable dt = new DataTable();
            if (Convert.ToInt64(obj.ProductCategoryID) > 0)
            {
                cmd.Parameters.AddWithValue("@CategoryID", Convert.ToInt64(obj.ProductCategoryID));
                cmd.Parameters.AddWithValue("@StatusID", Convert.ToInt64(obj.StatusId));
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_upd_master_ProductCategory", connectionstring);
            }
            else
            {
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_master_ProductCategory", connectionstring);
            }
             
            return dt;
        }

}
