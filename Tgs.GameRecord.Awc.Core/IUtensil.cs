using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Awc.Core;

public interface IUtensil
{
    WagerStates GetWagerStates(bool isVoid, decimal realWinAmount);

    bool IsClosed(int txStatus);

    bool IsVoid(int txStatus, int settleStatus);
}
