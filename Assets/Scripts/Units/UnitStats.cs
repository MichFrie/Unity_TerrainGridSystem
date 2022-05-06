using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new Unit", menuName = "Create new Unit")]
public class UnitStats : ScriptableObject
{
    public new string name;
    public int strength;
    public int experience;
    public int attackPower;
    public int morale;
    public int cohesion;
    public int fatigue;

    public int movementPoints;

}
