using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapUnitPlacementData", menuName = "Scriptable Objects/MapUnitPlacementData")]
public class MapUnitPlacementData : ScriptableObject
{
    public List<Vector2Int> placementPositions;
}
