using Microsoft.AspNetCore.Identity;

namespace TwitterAPI.Helpers;

public class RegisterUserReturnType
{
    public bool success { get; set; }
    public IEnumerable<IdentityError>? errors { get; set; }
}