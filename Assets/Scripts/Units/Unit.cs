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
   
    //Cell Tags
      int cellEmpty = 1;
      int cellOccupied = 2;
      
   enum MOVEMENTSTATE
   {
      Idle,
      Moving,
      MoveSelected
   }

   enum SELECTIONSTATE
   {
       Selected,
       Deselected
   }
   
   enum CELLSIDES
   {
       FrontOFCell, 
       BackOfCell, 
       TopLeftOfCell, 
       TopRightOfCell, 
       BottomLeftOfCell, 
       BottomRightOfCell
   }
   
   public Cell backOfCell;
   public Cell frontOfCell;
   public Cell topLeftOfCell;
   public Cell topRightOfCell;
   public Cell bottomLeftOfCell;
   public Cell bottomRightOfCell;

   public UnitStats unitStats;
   
   MOVEMENTSTATE movementState;
   SELECTIONSTATE selectionState;
   
   void Start()
   {
      tgs = TerrainGridSystem.instance;
      movementState = MOVEMENTSTATE.MoveSelected;
      selectionState = SELECTIONSTATE.Deselected;
   }
   
   void Update()
   {
       CalculateMovement();
       SelectUnit();
       DeselectUnit();

       if (Input.GetKeyDown(KeyCode.O))
       {
           DefineFrontFacing();
       }
       if (Input.GetKeyDown(KeyCode.Q))
       {
           RotateLeft();
       }

       if (Input.GetKeyDown(KeyCode.E))
       {
           RotateRight();
       }
       if ((Input.GetKeyDown(KeyCode.I)))
       { 
           ShowCellSide();
       }

       if (Input.GetKeyDown(KeyCode.T))
       {
           ShowMovementRange();
       }
   }
   
   void CalculateMovement()
    {
        if(selectionState != SELECTIONSTATE.Selected)
            return;
        
        switch (movementState)
        {
            case MOVEMENTSTATE.Idle:
                break;

            case MOVEMENTSTATE.Moving:
                if (moveCounter < moveList.Count)
                {
                    Move(tgs.CellGetPosition(moveList[moveCounter]));
                }
                else
                {
                    moveCounter = 0;
                    movementState = MOVEMENTSTATE.MoveSelected;
                }
                break;

            case MOVEMENTSTATE.MoveSelected:
                if (Input.GetMouseButtonUp(0))
                {
                    int targetCell = tgs.cellHighlightedIndex;
                    if (targetCell != -1)
                    {
                        //startCell
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
                            movementState = MOVEMENTSTATE.Moving;
                            movementPoints -= totalCost;
                            Debug.Log("UnitMovementPoints: " + movementPoints);
                        }
                        else
                        {
                            Debug.Log("Movement Range exceeded");
                        }
                        tgs.CellSetTag(startCell, cellEmpty);
                        tgs.CellSetTag(targetCell, cellOccupied);
                        GridManager.Instance.UpdateOccupiedCellsList(startCell, cellEmpty, targetCell, cellOccupied);
                    }
                    else
                    {
                        Debug.Log("No Cell");
                    }
                }
                break;
        }
    }
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
               if (hit.transform.CompareTag("TestUnit"))
               {
                    Unit unitSelected = hit.transform.GetComponent<Unit>();
                    unitSelected.selectionState = SELECTIONSTATE.Selected;
               }
           }
       }
   }

   void DeselectUnit()
   {
       if (Input.GetMouseButtonDown(1))
       {
           foreach (var unit in UnitManager.Instance.playableUnits)
           {
               unit.selectionState = SELECTIONSTATE.Deselected;
           }
       }
   }
   
   void DefineFrontFacing()
   {
       int cellIndex = tgs.CellGetNeighbour(tgs.cellLastClickedIndex, CELL_SIDE.Bottom);
       tgs.CellFlash(cellIndex, Color.cyan, 1f);
   }
   
   void RotateRight()
   {
       if(selectionState != SELECTIONSTATE.Selected) 
           return;
       
       transform.rotation *= Quaternion.Euler(0, 60, 0);
   }

   void RotateLeft()
   {
       if(selectionState != SELECTIONSTATE.Selected) 
           return;
       
       transform.rotation *= Quaternion.Euler(0, -60, 0);
   }
   
   public void CalculateCellSide()
   {
       int angle = Mathf.Abs((int)transform.eulerAngles.y);

       switch (angle)
       {
           case 0: CheckAnglesFor0(); break;
           case 60: CheckAnglesFor60(); break;
           case 120: CheckAnglesFor120(); break;
           case 180: CheckAnglesFor180(); break;
           case 240: CheckAnglesFor240(); break;
           case 300: CheckAnglesFor300(); break;
           default: break;
       }
   }
   
   void ShowCellSide()
   {
       if (selectionState == SELECTIONSTATE.Deselected)
           return;
       
       CalculateCellSide();
       tgs.CellColorTemp(frontOfCell, Color.green, 3f);
       tgs.CellColorTemp(topLeftOfCell, Color.green, 3f);
       tgs.CellColorTemp(topRightOfCell, Color.green, 3f);
       tgs.CellColorTemp(backOfCell, Color.red, 3f);
       tgs.CellColorTemp(bottomLeftOfCell, Color.red, 3f);
       tgs.CellColorTemp(bottomRightOfCell, Color.red, 3f);
   }
