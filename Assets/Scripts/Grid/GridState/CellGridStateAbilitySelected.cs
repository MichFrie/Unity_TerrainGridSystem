﻿using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CellGridStateAbilitySelected : CellGridState
{
    List<Ability> _abilities;
    Unit _unit;

    //ok
    public CellGridStateAbilitySelected(GameManager gameManager, Unit unit, List<Ability> abilities) : base(gameManager)
    {
        if (abilities.Count == 0)
        {
            Debug.LogError("No abilities were selected, check if unit has any abilities attached");
        }

        _abilities = abilities;
        _unit = unit;
    }

    //ok
    public override void OnUnitClicked(Unit unit)
    {
        _abilities.ForEach(a => a.OnUnitClicked(unit, gameManager));
    }
}
