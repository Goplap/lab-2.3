using System;
using System.Collections.Generic;

namespace BulletinBoard.WebApplication.Models
{
    public class AdDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public UserDto User { get; set; }
        public CategoryDto Category { get; set; }
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}