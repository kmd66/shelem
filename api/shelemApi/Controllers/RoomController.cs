using shelemApi.Models;
using Microsoft.AspNetCore.Mvc;
using shelemApi.Helper;
using System.Text.Json;

namespace shelemApi.Controllers;

public class RoomController(RoomService roomService) : ControllerBase
{
    private readonly RoomService _roomService = roomService;


    [HttpPost("api/createRoom")]
    public Task<Result<HubUserGameResult>> CreateRoom([FromBody] CreateRoomRequest model)
    {
        try
        {
            if (model.key != AppStrings.ApiKey)
                return Result<HubUserGameResult>.FailureAsync(message: "RoomId is* required", code: 401);
            if (model.users?.Count != 2)
                return Result<HubUserGameResult>.FailureAsync(message: "bad required");

            var roomId = Guid.NewGuid();
            List<RoomUser> users = [];

            foreach (var u in model.users)
            {
                users.Add(new RoomUser
                {
                    Id = u.Id,
                    Key = Guid.NewGuid(),
                    Info = u
                });
            }
            users[0].FirstUser = true;

            var room = new Room { Id = roomId, Users = users };

            _roomService.AddRoom(room);

            var result = new HubUserGameResult(roomId, RoomUser.HubUserGameResultItem(users));

            return Result<HubUserGameResult>.SuccessfulAsync(data: result);

        }
        catch (Exception e)
        {
            return Result<HubUserGameResult>.FailureAsync(message: e.Message);
        }
    }


    [HttpGet("api/createTestRoom")]
    public IActionResult CreateTestRoom()
    {
        _roomService.ClearRoom();

        var roomId = Guid.NewGuid();
        List<RoomUser> users = [];
        var id1 = 1;
        var id2 = 2;

        users.Add(new RoomUser
        {
            Id = id1,
            Key = Guid.NewGuid(),
            Info = new UserRequest(id1, null, "/test/p1", "F 1", "L 1", "U 1", DateTime.Parse("9/20/2025"))
        });


        users.Add(new RoomUser
        {
            Id = id2,
            Key = Guid.NewGuid(),
            Info = new UserRequest(id2, null, "/test/p2", "F 2", "L 2", "U 2", DateTime.Parse("5/20/2022"), 114),
        });

        users[0].FirstUser = true;

        var room = new Room { Id = roomId, Users = users };

        _roomService.AddRoom(room);

        var usersJson = JsonSerializer.Serialize(users.Select(u => new {
            userId = u.Id,
            userKey = u.Key
        }));
        string htmlContent = $@" <html lang='en'><head><meta charset='utf-8' /></head><body>
        <script> const roomId = ""{roomId}""; const users = {usersJson};
            users.forEach((x, index) => {{
                setTimeout(() => {{
                    const id = x.userId;
                    const key = x.userKey;
                    window.open(`{AppStrings.ClientUrl}/?roomId=${{roomId}}&userKey=${{key}}&userId=${{id}}`, ""_blank"");
                }}, index * 100); // تأخیر 1 ثانیه بین هر پنجره
            }});
        </script>
        </body>
        </html>
        ";

        return Content(htmlContent, "text/html");

    }

}
