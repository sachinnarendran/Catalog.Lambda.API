﻿using Catalog.Lambda.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Lambda.API.Services
{
    public interface IS3Service
    {
        Task<bool> AddContentToS3(Product product);

        Task<Product> GetProductFromS3(string id);

        Task<IEnumerable<Product>> GetAllProductsFromS3();
    }
}
