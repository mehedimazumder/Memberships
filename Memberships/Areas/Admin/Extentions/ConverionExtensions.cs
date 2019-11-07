﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Memberships.Areas.Admin.Models;
using Memberships.Entities;
using Memberships.Models;

namespace Memberships.Areas.Admin.Extentions
{
    public static class ConverionExtensions
    {
        public static async Task<IEnumerable<ProductModel>> Convert(this IEnumerable<Product> products,
            ApplicationDbContext db)
        {
            if (products.Count().Equals(0))
                return new List<ProductModel>();

            var links = await db.ProductLinkTexts.ToListAsync();
            var types= await db.ProductTypes.ToListAsync();

            return from product in products
                select new ProductModel
                {
                    Id = product.Id,
                    Title = product.Title,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl,
                    ProductLinkTextId = product.ProductLinkTextId,
                    ProductTypeId = product.ProductTypeId,
                    ProductTypes = types,
                    ProductLinkTexts = links
                };
        }

        public static async Task<ProductModel> Convert(this Product product,
            ApplicationDbContext db)
        {
            var link = await db.ProductLinkTexts.SingleOrDefaultAsync(p => p.Id.Equals(product.ProductLinkTextId));
            var type = await db.ProductTypes.SingleOrDefaultAsync(p => p.Id.Equals(product.ProductTypeId));

            var model = new ProductModel
                {
                    Id = product.Id,
                    Title = product.Title,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl,
                    ProductLinkTextId = product.ProductLinkTextId,
                    ProductTypeId = product.ProductTypeId,
                    ProductTypes = new List<ProductType>(),
                    ProductLinkTexts = new List<ProductLinkText>()
                };
            model.ProductTypes.Add(type);
            model.ProductLinkTexts.Add(link);

            return model;
        }
    }
}