using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
  public int PlayerNumber;
  
  public virtual void Initialize(GameManager gameManager){}

  public abstract void Play(GameManager gameManager);
}
