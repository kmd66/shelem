using shelemApi.Helper;
using System;
using System.Collections.Generic;

namespace shelemApi.Models;

public class Room
{
    public RoomProperty Property ;
    public RoomReading Reading;
    public RoomBurning Burning;
    public RoomGame Game;

    public Room(Guid roomId, List<RoomUser> users)
    {
        Property = new(roomId, users);
        Game = new(Property);
        Burning = new(Property);
        Reading = new(Property);
        SubscribeToEvents();
    }

    public async Task Start()
    {
        if (Property.IsStart)
            return;

        Property.StartToken = new CancellationTokenSource();
        Property.FinishToken = new CancellationTokenSource();
        Property.InitStrat = [];

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(Property.StartWait), Property.StartToken.Token);
        }
        finally
        {
            try
            {
                _= extensionStart();
            }
            catch (Exception) { }
        }
    }
    private async Task extensionStart()
    {
        try
        {
            bool _start = Property.IsStart;
        }
        catch (Exception)
        {
            _ = Property.ReceiveCansel();
            return;
        }

        if (!Property.IsStart)
        {
            _ = Property.ReceiveCansel();
        }
        else
        {
            Property.Tourner = Property.Users.Find(x => x.FirstUser == true).Id;
            await Task.Delay(500);
            Property.StartGameAt = DateTime.Now;
            await Property.ReceiveInit();
            await Property.ReceivePhysicsStandard();
            _ = Reading.Start();
        }
    }

    public void FinishGame()
    {
        if (Property.IsStart && Property.IsFinish) return;

        bool isReload = false;
        bool start = Property.IsStart;
        if (Property.IsStart && Property.FirstOflineCount < 3 && Property.SecondOflineCount < 3)
            isReload = true;
        dispose();

        var id1 =  Property.Users.First(x => x.FirstUser).Id;
        var id2 = Property.Users.First(x => !x.FirstUser).Id;
        long winner = 0;
        if (Property.FirstOflineCount > 2 || Property.SecondOflineCount > 2)
        {
            winner = Property.FirstOflineCount > 2 ? id2 : id1;
        }
        //else if (firstUserGoal != secondUserGoal)
        //{
        //    winner = firstUserGoal > secondUserGoal ? id1 : id2;
        //}
        //else
        //{
        //    winner = _random.Next(0, 2) == 0 ? id1 : id2;
        //}


        var reloadModel = new FinishModelRequest(
            key: AppStrings.ApiKey,
            roomId: Property.Id,
            start: start,
            isReload: isReload,
            winner: winner,
            user1: new FinishUser(Property.Users[0].ConnectionId, Property.Users[0].Key, Property.Users[0].Id),
            user2: new FinishUser(Property.Users[1].ConnectionId, Property.Users[1].Key, Property.Users[1].Id),
            BaseUrl: Helper.AppStrings.MainUrl
            );
        //OnReload(reloadModel, goals);
    }

    private void Main()
    {
        switch (Property.State)
        {
            case GameState.Reading: _ = Reading.Start(); break;
            case GameState.Burning: _ = Burning.StartBurning(); break;
            case GameState.Determination: _ = Burning.StartDetermination(); break;
            case GameState.Game: _ = Game.Start(); break;
        }
    }

    private async Task CompletSetReading(int reading)
    {
        if (reading > 360)
        {
            Property.MinReading = 365;
            Property.HakemUserId = Property.Users.First(x => x.Id == Property.Tourner).Id;
        }
        else if (reading < 100)
        {
            Property.HakemUserId = Property.Users.First(x => x.Id != Property.Tourner).Id;
        }
        if (Property.MinReading < 100) Property.MinReading = 100;

        var user = Property.Users.First(x => x.Id == Property.HakemUserId);
        if (user.FirstUser)
        {
            Property.Cards1.AddRange([.. Property.CardsGround0]);
        }
        else
        {
            Property.Cards2.AddRange([.. Property.CardsGround0]);
        }
        Property.CardsGround0 = [];

        Property.State = GameState.Burning;

        await Property.ReceiveCards();
        await Property.ReceiveHakem();
        Main();
    }
    private async Task CompletBurning(int reading)
    {
    }
    private async Task CompletDetermination(int reading)
    {
    }

    private async Task CompletGame(int reading)
    {
    }



    private void SubscribeToEvents()
    {
        Reading.CompletSetReading += async (reading) => await CompletSetReading(reading);
        Burning.CompletBurning += async (reading) => await CompletBurning(reading);
        Burning.CompletDetermination += async (reading) => await CompletDetermination(reading);
        Game.CompletGame += async (reading) => await CompletGame(reading);
    }
    private void UnsubscribeFromEvents()
    {
        Reading.CompletSetReading -= async (reading) => await CompletSetReading(reading);
        Burning.CompletBurning -= async (reading) => await CompletBurning(reading);
        Burning.CompletDetermination -= async (reading) => await CompletDetermination(reading);
        Game.CompletGame -= async (reading) => await CompletGame(reading);
    }

    private void dispose()
    {
        UnsubscribeFromEvents();
        Property.dispose();
        Reading.dispose();
        Burning.dispose();
        Game.dispose();
    }

}

