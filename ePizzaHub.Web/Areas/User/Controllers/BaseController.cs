using ePizzaHub.Models;
using ePizzaHub.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ePizzaHub.Web.Areas.User.Controllers
{
    [CustomAuthorize(Roles = "User")]
    [Area("User")]
    public class BaseController : Controller
    {
        public UserModel CurrentUser
        {
            get
            {
                if
                    (User.Claims.Count() > 0)
                {
                    string userData = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)!.Value;
                    var user = JsonConvert.DeserializeObject<UserModel>(userData);
                    return user;
                }
                return null;
            }
        }
    }
}
