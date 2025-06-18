using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// 地形の種類を定義する列挙型
/// </summary>
public enum TerrainType
{
    Plain,//平原
    Forest,//森
    Mountain,//山
    Desert,//砂漠
    Water,//水場
    River,//川
    Snow,//積雪
    Flooded,//水害
    Landslide,//土砂崩れ
    Paved//舗装
}


/// <summary>
/// マップ上の各グリッドの情報を保持するクラス
/// </summary>
public partial class Tile
{
    [field:SerializeField]public Vector2Int GridPosition { get; private set; }//グリッドの座標
    [field:SerializeField]public TerrainType Type { get; private set; }//地形の種類
    [field:SerializeField]public Unit OccupyingUnit { get; private set; }//そのグリッドに存在するユニット

    //コンストラクタ
    public Tile(Vector2Int position, TerrainType type)
    {
        GridPosition = position;
        Type = type;
        OccupyingUnit = null;//初期状態ではユニットはいない
    }

    /// <summary>
    /// このタイルにユニットを設定する
    /// </summary>
    /// <param name="unit">設定するユニット</param>
    public void SetOccupyingUnit(Unit unit)
    {
        OccupyingUnit = unit;
    }

    /// <summary>
    /// このタイルの防御ボーナスを取得
    /// </summary>
    /// c<return>防御ボーナス</return>
    public int GetDefenseBonus()
    {
        switch (Type)
        {
            case TerrainType.Forest: return 1;
            case TerrainType.Mountain: return 1;
            case TerrainType.Desert: return 1;
            case TerrainType.Snow: return 1;
            //case TerrainType.Flooded: return -1;
            //case TerrainType.Landslide: return -1;
            default: return 0;
        }
    }

    /// <summary>
    /// このタイルの回避ボーナスを取得
    /// </summary>
    /// c<return>回避ボーナス</return>
    public int GetEvadeBonus()
    {
        switch (Type)
        {
            case TerrainType.Forest: return 10;
            case TerrainType.Mountain: return 10;
            //case TerrainType.Desert: return -10;
            //case TerrainType.Snow: return -10;
            //case TerrainType.Flooded: return -10;
            //case TerrainType.Landslide: return -10;
            default: return 0;
        }
    }

    /// <summary>
    /// タイルの地形タイプを設定する
    /// (Typeプロパティがprivate　setのため、外部から変更するためのメソッド)
    /// </summary>
    /// <param name="newType"></param>
    public void SetType(TerrainType newType)
    {
        Type = newType;
    }
}


