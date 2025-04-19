namespace IdentityService.DTOs; 
class LoginResult
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public string ErrorMessage { get; set; }

  private LoginResult(){}

  public static LoginResult Success(string token)
  {
    return new LoginResult
    {
      Success = true,
      Token = token
    };
  }
    public static LoginResult Failure(string errorMessage)
    {
        return new LoginResult
        {
        Success = false,
        ErrorMessage = errorMessage
        };
    }
    
}