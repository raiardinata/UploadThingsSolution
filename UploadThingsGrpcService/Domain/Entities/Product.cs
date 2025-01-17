﻿using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using UploadThingsGrpcService.Domain.Interfaces;

namespace UploadThingsGrpcService.Domain.Entities
{
    public class Product : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Type of product is required")]
        public string? ProductType { get; set; }

        public string? ProductImagePath { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Precision(18, 4)]
        public decimal ProductPrice { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Product product && Id == product.Id && ProductName == product.ProductName && ProductType == product.ProductType && ProductImagePath == product.ProductImagePath && ProductPrice == product.ProductPrice;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ProductName, ProductType, ProductImagePath, ProductPrice);
        }
    }
}
