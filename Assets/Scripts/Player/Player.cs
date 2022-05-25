using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
  public int PlayerNumber;
  
  public virtual void Initialize(GameManager gameManager){}
  
  /// <summary>
  /// Method is called every turn. Allows player to interact with his units.
  /// </summary>         
  public abstract void Play(GameManager gameManager);
}
