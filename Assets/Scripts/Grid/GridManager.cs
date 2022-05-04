using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;

public class GridManager : MonoBehaviour
{
    TerrainGridSystem tgs;
    GameObject testUnit;

    public List<Cell> cellList = new List<Cell>();
    void Start()
    {
        tgs = TerrainGridSystem.instance;
        testUnit = GameObject.Find("TestUnit");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SnapToCellCenter();
            GetAllCells();
        }
    }

    //Snaps unit to center of gridcell
    void SnapToCellCenter() {
        Vector3 pos = tgs.SnapToCell(testUnit.transform.position);
        // Shift pos a bit upwards
        pos -= tgs.transform.forward;
        testUnit.transform.position = pos;
    }

    void GetAllCells()
    {
        print("Test");
        cellList = tgs.cells;
    }
}