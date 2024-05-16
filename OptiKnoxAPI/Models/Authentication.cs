using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using System.Net;
namespace OptiKnoxAPI.Models
{
    public class Authentication
    {
        private readonly IConfiguration _configuration;
        string connectionstring = " ";
        string TenantName = "";
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string logincount { get; set; }

        public Authentication()
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
            //connectionstring += $";Initial Catalog={clsSingleton.TenantName}";
           

        }

        public DataSet ValidateLogin()
        {
            DataSet Ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@UserId", UserName);
            cmd.Parameters.AddWithValue("@Password", Password);
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_validate_Userlogin", connectionstring);
            Ds.Tables.Add(dt);
            //if (Convert.ToInt64(dt.Rows[0]["Status"]) > 0)
            //{
            //    SqlCommand cmd1 = new SqlCommand();
            //    cmd1.Parameters.AddWithValue("@UserId", UserName);
            //    DataTable dt1 = SQLHelper.ExecuteAdapterTable(cmd1, CommandType.StoredProcedure, "Sp_upd_audit_login", connectionstring);
            //    Ds.Tables.Add(dt1);
            //}
            return Ds;
        }
        public DataTable GetUserDetails()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@UserId", UserName);
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_get_Userdetails", connectionstring);
            return dt;
        }

        public DataTable GetRecentLogins()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@UserId", UserName);
            cmd.Parameters.AddWithValue("@LoginCount", logincount);
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "sp_get_audit_Logins", connectionstring);
            return dt;
        }
        public DataTable CheckUser()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@UserId", UserName);
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_get_Checkuser", connectionstring);
            return dt;
        }

        public DataTable CheckOrganition(string OrganizationName, string ContactName, string ContactNo, string UserName, string Email)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@OrganizationName", OrganizationName);
            cmd.Parameters.AddWithValue("@ContactName", ContactName);
            cmd.Parameters.AddWithValue("@ContactNo", ContactNo);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@Email", Email);
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_validate_OrgSignup", connectionstring);
            return dt;
        }


        public DataTable Generatepassword()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@UserId", UserName);
            cmd.Parameters.AddWithValue("@NewPassword", NewPassword);
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_upd_master_password", connectionstring);           
            return dt;
        }

        public DataTable GetSubscriptions(string OrgId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@Organizationid", OrgId);
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_get_Subscriptions", connectionstring);
            return dt;
        }


        public async Task<string> SendEmailAsync()
        {
            string Mess = "";
            string fromAddress = "hr@cryoknox.com";
            string toAddress = "chinnahanumanthu8333@gmail.com";
            string subject = "Test Email";
            string body = "This is a test email.";

            string smtpServer = "smtpout.secureserver.net"; // Replace with your SMTP server address
            int port = 465; // Replace with your SMTP port number
            string usernameval = "hr@cryoknox.com"; // Replace with your email username
            string passwordval = "Sayani121"; // Replace with your email password

            try
            {
                using (var client = new SmtpClient(smtpServer, port))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(usernameval, passwordval);
                    client.EnableSsl = true;

                    var message = new MailMessage(fromAddress, toAddress, subject, body);
                    message.IsBodyHtml = true;

                    await client.SendMailAsync(message);

                    //Console.WriteLine("Email sent successfully!");
                    return "Email sent successfully!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return $"Error sending email: {ex.Message}";
            }
        }
    }
    public class Organzation
    {

        string connectionstring = ""; 
        public Organzation()
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
        public string TenantName { get; set; }
        public string OrganizationName { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public DataTable SignupOrganization(Organzation obj)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
             cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
             cmd.Parameters.AddWithValue("@OrganizationName", obj.OrganizationName);
             cmd.Parameters.AddWithValue("@ContactName", obj.ContactName);
             cmd.Parameters.AddWithValue("@ContactNo", obj.ContactNumber);
             cmd.Parameters.AddWithValue("@UserName", obj.UserName);
             cmd.Parameters.AddWithValue("@Password", obj.Password);
             cmd.Parameters.AddWithValue("@Email", obj.Email);
            dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_ins_OrgSignup", connectionstring);
            return dt;
        }
    }

    public class EmailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromAddress { get; set; }

      
    }
}
