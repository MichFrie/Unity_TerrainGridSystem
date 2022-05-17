using UnityEngine;

    public abstract class CellGridState
    {       
        protected GameManager gameManager;
        public CellGridState(GameManager _gameManager)
        {
            gameManager = _gameManager;
        }
    }
