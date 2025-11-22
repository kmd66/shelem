using Microsoft.AspNetCore.Mvc;

namespace shelemApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    //[HttpGet(Name = "GetWeatherForecast")]
    //public List<dynamic> Get()
    //{
    //    var list = PhysicsHub.service?.GetObj();
    //    var data = list?.Select(x => new {
    //        x.Position.X,
    //        x.Position.Y,
    //        x.Rotation,
    //        UserData = PhysicsHub.service?.getUserData(x)
    //    }).Cast<dynamic>().ToList();
    //    return data;
    //}

}
