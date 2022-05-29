using UnityEngine;

    public class HumanPlayer : Player
    {
        public override void Play(GameManager gameManager)
        {
            gameManager.CellGridState = new CellGridStateWaitingForInput(gameManager);
        }
    }
