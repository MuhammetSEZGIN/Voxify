namespace IdentityService.DTOs; 
public class LoginResult
{
    public bool Succeeded { get; set; }
    public string Token { get; set; }
    public string ErrorMessage { get; set; }
    public string UserId { get; set; }

  private LoginResult(){}

  public static LoginResult Success(string token, string userId)
  {
    return new LoginResult
    {
      Succeeded = true,
      Token = token,
      UserId = userId
    };
  }
    public static LoginResult Failure(string errorMessage)
    {
        return new LoginResult
        {
        Succeeded = false,
        ErrorMessage = errorMessage
        };
    }
    
}