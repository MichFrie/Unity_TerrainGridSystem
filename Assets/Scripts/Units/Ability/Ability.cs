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

    public virtual IEnumerator Act(GridManager gridManager)
    {
        yield return 0;
    }

    IEnumerator Act(GridManager gridManager, Action<GridManager> preAction, Action<GridManager> postAction)
    {
        preAction(gridManager);
        yield return StartCoroutine(Act(gridManager));
        postAction(gridManager);

        yield return 0;
    }
}
