using shelemApi.Helper;
using System;
using System.Collections.Generic;

namespace shelemApi.Models;

public class Room
{
    public RoomProperty _p ;
    public RoomReading _reading;
    public RoomBurning _burning;
    public RoomGame _game;

    public Room(Guid roomId, List<RoomUser> users)
    {
        _p = new(roomId, users);
        _game = new(_p);
        _burning = new(_p);
        _reading = new(_p);
        SubscribeToEvents();
    }

    public async Task Start()
    {
        if (_p.IsStart)
            return;

        _p.StartToken = new CancellationTokenSource();
        _p.FinishToken = new CancellationTokenSource();
        _p.InitStrat = [];

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(_p.StartWait), _p.StartToken.Token);
        }
        finally
        {
            try
            {
                _= ExtensionStart();
            }
            catch (Exception) { }
        }
    }

    private async Task ExtensionStart()
    {
        try
        {
            bool _start = _p.IsStart;
        }
        catch (Exception)
        {
            _ = _p.ReceiveCansel();
            return;
        }

        if (!_p.IsStart)
        {
            _ = _p.ReceiveCansel();
        }
        else
        {
            _p.Tourner = _p.Users.Find(x => x.FirstUser == true).Id;
            await Task.Delay(500);
            _p.StartGameAt = DateTime.Now;
            await _p.InitEvent("ReceiveInit");
            await _p.ReceivePhysicsStandard();
            _ = _reading.Start();
        }
    }

    private void Main()
    {
        if (CheckEndGame())
        {
            EndGame();
            return;
        }

        switch (_p.State)
        {
            case GameState.Reading: _ = _reading.Start(); break;
            case GameState.Burning: _burning.StartBurning(); break;
            case GameState.Determination: _burning.StartDetermination(); break;
            case GameState.Game: _ = _game.Main(); break;
        }
    }


    #region Complet event
    private async Task CompletSetReading(int reading)
    {
        if (reading > 160)
        {
            _p.MinReading = 165;
            _p.HakemUserId = _p.Users.First(x => x.Id == _p.Tourner).Id;
        }
        else if (reading < 100)
        {
            _p.HakemUserId = _p.Users.First(x => x.Id != _p.Tourner).Id;
        }
        _p.Tourner = _p.HakemUserId;

        if (_p.MinReading < 100) _p.MinReading = 100;

        var user = _p.Users.First(x => x.Id == _p.HakemUserId);
        if (user.FirstUser)
        {
            _p.Cards1.AddRange([.. _p.CardsGround0]);
        }
        else
        {
            _p.Cards2.AddRange([.. _p.CardsGround0]);
        }
        _p.CardsGround0 = [];

        _p.State = GameState.Burning;

        await _p.ReceiveCards();
        await _p.ReceiveHakem();
        Main();
    }

    private void CompletBurning()
    {
        _p.State = GameState.Determination;
        Main();
    }

    private async Task CompletDetermination()
    {
        if (CheckEndGame())
        {
            EndGame();
            return;
        }
        _p.State = GameState.Game;
        _game.SetCardGroup();
        await _p.ReceiveCards();
        await Task.Delay(TimeSpan.FromSeconds(5));
        _ = _game.Main(true);
    }

    private async Task CompletGame()
    {
        await Task.Delay(50);
    }

    #endregion

    #region win

    private bool CheckEndGame()
    {
        if (_p.CheckOflineCount)
            return true;
        if (_p.CheckTotalScore)
            return true;
        if (_p.StartGameAt.AddSeconds(_p.GameTime) < DateTime.Now)
            return true;
        return false;
    }

    private void EndGame()
    {
    }

    public void FinishGame()
    {
        if (_p.IsStart && _p.IsFinish) return;

        bool isReload = false;
        bool start = _p.IsStart;
        if (_p.IsStart && _p.FirstOflineCount < 3 && _p.SecondOflineCount < 3)
            isReload = true;
        dispose();

        var id1 = _p.Users.First(x => x.FirstUser).Id;
        var id2 = _p.Users.First(x => !x.FirstUser).Id;
        long winner = 0;
        if (_p.FirstOflineCount > 2 || _p.SecondOflineCount > 2)
        {
            winner = _p.FirstOflineCount > 2 ? id2 : id1;
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
            roomId: _p.Id,
            start: start,
            isReload: isReload,
            winner: winner,
            user1: new FinishUser(_p.Users[0].ConnectionId, _p.Users[0].Key, _p.Users[0].Id),
            user2: new FinishUser(_p.Users[1].ConnectionId, _p.Users[1].Key, _p.Users[1].Id),
            BaseUrl: Helper.AppStrings.MainUrl
            );
        //OnReload(reloadModel, goals);
    }

    #endregion

    #region dispose


    private void SubscribeToEvents()
    {

        _reading.CompletSetReading += CompletSetReading;
        _burning.CompletBurning += CompletBurning;
        _burning.CompletDetermination += CompletDetermination;
        _game.CompletGame += CompletGame;

    }
    
    private void UnsubscribeFromEvents()
    {
        _reading.CompletSetReading -= CompletSetReading;
        _burning.CompletBurning -= CompletBurning;
        _burning.CompletDetermination -= CompletDetermination;
        _game.CompletGame -= CompletGame;
    }

    private void dispose()
    {
        UnsubscribeFromEvents();
        _p.dispose();
        _reading.dispose();
        _burning.dispose();
        _game.dispose();
    }

    #endregion
}

