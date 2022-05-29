using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CellGridStateWaitingForInput : CellGridState
{
    public CellGridStateWaitingForInput(GameManager gameManager) : base(gameManager)
    {
        
    }
    public override void OnUnitClicked(Unit unit)
    {
        if (gameManager.GetCurrentPlayerUnits().Contains(unit))
        {
            gameManager.CellGridState = new CellGridStateAbilitySelected(gameManager, unit, unit.GetComponents<Ability>().ToList());
           
        }
        Debug.Log("Waiting");
    }
}
