using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Memberships.Areas.Admin.Models;
using Memberships.Entities;
using Memberships.Models;
using TransactionScope = System.Transactions.TransactionScope;

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

        public static async Task<IEnumerable<ProductItemModel>> Convert(this IQueryable<ProductItem> productItems,
            ApplicationDbContext db)
        {
            if (productItems.Count().Equals(0))
                return new List<ProductItemModel>();


            return await (from productItem in productItems
                select new ProductItemModel
                {
                  ItemId = productItem.ItemId,
                  ProductId = productItem.ProductId,
                  ItemTitle =  db.Items.FirstOrDefault(
                      it => it.Id.Equals(productItem.ItemId)).Title,
                  ProductTitle =  db.Products.FirstOrDefault(
                      pt => pt.Id.Equals(productItem.ProductId)).Title
                }).ToListAsync();
        }

        public static async Task<ProductItemModel> Convert(this ProductItem productItem,
            ApplicationDbContext db, bool addListData = true)
        {
            var model = new ProductItemModel
            {
                ItemId = productItem.ItemId,
                ProductId = productItem.ProductId,
                Items = addListData? await db.Items.ToListAsync() : null,
                Products = addListData? await db.Products.ToListAsync():null,
                ProductTitle = (await db.ProductTypes.FirstOrDefaultAsync(p => p.Id.Equals(productItem.ProductId)))?.Title,
                ItemTitle = (await db.Items.FirstOrDefaultAsync(i => i.Id.Equals(productItem.ItemId)))?.Title
            };

            return model;
        }

        public static async Task<bool> CanChange(this ProductItem productItem, ApplicationDbContext db)
        {
            var oldPi = await db.ProductItems.CountAsync(pi => pi.ProductId.Equals(
                                                                   productItem.OldProductId) &&
                                                               pi.ItemId.Equals(productItem.OldItemId));

            var newPi = await db.ProductItems.CountAsync(pi => pi.ProductId.Equals(
                                                                   productItem.ProductId) &&
                                                               pi.ItemId.Equals(productItem.ItemId));

            return oldPi.Equals(1) && newPi.Equals(0);
        }

        public static async Task Change(this ProductItem productItem, ApplicationDbContext db)
        {
            var oldProductItem = await db.ProductItems.FirstOrDefaultAsync(
                pi => pi.ProductId.Equals(productItem.OldProductId) && pi.ItemId.Equals(productItem.OldProductId));

            var newProductItem = await db.ProductItems.FirstOrDefaultAsync(
                pi => pi.ProductId.Equals(productItem.ProductId) && pi.ItemId.Equals(productItem.ItemId));

            if (oldProductItem != null && newProductItem == null)
            {
                newProductItem = new ProductItem
                {
                    ItemId = productItem.ItemId,
                    ProductId = productItem.ProductId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.ProductItems.Remove(oldProductItem);
                        db.ProductItems.Add(newProductItem);

                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch { transaction.Dispose();}
                }
            } 


        }
    }
}