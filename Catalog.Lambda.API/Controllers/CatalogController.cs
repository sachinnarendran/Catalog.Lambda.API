using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Lambda.API.Models;
using Catalog.Lambda.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Lambda.API.Controllers
{
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {


        private readonly IS3Service s3Service;

        public CatalogController(IS3Service _s3service)
        {
            s3Service = _s3service;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}


        [HttpPost]
        [Route("UploadCatalog")]
        public async Task<IActionResult> UploadCatalogDetails([FromBody] Product product)
        {
            return Ok(await s3Service.AddContentToS3(product));
        }

        [HttpGet("{key}", Name = "GetProductFromS3")]
        public async Task<IActionResult> GetCatalogById(string key)
        {
            return Ok(await s3Service.GetProductFromS3(key));
        }

        [HttpGet]
        [Route("GetAllCatalogDetails")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllCatalogDetails()
        {
            return Ok(await s3Service.GetAllProductsFromS3());
        }
    }
}
