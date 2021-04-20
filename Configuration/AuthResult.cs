using System.Collections.Generic;

namespace netcore_rest_api.Configuration {
  public class AuthResult {
    public string Token { get; set; }
    public List<string> Errors { get; set; }
  }
}