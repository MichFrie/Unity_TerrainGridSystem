using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
   public static UnitManager Instance { get; private set; }

   public List<Unit> Units = new List<Unit>();

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

   //Debug Method
   public void FindPlayableUnits()
   {
      Units = FindObjectsOfType<Unit>().ToList();
   }
}
