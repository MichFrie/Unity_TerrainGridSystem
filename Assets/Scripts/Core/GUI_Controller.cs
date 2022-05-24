using System;
using UnityEngine.UI;
public class GUI_Controller
{
    public GameManager GameManager;
    public Button NextTurnButton;
    
    public Image UnitImage;
    public Text InfoText;
    public Text StatsText;

    void Awake()
    {
        GameManager.GameStarted += OnGameStarted;
    }

    void OnGameStarted(object sender, EventArgs e)
    {
        
    }

    void OnUnitHighlighted(object sender, EventArgs e)
    {
        
    }
}
