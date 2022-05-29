using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;

public class CellGridStateWaitingForInput : CellGridState
{
    public CellGridStateWaitingForInput(GameManager gameManager) : base(gameManager)
    {
        
    }
    //ok
    public override void OnUnitClicked(Unit unit)
    {
        if (gameManager.GetCurrentPlayerUnits().Contains(unit))
        {
            gameManager.CellGridState = new CellGridStateAbilitySelected(gameManager, unit, unit.GetComponents<Ability>().ToList());
        }
    }
}
