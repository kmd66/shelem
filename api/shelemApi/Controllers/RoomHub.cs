using Microsoft.AspNetCore.SignalR;
using shelemApi.Helper;
using System.Text.Json;

namespace shelemApi.Controllers;

public class RoomHub(RoomService roomService) : Hub
{

    private readonly RoomService _roomService = roomService;

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var roomIdStr = httpContext.Request.Query["roomId"];
        var keyStr = httpContext.Request.Query["key"];
        var userIdStr = httpContext.Request.Query["userId"];

        if (!Guid.TryParse(roomIdStr, out Guid roomId) ||
            !Guid.TryParse(keyStr, out Guid key) ||
            !long.TryParse(userIdStr, out long userId) ||
            roomId == Guid.Empty || key == Guid.Empty || userId == 0)
        {
            await Clients.Caller.SendAsync("Abort");
            await Task.Delay(1000);
            Context.Abort();
            return;
        }

        var user = _roomService.GetUser(roomId, key, userId);
        if (user == null)
        {
            await Clients.Caller.SendAsync("Abort");
            await Task.Delay(1000);
            Context.Abort();
            return;
        }

        if (!string.IsNullOrEmpty(user.ConnectionId))
            _ = Clients.Clients(user.ConnectionId).SendAsync("Disconnect", false);
        user.ConnectionId = Context.ConnectionId;

        await base.OnConnectedAsync();

        var roomInfo = _roomService.Info(roomId);
        var json = JsonSerializer.Serialize(roomInfo);
        await Clients.Caller.SendAsync("ReceiveRoomInfo", json);
        var room = _roomService.GetRoom(roomId);
        if (room.isStart)
        {
            await Task.Delay(1000);
            //await room.ReceivePhysicsStandard();
        }
    }

    public void Start(Guid roomId, Guid key)
    {
        try
        {
            var room = _roomService.GetRoom(roomId);
            if (room == null || room.isStart) return;
            room.SetStart(key);
        }
        catch (Exception)
        {
        };
    }

    public async Task AddSticker(Guid roomId, Guid key, byte id)
    {
        try
        {
            var room = _roomService.GetRoom(roomId);
            if (room == null) return;
            var user = room.Users.FirstOrDefault(x => x.Key == key);
            if (user == null) return;
            await Clients.Clients(room.Users.Select(x => x.ConnectionId)).SendAsync("ReceiveAddSticker", new { id, userId = user.Id });
        }
        catch (Exception)
        {
        };
    }

    public async Task AddMessage(Guid roomId, Guid key, byte id)
    {
        try
        {
            var room = _roomService.GetRoom(roomId);
            if (room == null) return;
            var user = room.Users.FirstOrDefault(x => x.Key == key);
            if (user == null) return;
            await Clients.Clients(room.Users.Select(x => x.ConnectionId)).SendAsync("ReceiveAddMessage", new { id, userId = user.Id });
        }
        catch (Exception)
        {
        };
    }

    public void ReloadRequest(Guid finishId, Guid key)
    {
        try
        {
            _roomService.ReloadRequest(finishId, key);
        }
        catch (Exception)
        { };
    }

}
