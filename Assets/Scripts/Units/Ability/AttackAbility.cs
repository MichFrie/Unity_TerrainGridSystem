using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackAbility: Ability
{
    public Unit UnitToAttack { get; set; }
    List<Unit> inAttackRange;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Display(GameManager.Instance);
        }
    }

    public override void Display(GameManager gameManager)
    {
        var unit = GetComponent<Unit>();
        var enemyUnits = gameManager.GetEnemyUnits(gameManager.CurrentPlayer);
        inAttackRange = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u, GridManager.Instance.GetLastClickedPosition() ,GridManager.Instance.GetUnitPosition())).ToList();
    }
}