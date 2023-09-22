using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Awc.Core;

internal class UtensilRepository : IUtensil
{
    public WagerStates GetWagerStates(bool isVoid, decimal realWinAmount)
        => isVoid ? WagerStates.Void : realWinAmount > 0 ? WagerStates.Win : WagerStates.Lose;

    public bool IsClosed(int txStatus)
        => txStatus switch
        {
            0 => false,
            _ => true
        };

    public bool IsVoid(int txStatus, int settleStatus)
        => txStatus switch
        {
            0 => false,
            2 => true,
            1 => settleStatus is not (0 or 1),
            _ => true
        };
}
