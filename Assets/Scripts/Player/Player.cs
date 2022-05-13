using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
  public int PlayerNumber;
  
  public virtual void Initialize(GridManager gridManager){}

  public abstract void Play(GridManager gridManager);
}
