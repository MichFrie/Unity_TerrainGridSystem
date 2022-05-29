using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager Instance;

    //EventHandler
    // LevelLoading event is invoked before Initialize method is run.
    public event EventHandler LevelLoading;
    

    // LevelLoadingDone event is invoked after Initialize method has finished running.
    public event EventHandler LevelLoadingDone;
    
    //GameStarted event is invoked at the beginning of StartGame method.
    public event EventHandler GameStarted;
    
    //GameEnded event is invoked when there is a single player left in the game.
    public event EventHandler<GameEndedArgs> GameEnded;
    
    //Turn ended event is invoked at the end of each turn.
    public event EventHandler TurnEnded;

    //CellGridState
    CellGridState cellGridState;
    
    //TODO:CellGridState is not initialized
    //Property for CellGridState
    public CellGridState CellGridState
    {
        get { return cellGridState; }
        set
        {
            CellGridState nextState;
            if (cellGridState != null)
            {
                // Method is called on transitioning out of a state.
                cellGridState.OnStateExit();
                //Sets next state, MakeTransition returns CellGridState nextState
                nextState = cellGridState.MakeTransition(value);
            }
            else
            {
                nextState = value;
            }
            cellGridState = nextState;
            cellGridState.OnStateEnter();
        }
    }
    
    //Number of Players
    public List<Player> Players { get; private set; }
    public int NumberOfPlayers
    {
        get { return Players.Count; }
    }
    
    //Current Player
    public int CurrentPlayerNumber { get; private set; }
    public Player CurrentPlayer
    {
        get { return Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)); }
    }

    //GameStatus
    public bool GameFinished { get; private set; }
    
    //PlayerParent
    public Transform PlayerParent;
    
    //Playable Units
    List<Unit> PlayableUnits = new List<Unit>();

    //Units, List must still be implemented
    public List<Unit> Units { get; private set; }

    //Awake
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        //Old Method only for testing, replace!
        UnitManager.Instance.FindPlayableUnits();
    
        //Event is implemented in GUI_Controller
        if (LevelLoading != null)
        {
            LevelLoading.Invoke(this, new EventArgs());
        }
        //Todo: UnitManager not working
        //PlayableUnits = UnitManager.Instance.playableUnits;
        Initialize();

        if (LevelLoadingDone != null)
        {
            LevelLoadingDone.Invoke(this, new EventArgs());
        }
        StartGame();
    }

    void Initialize()
    {
        GameFinished = false;

        Players = new List<Player>();

        //Working, Initializes Players
        for (int i = 0; i < PlayerParent.childCount; i++)
        {
            var player = PlayerParent.GetChild(i).GetComponent<Player>();
            if(player != null && player.gameObject.activeInHierarchy)
            {
                //TODO: will get overriden by AiPlayer, not yet implemented. Human Player just calls base method
                player.Initialize(this);
                Players.Add(player);
            }
        }
        
        //GridManager will iterate over cells

        //UnitManager will iterate over units
        Units = new List<Unit>();

        foreach (var unit in UnitManager.Instance.Units)
        {
            AddUnit(unit.GetComponent<Transform>());
        }
    }

    public void StartGame()
    {
        if (GameStarted != null)
        {
            GameStarted.Invoke(this, new EventArgs());
        }
        //calls one of the TurnResolvers, eg SubsequentTurnResolver. all TurnResolvers inherit from abstract class TurnResolver. SubseqneutTurnResolver overrides ResolveStart Method. REsolveStart finds nextPlayerNmber
        //NextPlayer and all allowed units
        TransitionResult transitionResult = GetComponent<TurnResolver>().ResolveStart(this);

        //Stores all playable units in list
        PlayableUnits = transitionResult.PlayableUnits;
        
        CurrentPlayerNumber = transitionResult.NextPlayer.PlayerNumber;
        
        //removes buffs that have run out of time
        PlayableUnits.ForEach(u => { u.GetComponents<Ability>().ToList().ForEach(a => a.OnTurnStart(this)); u.OnTurnStart(); });
        
        CurrentPlayer.Play(this);
        Debug.Log("GameStarted");
    
    }

    public void EndTurn()
    {
        //blocks PlayerInput while CPU is playing?
        cellGridState = new CellGridStateBlockInput(this);

        //checks if Game is finished
        bool isGameFinished = CheckGameFinished();
        if (isGameFinished)
            return;
        
        //reduces the duration of buffs by 1 (Unit.cs), Ability.cs is abstract call?
        PlayableUnits.ForEach(u => { if (u != null) { u.OnTurnEnd(); u.GetComponents<Ability>().ToList().ForEach(a => a.OnTurnEnd(this)); } });

        //resolves Turn and calls next player
        TransitionResult transitionResult = GetComponent<TurnResolver>().ResolveTurn(this);
        PlayableUnits = transitionResult.PlayableUnits;
        CurrentPlayerNumber = transitionResult.NextPlayer.PlayerNumber;

        if (TurnEnded != null)
        {
            TurnEnded.Invoke(this, new EventArgs());
        }
        
        Debug.Log(string.Format("Player {0} turn", CurrentPlayerNumber));
        
        //removes all buffs that have run out of time
        PlayableUnits.ForEach(u => { u.GetComponents<Ability>().ToList().ForEach(a => a.OnTurnStart(this)); u.OnTurnStart(); });
        CurrentPlayer.Play(this);
    }
    public List<Unit> GetCurrentPlayerUnits()
    {
        return PlayableUnits;
    }
    public List<Unit> GetEnemyUnits(Player player)
    {
        return Units.FindAll(u => u.PlayerNumber != player.PlayerNumber);
    }
    public List<Unit> GetPlayerUnits(Player player)
    {
        return Units.FindAll(u => u.PlayerNumber == player.PlayerNumber);
    }
    
    public void AddUnit(Transform unit)
    {
        Units.Add(unit.GetComponent<Unit>());
        unit.GetComponent<Unit>().UnitClicked += OnUnitClicked;
        // unit.GetComponent<Unit>().UnitHighlighted += OnUnitHighlighted;
        // unit.GetComponent<Unit>().UnitDehighlighted += OnUnitDehighlighted;
        // unit.GetComponent<Unit>().UnitDestroyed += OnUnitDestroyed;
        // unit.GetComponent<Unit>().UnitMoved += OnUnitMoved;
        
        //UnitAdded event is invoked each time AddUnit method is called.
        // if (UnitAdded != null)
        //     UnitAdded.Invoke(this, new UnitCreatedEventArgs(unit));
    }
 
    //TODO: implementation missing, should be called each time after unit is moving to a new cell
    public void OnUnitMoved(object sender, EventArgs e)
    {
        CheckGameFinished();
    }
    
    //ok
    void OnUnitClicked(object sender, EventArgs e)
    {
        CellGridState.OnUnitClicked(sender as Unit);
    }

    
    public bool CheckGameFinished()
    {
        //looks for GameEndCondition as ParentClass for different WinConditions, CheckConditions can be implemented differently depending on condition
        //called every Time after a unit is moved
        List<GameResult> gameResults =
            GetComponents<GameEndCondition>()
                .Select(c => c.CheckCondition(this))
                .ToList();

        foreach (var gameResult in gameResults)
        {
            if (gameResult.IsFinished)
            {
                cellGridState = new CellGridStateGameOver(this);
                GameFinished = true;
                if (GameEnded != null)
                {
                    GameEnded.Invoke(this, new GameEndedArgs(gameResult));
                }
                break;
            }
        }
        return GameFinished;
    }
}
