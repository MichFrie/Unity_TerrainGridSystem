using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public Unit UnitReference { get; internal set; }


    public virtual void Awake()
    {
        UnitReference = GetComponent<Unit>();
    }

    public void Execute(GridManager gridManager, Action<GridManager> preAction, Action<GridManager> postAction)
    {
        
    }

    public void HumanExecute(GridManager gridManager)
    { 
        
    }

    public void AiExecute(GridManager gridManager)
    {
        
    }

    public virtual IEnumerator Act(GameManager gameManager)
    {
        yield return 0;
    }

    public virtual IEnumerator Act(GameManager gameManager, Action<GameManager> preAction, Action<GameManager> postAction)
    {
        preAction(gameManager);
        yield return StartCoroutine(Act(gameManager));
        postAction(gameManager);

        yield return 0;
    }
    
    public virtual void Display(GameManager gameManager) { }
    public virtual void OnTurnEnd(GameManager gameManager) { }
    public virtual void OnTurnStart(GameManager gameManager) { }
    public virtual void OnUnitClicked(Unit unit, GameManager gameManager) { }
    public virtual void OnAbilitySelected(GameManager gameManager) { }
    public virtual bool CanPerform(GameManager gameManager) { return false; }
}
