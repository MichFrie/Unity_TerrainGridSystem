using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //EventHandler
    public event EventHandler GameStarted;
    public event EventHandler GameEnded;
    public event EventHandler TurnEnded;

    //CellGridState
    public CellGridState CellGridState;
    
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
        PlayableUnits = UnitManager.Instance.playableUnits;
        //Initialize();
        //FindPlayableUnits in Unitmanager deaktiviert();
        //StartGame();
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
        
        //UnitManager
    }

    public void StartGame()
    {
        if (GameStarted != null)
        {
            GameStarted.Invoke(this, new EventArgs());

            TransitionResult transitionResult = GetComponent<TurnResolver>().ResolveStart(this);

            PlayableUnits = transitionResult.PlayableUnits;
            
            CurrentPlayerNumber = transitionResult.NextPlayer.PlayerNumber;
            
            //PlayableUnits.ForEach(u => { u.GetComponents<Ability>().ToList().ForEach(a => a.OnTurnStart(this)); u.OnTurnStart(); });
            
            CurrentPlayer.Play(this);
            Debug.Log("GameStarted");
        } 
    }

    public void EndTurn()
    {
        //blocks PlayerInput while CPU is playing?
        CellGridState = new CellGridStateBlockInput(this);

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

    public bool IsGameFinished()
    {
        
    }
}
