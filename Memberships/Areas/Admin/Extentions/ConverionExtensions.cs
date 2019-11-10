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
        #region Product
        public static async Task<IEnumerable<ProductModel>> Convert(this IEnumerable<Product> products,
            ApplicationDbContext db)
        {
            var enumerable = products.ToList();
            if (enumerable.Count().Equals(0))
                return new List<ProductModel>();

            var links = await db.ProductLinkTexts.ToListAsync();
            var types= await db.ProductTypes.ToListAsync();

            return from product in enumerable
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
        #endregion

        #region Product Item

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

        public static async Task<bool> CanChange(
            this ProductItem productItem, ApplicationDbContext db)
        {
            var oldPI = await db.ProductItems.CountAsync(pi =>
                pi.ProductId.Equals(productItem.OldProductId) &&
                pi.ItemId.Equals(productItem.OldItemId));

            var newPI = await db.ProductItems.CountAsync(pi =>
                pi.ProductId.Equals(productItem.ProductId) &&
                pi.ItemId.Equals(productItem.ItemId));

            return oldPI.Equals(1) && newPI.Equals(0);
        }

        public static async Task Change(
            this ProductItem productItem, ApplicationDbContext db)
        {
            var oldProductItem = await db.ProductItems.FirstOrDefaultAsync(
                pi => pi.ProductId.Equals(productItem.OldProductId) &&
                      pi.ItemId.Equals(productItem.OldItemId));

            var newProductItem = await db.ProductItems.FirstOrDefaultAsync(
                pi => pi.ProductId.Equals(productItem.ProductId) &&
                      pi.ItemId.Equals(productItem.ItemId));

            if (oldProductItem != null && newProductItem == null)
            {
                newProductItem = new ProductItem
                {
                    ItemId = productItem.ItemId,
                    ProductId = productItem.ProductId
                };

                using (var transaction = new TransactionScope(
                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.ProductItems.Remove(oldProductItem);
                        db.ProductItems.Add(newProductItem);

                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch { transaction.Dispose(); }
                }
            }
        }

        #endregion

        #region Subscription Product

        public static async Task<IEnumerable<SubscriptionProductModel>> Convert(this IQueryable<SubscriptionProduct> subscriptionProducts,
           ApplicationDbContext db)
        {
            if (subscriptionProducts.Count().Equals(0))
                return new List<SubscriptionProductModel>();


            return await (from subscriptionProduct in subscriptionProducts
                          select new SubscriptionProductModel
                          {
                              SubscriptionId = subscriptionProduct.SubscriptionId,
                              ProductId = subscriptionProduct.ProductId,
                              ProductTitle = db.Products.FirstOrDefault(
                                it => it.Id.Equals(subscriptionProduct.ProductId)).Title,
                              SubscriptionTitle = db.Subscriptions.FirstOrDefault(
                                pt => pt.Id.Equals(subscriptionProduct.SubscriptionId)).Title
                          }).ToListAsync();
        }

        public static async Task<SubscriptionProductModel> Convert(this SubscriptionProduct subscriptionProduct,
            ApplicationDbContext db, bool addListData = true)
        {
            var model = new SubscriptionProductModel
            {
                SubscriptionId = subscriptionProduct.SubscriptionId,
                ProductId = subscriptionProduct.ProductId,
                Subscriptions = addListData ? await db.Subscriptions.ToListAsync() : null,
                Products = addListData ? await db.Products.ToListAsync() : null,
                ProductTitle = (await db.ProductTypes.FirstOrDefaultAsync(p => p.Id.Equals(subscriptionProduct.ProductId)))?.Title,
                SubscriptionTitle = (await db.Items.FirstOrDefaultAsync(i => i.Id.Equals(subscriptionProduct.SubscriptionId)))?.Title
            };

            return model;
        }

        public static async Task<bool> CanChange(
          this SubscriptionProduct subscriptionProduct,
          ApplicationDbContext db)
        {
            var oldSP = await db.SubscriptionProducts.CountAsync(sp =>
                sp.ProductId.Equals(subscriptionProduct.OldProductId) &&
                sp.SubscriptionId.Equals(subscriptionProduct.OldSubscriptionId));

            var newSP = await db.SubscriptionProducts.CountAsync(sp =>
                sp.ProductId.Equals(subscriptionProduct.ProductId) &&
                sp.SubscriptionId.Equals(subscriptionProduct.SubscriptionId));

            return oldSP.Equals(1) && newSP.Equals(0);
        }

        public static async Task Change(
            this SubscriptionProduct subscriptionProduct,
            ApplicationDbContext db)
        {
            var oldSubscriptionProduct = await db.SubscriptionProducts.FirstOrDefaultAsync(
                    sp => sp.ProductId.Equals(subscriptionProduct.OldProductId) &&
                    sp.SubscriptionId.Equals(subscriptionProduct.OldSubscriptionId));

            var newSubscriptionProduct = await db.SubscriptionProducts.FirstOrDefaultAsync(
                sp => sp.ProductId.Equals(subscriptionProduct.ProductId) &&
                sp.SubscriptionId.Equals(subscriptionProduct.SubscriptionId));

            if (oldSubscriptionProduct != null && newSubscriptionProduct == null)
            {
                newSubscriptionProduct = new SubscriptionProduct
                {
                    SubscriptionId = subscriptionProduct.SubscriptionId,
                    ProductId = subscriptionProduct.ProductId
                };

                using (var transaction = new TransactionScope(
                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.SubscriptionProducts.Remove(oldSubscriptionProduct);
                        db.SubscriptionProducts.Add(newSubscriptionProduct);

                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch { transaction.Dispose(); }
                }
            }
        }

        #endregion

    }
}