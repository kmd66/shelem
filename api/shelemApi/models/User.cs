namespace shelemApi.Models;

public record HubUserGameResult(Guid roomId, List<HubUserGameResultItem> users);
public record HubUserGameResultItem(long userId, Guid userKey);
public class RoomUser
{
    public long Id { get; set; }
    public Guid Key { get; set; }
    public bool FirstUser { get; set; }
    public UserRequest Info { get; set; }
    public string ConnectionId { get; set; }
    public int Level { get; set; } = 1;

    public static List<HubUserGameResultItem> HubUserGameResultItem(List<RoomUser> model)
    {
        var r = new List<HubUserGameResultItem>();
        foreach (var m in model)
            r.Add(new HubUserGameResultItem(m.Id, m.Key));
        return r;
    }
}

