using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OptiKnoxAPI.Models;

namespace OptiKnoxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        Inventory objinventory = new Inventory();
        Vendar objvendar = new Vendar();
        Product objproduct = new Product();
        #region Create Vendar
        [HttpPost]
        [Route("Vendar-Creation")]
        public IActionResult CreateVendar(Vendar obj)
        {
            var dtVendarDetails = obj.CreateVendar(obj);
            return Ok(JsonConvert.SerializeObject(dtVendarDetails, new JsonSerializerSettings()));
        }

        #endregion


        #region Create Customer
        [HttpPost]
        [Route("Customer-Creation")]
        public IActionResult CreateCustomer(Customer obj)
        {
            var dtCustomerDetails = obj.CreateCustomers(obj);
            return Ok(JsonConvert.SerializeObject(dtCustomerDetails, new JsonSerializerSettings()));
        }
        #endregion


        #region


        [HttpPost]
        [Route("Brand-Creation")]
        public IActionResult CreateBrand(Brand obj)
        {
            var dtBrandDetails = obj.CreateBrand(obj);
            return Ok(JsonConvert.SerializeObject(dtBrandDetails, new JsonSerializerSettings()));
        }

        [HttpPost]
        [Route("Category-Creation")]
        public IActionResult CreateCategory(Category obj)
        {

            var dtBrandDetails = obj.CreateBrand(obj);
            return Ok(JsonConvert.SerializeObject(dtBrandDetails, new JsonSerializerSettings()));
        }

        [HttpPost]
        [Route("Product-Creation")]
        public IActionResult CreateProduct(Product obj)
        {

            var dtProductDetails = obj.CreateProduct(obj);
            return Ok(JsonConvert.SerializeObject(dtProductDetails, new JsonSerializerSettings()));
        }


        #endregion


        #region  master Customers

        [HttpGet]
        [Route("master-Customers")]
        public IActionResult Customersdetails(string Organizationid)
        {
            var dtcustomers = objvendar.Customers(Organizationid);
            return Ok(JsonConvert.SerializeObject(dtcustomers, new JsonSerializerSettings()));
        }
        #endregion

        #region Vendors

        [HttpGet]
        [Route("master_Vendors")]
        public IActionResult Vendors(string Organizationid)
        {
            var dtvendors = objvendar.GetVenders(Organizationid);
            return Ok(JsonConvert.SerializeObject(dtvendors, new JsonSerializerSettings()));
        }
        #endregion

        #region Product Brand

        [HttpGet]
        [Route("master_ProductBrand")]
        public IActionResult ProductBrand(string BranchId, string Organizationid)
        {
            var dtBrand = objvendar.ProductBrand(BranchId, Organizationid);
            return Ok(JsonConvert.SerializeObject(dtBrand, new JsonSerializerSettings()));
        }
        #endregion

        #region Product Brand By Category

        [HttpGet]
        [Route("master_ProductBrand-by-Category")]
        public IActionResult ProductBrandbyCategoiry(string CategoryID, string Organizationid)
        {
            var dtBrand = objvendar.ProductBrandByCategory(CategoryID, Organizationid);
            return Ok(JsonConvert.SerializeObject(dtBrand, new JsonSerializerSettings()));
        }
        #endregion

        #region ProductCategory

        [HttpGet]
        [Route("master_ProductCategory")]
        public IActionResult ProductCategory(string BranchId, string Organizationid)
        {
            var dtcategory = objvendar.ProductCategory(BranchId, Organizationid);
            return Ok(JsonConvert.SerializeObject(dtcategory, new JsonSerializerSettings()));
        }
        #endregion

        #region  master Customers

        [HttpGet]
        [Route("master-products")]
        public IActionResult GetProducts(string Organizationid)
        {
            var dtcustomers = objproduct.GetProducts(Organizationid);
            return Ok(JsonConvert.SerializeObject(dtcustomers, new JsonSerializerSettings()));
        }
        #endregion

        #region  master By Category Brand

        [HttpGet]
        [Route("master-products-ByCategoryBrand")]
        public IActionResult GetProductsCategoryBrand(string Organizationid, string CateoryId, string BrandId)
        {
            var dtcustomers = objproduct.GetProductsByCategforyBrand(Organizationid, CateoryId, BrandId);
            return Ok(JsonConvert.SerializeObject(dtcustomers, new JsonSerializerSettings()));
        }
        #endregion

        #region  masteer Size

        [HttpGet]
        [Route("master-Size")]
        public IActionResult GetProductsSize(string Organizationid)
        {
            var dtcustomers = objproduct.GetProductsSize(Organizationid);
            return Ok(JsonConvert.SerializeObject(dtcustomers, new JsonSerializerSettings()));
        }
        #endregion
        #region  masteer Color

        [HttpGet]
        [Route("master-Color")]
        public IActionResult GetProductsColour(string Organizationid)
        {
            var dtcustomers = objproduct.GetProductsColour(Organizationid);
            return Ok(JsonConvert.SerializeObject(dtcustomers, new JsonSerializerSettings()));
        }
        #endregion
        #region  masteer Payments

        [HttpGet]
        [Route("master-payments")]
        public IActionResult Getpayments()
        {
            var dtcustomers = objproduct.GetPaymentTypes();
            return Ok(JsonConvert.SerializeObject(dtcustomers, new JsonSerializerSettings()));
        }
        #endregion
    }
}
