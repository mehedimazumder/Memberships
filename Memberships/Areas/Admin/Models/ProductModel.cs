﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Memberships.Entities;

namespace Memberships.Areas.Admin.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        [MaxLength(255)]
        [Required]
        public string Title { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }

        [MaxLength(1024)]
        [Display(Name = "Image Url")]
        public string ImageUrl { get; set; }

        public int ProductLinkTextId { get; set; }
        public int ProductTypeId { get; set; }

        [DisplayName("Product Types")]
        public ICollection<ProductType> ProductTypes { get; set; }

        [DisplayName("Product Link Texts")]
        public ICollection<ProductLinkText> ProductLinkTexts { get; set; }

        public string ProductType { get
            {
                return ProductTypes == null || ProductTypes.Count.Equals(0)
                    ? String.Empty
                    : ProductTypes.FirstOrDefault(pt => pt.Id == ProductTypeId)?.Title;
            } }

        public string ProductLinkText
        {
            get
            {
                return ProductLinkTexts == null || ProductLinkTexts.Count.Equals(0)
                    ? String.Empty
                    : ProductLinkTexts.FirstOrDefault(pt => pt.Id == ProductLinkTextId)?.Title;
            }
        }
    }
}