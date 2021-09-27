using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Catalog.Lambda.API.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Catalog.Lambda.API.Services
{
    public class S3Service:IS3Service
    {
        private readonly IAmazonS3 amazonS3;
        private readonly IConfiguration configuration;
        AmazonS3Client amazonS3Client;
        string accessKey;
        string secretKey;

        public S3Service(IAmazonS3 _amazonS3,IConfiguration _configuration)
        {
            amazonS3 = _amazonS3;
            configuration = _configuration;
            accessKey = configuration.GetValue<string>("ServiceConfiguration:accessKey");
            secretKey = configuration.GetValue<string>("ServiceConfiguration:secretKey");
            amazonS3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USEast2);
        }

        public async Task<bool> AddContentToS3(Product product)
        {
            var request = new PutObjectRequest
            {
                BucketName = configuration.GetValue<string>("ServiceConfiguration:BucketName"),
                Key = product.Id,
                ContentType = "application/json",
                ContentBody = JsonSerializer.Serialize(product)
            };
            var response = await amazonS3Client.PutObjectAsync(request);
            return true;
        }

        public async Task<Product> GetProductFromS3(string Id)
        {
            var response = await amazonS3Client.GetObjectAsync(configuration.GetValue<string>("ServiceConfiguration:BucketName"), Id);
            StreamReader reader = new StreamReader(response.ResponseStream);
            var content = reader.ReadToEnd();
            var product = JsonSerializer.Deserialize<Product>(content);
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllProductsFromS3()
        {
            try
            {

                AmazonS3Client amazonS3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USEast2);
                
                List<Product> products = new List<Product>();
                ListObjectsRequest listObjectsRequest = new ListObjectsRequest
                {
                    BucketName = configuration.GetValue<string>("ServiceConfiguration:BucketName")
                };
                var listObjectResponse = await amazonS3Client.ListObjectsAsync(listObjectsRequest);
                foreach (var item in listObjectResponse.S3Objects)
                {
                    var fileContent = await amazonS3Client.GetObjectAsync(configuration.GetValue<string>("ServiceConfiguration:BucketName"), item.Key);
                    StreamReader reader = new StreamReader(fileContent.ResponseStream);
                    var content = reader.ReadToEnd();
                    var productDetail = JsonSerializer.Deserialize<Product>(content);
                    products.Add(productDetail);
                }
                return products;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return new List<Product>();
            }
        }

    }
}
