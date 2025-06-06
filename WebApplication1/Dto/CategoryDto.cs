﻿using System.Collections.Generic;

namespace BulletinBoard.WebApplication.Models
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public List<CategoryDto> Subcategories { get; set; }
    }
}