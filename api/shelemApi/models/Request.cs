namespace shelemApi.Models;

public record CancelModelRequest(string key, Guid roomId);
public record FinishUser(string connectionId, Guid userKey, long userId, bool accept = false);
public record FinishModelRequest(string key, Guid roomId, bool start, bool isReload, long winner, FinishUser user1, FinishUser user2
    , string BaseUrl , byte gameType = 104);

public record UserRequest
(
long Id ,
string Bio ,
string Img ,
string FirstName ,
string LastName ,
string UserName ,
DateTime BirthDate,
int Level = 1
);

public record CreateRoomRequest(string key, List<UserRequest> users);
public record ReloadRequest(List<Guid> keys, List<UserRequest> users);
public record ReloadDoublesGameModel(Guid roomId, List<Guid> keys, List<UserRequest> users);
