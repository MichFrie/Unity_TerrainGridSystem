using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TGS;

public class GridManager : MonoBehaviour
{
    TerrainGridSystem tgs;
    
    //Cell Tags
    int cellEmpty = 1;
    int cellOccupied = 2;

    public Dictionary<int, int> occupiedCells = new Dictionary<int, int>();

    public static GridManager Instance;

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

    void Start()
    {
        tgs = TerrainGridSystem.instance;
        CreateOccupiedCellsList();
    }

    void Update()
    {
       
    }

    void CreateOccupiedCellsList()
    {
        GameObject[] gameobjects;
        gameobjects = GameObject.FindGameObjectsWithTag("TestUnit");
        foreach (GameObject g in gameobjects)
        {
            Cell cell = tgs.CellGetAtPosition(g.transform.position, true);
            int cellIndex = tgs.CellGetIndex(cell);
            tgs.CellSetTag(cell, cellOccupied);
            if (!occupiedCells.ContainsKey(cellIndex))
            {
                occupiedCells.Add(cellIndex, cellOccupied);
            }
            foreach (var v in occupiedCells)
            {
                Cell occCell = tgs.CellGetWithTag(cellOccupied);
                tgs.CellFlash(occCell, Color.green, 4);
            }
        }
    }
    
    public void UpdateOccupiedCellsList(int startCell, int startCellStatus, int targetCell, int targetCellStatus)
    {
        if (!occupiedCells.ContainsKey(startCell))
        {
            occupiedCells.Add(startCell, startCellStatus);
        }

        if (!occupiedCells.ContainsKey(targetCell))
        {
            occupiedCells.Add(targetCell, targetCellStatus);
        }

        foreach (var v in occupiedCells)
        {
            Cell occCell = tgs.CellGetWithTag(startCellStatus);
            tgs.CellFlash(occCell, Color.green, 4);
        }

    }
    
    //Snaps unit to center of gridcell
    // void SnapToCellCenter() {
    //     Vector3 pos = tgs.SnapToCell(testUnit.transform.position);
    //     // Shift pos a bit upwards
    //     pos -= tgs.transform.forward;
    //     testUnit.transform.position = pos;
    //}
}