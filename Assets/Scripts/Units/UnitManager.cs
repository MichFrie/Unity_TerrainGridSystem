using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
   public static UnitManager Instance { get; private set; }

   public List<Unit> playableUnits = new List<Unit>();
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

   public void FindPlayableUnits()
   {
      playableUnits = FindObjectsOfType<Unit>().ToList();
   }
}
