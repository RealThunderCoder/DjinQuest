using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks the positions of all the objects on the grid.
/// </summary>
public class GridData
{
    private Dictionary<Vector3, PlacementData> _placedObjects = new();

    /// <summary>
    /// Adds an object to the grid's data.
    /// </summary>
    /// <param name="gridPosition"> Position of the object being placed. </param>
    /// <param name="objectData"> The object's data. </param>
    /// <exception cref="System.Exception"></exception>
    public void AddObject(Vector3 gridPosition, BuildableObjectDataSO objectData)
    {
        if (!CanPlaceAt(gridPosition, objectData.Size))
        {
            Debug.Log($"Attempted to place {objectData.Name} at invalid position {gridPosition}");
            return;
        }

        List<Vector3> positionsToOccupy = CalculateOccupiedPositions(gridPosition, objectData.Size);
        PlacementData data = new PlacementData(positionsToOccupy, objectData);

        // Add positions to dictionary
        foreach (Vector3 pos in positionsToOccupy)
        {
            if (_placedObjects.ContainsKey(pos))
                throw new System.Exception($"Grid data already contains {pos} (Will is bad at matrix math)");
            _placedObjects[pos] = data;
        }
    }

    /// <summary>
    /// Removes an object from the grid's data.
    /// </summary>
    /// <param name="gridPosition"> Position of the object being removed. </param>
    /// <param name="objectData"> The object's data. </param>
    /// <exception cref="System.Exception"></exception>
    public void RemoveObject(Vector3 gridPosition, BuildableObjectDataSO objectData)
    {
        List<Vector3> occupiedPositions = CalculateOccupiedPositions(gridPosition, objectData.Size);
        foreach (Vector3 pos in occupiedPositions)
        {
            _placedObjects.Remove(pos);
        }
    }

    /// <summary>
    /// Determines all the grid positions an object will occupy based on it's size.
    /// </summary>
    /// <param name="gridPosition"> The object's position. </param>
    /// <param name="size"> The object's size. </param>
    /// <returns> A list of Vector3 positions the object will occupy. </returns>
    private List<Vector3> CalculateOccupiedPositions(Vector3 gridPosition, Vector2Int size)
    {
        List<Vector3> occupiedPos = new List<Vector3>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                occupiedPos.Add(gridPosition + new Vector3(x, 0, y));
            }
        }
        return occupiedPos;
    }

    /// <summary>
    /// Determines if an object can be placed at the specified position.
    /// </summary>
    /// <param name="gridPosition"> Position to check. </param>
    /// <param name="size"> The object's size. </param>
    /// <returns> True if it can be placed, false otherwise. </returns>
    public bool CanPlaceAt(Vector3 gridPosition, Vector2Int size)
    {
        foreach (Vector3 pos in CalculateOccupiedPositions(gridPosition, size))
        {
            if (_placedObjects.ContainsKey(pos)) return false;
        }
        return true;
    }
}

/// <summary>
/// Class containing the object's data and occupied positions on a grid.
/// </summary>
public class PlacementData
{
    public List<Vector3> occupiedPositions = new();
    public BuildableObjectDataSO objectData;

    public PlacementData(List<Vector3> occupiedPositions, BuildableObjectDataSO objectData)
    {
        this.occupiedPositions = occupiedPositions;
        this.objectData = objectData;
    }
}