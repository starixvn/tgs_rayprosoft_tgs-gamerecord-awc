using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Awc.Core.UnitTest;

public class UtensilRepositoryUnitTest
{
    [Fact]
    public void GameRound_Client_Status_Is_Loss()
    {
        var repository = new UtensilRepository();

        var actual = repository.GetWagerStates(isVoid: false, realWinAmount: 0);

        Assert.Equal(WagerStates.Lose, actual);
    }

    [Fact]
    public void GameRound_Client_Status_Is_Void()
    {
        var repository = new UtensilRepository();

        var actual = repository.GetWagerStates(isVoid: true, realWinAmount: 0);

        Assert.Equal(WagerStates.Void, actual);
    }

    [Fact]
    public void GameRound_Client_Status_Is_Win()
    {
        var repository = new UtensilRepository();

        var actual = repository.GetWagerStates(isVoid: false, realWinAmount: 10);

        Assert.Equal(WagerStates.Win, actual);
    }

    [Fact]
    public void TxStatus_0_Is_Closed_False()
    {
        var repository = new UtensilRepository();

        var actual = repository.IsClosed(0);

        Assert.False(actual);
    }

    [Fact]
    public void TxStatus_1_Is_Closed_True()
    {
        var repository = new UtensilRepository();

        var actual = repository.IsClosed(2);

        Assert.True(actual);
    }

    [Fact]
    public void TxStatus_1_SettleStatus_0_Is_Void_False()
    {
        var repository = new UtensilRepository();

        var actual = repository.IsVoid(1, 0);

        Assert.False(actual);
    }

    [Fact]
    public void TxStatus_1_SettleStatus_1_Is_Void_False()
    {
        var repository = new UtensilRepository();

        var actual = repository.IsVoid(1, 1);

        Assert.False(actual);
    }

    [Fact]
    public void TxStatus_1_SettleStatus_Minus_1_Is_Void_True()
    {
        var repository = new UtensilRepository();

        var actual = repository.IsVoid(1, -1);

        Assert.True(actual);
    }

    [Fact]
    public void TxStatus_2_SettleStatus_Minus_1_Is_Void_True()
    {
        var repository = new UtensilRepository();

        var actual = repository.IsVoid(2, -1);

        Assert.True(actual);
    }

    [Fact]
    public void TxStatus_3_SettleStatus_Minus_1_Is_Void_True()
    {
        var repository = new UtensilRepository();

        var actual = repository.IsVoid(3, -1);

        Assert.True(actual);
    }
}
