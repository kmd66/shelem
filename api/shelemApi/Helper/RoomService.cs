using Microsoft.AspNetCore.SignalR;
using shelemApi.Controllers;
using System.Collections.Concurrent;
using shelemApi.Helper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using shelemApi.Models;

namespace shelemApi.Helper;

public class RoomService(IHubContext<RoomHub> hubContext)
{
    private readonly IHubContext<RoomHub> _hubContext= hubContext;
    private readonly ConcurrentDictionary<Guid, Room> _rooms = [];
    private readonly ConcurrentDictionary<Guid, List<FinishUser>> _finishUser = [];


    public void AddRoom(Room room)
    {
        if (_rooms.TryAdd(room._p.Id, room))
        {
            SubscribeToRoomEvents(room);
            room._p.CreatedAt = DateTime.Now;
            _ = room.Start();
        }
    }

    public void RemoveRoom(Guid roomId)
    {
        if (_rooms.TryRemove(roomId, out var room))
        {
            room.FinishGame();
            UnsubscribeFromRoomEvents(room);
        }
    }

    public void ClearRoom()
    {
        var rooms = _rooms.Values.ToList();
        _rooms.Clear();

        foreach (var room in rooms)
        {
            room.FinishGame();
        }
    }

    #region Reload

    public async Task ReloadRoom(FinishModelRequest model, List<byte> goals)
    {
        if (!model.start) return;

        var connectionIds = new List<string> { model.user1.connectionId, model.user2.connectionId };

        await Task.Delay(10);
        await _hubContext.Clients.Clients(connectionIds).SendAsync("FinishGameResult", new { model.winner, goals });

        var url = $"{Helper.AppStrings.ApiUrl}/resultGame/finishDoublesGame";
        var result = await new Helper.AppRequest().Post(model, url);
        await Task.Delay(TimeSpan.FromSeconds(5));

        if (!model.isReload)
        {
            _ = _hubContext.Clients.Clients(connectionIds).SendAsync("Disconnect", true);
            return;
        }

        var finishModel = new List<FinishUser> { model.user1, model.user2 };
        var finishId = Guid.NewGuid();
        try
        {
            if (_finishUser.TryAdd(finishId, finishModel))
            {
                await _hubContext.Clients.Clients(connectionIds).SendAsync("ReloadRequest", finishId);
                await Task.Delay(TimeSpan.FromSeconds(20));
            }
        }
        finally
        {
            _ = _hubContext.Clients.Clients(connectionIds).SendAsync("Disconnect", true);
            _finishUser.TryRemove(finishId, out _);
        }
    }

    public void ReloadRequest(Guid finishId, Guid userKey)
    {
        if (_finishUser.TryGetValue(finishId, out var users))
        {
            var userIndex = users.FindIndex(x => x.userKey == userKey);
            var userOther = users.FirstOrDefault(x => x.userKey != userKey);
            if (userOther != null && userIndex >= 0 && !users[userIndex].accept)
            {
                _ = _hubContext.Clients.Client(users[userIndex].connectionId).SendAsync("ReceiveReloadRequest", true);
                if (userOther.accept)
                {
                    _finishUser.TryRemove(finishId, out _);
                    _= ReloadRequest2(userOther, users[userIndex]);
                    return;
                }
                var updatedUser = users[userIndex] with { accept = true };
                users[userIndex] = updatedUser;
                _finishUser.TryAdd(finishId, users);
            }
        }
    }
    private async Task ReloadRequest2(FinishUser user1, FinishUser user2)
    {
        var reloadModel = new FinishModelRequest(
            key: Helper.AppStrings.ApiKey,
            roomId: Guid.Empty,
            start: false,
            isReload: false,
            winner: 0,
            user1: user1,
            user2: user2,
            BaseUrl: Helper.AppStrings.MainUrl
            );
        var urlReload = $"{Helper.AppStrings.ApiUrl}/resultGame/reloadDoublesGame";
        var resultReload = await new Helper.AppRequest().Post<ReloadDoublesGameModel>(reloadModel, urlReload, ignoreCase: true);

        if (resultReload.success)
        {
            var u1Index = resultReload.data.users.FindIndex(x => x.Id == user1.userId);
            var u2Index = resultReload.data.users.FindIndex(x => x.Id == user2.userId);
            if (u1Index < 0 || u2Index < 0) return;
            List<RoomUser> users = new List<RoomUser> {
                new RoomUser
                {
                    Id = resultReload.data.users[u1Index].Id,
                    Key = resultReload.data.keys[u1Index],
                    Info = resultReload.data.users[u1Index],
                    FirstUser = true
                },new RoomUser
                {
                    Id = resultReload.data.users[u2Index].Id,
                    Key = resultReload.data.keys[u2Index],
                    Info = resultReload.data.users[u2Index],
                    FirstUser = false
                }
            };

            var room = new Room(resultReload.data.roomId, users);
            AddRoom(room);
            var url1 = new
            {
                resultReload.data.roomId,
                userKey = resultReload.data.keys[u1Index],
                userId = resultReload.data.users[u1Index].Id,
            };
            var url2 = new
            {
                resultReload.data.roomId,
                userKey = resultReload.data.keys[u2Index],
                userId = resultReload.data.users[u2Index].Id,
            };
            await Task.Delay(100);
            await _hubContext.Clients.Client(user1.connectionId).SendAsync("ReceiveReloadInit", url1);
            await _hubContext.Clients.Client(user2.connectionId).SendAsync("ReceiveReloadInit", url2);
        }
    }

