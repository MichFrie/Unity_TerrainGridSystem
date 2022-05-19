using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager Instance;

    //EventHandler
    public event EventHandler LevelLoading;
    public event EventHandler LevelLoadingDone;
    public event EventHandler GameStarted;
    public event EventHandler GameEnded;
    public event EventHandler TurnEnded;

    //CellGridState
    CellGridState cellGridState;

    //Property for CellGridState
    public CellGridState CellGridState
    {
        get { return cellGridState; }
        set
        {
            CellGridState nextState;
            if (cellGridState != null)
            {
                cellGridState.OnStateExit();
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
    
        // if (LevelLoading != null)
        // {
        //     LevelLoading.Invoke(this, new EventArgs());
        // }
        //PlayableUnits = UnitManager.Instance.playableUnits;
        Initialize();

        // if (LevelLoadingDone != null)
        // {
        //     LevelLoadingDone.Invoke(this, new EventArgs());
        // }
        StartGame();
    }

    void Initialize()
    {
        GameFinished = false;

        Players = new List<Player>();

        //Initialize Players
        for (int i = 0; i < PlayerParent.childCount; i++)
        {
            var player = PlayerParent.GetChild(i).GetComponent<Player>();
            if(player != null && player.gameObject.activeInHierarchy)
            {
                player.Initialize(this);
                Players.Add(player);
            }
        }
        
        //GridManager will iterate over cells
        
        //Event Methods
        
        //UnitManager will iterate over units
        Units = new List<Unit>();
        UnitManager.Instance.FindPlayableUnits();

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

        TransitionResult transitionResult = GetComponent<TurnResolver>().ResolveStart(this);

        //PlayableUnits = transitionResult.PlayableUnits;
        
        CurrentPlayerNumber = transitionResult.NextPlayer.PlayerNumber;
        
        //PlayableUnits.ForEach(u => { u.GetComponents<Ability>().ToList().ForEach(a => a.OnTurnStart(this)); u.OnTurnStart(); });
        
        CurrentPlayer.Play(this);
        Debug.Log("GameStarted");
    
    }

    public void EndTurn()
    {
        //blocks PlayerInput while CPU is playing?
        cellGridState = new CellGridStateBlockInput(this);

        bool isGameFinished = IsGameFinished();
        if (isGameFinished)
            return;
        
        //PlayableUnits.ForEach(u => { if (u != null) { u.OnTurnEnd(); u.GetComponents<Ability>().ToList().ForEach(a => a.OnTurnEnd(this)); } });

        TransitionResult transitionResult = GetComponent<TurnResolver>().ResolveTurn(this);
        PlayableUnits = transitionResult.PlayableUnits;
        CurrentPlayerNumber = transitionResult.NextPlayer.PlayerNumber;

        if (TurnEnded != null)
        {
            TurnEnded.Invoke(this, new EventArgs());
        }
        Debug.Log(string.Format("Player {0} turn", CurrentPlayerNumber));
        //PlayableUnits.ForEach(u => { u.GetComponents<Ability>().ToList().ForEach(a => a.OnTurnStart(this)); u.OnTurnStart(); });
        CurrentPlayer.Play(this);
    }
    public List<Unit> GetCurrentPlayerUnits()
    {
        return PlayableUnits;
    }
    // public List<Unit> GetEnemyUnits(Player player)
    // {
    //     return Units.FindAll(u => u.PlayerNumber != player.PlayerNumber);
    // }
    // public List<Unit> GetPlayerUnits(Player player)
    // {
    //     return Units.FindAll(u => u.PlayerNumber == player.PlayerNumber);
    // }

    
    //Event handler Methods

    void OnUnitClicked(object sender, EventArgs e)
    {
        cellGridState.OnUnitClicked(sender as Unit);
    }
    public void AddUnit(Transform unit)
    {
        unit.GetComponent<Unit>().Unitclicked += OnUnitClicked;
    }
    
    public bool IsGameFinished()
    {
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
