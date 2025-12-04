namespace shelemApi.Models;

public enum userInGameStatusType : byte
{
    faal = 1, //فعال
    koshte = 2,

    ofline = 10,
    ekhraj = 11,
}
public enum GameState
{
    Reading, //خوندن کارت
    Burning, //سوزندن کارت
    Determination, //تعیین
    Game//بازی
}
