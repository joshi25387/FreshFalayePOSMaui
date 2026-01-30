using System.ComponentModel.DataAnnotations;

namespace FreshFalaye.Pos.Shared.Models
{
    public class LocalUser
    {
        [Key]
        public int LocalUserId { get; set; }

        public string Username { get; set; } = null!;

        // Store hashed password (never plain text)
        public string PasswordHash { get; set; } = null!;
        public string PinHash { get; set; } = null!;

        public string Role { get; set; } = "Cashier"; // Admin / Cashier

        public bool IsActive { get; set; } = true;
    }
}
