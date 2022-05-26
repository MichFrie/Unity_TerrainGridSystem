using System;
using System.Collections.Generic;
using UnityEngine;
using TGS;

//BaseClass
public class Unit : MonoBehaviour
{ 
    //PlayerNumber
    public int PlayerNumber;
    
    //Movement Fields
   int startCellIndex;
   float debugMovementPoints = 100;
   short moveCounter;
   List<int> moveList;
   TerrainGridSystem tgs;
   
    //Cell Tags
      int cellEmpty = 1;
      int cellOccupied = 2;
      
      
    //Buffs
    List<(Buff buff, int timeLeft)> Buffs;
    
    //Unit Attributes
    public int TotalHitPoints { get; private set; }
    public float TotalMovementPoints { get; private set; }
    public float TotalActionPoints { get; private set; }  
    
    public int HitPoints;
    public int AttackRange;
    public int AttackFactor;
    public int DefenceFactor;
    
    [SerializeField]
    float movementPoints;
    public virtual float MovementPoints
    {
        get
        {
            return movementPoints;
        }
        protected set
        {
            movementPoints = value;
        }
    }
    [SerializeField]
    float actionPoints = 1;
    public float ActionPoints
    {
        get
        {
            return actionPoints;
        }
        set
        {
            actionPoints = value;
        }
    }
    
    
    //Unit Cell
    Cell cell;  
    public Cell Cell
    {
        get { return cell; }
        set
        {
            cell = value;
        }
    }

    public UnitState UnitState { get; set; }

    //Enumerations
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
   enum FACING
   {
       Facing0,
       Facing60,
       Facing120,
       Facing180,
       Facing240,
       Facing300
   }

   [SerializeField] FACING facing;
   [SerializeField] int cone = 95;
   
   public Cell backOfCell;
   public Cell frontOfCell;
   public Cell topLeftOfCell;
   public Cell topRightOfCell;
   public Cell bottomLeftOfCell;
   public Cell bottomRightOfCell;

   MOVEMENTSTATE movementState;
   SELECTIONSTATE selectionState;

   Cell targetPoint;
   
