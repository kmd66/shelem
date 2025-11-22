using shelemApi.Helper;

namespace shelemApi.Models;

public class Room : RoomProperty
{

    public async Task Start()
    {
        if (isStart)
            return;

        startToken = new CancellationTokenSource();
        finishToken = new CancellationTokenSource();
        initStrat = [];


        try
        {
            await Task.Delay(startWait * 1000, startToken.Token);
        }
        finally
        {
            try
            {
                extensionStart();
            }
            catch (Exception) { }
        }
    }
    public async void SetStart(Guid key)
    {
    }

    private async void extensionStart()
    {
    }

    private async void cancelRoom()
    {
    }


    private async void main()
    {
    }

    private void mainJob()
    {
    }

    private bool checkOflineCount()
    {

        return true;
    }

    private async void completeSendData()
    {
    }


    public override void FinishGame()
    {
    }

    private long setWinner()
    {
        return 0;
    }

    private void dispose()
    {
    }

}

