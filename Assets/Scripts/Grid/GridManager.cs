using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TGS;

public class GridManager : MonoBehaviour
{
    TerrainGridSystem tgs;
    
    void Start()
    {
        tgs = TerrainGridSystem.instance;
    }

    void Update()
    {
    }
    
    
    //Snaps unit to center of gridcell
    // void SnapToCellCenter() {
    //     Vector3 pos = tgs.SnapToCell(testUnit.transform.position);
    //     // Shift pos a bit upwards
    //     pos -= tgs.transform.forward;
    //     testUnit.transform.position = pos;
    //}
}