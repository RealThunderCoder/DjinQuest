using UnityEngine;

/// <summary>
/// Scriptable Object containing the data for the structures being placed.
/// </summary>
[CreateAssetMenu(menuName = "BuildingSystem/BuildableData")]
public class BuildableObjectDataSO : ScriptableObject
{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public Vector2Int Size { get; private set; }
    [field: SerializeField]
    public Vector2Int Offset { get; private set; }
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}
