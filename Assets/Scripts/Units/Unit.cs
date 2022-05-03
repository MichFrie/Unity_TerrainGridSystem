using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;

//BaseClass
public class Unit : MonoBehaviour
{ 
    //Movement Fields
   int startCellIndex;
   float movementPoints = 10;
   short moveCounter;
   List<int> moveList;
   
   TerrainGridSystem tgs;
   
   enum MOVEMENTSTATE
   {
      idle,
      moving,
      moveSelected
   }

   enum SELECTIONSTATE
   {
       selected,
       deselected
   }
   
   MOVEMENTSTATE movementState;
   SELECTIONSTATE selectionState;
   
   void Start()
   {
      tgs = TerrainGridSystem.instance;
      movementState = MOVEMENTSTATE.moveSelected;
      selectionState = SELECTIONSTATE.deselected;
   }

   void Update()
   {
       CalculateMovement();
       SelectUnit();
       DeselectUnit();
   }

   void CalculateMovement()
    {
        switch (movementState)
        {
            case MOVEMENTSTATE.idle:
                break;

            case MOVEMENTSTATE.moving:
                if (moveCounter < moveList.Count && selectionState == SELECTIONSTATE.selected)
                {
                    Move(tgs.CellGetPosition(moveList[moveCounter]));
                }
                else
                {
                    moveCounter = 0;
                    movementState = MOVEMENTSTATE.moveSelected;
                }
                break;

            case MOVEMENTSTATE.moveSelected:
                if (Input.GetMouseButtonUp(0))
                {   //definition of targetCell
                    int targetCell = tgs.cellHighlightedIndex;
                    if (targetCell != -1)
                    {
                        //definition of startCell
                        int startCell = tgs.CellGetIndex(tgs.CellGetAtPosition(transform.position, true));
                        float totalCost;
                        //builds a path from startCell to targetCell
                        moveList = tgs.FindPath(startCell, targetCell, out totalCost);
                        if (moveList == null)
                            return;

                        //check if path exceeds unitRange
                        if (movementPoints >= totalCost)
                        {
                            moveCounter = 0;
                            movementState = MOVEMENTSTATE.moving;
                            movementPoints -= totalCost;
                            Debug.Log("UnitMovementPoints: " + movementPoints);
                            
                        }
                        else
                        {
                            Debug.Log("Movement Range exceeded");
                        }
                    }
                    else
                    {
                        Debug.Log("No Cell");
                    }
                }
                break;
        }
    }
   //Moves Unit, checks if unit has reached next cell and updates movecounter if not 
   void Move(Vector3 targetPos)
   {
      float speed = 10;
      float step = speed * Time.deltaTime;

      transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
      
      float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetPos.x, targetPos.z));
      if (dist < 0.1f)
      {
          moveCounter++;
      }
   }

   void SelectUnit()
   {
       if (Input.GetMouseButtonDown(0))
       {
           Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
           RaycastHit hit;
           if (Physics.Raycast(ray, out hit))
           {
             Unit unitSelected = hit.transform.GetComponent<Unit>();
             unitSelected.selectionState = SELECTIONSTATE.selected;
           }
       }
   }

   void DeselectUnit()
   {
       if (Input.GetMouseButtonDown(1))
       {
           foreach (var unit in UnitManager.Instance.playableUnits)
           {
               unit.selectionState = SELECTIONSTATE.deselected;
           }
       }
   }
}

