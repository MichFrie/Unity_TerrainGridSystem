    using UnityEngine;

    //GameEndCondition is Parentclass for different winConditions
    public abstract class GameEndCondition : MonoBehaviour
    {
        public abstract GameResult CheckCondition(GameManager gameManager);
    }
