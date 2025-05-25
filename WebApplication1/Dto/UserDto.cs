using System;

namespace BulletinBoard.WebApplication.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime RegisteredAt { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }
    }
}