/// <summary>
/// ゲームマップの生成、管理、地形効果などを扱うシングルトンクラス
/// </summary>
public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MapManager>();
                if(_instance == null)
                {
                    GameObject singletonObject = new GameObject("MapManager");
                    _instance = singletonObject.AddComponent<MapManager>();
                }
            }
            return _instance;
        }
    }

    [field: SerializeField] private Vector2Int _gridSize;//マップのグリッドサイズ
    public Vector2Int GridSize => _gridSize;
    [SerializeField]private Tile[,] _tileGrid;//各グリッドの情報を格納する2次元配列(Inspector表示不可のため[SerializeField]は無効)

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    /// <summary>
    /// ステージIDに基づいてマップデータをロードし、TileGridを生成する
    /// </summary>
    /// <param name="stageId">ロードするステージID</param>
    public void LoadMap(int stageId)
    {
        Debug.Log($"MapManager:ステージ{stageId}のマップをロードします");
        //DataManagerからのマップデータの取得


        //仮マップを生成
        _gridSize = new Vector2Int(10, 10);
        _tileGrid = new Tile[_gridSize.x, _gridSize.y];

        for (int y = 0; y < _gridSize.y; y++)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                //地形タイプを設定するロジック（マップデータからの読み込み）
                //現在は仮のマップを生成する　2025/06
                TerrainType type = TerrainType.Plain;
                if(x == 5 && y == 5) { type = TerrainType.Forest; }//仮の森
                if(x== 0 || y == 0 || x == _gridSize.x -1 || y == _gridSize.y -1) { type = TerrainType.Water; }//仮の水場　外周
                if(x % 2 == 0 && y % 2 == 0) { type = TerrainType.Desert; }//仮の砂漠
                _tileGrid[y, x] = new Tile(new Vector2Int(x, y), type);
            }
        }
        Debug.Log($"MapManager:マップサイズ{_gridSize.x}×{_gridSize.y}で生成");

        //仮：タイルの視覚的に表示する処理関係
    }

    /// <summary>
    /// 指定されたグリッド座標のタイル情報を取得する
    /// </summary>
    /// <param name="position">グリッド座標</param>
    /// <return>Tileオブジェクト、範囲外ならnull</return>
    public Tile GetTileAt(Vector2Int position)
    {
        if(position.x >= 0 && position.x <= _gridSize.x && position.y >= 0 && position.y < _gridSize.y)
        {
            return _tileGrid[position.x, position.y];
        }
        return null;
    }

    /// <summary>
    /// 指定グリッドとユニットタイプに応じた移動コストを返す
    /// </summary>
    /// <param name="positon">グリッド座標</param>
    /// <param name="unitType">ユニットタイプ</param>
    /// <return>移動コスト</return>
    public int GetMovementCost(Vector2Int position, UnitType unitType)
    {
        Tile tile = GetTileAt(position);
        if(tile == null) return int.MinValue; //範囲外は移動不可

        //地形とユニットタイプに応じた移動コストのロジック
        switch (tile.Type)
        {
            case TerrainType.Plain://平地
                return 1;
            case TerrainType.Forest://森
                    return 2;
            case TerrainType.Mountain://山
                    if(unitType == UnitType.Flying) { return 1; }//飛行ユニットはコスト低
                    if(unitType == UnitType.Mountain) { return 1; }//山賊ユニットはコスト低
                return 3;
            case TerrainType.Water://水場
                if (unitType == UnitType.Flying) { return 1; }//飛行ユニットはコスト低
                if (unitType == UnitType.Aquatic) { return 1; }//水棲ユニットはコスト低
                return 4;
            case TerrainType.Desert://砂漠
                if (unitType == UnitType.Flying) { return 1; }//飛行ユニットはコスト低
                return 2;
            case TerrainType.Snow://積雪
                if (unitType == UnitType.Flying) { return 1; }//飛行ユニットはコスト低
                return 2;
            case TerrainType.River://川
                if (unitType == UnitType.Flying) { return 1; }//飛行ユニットはコスト低
                if (unitType == UnitType.Aquatic) { return 1; }//水棲ユニットはコスト低
                return int.MinValue;//他のユニットは移動不可
            case TerrainType.Flooded://水害
                if (unitType == UnitType.Flying) { return 1; }//飛行ユニットはコスト低
                if (unitType == UnitType.Aquatic) { return 1; }//水棲ユニットはコスト低
                return 4;
            case TerrainType.Landslide://土砂
                if (unitType == UnitType.Flying) { return 1; }//飛行ユニットはコスト低
                return 3;
            case TerrainType.Paved://舗装
                return 1;
                default:
                return 1;
        }
    }

    /// <summary>
    /// 特定のグリッドの地形タイプを変更する(不確定要素)
    /// </summary>
    /// <param name="postion">グリッド座標</param>
    /// <param name="newType">新しい地形タイプ</param>
    public void ChangeTerrain(Vector2Int position, TerrainType newType)
    {
        Tile tile = GetTileAt(position);
        if(tile == null)
        {
            Debug.Log($"MapManager:{position}の地形を{tile.Type}から{newType}変化しました");
            //見た目の変化のため視覚的処理を指示
        }
    }

    /// <summary>
    /// TileクラスにTypeをセットする
    /// </summary>
    /// <param name="positon"></param>
    /// <param name="newType"></param>
    public void SetTileType(Vector2Int positon, TerrainType newType)
    {
        Tile tile = GetTileAt(positon);
        if(tile != null)
        {
            tile.SetType(newType);
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
