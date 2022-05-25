using System.Collections.Generic;
using System.Linq;

public class ObjectiveDestroyCondition : GameEndCondition
{
    public Unit Objective;


    public override GameResult CheckCondition(GameManager gameManager)
    {
        //checks if ObjectiveDestroyed Object exists in Unit List???
        var ObjectiveDestroyed = !gameManager.Units.Exists(u => Equals(Objective));

        if (ObjectiveDestroyed)
        {
            List<int> winningPlayers = gameManager.Players.FindAll(p => p.PlayerNumber != Objective.PlayerNumber)
                .Select(p => p.PlayerNumber).ToList();
            
            List<int> losingPlayers = new List<int>() {Objective.PlayerNumber};
            return new GameResult(true, winningPlayers, losingPlayers);
        }
        else
        {
            return new GameResult(false, new List<int>(), new List<int>());
        }
    }
}
