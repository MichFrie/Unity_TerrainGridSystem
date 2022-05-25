using System;
using UnityEngine;
using UnityEngine.UI;
public class GUI_Controller : MonoBehaviour
{
    public Button NextTurnButton;
    // public Image UnitImage;
    // public Text InfoText;
    // public Text StatsText;

    void Start()
    {
        GameManager.Instance.LevelLoading += OnLevelLoading;
        GameManager.Instance.LevelLoadingDone += OnLevelLoadingDone;
        GameManager.Instance.TurnEnded += OnTurnEnded;
        GameManager.Instance.GameStarted += OnGameStarted;
    }
    

    void OnLevelLoading(object sender, EventArgs e)
    {
        Debug.Log("Level Loaded");
    } 
    void OnLevelLoadingDone(object sender, EventArgs e)
     {
         Debug.Log("Level loading done");
     }
    //disables NextTurnButton if next Player is not human
    void OnTurnEnded(object sender, EventArgs e)
    {
        Debug.Log("Turn ended");
        if (NextTurnButton != null)
        {
            NextTurnButton.interactable = GameManager.Instance.CurrentPlayer is HumanPlayer;
        }
    }   
    //disables NextTurnButton if next Player is not human
    void OnGameStarted(object sender, EventArgs e)
    {
         Debug.Log("Game started");
         if (NextTurnButton != null)
         {
             NextTurnButton.interactable = GameManager.Instance.CurrentPlayer is HumanPlayer;
         }
    }
    void OnUnitHighlighted(object sender, EventArgs e)
    {
         
    }
}
