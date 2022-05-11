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
    
    //Cell Groups
    public const int cellGroupEmpty = 1;
    public const int cellGroupOccupied = 2;
    
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
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ShowCellGroup();
        }   
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
            }
        }
    }
    
    public void UpdateOccupiedCellsList(int startCell, int startCellStatus, int targetCell, int targetCellStatus)
    {
        if (!occupiedCells.ContainsKey(startCell))
        {
            occupiedCells.Add(startCell, startCellStatus);
            tgs.CellSetGroup(startCell, cellGroupEmpty);
        }
        else
        {
            tgs.CellSetGroup(startCell, cellGroupEmpty);
        }
        if (!occupiedCells.ContainsKey(targetCell))
        {
            occupiedCells.Add(targetCell, targetCellStatus);
            tgs.CellSetGroup(targetCell, cellGroupOccupied);
        }
        else
        {
            tgs.CellSetGroup(targetCell, cellGroupOccupied);
        }
    }

    void ShowCellGroup()
    {
        List<Cell> cells = tgs.cells;
        foreach (Cell c in cells)
        {
            int groupIndex = tgs.CellGetGroup(tgs.CellGetIndex(c));
            if (groupIndex == cellGroupEmpty)
            {
                tgs.CellFlash(tgs.CellGetIndex(c), Color.green, 2f);
            }
            else if (groupIndex == cellGroupOccupied)
            {
                tgs.CellFlash(tgs.CellGetIndex(c), Color.red, 2f);
            }
        }
    }
}