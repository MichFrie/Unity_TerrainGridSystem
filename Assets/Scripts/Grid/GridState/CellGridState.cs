using UnityEngine;

    public abstract class CellGridState
    {       
        protected GameManager gameManager;
        public CellGridState(GameManager _gameManager)
        {
            gameManager = _gameManager;
        }
        public virtual CellGridState MakeTransition(CellGridState nextState)
        {
            return nextState;
        }
        public virtual void OnStateExit()
        {
        }
    }
