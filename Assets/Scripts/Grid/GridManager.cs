using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TGS;

public class GridManager : MonoBehaviour
{
    TerrainGridSystem tgs;
    List<Cell> allCells;

    //Cell Tags
    int cellEmpty = 1;
    int cellOccupied = 2;
    
    //Cell Groups
    public const int cellEmpty_Group = 1;
    public const int cellOccupied_Group = 2;
    
    //Movement Costs
    int grassCost = 2;
    int roadCost = 1;
    int rockCost = 4;
    
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
        InitialCellBehaviour();
        CreateOccupiedCellsList();
    }
    
    void InitialCellBehaviour()
    {
        allCells = tgs.cells;
        foreach (Cell cell in allCells)
        {
            int cellIndex = tgs.CellGetIndex(cell);
            if (tgs.CellGetTexture(cellIndex) == tgs.textures[1])
            {
                tgs.CellSetCrossCost(cellIndex, grassCost);
                tgs.CellToggleRegionSurface(cellIndex, false, tgs.textures[1]);
            }
            else if (tgs.CellGetTexture(cellIndex) == tgs.textures[2])
            {
                tgs.CellSetCrossCost(cellIndex, roadCost);
                tgs.CellToggleRegionSurface(cellIndex, false, tgs.textures[2]);
            }
            else if (tgs.CellGetTexture(cellIndex) == tgs.textures[3])
            {
                tgs.CellSetCrossCost(cellIndex, rockCost);
                tgs.CellToggleRegionSurface(cellIndex, false, tgs.textures[3]);
            }
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
            tgs.CellSetGroup(startCell, cellEmpty_Group);
        }
        else
        {
            tgs.CellSetGroup(startCell, cellEmpty_Group);
        }
        if (!occupiedCells.ContainsKey(targetCell))
        {
            occupiedCells.Add(targetCell, targetCellStatus);
            tgs.CellSetGroup(targetCell, cellOccupied_Group);
        }
        else
        {
            tgs.CellSetGroup(targetCell, cellOccupied_Group);
        }
    }

    //DebugFunction to show Cell groups
    void ShowCellGroup()
    {
        List<Cell> cells = tgs.cells;
        foreach (Cell c in cells)
        {
            int groupIndex = tgs.CellGetGroup(tgs.CellGetIndex(c));
            if (groupIndex == cellEmpty_Group)
            {
                tgs.CellFlash(tgs.CellGetIndex(c), Color.green, 2f);
            }
            else if (groupIndex == cellOccupied_Group)
            {
                tgs.CellFlash(tgs.CellGetIndex(c), Color.red, 2f);
            }
        }
    }

    public int GetDistance(int sourceCell, int targetCell)
    {
        return tgs.CellGetHexagonDistance(sourceCell, targetCell);
    }

    public int GetUnitPosition()
    {   
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        int cellIndex = tgs.CellGetIndex(cell);
        return cellIndex;
    }

    public int GetLastClickedPosition()
    {
        return tgs.cellLastClickedIndex;
    }
}