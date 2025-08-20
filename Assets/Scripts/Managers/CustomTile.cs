using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Custom Tile", menuName = "2D/Tiles/Custom Tile")]

public class CustomTile : TileBase
{
    public Sprite sprite;

    public TerrainType terrainType;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = this.sprite;
    }
}