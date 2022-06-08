using System.Collections.Generic;
using System.Linq;
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
    public CellGridStateAbilitySelected(GameManager gameManager, Unit unit, Ability ability) : this(gameManager, unit, new List<Ability>() { ability }) {}
    //ok
    public override void OnUnitClicked(Unit unit)
    {
        _abilities.ForEach(a => a.OnUnitClicked(unit, gameManager));
        Debug.Log("CellGridStateAbilitySelected");
    }

    //TODO:function gets called, but unit is null???
    public override void OnStateEnter()
    {
        _unit?.OnUnitSelected();
        
        _abilities.ForEach(a => a.OnAbilitySelected(gameManager));
        _abilities.ForEach(a=>a.Display(gameManager));

        
        var canPerformAction = _abilities.Select(a=>a.CanPerform(gameManager)).DefaultIfEmpty().Aggregate((result, next) => result || next);

        if (!canPerformAction)
        {
            _unit?.SetState(new UnitStateMarkedAsFinished(_unit));
            
        }
        else
        {
            _unit?.SetState(new UnitStateNormal(_unit));
        }
    }

}
