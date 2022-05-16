using UnityEngine;

    public abstract class TurnResolver : MonoBehaviour
    {
        public abstract TransitionResult ResolveStart(GameManager gameManager);
        public abstract TransitionResult ResolveTurn(GameManager gameManager);
    }
        
