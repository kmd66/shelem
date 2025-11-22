using shelemApi.Helper;
using System;

namespace shelemApi.Models;

public abstract class RoomProperty
{
    protected static Random _random = new();

    public abstract void FinishGame();

    #region events

    //public event Func<string, string, object, Task> NotifyUserAsync;
    //protected Task OnUserAsync(string conectionId, string eventName, object data)
    //    => NotifyUserAsync?.Invoke(conectionId, eventName, data) ?? Task.CompletedTask;

    //public event Func<IEnumerable<string>, string, object, Task> NotifyUsersAsync;
    //protected Task OnNotifyUsersAsync(IEnumerable<string> connectionIds, string eventName, object data)
    //    => NotifyUsersAsync?.Invoke(connectionIds, eventName, data) ?? Task.CompletedTask;

    //public event Action<Guid> Remove;
    //protected virtual void OnRemove(Guid id) => Remove?.Invoke(id);
    //public event Action<FinishModelRequest, List<byte>> Reload;
    //protected virtual void OnReload(FinishModelRequest model, List<byte> goals) => Reload?.Invoke(model, goals);

    //protected Task OnReceiveMainTime(int i)
    //    => NotifyUsersAsync?.Invoke(Users.Select(x => x.ConnectionId), "ReceiveMainTime", new
    //    {
    //        shotTime,
    //        i,
    //        tourner,
    //    }) ?? Task.CompletedTask;
    #endregion

    #region property
    public Guid Id { get; set; }
    public dynamic Info { get; set; }
    public List<RoomUser> Users { get; set; }
    public DateTime CreatedAt { get; set; }

    #endregion
    public CancellationTokenSource startToken;
    public CancellationTokenSource finishToken;
    public CancellationTokenSource actionToken;

    protected List<long> initStrat;

    public bool isStart => startToken != null ? startToken.Token.IsCancellationRequested : false;
    public bool isFinish => finishToken != null ? finishToken.Token.IsCancellationRequested : false;
    public long tourner = 0; // نوبت

    public List<byte> oflineCounts => [firstOflineCount, secondOflineCount];
    protected byte firstOflineCount = 0;
    protected byte secondOflineCount = 0;

    protected const int startWait = 60;
    protected const int gameTime = 30 * 60;
    protected const int actionTime = 30;
}

