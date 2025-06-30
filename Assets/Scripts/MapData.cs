using UnityEngine;

public class MapData
{
    public int Width { get; private set; }//マップの幅（X軸方向)
    public int Height { get; private set; }//マップの高さ(Y軸方向)
    private TerrainType[,] _terrainTypes;//各座標の地形タイプを保持する2次元配列

    //コンストラクタ
    public MapData(int width, int height)
    {
        Width = width;
        Height = height;
        _terrainTypes = new TerrainType[Width, Height];
    }

    //指定された座標の地形タイプを設定
    public void SetTerrainType(int x, int y, TerrainType type)
    {
        if(x >= 0 && x < Width && y >= 0 && y < Height)
        {
            _terrainTypes[x, y] = type;
        }
        else
        {
            Debug.LogWarning($"MapData.SetTerrainType:範囲外の座標（{x},{y}）に地形タイプを設定しようとしまいました");
        }
    }

    //指定された座標の地形タイプを取得
    public TerrainType GetTerrainType(Vector2Int position)
    {
        if(position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height)
        {
            return _terrainTypes[position.x, position.y];
        }
        //範囲外の場合はエラーログを通知
        Debug.LogWarning($"MapData.GetTerrainType:({position.x},{position.y})の地形タイプを取得しようとしまいました");
        return TerrainType.None;
    }
}
