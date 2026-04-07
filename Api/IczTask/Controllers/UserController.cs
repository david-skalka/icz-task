using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace SlotMachine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController() : ControllerBase
{



  
    [HttpGet("me")]
    [Authorize(Roles = "Admin,Game")]
    public string? Me()
    {
        return User.Identity?.Name;
    }
    
    
    
}
