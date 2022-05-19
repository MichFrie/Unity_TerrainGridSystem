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
        var nextPlayerNumber = (gameManager.CurrentPlayerNumber + 1) % gameManager.NumberOfPlayers;
        while (gameManager.Units.FindAll(u => u.PlayerNumber.Equals(nextPlayerNumber)).Count == 0)
        {
            nextPlayerNumber = (nextPlayerNumber + 1) % gameManager.NumberOfPlayers;
        }

        var nextPlayer = gameManager.Players.Find(p => p.PlayerNumber == nextPlayerNumber);
        var allowedUnits = gameManager.Units.FindAll(u => u.PlayerNumber == nextPlayerNumber);

        return new TransitionResult(nextPlayer, allowedUnits);
    }
}
