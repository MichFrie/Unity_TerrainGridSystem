using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AttackAbility: Ability
{
    public Unit UnitToAttack { get; set; }
    List<Unit> inAttackRange;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Display(GameManager.Instance);
        }
    }

    public override IEnumerator Act(GameManager gameManager)
    {
        if (UnitReference.IsUnitAttackable(UnitToAttack))
        {
            UnitReference.AttackHandler(UnitToAttack);
            yield return new WaitForSeconds(0.5f);
        }
        yield return 0;
    }
    
    public override void Display(GameManager gameManager)
    {
        var unit = GetComponent<Unit>();
        var enemyUnits = gameManager.GetEnemyUnits(gameManager.CurrentPlayer);
        //inAttackRange = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u, GridManager.Instance.GetLastClickedPosition() ,GridManager.Instance.GetUnitPosition())).ToList();
        inAttackRange.ForEach(u=>u.MarkAsReachableEnemy());
    }

    //generally ok, if part not working
    public override void OnUnitClicked(Unit unit, GameManager gameManager)
    {
        if (unit.IsUnitAttackable(unit))
        {
            UnitToAttack = unit;
            Debug.Log("TestAttack");
        }
        Debug.Log("AttackAbility");
    }
}