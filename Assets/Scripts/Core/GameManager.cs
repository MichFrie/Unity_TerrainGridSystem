using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //EventHandler
    public event EventHandler GameStarted;
    public event EventHandler GameEnded;

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
        //Initialize();
        UnitManager.Instance.FindPlayableUnits();
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
            
            List<Unit> PlayableUnits = UnitManager.Instance.playableUnits; 
            PlayableUnits = transitionResult.PlayableUnits;
            
            //PlayableUnits.ForEach...
            
            CurrentPlayerNumber = transitionResult.NextPlayer.PlayerNumber;
            
            CurrentPlayer.Play(this);
            Debug.Log("GameStarted");
        } 
    }

    public void EndTurn()
    {
        
    }
}
