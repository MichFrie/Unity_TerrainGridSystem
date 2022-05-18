using System.Linq;

public class SubsequentTurnResolver : TurnResolver
{
    public override TransitionResult ResolveStart(GameManager gameManager)
    {
        var nextPlayerNumber = gameManager.Players.Min(p => p.PlayerNumber);
        var nextPlayer = gameManager.Players.Find(p => p.PlayerNumber == nextPlayerNumber);
        var allowedUnits = gameManager.Units.FindAll(u=>u.PlayerNumber == nextPlayerNumber);

        return new TransitionResult(nextPlayer, allowedUnits);
    }

    public override TransitionResult ResolveTurn(GameManager gameManager)
    {
        throw new System.NotImplementedException();
    }
}