void CheckAnglesFor0()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        backOfCell = tgs.CellGetAtPosition(column, row - 1);
        frontOfCell = tgs.CellGetAtPosition(column, row + 1);
        topLeftOfCell = tgs.CellGetAtPosition(column -1, row + 1);
        topRightOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        bottomLeftOfCell = tgs.CellGetAtPosition(column - 1, row);
        bottomRightOfCell = tgs.CellGetAtPosition(column + 1, row);
    }


    void CheckAnglesFor60()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        
        int row = cell.row;
        int column = cell.column;

        backOfCell = tgs.CellGetAtPosition(column - 1, row);
        frontOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        topLeftOfCell = tgs.CellGetAtPosition(column, row + 1);
        topRightOfCell = tgs.CellGetAtPosition(column + 1, row);
        bottomLeftOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        bottomRightOfCell = tgs.CellGetAtPosition(column, row -1);
    }

    void CheckAnglesFor120()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        backOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        frontOfCell = tgs.CellGetAtPosition(column + 1, row);
        topLeftOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        topRightOfCell = tgs.CellGetAtPosition(column, row - 1);
        bottomLeftOfCell = tgs.CellGetAtPosition(column, row + 1);
        bottomRightOfCell = tgs.CellGetAtPosition(column - 1, row);
    }  
    void CheckAnglesFor180()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        backOfCell = tgs.CellGetAtPosition(column, row + 1);
        frontOfCell = tgs.CellGetAtPosition(column, row - 1);
        topLeftOfCell = tgs.CellGetAtPosition(column + 1, row);
        topRightOfCell = tgs.CellGetAtPosition(column - 1, row);
        bottomLeftOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        bottomRightOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
    }

    void CheckAnglesFor240()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        backOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        frontOfCell = tgs.CellGetAtPosition(column - 1, row);
        topLeftOfCell = tgs.CellGetAtPosition(column, row - 1);
        topRightOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        bottomLeftOfCell = tgs.CellGetAtPosition(column + 1, row);
        bottomRightOfCell = tgs.CellGetAtPosition(column, row + 1);
    }
    void CheckAnglesFor300()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        backOfCell = tgs.CellGetAtPosition(column + 1, row);
        frontOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        topLeftOfCell = tgs.CellGetAtPosition(column - 1, row);
        topRightOfCell = tgs.CellGetAtPosition(column, row + 1);
        bottomLeftOfCell = tgs.CellGetAtPosition(column, row - 1);
        bottomRightOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
    }  
    void ShowMovementRange()
    {     
        if (selectionState == SELECTIONSTATE.Deselected)
            return;
        
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        int cellIndex = tgs.CellGetIndex(cell);
        List<int> neighbours = tgs.CellGetNeighbours(cellIndex, (int)movementPoints);
       
        if (neighbours != null)
        {
            tgs.CellFlash(neighbours, Color.yellow, 1f);
        }
    }
}

