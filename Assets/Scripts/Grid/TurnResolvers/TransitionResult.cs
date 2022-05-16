using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionResult
{
    public Player NextPlayer { get; private set; }
    public List<Unit> PlayableUnits { get; private set; }

    public TransitionResult(Player nextPlayer, List<Unit> allowedUnits)
    {
        NextPlayer = nextPlayer;
        PlayableUnits = allowedUnits;
    }
}
