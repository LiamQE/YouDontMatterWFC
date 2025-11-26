using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


[System.Serializable]
public class WFC : MonoBehaviour
{
    [SerializeField]
    int grid_width, grid_height;
    [Header("Tile Set")]
    public TileSetData tileSetData;
    // public Cell cell;

    public void Start()
    {
        tileSetData.BuildCompatibilityMap();
        WaveFunctionCollapse();
    }

    public Cell[,] WaveFunctionCollapse()
    {
        Cell[,] grid_output = new Cell[grid_width, grid_height];    //using array as quicker to access elements

        int collapsedCells = 0;
        int totalCells = grid_height * grid_width;

        //populate all cells
        for (var x = 0; x < grid_width; x++)
        {
            for (var y = 0; y < grid_height; y++)
            {
                grid_output[x, y] = new Cell(tileSetData.tiles, (x,y));
                //Debug.Log("grid output: " + x + " " + y + " " + grid_output[x,y].possibleTiles[0]);
                //Debug.Log("compatibility map: " + tileSetData.compatibilityMap[y].neighbors[0].compatibleNeighbors[0].Name);
            }
        }

        Cell cell;
        while (collapsedCells < totalCells)
        {
            if (collapsedCells == 0) //selecting random first cell
            {
                cell = SelectRandomCell(grid_output);
            } else
            {
                cell = SelectCell(grid_output);    //selecting cell with lowest entropy
            }

            //print("checking whether cell has been collapsed: "+grid_output[cell.Item1, cell.Item2].IsCollapsed + " and how many tiles possible: " +grid_output[cell.Item1, cell.Item2].possibleTiles.Count);

            cell.Collapse();

            Propagation(cell, grid_output);
            
            //print("checking whether cell has been collapsed: "+grid_output[cell.Item1, cell.Item2].IsCollapsed + " and what the tile is: " +grid_output[cell.Item1, cell.Item2].possibleTiles[0]);

            // print("comp map count: " + tileSetData.compatibilityMap[0].neighbors[0].compatibleNeighbors.Count);
            // print("comp map tile, direction and compatible tiles: " + tileSetData.compatibilityMap[1].tile + "  "+ tileSetData.compatibilityMap[1].neighbors[0].direction +  "  "+tileSetData.compatibilityMap[1].neighbors[0].compatibleNeighbors[0] +  "  "+ tileSetData.compatibilityMap[1].neighbors[0].compatibleNeighbors[1] + "  "+ tileSetData.compatibilityMap[1].neighbors[0].compatibleNeighbors[2] + "  "+  tileSetData.compatibilityMap[1].neighbors[0].compatibleNeighbors[3] );

            collapsedCells++;
        }

        // Debug.Log("yo in WFC script");

        return grid_output;
    }
    
    public Cell SelectRandomCell(Cell[,] grid)
    {
        var rnd = new System.Random();
        int xRand = rnd.Next(0, grid_width);
        int yRand = rnd.Next(0, grid_height);

        Cell selectedCell = grid[xRand, yRand];
        
        return selectedCell;
    }

    public Cell SelectCell(Cell[,] grid)
    {
        int entropy = int.MaxValue;  //set entropy to max value
        List<Cell> lowestEntropyCells = new();
        Cell[,] cell = new Cell[0, 0];
        var rnd = new System.Random();
    
        for (var x = 0; x < grid_width; x++)
        {
            for (var y = 0; y < grid_height; y++)
            {
                
                if (grid[x, y].IsCollapsed) continue;
                if (grid[x, y].possibleTiles.Count <= entropy)
                {
                    lowestEntropyCells.Add(grid[x, y]);
                    entropy = grid[x, y].possibleTiles.Count;
                }
            }
        }
        int selectedCellIndex = rnd.Next(0, lowestEntropyCells.Count);
        Cell selectedCell = lowestEntropyCells[selectedCellIndex];
        return selectedCell;
    }

    public void Propagation(Cell cell, Cell[,] grid)
    {
        Queue<Cell> q = new Queue<Cell>();

        q.Enqueue(cell);
        cell.addedToQueue = true;
        var (dx,dy) = (0,0);

        //add adjacent cells to queue
        
        while(q.Count > 0)
        {   
            print("------- NEW CELL -------");
            print("LENGTH OF QUEUE AFTER NEW CELL: "+q.Count);
            Cell queuedCell = q.Dequeue();
            print("LENGTH OF QUEUE AFTER DEQUEUE: "+q.Count);
            print("queued cell touched: "+queuedCell.touchedThisTurn);
            if(queuedCell.touchedThisTurn == true) continue;            //possibly dead code
            foreach(Direction direction in Enum.GetValues(typeof(Direction))){
                print("length of queue: "+q.Count);
                print("cell coords: "+queuedCell.coords.Item1+" "+queuedCell.coords.Item2+" direction: "+direction+" "+DirectionUtil.DirectionVec[direction]);
                
                //print("direction: "+direction+" direction vec: "+DirectionUtil.DirectionVec[direction]);
                (dx,dy) = DirectionUtil.DirectionVec[direction];

                (int nx, int ny) = (queuedCell.coords.Item1 + dx, queuedCell.coords.Item2 + dy);

                if(!(nx < 0 || nx >= grid_width || ny < 0 || ny >= grid_height)){
                    print("cell "+ grid[nx,ny].coords+" already added to queue: "+grid[nx,ny].addedToQueue);

                    if(grid[nx,ny].addedToQueue == false)
                {
                    print("added "+nx+" "+ny+" to queue");
                    q.Enqueue(grid[nx,ny]);                     //only add to queue if its in bounds
                    grid[nx,ny].addedToQueue = true;
                }
                }
                
            };
            queuedCell.touchedThisTurn = true;          //also possibly dead
        }
    }
    public void UpdateAdjacentCells(int x, int y)
    {
        
    }
}


public class Cell
{
    public List<TileData> possibleTiles;
    public bool IsCollapsed => possibleTiles.Count == 1;
    public bool touchedThisTurn = false;
    public bool addedToQueue = false;
    public (int, int) coords;

    public Cell(List<TileData> allTiles, (int,int) givenCoords)    //constructor
    {
        possibleTiles = new List<TileData>(allTiles);
        coords = givenCoords;
    }

    public void Collapse()
    {
        //this is the random collapse NOT using entropy
        int index = UnityEngine.Random.Range(0, possibleTiles.Count);

        TileData chosenTile = possibleTiles[index];
        possibleTiles = new List<TileData> { chosenTile };
    }

    public void EntropyCollapse()
    {

    }

    public void CalculateShannonEntropy(int index)
    {
        double sumWeight = 0;
        foreach(TileData tile in possibleTiles)
        {
            sumWeight += sumWeight + tile.weight;
        }

    }
}