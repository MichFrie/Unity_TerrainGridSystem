    using UnityEngine;

    public abstract class GameEndCondition : MonoBehaviour
    {
        public abstract GameResult CheckCondition(GameManager gameManager);
    }
