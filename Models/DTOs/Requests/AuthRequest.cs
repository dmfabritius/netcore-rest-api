using System.ComponentModel.DataAnnotations;

namespace netcore_rest_api.Models.DTOs.Requests {

  public class AuthRequest {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
  }
}