   //EventHandler
   public event EventHandler UnitClicked;
   public event EventHandler UnitSelected;
   public event EventHandler UnitDeselected;
   public event EventHandler UnitHighlighted;
   public event EventHandler UnitDehighlighted;
   public event EventHandler<MovementEventArgs> UnitMoved;
   //TODO: Initialize doesnt get called
   public virtual void Initialize()
   {
       Buffs = new List<(Buff, int)>();
       
       //UnitState = new UnitStateNormal(this);

       TotalHitPoints = HitPoints;
       TotalMovementPoints = MovementPoints;
       TotalActionPoints = ActionPoints;
   }
   
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
           OnUnitSelected();
       }

       if (Input.GetKeyDown(KeyCode.T))
       {
           ShowMovementRange();
       }

       if (Input.GetKeyDown(KeyCode.H))
       {
           CheckConeAngleViaTargetPoint();
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
                        moveList = tgs.FindPath(startCell, targetCell, out totalCost, 0, 0, GridManager.cellEmpty_Group);
                        if (moveList == null)
                            return;
                                                                
                       
                        //check if path exceeds unitRange
                        if (debugMovementPoints >= totalCost)
                        {
                            moveCounter = 0;
                            movementState = MOVEMENTSTATE.Moving;
                            debugMovementPoints -= totalCost;
                            Debug.Log("UnitMovementPoints: " + debugMovementPoints);
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
           foreach (var unit in UnitManager.Instance.Units)
           {
               if (unit.movementState == MOVEMENTSTATE.MoveSelected)
               { 
                   unit.selectionState = SELECTIONSTATE.Deselected;
               }
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
       CalculateFacing();
   }

   void RotateLeft()
   {
       if(selectionState != SELECTIONSTATE.Selected) 
           return;
       
       transform.rotation *= Quaternion.Euler(0, -60, 0);
       CalculateFacing();
   }
   
   public void CalculateFacing()
   {
       int angle = Mathf.Abs((int)transform.eulerAngles.y);
        
       switch (angle)
       {
           case 0: CheckAnglesFor0();
               facing = FACING.Facing0;
               break;
           case 60: CheckAnglesFor60();
               facing = FACING.Facing60;
               break;
           case 120: CheckAnglesFor120();
               facing = FACING.Facing120;
               break;
           case 180: CheckAnglesFor180();
               facing = FACING.Facing180;
               break;
           case 240: CheckAnglesFor240();
               facing = FACING.Facing240;
               break;
           case 300: CheckAnglesFor300();
               facing = FACING.Facing300;
               break;
           default: break;
       }
   }
   
   void ShowCellSide()
   {
       if (selectionState == SELECTIONSTATE.Deselected)
           return;
       
       CalculateFacing();
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
        List<int> neighbours = tgs.CellGetNeighbours(cellIndex, (int)debugMovementPoints);
       
        if (neighbours != null)
        {
            tgs.CellFlash(neighbours, Color.yellow, 1f);
        }
    }

    void CheckConeAngleViaLastClickedHex()
    {
        List<int> coneIndices = new List<int>();
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        int cellIndex = tgs.CellGetIndex(cell);

        Vector2 startPos = tgs.cells[cellIndex].center;
        Vector2 endPos = tgs.cells[tgs.cellLastClickedIndex].center;
        Vector2 direction = endPos - startPos;
        float maxDistance = Vector2.Distance(startPos, endPos);
        direction /= maxDistance;
        
        tgs.CellGetWithinCone(cellIndex, direction, maxDistance, 95.0f, coneIndices);
        
        foreach (var c in coneIndices)
        {
            tgs.CellFlash(c, Color.cyan, 2f);
        }
    }
    
    void CheckConeAngleViaTargetPoint()
    {
        if (selectionState == SELECTIONSTATE.Deselected)
            return;
        
        List<int> coneIndices = new List<int>();
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        int cellIndex = tgs.CellGetIndex(cell);
        CalculateConesViaTargetPoints();
        int targetCellIndex = tgs.CellGetIndex(targetPoint);
        tgs.GetCellsWithinCone(cellIndex, targetCellIndex,  cone, coneIndices);

        foreach (var c in coneIndices)
        {
            tgs.CellFlash(c, Color.cyan, 2f);
            
        }
    }

    void CalculateConesViaTargetPoints()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        switch (facing)
        {
            case FACING.Facing0: targetPoint = tgs.CellGetAtPosition(column, row +3);
                break;
            case FACING.Facing60: targetPoint = tgs.CellGetAtPosition(column + 3, row + 2);
                break;
            case FACING.Facing120: targetPoint = tgs.CellGetAtPosition(column + 3, row - 1);
                break;
            case FACING.Facing180: targetPoint = tgs.CellGetAtPosition(column, row - 3);
                break;
            case FACING.Facing240: targetPoint = tgs.CellGetAtPosition(column - 3, row - 1);
                break;
            case FACING.Facing300: targetPoint = tgs.CellGetAtPosition(column - 3, row + 2);
                break;
                
            default: break;
        }
    }
    
    public virtual bool IsUnitAttackable(Unit target, int targetCell, int sourceCell)
    {
        return GridManager.Instance.GetDistance(sourceCell, targetCell) <= AttackRange
               && target.PlayerNumber != PlayerNumber
               && ActionPoints >= 1;
    }
    
    //Events Functions, get called in CellGridState
    public void OnMouseDown()
    {
        if (UnitClicked != null)
        {
            UnitClicked.Invoke(this, new EventArgs());
        }
    }

    protected virtual void OnMouseEnter()
    {
        if (UnitHighlighted != null)
        {
            UnitHighlighted.Invoke(this, new EventArgs());
        }
    }
    protected virtual void OnMouseExit()
    {
        if (UnitDehighlighted != null)
        {
            UnitDehighlighted.Invoke(this, new EventArgs());
        }
    }

    public void SetState(UnitState state)
     {
         UnitState.MakeTransition(state);
         Debug.Log(string.Format("{0} - {1}", gameObject, UnitState));
     }
    
    public virtual void OnUnitSelected()
    {
        
        if (FindObjectOfType<GameManager>().GetCurrentPlayerUnits().Contains(this))
        {
            Debug.Log("Test on unit selected");
            SetState(new UnitStateMarkedAsSelected(this));
        }
        if (UnitSelected != null)
        {
            UnitSelected.Invoke(this, new EventArgs());
        }
    }

    public virtual void OnTurnStart()
    {
        //cachedPaths = null;
        
        Buffs.FindAll(b=>b.timeLeft == 0).ForEach(b => { b.buff.Undo(this);});
        Buffs.RemoveAll(b => b.timeLeft == 0);
        var name = this.name;
        var state = UnitState;
        //SetState(new UnitStateMarkedAsFriendly(this));
    }

    public virtual void OnTurnEnd()
    {
        for (int i = 0; i < Buffs.Count; i++)
        {
            (Buff buff, int timeLeft) = Buffs[i];
            Buffs[i] = (buff, timeLeft - 1);
        }

        // MovementPoints = TotalMovementPoints;
        // ActionPoints = TotalActionPoints;
    }
    /// <summary>
    /// Unit State Methods
    /// </summary>


    public virtual void MarkAsSelected()
    {
        
    }

    //MovementEventArgs
    public class MovementEventArgs : EventArgs
    {
        public Cell OriginCell;
        public Cell DestinationCell;
        public List<Cell> Path;
        public Unit Unit;

        public MovementEventArgs(Cell sourceCell, Cell destinationCell, List<Cell> path, Unit unit)
        {
            OriginCell = sourceCell;
            DestinationCell = destinationCell;
            Path = path;
            Unit = unit;
        }
    }


    //AttackEventArgs
    public class AttackEventArgs : EventArgs
    {
        public Unit Attacker;
        public Unit Defender;
        public int Damage;
        public AttackEventArgs(Unit attacker, Unit defender, int damage)
        {
            Attacker = attacker;
            Defender = defender;
            Damage = damage;
        }
    }
    /// <summary>
    /// Gives visual indication that the unit is under attack.
    /// </summary>
    /// <param name="aggressor">
    /// Unit that is attacking.
    /// </param>
    
    public virtual void MarkAsFriendly()
    {
        // if (UnitHighlighterAggregator != null)
        // {
        //     UnitHighlighterAggregator.MarkAsFriendlyFn?.ForEach(o => o.Apply(this, null));
        // }
    }
    
    //Units can see through other units but not wood etc.
    // void ShowLineOfSight()
    // {
    //     List<int> neighbours = tgs.CellGetNeighbours(tgs.cellLastClickedIndex, 10, tgs.CellGetGroup(GridManager.cellOccupied_Group));
    //     if(neighbours != null)
    //     {
    //         tgs.CellTestLineOfSight(tgs.cellHighlightedIndex, neighbours);
    //         tgs.CellFlash(neighbours, Color.red, 1f);
    //     }
    // }
}

