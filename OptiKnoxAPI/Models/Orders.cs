using Microsoft.AspNetCore.Components.Web;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OptiKnoxAPI.Models
{
 
    public class PurchaseOrder
    {
        string connectionstring = "";public long PurchaseId = 0;string Message = "";
        public PurchaseOrder()
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
        
        public string PurchaseID { get; set; }
        public string PurchaseDetailsID { get; set; }
        
        public string TenantName { get; set; }
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string BillNo { get; set; }
        public string VendorId { get; set; }
        public string InvoiceAmount { get; set; }
        public string GSTAmount { get; set; }
        public string InvoiceNetAmount { get; set; }
        public string PaidAmount { get; set; }
        public string PaymentTypeiD { get; set; }
        public string PaymentReference { get; set; }
        public string userId { get; set; }
        public List<Products> Productslist { get; set; }

        public DataTable GetPurchaseOrders(string BranchId, string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            cmd.Parameters.AddWithValue("@BranchID", Convert.ToInt64(BranchId));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_PurchaseInvoice", connectionstring);
            return dt;
        }

        public DataTable GetInventeryStock(string BranchId, string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@BranchID", Convert.ToInt64(BranchId));
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_Inventory_Stock", connectionstring);
            return dt;
        }

        public DataTable GetProductDetasilsbyBarcode(string BranchId, string Organizationid,string ProductCode)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@BranchID", Convert.ToInt64(BranchId));
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            cmd.Parameters.AddWithValue("@ProductCode", ProductCode.ToString());
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_Search_Product_by_Barcode", connectionstring);
            return dt;
        }
        public DataSet GetPurchaseInvoice(string BranchId, string Organizationid, string PurchaseId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@BranchID", Convert.ToInt64(BranchId));
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            cmd.Parameters.AddWithValue("@PurchaseID", Convert.ToInt64(PurchaseId));
            DataSet ds = SQLHelper.ExecuteAdapter(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_details_PurchaseInvoiceDetails", connectionstring);
            return ds;
        }

        public DataTable GetPurchaseInventoryBarcodes(string BranchId, string Organizationid, string PurchaseId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@BranchID", Convert.ToInt64(BranchId));
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            cmd.Parameters.AddWithValue("@PurchaseID", Convert.ToInt64(PurchaseId));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_BarcodeDetails", connectionstring);
            return dt;
        }
        public DataTable UpdateInventoryStockStatus(string BranchId, string Organizationid, string InventoryID, string Status)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@BranchID", Convert.ToInt64(BranchId));
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            cmd.Parameters.AddWithValue("@InventoryID", Convert.ToInt64(InventoryID));
            cmd.Parameters.AddWithValue("@Status", Convert.ToInt64(Status));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_Inventory_stock_status", connectionstring);
            return dt;
        }
        public string SavePurchaseOrders(PurchaseOrder obj)
        {
            bool IsSaveSuccess = false;
            try
            {
                string Messge = "";
                bool IsSaveInvoice = SaveInvoiceDertails(obj);
                if (IsSaveInvoice && Message == "Invoice Details Save Success")
                {
                    bool IsSaveProdutList = SaveProductList(obj);
                    if (IsSaveProdutList && Message == "Invoice Details Save Success")
                    {
                        if (Convert.ToDouble(obj.PaidAmount) > 0)
                        {
                            bool IsPaymentSave = SavePaymentDetails(obj);
                        }
                        Message = PurchaseId + "_Save Success";
                    }
                    return Message;
                }
                else
                {
                    return Message;
                }
                //if (IsSaveSuccess)
                //{
                //    Messge = PurchaseID + "_Save Success";
                //}
                
            }
            catch (Exception e)
            {
                IsSaveSuccess = false;
                return e.Message.ToString();
            }
        }
        public bool SaveInvoiceDertails(PurchaseOrder obj)
        {
            SqlCommand cmdProd = new SqlCommand();
            SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();bool isSave = false;
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
                cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(obj.OrganizationId));
                cmd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
                cmd.Parameters.AddWithValue("@InvoiceNo", Convert.ToInt64(obj.InvoiceNo));
                cmd.Parameters.AddWithValue("@InvoiceDate", Convert.ToDateTime(DateTime.ParseExact(obj.InvoiceDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)));
                //cmd.Parameters.AddWithValue("@InvoiceDate", Convert.ToDateTime(obj.InvoiceDate));
                cmd.Parameters.AddWithValue("@BillNo", obj.BillNo);
                cmd.Parameters.AddWithValue("@VendorId", Convert.ToInt64(obj.VendorId));
                cmd.Parameters.AddWithValue("@InvoiceAmount", Convert.ToDouble(obj.InvoiceAmount));
                cmd.Parameters.AddWithValue("@GSTAmount", Convert.ToDouble(obj.GSTAmount));
                cmd.Parameters.AddWithValue("@InvoiceNetAmount", Convert.ToDouble(obj.InvoiceNetAmount));
                cmd.Parameters.AddWithValue("@PaidAmount", Convert.ToDouble(obj.PaidAmount));
                cmd.Parameters.AddWithValue("@userId", Convert.ToInt64(obj.userId));
                DataTable dtInv = new DataTable();
                if (Convert.ToInt64(obj.PurchaseID) > 0)
                {
                    cmd.Parameters.AddWithValue("@PurchaseID", Convert.ToDouble(obj.PurchaseID));
                    cmd.Parameters.AddWithValue("@StatusID", Convert.ToInt64(1));
                    dtInv = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_upd_master_PurchaseInvoice", connectionstring);
                    object messageValue = dtInv.Rows[0]["Message"];
                    // Check if the value is a string
                    if (messageValue is string)
                    {
                        Message = dtInv.Rows[0]["Message"].ToString();
                        if (Message == "Invoice Details Save Success" || Message == "Order Updated Successfully")
                            isSave = true;
                        else
                            isSave = false;
                    }
                    // Check if the value is a number (int, float, double, etc.)
                    else if (messageValue is int)
                    {
                        PurchaseId = Convert.ToInt64(dtInv.Rows[0]["Message"]);
                        Message = "Order Updated Successfully";
                        transaction.Commit();
                        isSave = true;
                    }
                }
                else
                {
                    dtInv = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_master_PurchaseInvoice", connectionstring);
                    object messageValue = dtInv.Rows[0]["Message"];
                    // Check if the value is a string
                    if (messageValue is string)
                    {

                        if (dtInv.Rows[0]["Message"].ToString() != "Invoice Number Already Exists for this Customer")
                        {
                            Message = "Invoice Details Save Success";
                            PurchaseId = Convert.ToInt64(dtInv.Rows[0]["Message"]);
                            isSave = true;
                        }
                        else
                        {
                            Message = dtInv.Rows[0]["Message"].ToString();
                            isSave = false;
                        }

                    }
                    // Check if the value is a number (int, float, double, etc.)
                    else if (messageValue is int)
                    {

                        PurchaseId = Convert.ToInt64(dtInv.Rows[0]["Message"]);
                        Message = "Invoice Details Save Success";
                        transaction.Commit();
                        isSave = true;
                    }
                }

                return isSave;
                //if (dtInv.Rows[0]["Message"].ToString()== "Invoice Number Already Exists for this vendor")
                //{
                //    Message = dtInv.Rows[0]["Message"].ToString();
                //    return false;
                //}
                //else if(Convert.ToInt64(dtInv.Rows[0]["Message"]) > 0)
                //{
                //    Message = "Invoice Details Save Success";
                //    PurchaseId = Convert.ToInt64(dtInv.Rows[0]["Message"]);
                //    transaction.Commit();
                //    return true;
                //}
                //else
                //{
                //    Message = dtInv.Rows[0]["Message"].ToString();
                //    return false;
                //}
            }
            catch(Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                }
                return false;
            }
        }
        
        public bool SaveProductList(PurchaseOrder obj)
        {
            try
            {
                DataSet dtProdctlist = new DataSet();
                for (int i = 0; i < obj.Productslist.Count; i++)
                {
                    SqlCommand cmdProd = new SqlCommand();
                    SqlConnection connection = new SqlConnection(connectionstring);
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    try
                    {
                        cmdProd.Parameters.AddWithValue("@TenantName", TenantName);
                        cmdProd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(obj.OrganizationId));
                        cmdProd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
                        cmdProd.Parameters.AddWithValue("@PurchaseID", Convert.ToInt64(PurchaseId));
                        cmdProd.Parameters.AddWithValue("@CategoryID", Convert.ToInt64(obj.Productslist[i].CategoryId));
                        cmdProd.Parameters.AddWithValue("@BrandID", Convert.ToInt64(obj.Productslist[i].BrandId));
                        cmdProd.Parameters.AddWithValue("@Size", obj.Productslist[i].Size);
                        cmdProd.Parameters.AddWithValue("@Color", obj.Productslist[i].Colour);
                        cmdProd.Parameters.AddWithValue("@ProductName", obj.Productslist[i].Productname);
                        cmdProd.Parameters.AddWithValue("@Description", obj.Productslist[i].Description);
                        cmdProd.Parameters.AddWithValue("@Qty", Convert.ToInt64(obj.Productslist[i].Qty));
                        cmdProd.Parameters.AddWithValue("@PurchasePrice", Convert.ToDouble(obj.Productslist[i].PurchasePriceAmount));
                        cmdProd.Parameters.AddWithValue("@GSTAmount", Convert.ToDouble(obj.Productslist[i].GSTAmount));
                        cmdProd.Parameters.AddWithValue("@MRP", Convert.ToDouble(obj.Productslist[i].MRPAmount));
                        cmdProd.Parameters.AddWithValue("@SalePrice", Convert.ToDouble(0));
                        cmdProd.Parameters.AddWithValue("@TotalAmount", Convert.ToDouble(obj.Productslist[i].TotalAmount));
                        if (Convert.ToInt64(obj.PurchaseDetailsID) > 0)
                        {
                            cmdProd.Parameters.AddWithValue("@PurchaseDetailsID", Convert.ToInt64(obj.PurchaseDetailsID));
                            cmdProd.Parameters.AddWithValue("@StatusId", Convert.ToInt64(1));
                            dtProdctlist = SQLHelper.ExecuteAdapter(cmdProd, CommandType.StoredProcedure, TenantName + "." + "sp_upd_details_PurchaseInvoice", connectionstring);
                        }
                        else
                        {
                            dtProdctlist = SQLHelper.ExecuteAdapter(cmdProd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_details_PurchaseInvoice", connectionstring);
                        }
                        
                        transaction.Commit();
                        
                    
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                        }
                        return false;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                if (dtProdctlist.Tables.Count > 0)
                {
                    Message = "Invoice Details Save Success";
                    return true;
                }
                else
                {
                    Message = "Faild To Save";
                    return false;
                }
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool SavePaymentDetails(PurchaseOrder obj)
        {
            try
            {
                if (Convert.ToDouble(obj.PaidAmount) > 0)
                {
                    SqlCommand cmdpayment = new SqlCommand();
                    cmdpayment.Parameters.AddWithValue("@TenantName", obj.TenantName);
                    cmdpayment.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(obj.OrganizationId));
                    cmdpayment.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
                    cmdpayment.Parameters.AddWithValue("@PurchaseID", Convert.ToInt64(PurchaseId));
                    cmdpayment.Parameters.AddWithValue("@PurchaseAmount", Convert.ToDouble(obj.PaidAmount));
                    cmdpayment.Parameters.AddWithValue("@PaymentTypeiD", Convert.ToInt64(1));
                    cmdpayment.Parameters.AddWithValue("@PaymentReference", obj.PaymentReference);
                    cmdpayment.Parameters.AddWithValue("@UserID", Convert.ToInt64(obj.userId));
                    DataTable dt = SQLHelper.ExecuteAdapterTable(cmdpayment, CommandType.StoredProcedure, TenantName + "." + "sp_ins_Details_PaymentPurchaseInvoice", connectionstring);
                }
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }

    public class Products
    {
        public string Id{ get; set; }
        public string CategoryId{ get; set; }
        public string CategoryName{ get; set; }
        public string BrandId{ get; set; }
        public string BrandName{ get; set; }
        public string Size { get; set; }     
        public string Colour{ get; set; }
        public string PurchaseDetailsID{ get; set; }
        public string ProductID{ get; set; }
        public string Productname{ get; set; }
        public string Description{ get; set; } 
        public string GST{ get; set; }
        public string Qty{ get; set; }
        public string GSTAmount{ get; set; }
        public string MRPAmount{ get; set; }
        //public string SalePriceAmount{ get; set; }
        public string TotalAmount{ get; set; }
        public string PurchasePriceAmount{ get; set; }
        public string ProductAmoount{ get; set; }


        //public string CategoryID { get; set; }
        //public string BrandID { get; set; }
        ////public string Size { get; set; }
        //public string Color { get; set; }
        //public string ProductName { get; set; }
        //public string Description { get; set; }
        //public string Qty { get; set; }
        //public string PurchasePrice { get; set; }
        //public string GSTAmount { get; set; }
        //public string MRP { get; set; }
        //public string SalePrice { get; set; }
        //public string TotalAmount { get; set; }

    }

    public class EyeTesting
    {

        string connectionstring = "";
        public EyeTesting()
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

        public string CaseID { get; set; }
        public string TenantName { get; set; }
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string CaseDate { get; set; }
        public string CustomerID { get; set; }
        public string ReferedDoctor { get; set; }
        public string IPD { get; set; }
        public string TypeOFLens { get; set; }
        public string InstructiontoPatient { get; set; }
        public string InstructiontoOptician { get; set; }
        public string Attendent { get; set; }
        public string TotalAmount { get; set; }
        public string GST { get; set; }
        public string NetAmount { get; set; }
        public string PaymentStatusID { get; set; }
        public string CreatedBy { get; set; }
        
        
        public DataTable GetEyeTestingMasterData(string BranchId, string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@BranchID", Convert.ToInt64(BranchId));
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_EyeTesting_Invoice", connectionstring);
            return dt;
        }

        public DataTable GetEyeTestingInvoiceData(string BranchId, string Organizationid,string CaseId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@BranchID", Convert.ToInt64(BranchId));
            cmd.Parameters.AddWithValue("@CaseID", Convert.ToInt64(CaseId));
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_details_EyeTesting_Prescription", connectionstring);
            return dt;
        }
        public DataTable SaveEyeTesting(EyeTesting obj)
        {
            DataTable dt = new DataTable();
            SqlCommand cmdProd = new SqlCommand();
            SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
                cmd.Parameters.AddWithValue("@OrganizationId",Convert.ToInt64(obj.OrganizationId));
                cmd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
                cmd.Parameters.AddWithValue("@CaseDate", Convert.ToDateTime(DateTime.ParseExact(obj.CaseDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)));
                cmd.Parameters.AddWithValue("@CustomerID", Convert.ToInt64(obj.CustomerID));
                cmd.Parameters.AddWithValue("@ReferedDoctor", obj.ReferedDoctor);
                cmd.Parameters.AddWithValue("@IPD", obj.IPD);
                cmd.Parameters.AddWithValue("@TypeOFLens", obj.TypeOFLens);
                cmd.Parameters.AddWithValue("@InstructiontoPatient", obj.InstructiontoPatient);
                cmd.Parameters.AddWithValue("@InstructiontoOptician", obj.InstructiontoOptician);
                cmd.Parameters.AddWithValue("@Attendent", obj.Attendent);
                cmd.Parameters.AddWithValue("@TotalAmount",Convert.ToDecimal(obj.TotalAmount));
                cmd.Parameters.AddWithValue("@GST", Convert.ToDecimal(obj.GST));
                cmd.Parameters.AddWithValue("@NetAmount", Convert.ToDecimal(obj.NetAmount));
                cmd.Parameters.AddWithValue("@PaymentStatusID",Convert.ToInt64(obj.PaymentStatusID));
                cmd.Parameters.AddWithValue("@CreatedBy", Convert.ToInt64(obj.CreatedBy));
                if (Convert.ToInt64(obj.CaseID) > 0) {
                    cmd.Parameters.AddWithValue("@CaseID", obj.CaseID);
                    cmd.Parameters.AddWithValue("@StatusID", Convert.ToInt64(1));
                    dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_upd_master_EyeTesting", connectionstring);
                }
                else
                {
                    dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_master_EyeTesting", connectionstring);
                }
                transaction.Commit();
                return dt;
            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                }
                return dt;
            }
        }

    }
    public class EyeTestingPrescription
    {

        string connectionstring = "";
        public EyeTestingPrescription()
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
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string ID { get; set; }
        public string Caseid { get; set; }
        public string PrescriptionTypeID { get; set; }
        
        public string DV_SPH1 { get; set; }
        public string DV_CYL1 { get; set; }
        public string DV_AXIS1 { get; set; }
        public string DV_VA1 { get; set; }
        public string NV_SPH1 { get; set; }
        public string NV_CYL1 { get; set; }
        public string NV_AXIS1 { get; set; }
        public string NV_VA1 { get; set; }
        public string DV_SPH { get; set; }
        public string ADD1 { get; set; }
        public string RPD { get; set; }
        public string DV_SPH2 { get; set; }
        public string DV_CYL2 { get; set; }
        public string DV_AXIS2 { get; set; }
        public string DV_VA2 { get; set; }
        public string NV_SPH2 { get; set; }
        public string NV_CYL2 { get; set; }
        public string NV_AXIS2 { get; set; }
        public string NV_VA2 { get; set; }
        public string ADD2 { get; set; }
        public string LPD { get; set; }


        public DataTable SavePrescription(EyeTestingPrescription obj)
        {
            DataTable dt = new DataTable();
            SqlCommand cmdProd = new SqlCommand();
            SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
                cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(obj.OrganizationId));
                cmd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
                cmd.Parameters.AddWithValue("@Caseid", Convert.ToInt64(obj.Caseid));
                cmd.Parameters.AddWithValue("@PrescriptionTypeID", Convert.ToInt64(obj.PrescriptionTypeID));
                cmd.Parameters.AddWithValue("@DV_SPH1",Convert.ToDouble(obj.DV_SPH1));
                cmd.Parameters.AddWithValue("@DV_CYL1", Convert.ToDouble(obj.DV_CYL1));
                cmd.Parameters.AddWithValue("@DV_AXIS1", Convert.ToDouble(obj.DV_AXIS1));
                cmd.Parameters.AddWithValue("@DV_VA1", Convert.ToDouble(obj.DV_VA1));
                cmd.Parameters.AddWithValue("@NV_SPH1", Convert.ToDouble(obj.NV_SPH1));
                cmd.Parameters.AddWithValue("@NV_CYL1", Convert.ToDouble(obj.NV_CYL1));
                cmd.Parameters.AddWithValue("@NV_AXIS1", Convert.ToDouble(obj.NV_AXIS1));
                cmd.Parameters.AddWithValue("@NV_VA1", Convert.ToDouble(obj.NV_VA1));
                cmd.Parameters.AddWithValue("@DV_SPH", Convert.ToDouble(obj.DV_SPH1));
                cmd.Parameters.AddWithValue("@ADD1", Convert.ToDouble(obj.ADD1));
                cmd.Parameters.AddWithValue("@RPD", Convert.ToDouble(obj.RPD));
                cmd.Parameters.AddWithValue("@DV_SPH2", Convert.ToDouble(obj.DV_SPH2));
                cmd.Parameters.AddWithValue("@DV_CYL2", Convert.ToDouble(obj.DV_CYL2));
                cmd.Parameters.AddWithValue("@DV_AXIS2", Convert.ToDouble(obj.DV_AXIS2));
                cmd.Parameters.AddWithValue("@DV_VA2", Convert.ToDouble(obj.DV_VA2));
                cmd.Parameters.AddWithValue("@NV_SPH2", Convert.ToDouble(obj.NV_SPH2));
                cmd.Parameters.AddWithValue("@NV_CYL2", Convert.ToDouble(obj.NV_CYL2));
                cmd.Parameters.AddWithValue("@NV_AXIS2", Convert.ToDouble(obj.NV_AXIS2));
                cmd.Parameters.AddWithValue("@NV_VA2", Convert.ToDouble(obj.NV_VA2));
                cmd.Parameters.AddWithValue("@ADD2", Convert.ToDouble(obj.ADD2));
                cmd.Parameters.AddWithValue("@LPD", Convert.ToDouble(obj.LPD));
                if (Convert.ToInt64(obj.ID) > 0)
                {
                    cmd.Parameters.AddWithValue("@ID", Convert.ToDouble(obj.ID));
                    cmd.Parameters.AddWithValue("@StatusID", Convert.ToDouble(1));
                    dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_upd_Details_EyeTesting_Prescription", connectionstring);
                }
                else
                {
                    dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_Details_EyeTesting_Prescription", connectionstring);
                }
                transaction.Commit();
                return dt;
               
            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                }
                return dt;
            }
        }

    }

    public class EyeTestingContactLensPrescriptionPrescription
    {

        string connectionstring = "";
        public EyeTestingContactLensPrescriptionPrescription()
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
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string Caseid { get; set; }
        public string PrescriptionTypeID { get; set; }

        public string CL_DV_SPH1 { get; set; }
        public string CL_DV_CYL1 { get; set; }
        public string CL_DV_AXIS1 { get; set; }
        public string CL_DV_VA1 { get; set; }
        public string CL_NV_SPH1 { get; set; }
        public string CL_NV_CYL1 { get; set; }
        public string CL_NV_AXIS1 { get; set; }
        public string CL_NV_VA1 { get; set; }  
        public string CL_ADD1 { get; set; }
        public string CL_RPD { get; set; }
        public string CL_DV_SPH2 { get; set; }
        public string CL_DV_CYL2 { get; set; }
        public string CL_DV_AXIS2 { get; set; }
        public string CL_DV_VA2 { get; set; }
        public string CL_NV_SPH2 { get; set; }
        public string CL_NV_CYL2 { get; set; }
        public string CL_NV_AXIS2 { get; set; }
        public string CL_NV_VA2 { get; set; }
        public string CL_ADD2 { get; set; }
        public string CL_LPD { get; set; }


        public DataTable SavePrescription(EyeTestingContactLensPrescriptionPrescription obj)
        {
            DataTable dt = new DataTable();
            SqlCommand cmdProd = new SqlCommand();
            SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
                cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(obj.OrganizationId));
                cmd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
                cmd.Parameters.AddWithValue("@Caseid", Convert.ToInt64(obj.Caseid));
                cmd.Parameters.AddWithValue("@PrescriptionTypeID", Convert.ToInt64(obj.PrescriptionTypeID));
                cmd.Parameters.AddWithValue("@CL_DV_SPH1", Convert.ToDouble(obj.CL_DV_SPH1));
                cmd.Parameters.AddWithValue("@CL_DV_CYL1", Convert.ToDouble(obj.CL_DV_CYL1));
                cmd.Parameters.AddWithValue("@CL_DV_AXIS1", Convert.ToDouble(obj.CL_DV_AXIS1));
                cmd.Parameters.AddWithValue("@CL_DV_VA1", Convert.ToDouble(obj.CL_DV_VA1));
                cmd.Parameters.AddWithValue("@CL_NV_SPH1", Convert.ToDouble(obj.CL_NV_SPH1));
                cmd.Parameters.AddWithValue("@CL_NV_CYL1", Convert.ToDouble(obj.CL_NV_CYL1));
                cmd.Parameters.AddWithValue("@CL_NV_AXIS1", Convert.ToDouble(obj.CL_NV_AXIS1));
                cmd.Parameters.AddWithValue("@CL_NV_VA1", Convert.ToDouble(obj.CL_NV_VA1));
                cmd.Parameters.AddWithValue("@CL_DV_SPH", Convert.ToDouble(obj.CL_DV_SPH1));
                cmd.Parameters.AddWithValue("@CL_ADD1", Convert.ToDouble(obj.CL_ADD1));
                cmd.Parameters.AddWithValue("@CL_RPD", Convert.ToDouble(obj.CL_RPD));
                cmd.Parameters.AddWithValue("@CL_DV_SPH2", Convert.ToDouble(obj.CL_DV_SPH2));
                cmd.Parameters.AddWithValue("@CL_DV_CYL2", Convert.ToDouble(obj.CL_DV_CYL2));
                cmd.Parameters.AddWithValue("@CL_DV_AXIS2", Convert.ToDouble(obj.CL_DV_AXIS2));
                cmd.Parameters.AddWithValue("@CL_DV_VA2", Convert.ToDouble(obj.CL_DV_VA2));
                cmd.Parameters.AddWithValue("@CL_NV_SPH2", Convert.ToDouble(obj.CL_NV_SPH2));
                cmd.Parameters.AddWithValue("@CL_NV_CYL2", Convert.ToDouble(obj.CL_NV_CYL2));
                cmd.Parameters.AddWithValue("@CL_NV_AXIS2", Convert.ToDouble(obj.CL_NV_AXIS2));
                cmd.Parameters.AddWithValue("@CL_NV_VA2", Convert.ToDouble(obj.CL_NV_VA2));
                cmd.Parameters.AddWithValue("@CL_ADD2", Convert.ToDouble(obj.CL_ADD2));
                cmd.Parameters.AddWithValue("@CL_LPD", Convert.ToDouble(obj.CL_LPD));
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_Details_EyeTesting_Prescription", connectionstring);
                transaction.Commit();
                return dt;

            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                }
                return dt;
            }
        }

    }

    public class EyeTestingTransposePrescriptionPrescription
    {

        string connectionstring = "";
        public EyeTestingTransposePrescriptionPrescription()
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
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string Caseid { get; set; }
        public string PrescriptionTypeID { get; set; }

        public string TP_DV_SPH1 { get; set; }
        public string TP_DV_CYL1 { get; set; }
        public string TP_DV_AXIS1 { get; set; }
        public string TP_DV_VA1 { get; set; }
        public string TP_NV_SPH1 { get; set; }
        public string TP_NV_CYL1 { get; set; }
        public string TP_NV_AXIS1 { get; set; }
        public string TP_NV_VA1 { get; set; }
        public string TP_ADD1 { get; set; }
        public string TP_RPD { get; set; }
        public string TP_DV_SPH2 { get; set; }
        public string TP_DV_CYL2 { get; set; }
        public string TP_DV_AXIS2 { get; set; }
        public string TP_DV_VA2 { get; set; }
        public string TP_NV_SPH2 { get; set; }
        public string TP_NV_CYL2 { get; set; }
        public string TP_NV_AXIS2 { get; set; }
        public string TP_NV_VA2 { get; set; }
        public string TP_ADD2 { get; set; }
        public string TP_LPD { get; set; }


        public DataTable SavePrescription(EyeTestingTransposePrescriptionPrescription obj)
        {
            DataTable dt = new DataTable();
            SqlCommand cmdProd = new SqlCommand();
            SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
                cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(obj.OrganizationId));
                cmd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
                cmd.Parameters.AddWithValue("@Caseid", Convert.ToInt64(obj.Caseid));
                cmd.Parameters.AddWithValue("@PrescriptionTypeID", Convert.ToInt64(obj.PrescriptionTypeID));
                cmd.Parameters.AddWithValue("@TP_DV_SPH1", Convert.ToDouble(obj.TP_DV_SPH1));
                cmd.Parameters.AddWithValue("@TP_DV_CYL1", Convert.ToDouble(obj.TP_DV_CYL1));
                cmd.Parameters.AddWithValue("@TP_DV_AXIS1", Convert.ToDouble(obj.TP_DV_AXIS1));
                cmd.Parameters.AddWithValue("@TP_DV_VA1", Convert.ToDouble(obj.TP_DV_VA1));
                cmd.Parameters.AddWithValue("@TP_NV_SPH1", Convert.ToDouble(obj.TP_NV_SPH1));
                cmd.Parameters.AddWithValue("@TP_NV_CYL1", Convert.ToDouble(obj.TP_NV_CYL1));
                cmd.Parameters.AddWithValue("@TP_NV_AXIS1", Convert.ToDouble(obj.TP_NV_AXIS1));
                cmd.Parameters.AddWithValue("@TP_NV_VA1", Convert.ToDouble(obj.TP_NV_VA1));
                cmd.Parameters.AddWithValue("@TP_DV_SPH", Convert.ToDouble(obj.TP_DV_SPH1));
                cmd.Parameters.AddWithValue("@TP_ADD1", Convert.ToDouble(obj.TP_ADD1));
                cmd.Parameters.AddWithValue("@TP_RPD", Convert.ToDouble(obj.TP_RPD));
                cmd.Parameters.AddWithValue("@TP_DV_SPH2", Convert.ToDouble(obj.TP_DV_SPH2));
                cmd.Parameters.AddWithValue("@TP_DV_CYL2", Convert.ToDouble(obj.TP_DV_CYL2));
                cmd.Parameters.AddWithValue("@TP_DV_AXIS2", Convert.ToDouble(obj.TP_DV_AXIS2));
                cmd.Parameters.AddWithValue("@TP_DV_VA2", Convert.ToDouble(obj.TP_DV_VA2));
                cmd.Parameters.AddWithValue("@TP_NV_SPH2", Convert.ToDouble(obj.TP_NV_SPH2));
                cmd.Parameters.AddWithValue("@TP_NV_CYL2", Convert.ToDouble(obj.TP_NV_CYL2));
                cmd.Parameters.AddWithValue("@TP_NV_AXIS2", Convert.ToDouble(obj.TP_NV_AXIS2));
                cmd.Parameters.AddWithValue("@TP_NV_VA2", Convert.ToDouble(obj.TP_NV_VA2));
                cmd.Parameters.AddWithValue("@TP_ADD2", Convert.ToDouble(obj.TP_ADD2));
                cmd.Parameters.AddWithValue("@TP_LPD", Convert.ToDouble(obj.TP_LPD));
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_Details_EyeTesting_Prescription", connectionstring);
                transaction.Commit();
                return dt;

            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                }
                return dt;
            }
        }

    }
    public class EyeTestingPayment
    {

        string connectionstring = "";
        public EyeTestingPayment()
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
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string CaseID { get; set; }
        public string PaymentTypeID { get; set; }
        public string PaymentReference { get; set; }
        public string Amount { get; set; }
        public DataTable SaveEyeTestingPayment(EyeTestingPayment obj)
        {
            DataTable dt = new DataTable();
            SqlCommand cmdProd = new SqlCommand();
            SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
                cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(obj.OrganizationId));
                cmd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
                cmd.Parameters.AddWithValue("@CaseID",Convert.ToInt64(obj.CaseID));
                cmd.Parameters.AddWithValue("@PaymentTypeID", Convert.ToInt64(obj.PaymentTypeID));
                cmd.Parameters.AddWithValue("@PaymentReference", obj.PaymentReference);
                cmd.Parameters.AddWithValue("@Amount",Convert.ToDouble(obj.Amount));
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_details_Payment_EyeTesting", connectionstring);
                transaction.Commit();
                return dt;

            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                }
                return dt;
            }
        }
    }

    public class salesOrder
    {
        string connectionstring = ""; public long SalesResultID = 0; string Message = "";
        public salesOrder()
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
        public string SalesID { get; set; }
        public string SalesDetailsID { get; set; }
        
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string BillNo { get; set; }
        public string CustomerId { get; set; }
        public string InvoiceAmount { get; set; }
        public string GSTAmount { get; set; }
        public string InvoiceNetAmount { get; set; }
        public string PaidAmount { get; set; }
        public string BalanceAmount { get; set; }
        public string PaymentStatusID { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentType { get; set; }
        public string AttendedBy { get; set; }
        public string PaymentAmount { get; set; }
        public string PaymentTypeId { get; set; }
        public string PaymentReference { get; set; }
        public string StatusID { get; set; }
        public string Createdby { get; set; }
        



        public List<SalesProducts> Productslist { get; set; }


        public DataSet GetAllSalesInvoice(string BranchId, string Organizationid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@BranchID", Convert.ToInt64(BranchId));
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            DataSet ds = SQLHelper.ExecuteAdapter(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_master_SalesInvoice", connectionstring);
            return ds;
        }

        public DataTable GetOrderStatus()
        {
            SqlCommand cmd = new SqlCommand();
            DataTable dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, "manage.sp_get_master_OrderStatus", connectionstring);
            return dt;
        }
        public DataSet GetSalesInvoiceDetails(string BranchId, string Organizationid,string SalesId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@TenantName", TenantName);
            cmd.Parameters.AddWithValue("@BranchID", Convert.ToInt64(BranchId));
            cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(Organizationid));
            cmd.Parameters.AddWithValue("@SalesID", Convert.ToInt64(SalesId));
            DataSet ds = SQLHelper.ExecuteAdapter(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_get_details_SalesInvoice", connectionstring);
            return ds;
        }
        public string SavesalesOrder(salesOrder obj)
        {
            bool IsSaveSuccess = false;
            try
            {
                string Messge = "";
                bool IsSaveInvoice = SaveInvoiceMasterDertails(obj);
                if (IsSaveInvoice && Message == "Invoice Details Save Success" || Message == "Order Updated Successfully")
                {
                    bool IsSaveProdutList = SaveProductList(obj);
                    if (IsSaveProdutList && Message == "Invoice Details Save Success")
                    {
                        
                        if (Convert.ToDouble(obj.PaidAmount) > 0)
                        {
                            bool IsPaymentSave = SavePaymentDetails(obj);
                        }
                        Message = SalesResultID + "_Save Success";
                    }
                    return Message;
                }
                else
                {
                    return Message;
                }
                //if (IsSaveSuccess)
                //{
                //    Messge = PurchaseID + "_Save Success";
                //}

            }
            catch (Exception e)
            {
                IsSaveSuccess = false;
                return e.Message.ToString();
            }
        }
        public bool SaveInvoiceMasterDertails(salesOrder obj)
        {
            SqlCommand cmdProd = new SqlCommand();bool issave = false;
            SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
                cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(obj.OrganizationId));
                cmd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
                cmd.Parameters.AddWithValue("@InvoiceNo", Convert.ToInt64(obj.InvoiceNo));
                cmd.Parameters.AddWithValue("@InvoiceDate", Convert.ToDateTime(DateTime.ParseExact(obj.InvoiceDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)));
                //cmd.Parameters.AddWithValue("@InvoiceDate", obj.InvoiceDate);
                cmd.Parameters.AddWithValue("@BillNo", obj.BillNo);
                cmd.Parameters.AddWithValue("@CustomerId", Convert.ToInt64(obj.CustomerId));
                cmd.Parameters.AddWithValue("@InvoiceAmount", Convert.ToDouble(obj.InvoiceAmount));
                cmd.Parameters.AddWithValue("@GSTAmount", Convert.ToDouble(obj.GSTAmount));
                cmd.Parameters.AddWithValue("@InvoiceNetAmount", Convert.ToDouble(obj.InvoiceNetAmount));
                cmd.Parameters.AddWithValue("@PaidAmount", Convert.ToDouble(obj.PaidAmount));
                cmd.Parameters.AddWithValue("@userId", Convert.ToInt64(obj.Createdby));
                cmd.Parameters.AddWithValue("@OrderStatus", Convert.ToInt64(obj.OrderStatus));
                cmd.Parameters.AddWithValue("@PaymentType", Convert.ToInt64(obj.PaymentType));
                cmd.Parameters.AddWithValue("@AttendedBy", Convert.ToInt64(obj.AttendedBy));
                cmd.Parameters.AddWithValue("@PaymentReference", obj.PaymentReference);

                DataTable dtInv = new DataTable();
                if (Convert.ToInt64(obj.SalesID) > 0)
                {
                    cmd.Parameters.AddWithValue("@SalesID", Convert.ToInt64(obj.SalesID));
                    cmd.Parameters.AddWithValue("@StatusID", Convert.ToInt64(1));
                    dtInv = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_upd_master_SalesInvoice", connectionstring);
                   
                    object messageValue = dtInv.Rows[0]["Message"];
                    // Check if the value is a string
                    if (messageValue is string)
                    {
                        Message = dtInv.Rows[0]["Message"].ToString();
                        if(Message== "Invoice Details Save Success" || Message== "Order Updated Successfully")
                            issave = true;
                        else
                            issave = false;
                    }
                    // Check if the value is a number (int, float, double, etc.)
                    else if (messageValue is int)
                    {
                        SalesResultID = Convert.ToInt64(dtInv.Rows[0]["Message"]);
                        Message = "Order Updated Successfully";
                        transaction.Commit();
                        issave = true;
                    }
                }
                else
                {
                    dtInv = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_master_SalesInvoice", connectionstring);
                    object messageValue = dtInv.Rows[0]["Message"];
                    // Check if the value is a string
                    if (messageValue is string)
                    {
                      
                        if(dtInv.Rows[0]["Message"].ToString() != "Invoice Number Already Exists for this Customer")
                        {
                            Message = "Invoice Details Save Success";
                            SalesResultID = Convert.ToInt64(dtInv.Rows[0]["Message"]);
                            issave = true;
                        }
                        else
                        {
                            Message = dtInv.Rows[0]["Message"].ToString();
                            issave = false;
                        }
                       
                    }
                    // Check if the value is a number (int, float, double, etc.)
                    else if (messageValue is int)
                    {
                       
                        SalesResultID = Convert.ToInt64(dtInv.Rows[0]["Message"]);
                        Message = "Invoice Details Save Success";
                        transaction.Commit();
                        issave = true;
                    }
                }

                return issave;



            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                }
                return false;
            }
        }

        public bool SaveProductList(salesOrder obj)
        {
            try
            {
                DataSet dtProdctlist = new DataSet();
                for (int i = 0; i < obj.Productslist.Count; i++)
                {
                    SqlCommand cmdProd = new SqlCommand();
                    SqlConnection connection = new SqlConnection(connectionstring);
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    try
                    {
                        cmdProd.Parameters.AddWithValue("@TenantName", TenantName);
                        cmdProd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(obj.OrganizationId));
                        cmdProd.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
                        cmdProd.Parameters.AddWithValue("@SalesID", Convert.ToInt64(SalesResultID));
                        cmdProd.Parameters.AddWithValue("@CategoryID", Convert.ToInt64(obj.Productslist[i].CategoryId));
                        cmdProd.Parameters.AddWithValue("@BrandID", Convert.ToInt64(obj.Productslist[i].BrandId));
                        cmdProd.Parameters.AddWithValue("@Size", obj.Productslist[i].Size);
                        cmdProd.Parameters.AddWithValue("@Color", obj.Productslist[i].Colour);
                        cmdProd.Parameters.AddWithValue("@ProductName", obj.Productslist[i].Productname);
                        cmdProd.Parameters.AddWithValue("@Description", obj.Productslist[i].Description);
                        cmdProd.Parameters.AddWithValue("@Qty", Convert.ToInt64(obj.Productslist[i].Qty));
                        cmdProd.Parameters.AddWithValue("@GSTAmount", Convert.ToDouble(obj.Productslist[i].GSTAmount));
                        cmdProd.Parameters.AddWithValue("@MRP", Convert.ToDouble(obj.Productslist[i].MRPAmount));
                        cmdProd.Parameters.AddWithValue("@SalePrice", Convert.ToDouble(0));
                        cmdProd.Parameters.AddWithValue("@ProductCode", obj.Productslist[i].ProductCode);
                        cmdProd.Parameters.AddWithValue("@TotalAmount", Convert.ToDouble(obj.Productslist[i].Amount));
                        if (Convert.ToInt64(obj.SalesDetailsID) > 0)
                        {
                            cmdProd.Parameters.AddWithValue("@SalesDetailsID", Convert.ToDouble(obj.SalesDetailsID));
                            cmdProd.Parameters.AddWithValue("@StatusId", Convert.ToDouble(1));
                            dtProdctlist = SQLHelper.ExecuteAdapter(cmdProd, CommandType.StoredProcedure, TenantName + "." + "sp_upd_details_SalesInvoice", connectionstring);
                        }
                        else
                        {
                            dtProdctlist = SQLHelper.ExecuteAdapter(cmdProd, CommandType.StoredProcedure, TenantName + "." + "sp_ins_details_SalesInvoice", connectionstring);
                        }
                        
                        
                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                        }
                        return false;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                if (dtProdctlist.Tables.Count > 0)
                {
                    Message = "Invoice Details Save Success";
                    return true;
                }
                else
                {
                    Message = "Faild To Save";
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool SavePaymentDetails(salesOrder obj)
        {
            try
            {
                if (Convert.ToDouble(obj.PaidAmount) > 0)
                {
                    SqlCommand cmdpayment = new SqlCommand();
                    cmdpayment.Parameters.AddWithValue("@TenantName", obj.TenantName);
                    cmdpayment.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(obj.OrganizationId));
                    cmdpayment.Parameters.AddWithValue("@BranchId", Convert.ToInt64(obj.BranchId));
                    cmdpayment.Parameters.AddWithValue("@SalesID", Convert.ToInt64(SalesResultID));
                    cmdpayment.Parameters.AddWithValue("@PaymentAmount", Convert.ToDouble(obj.PaidAmount));
                    cmdpayment.Parameters.AddWithValue("@PaymentTypeiD", Convert.ToInt64(obj.PaymentTypeId));
                    cmdpayment.Parameters.AddWithValue("@PaymentReference", obj.PaymentReference);
                    cmdpayment.Parameters.AddWithValue("@UserID", Convert.ToInt64(obj.Createdby));
                    DataTable dt = SQLHelper.ExecuteAdapterTable(cmdpayment, CommandType.StoredProcedure, TenantName + "." + "sp_ins_details_Payment_SalesInvoice", connectionstring);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool SavePrescription(SalesEyeTestingPrescription obj)
        {
            DataTable dt = new DataTable();
            SqlCommand cmdProd = new SqlCommand();
            SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {

                SqlCommand cmd = new SqlCommand();

                cmd.Parameters.AddWithValue("@TenantName", obj.TenantName);
                cmd.Parameters.AddWithValue("@OrganizationId", Convert.ToInt64(obj.OrganizationId));
                cmd.Parameters.AddWithValue("@BranchID", Convert.ToInt64(obj.BranchId));
                cmd.Parameters.AddWithValue("@SalesId", Convert.ToInt64(obj.SalesID));
                cmd.Parameters.AddWithValue("@DV_SPH1", Convert.ToDouble(obj.DV_SPH1)); 
                cmd.Parameters.AddWithValue("@DV_CYL1", Convert.ToDouble(obj.DV_CYL1));
                cmd.Parameters.AddWithValue("@DV_AXIS1", Convert.ToDouble(obj.DV_AXIS1)); 
                cmd.Parameters.AddWithValue("@DV_VA1", Convert.ToDouble(obj.DV_VA1)); 
                cmd.Parameters.AddWithValue("@NV_SPH1", Convert.ToDouble(obj.NV_SPH1)); 
                cmd.Parameters.AddWithValue("@NV_CYL1", Convert.ToDouble(obj.NV_CYL1));
                cmd.Parameters.AddWithValue("@NV_AXIS1", Convert.ToDouble(obj.NV_AXIS1)); 
                cmd.Parameters.AddWithValue("@NV_VA1", Convert.ToDouble(obj.NV_VA1));
                cmd.Parameters.AddWithValue("@DV_SPH", Convert.ToDouble(obj.DV_SPH));
                cmd.Parameters.AddWithValue("@ADD1", Convert.ToDouble(obj.ADD1));
                cmd.Parameters.AddWithValue("@RPD", Convert.ToDouble(obj.RPD));
                cmd.Parameters.AddWithValue("@DV_SPH2", Convert.ToDouble(obj.DV_SPH2));
                cmd.Parameters.AddWithValue("@DV_CYL2", Convert.ToDouble(obj.DV_CYL2));
                cmd.Parameters.AddWithValue("@DV_AXIS2", Convert.ToDouble(obj.DV_AXIS2));
                cmd.Parameters.AddWithValue("@DV_VA2", Convert.ToDouble(obj.DV_VA2));
                cmd.Parameters.AddWithValue("@NV_SPH2", Convert.ToDouble(obj.NV_SPH2));
                cmd.Parameters.AddWithValue("@NV_CYL2", Convert.ToDouble(obj.NV_CYL2));
                cmd.Parameters.AddWithValue("@NV_AXIS2", Convert.ToDouble(obj.NV_AXIS2));
                cmd.Parameters.AddWithValue("@NV_VA2", Convert.ToDouble(obj.NV_VA2));
                cmd.Parameters.AddWithValue("@ADD2", Convert.ToDouble(obj.ADD2));
                cmd.Parameters.AddWithValue("@LPD", Convert.ToDouble(obj.LPD));
                
                dt = SQLHelper.ExecuteAdapterTable(cmd, CommandType.StoredProcedure, obj.TenantName + "." + "sp_ins_Details_Sales_EyeTesting_Prescription", connectionstring);
                transaction.Commit();
                return true;

            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                }
                return false;
            }
        }
    }



   
    public class SalesProducts
    {

        public string Id { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public string Size { get; set; }
        public string Colour { get; set; }
        public string ProductID { get; set; }
        public string ProductCode { get; set; }
        public string Productname { get; set; }
        public string Description { get; set; }
        public string GST { get; set; }
        public string Qty { get; set; }
        public string GSTAmount { get; set; }
        public string MRPAmount { get; set; }
        public string Amount { get; set; }

        //public string CategoryID { get; set; }
        //public string BrandID { get; set; }
        //public string Size { get; set; }
        //public string Color { get; set; }
        //public string ProductName { get; set; }
        //public string Description { get; set; }
        //public string Qty { get; set; }
        //public string PurchasePrice { get; set; }
        //public string GSTAmount { get; set; }
        //public string MRP { get; set; }
        //public string SalePrice { get; set; }
        //public string TotalAmount { get; set; }

    }

    public class SalesEyeTestingPrescription
    {

        string connectionstring = "";
        public SalesEyeTestingPrescription()
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


        public string ID { get; set; }
        public string TenantName { get; set; }
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string SalesID { get; set; }
        public string DV_SPH1 { get; set; }
        public string DV_CYL1 { get; set; }
        public string DV_AXIS1 { get; set; }
        public string DV_VA1 { get; set; }
        public string NV_SPH1 { get; set; }
        public string NV_CYL1 { get; set; }
        public string NV_AXIS1 { get; set; }
        public string NV_VA1 { get; set; }
        public string DV_SPH { get; set; }
        public string ADD1 { get; set; }
        public string RPD { get; set; }
        public string DV_SPH2 { get; set; }
        public string DV_CYL2 { get; set; }
        public string DV_AXIS2 { get; set; }
        public string DV_VA2 { get; set; }
        public string NV_SPH2 { get; set; }
        public string NV_CYL2 { get; set; }
        public string NV_AXIS2 { get; set; }
        public string NV_VA2 { get; set; }
        public string ADD2 { get; set; }
        public string LPD { get; set; }


    }

}