    #endregion

    #region Notify
    
    private void SubscribeToRoomEvents(Room room)
    {
        room._p.NotifyUserAsync += async (key, eventName, data) => await NotifyUserAsync(key, eventName, data);
        room._p.NotifyUsersAsync += async (keys, eventName, data) => await NotifyUsersAsync(keys, eventName, data);
        room._p.Remove += (roomId) => RemoveRoom(roomId);
        room._p.Reload += async (model, goals) => await ReloadRoom(model, goals);
        room._p.CreatedAt = DateTime.Now;
    }
    private void UnsubscribeFromRoomEvents(Room room)
    {
        room._p.NotifyUserAsync -= async (key, eventName, data) => await NotifyUserAsync(key, eventName, data);
        room._p.NotifyUsersAsync -= async (keys, eventName, data) => await NotifyUsersAsync(keys, eventName, data);
        room._p.Remove -= (roomId) => RemoveRoom(roomId);
        room._p.Reload -= async (model, goals) => await ReloadRoom(model, goals);
    }


    private async Task NotifyUserAsync<T>(string key, string eventName, T data) 
        => await _hubContext.Clients.Client(key).SendAsync(eventName, data);
    private async Task NotifyUsersAsync<T>(IEnumerable<string> keys, string eventName, T data)
    {
        var k = keys.ToList();
        if (k.Count < 1) return;
        await _hubContext.Clients.Clients(k).SendAsync(eventName, data);
    }
    private async Task NotifyClientsAllAsync<T>(string eventName, T data)
        => await _hubContext.Clients.All.SendAsync(eventName, data);

    #endregion

    #region get
    public bool TryGetRoom(Guid roomId, out Room room)
    {
        return _rooms.TryGetValue(roomId, out room);
    }

    public Room GetRoom(Guid roomId)
    {
        _rooms.TryGetValue(roomId, out Room room);
        return room;
    }

    public IEnumerable<Room> GetAllRooms()
    {
        return _rooms.Values;
    }
    public RoomUser GetUser(Guid roomId, Guid key = default, long userId = 0)
    {
        if (userId == 0 && key == Guid.Empty)
            return null;
        if (_rooms.TryGetValue(roomId, out Room room))
        {
            return room._p.Users.FirstOrDefault(x =>
                (key == Guid.Empty || x.Key == key) &&
                (userId == 0 || x.Id == userId)
            );
        }
        return null;
    }
    #endregion

    public object Info(Guid roomId)
    {
        //if (_rooms.TryGetValue(roomId, out Room room))
        //{
        //    return new
        //    {
        //        start = room.isStart,
        //        users = room.Users.Select(u => new
        //        {
        //            u.FirstUser,
        //            u.Id,
        //            u.Level,
        //            u.Info
        //        }),
        //        delay = room.DelayTime,
        //        room.tourner,
        //        room.oflineCounts,
        //        room.ballLabels,
        //        goals1 = room.goals[0].Count,
        //        goals2 = room.goals[1].Count,
        //    };
        //}
        return null;
    }
}