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
        
        public virtual void OnStateEnter()
        {
            // foreach (var cell in gameManager.Cells)
            // {
            //     cell.UnMark();
            // }
        }
        public virtual void OnStateExit()
        {
        }

        public virtual void OnUnitClicked(Unit unit)
        {
            Debug.Log(unit.name);
        }
    }
