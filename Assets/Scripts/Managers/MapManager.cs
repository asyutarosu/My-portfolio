using UnityEngine;
using System.Collections.Generic;


/// < summary >
/// マップ上の各グリッドの情報を保持するクラス
/// </ summary >
//public partial class Tile
//{
//    [field: SerializeField] public Vector2Int GridPosition { get; private set; }//グリッドの座標
//    [field: SerializeField] public TerrainType Type { get; private set; }//地形の種類
//    [field: SerializeField] public Unit OccupyingUnit { get; private set; }//そのグリッドに存在するユニット

//    //コンストラクタ
//    public Tile(Vector2Int position, TerrainType type)
//    {
//        GridPosition = position;
//        Type = type;
//        OccupyingUnit = null;//初期状態ではユニットはいない
//    }

//    /// <summary>
//    /// このタイルにユニットを設定する
//    /// </summary>
//    /// <param name="unit">設定するユニット</param>
//    public void SetOccupyingUnit(Unit unit)
//    {
//        OccupyingUnit = unit;
//    }

//    /// <summary>
//    /// このタイルの防御ボーナスを取得
//    /// </summary>
//    /// c<return>防御ボーナス</return>
//    public int GetDefenseBonus()
//    {
//        switch (Type)
//        {
//            case TerrainType.Forest: return 1;
//            case TerrainType.Mountain: return 1;
//            case TerrainType.Desert: return 1;
//            case TerrainType.Snow: return 1;
//            //case TerrainType.Flooded: return -1;
//            //case TerrainType.Landslide: return -1;
//            default: return 0;
//        }
//    }

//    /// <summary>
//    /// このタイルの回避ボーナスを取得
//    /// </summary>
//    /// c<return>回避ボーナス</return>
//    public int GetEvadeBonus()
//    {
//        switch (Type)
//        {
//            case TerrainType.Forest: return 10;
//            case TerrainType.Mountain: return 10;
//            //case TerrainType.Desert: return -10;
//            //case TerrainType.Snow: return -10;
//            //case TerrainType.Flooded: return -10;
//            //case TerrainType.Landslide: return -10;
//            default: return 0;
//        }
//    }

//    /// <summary>
//    /// タイルの地形タイプを設定する
//    /// (Typeプロパティがprivate　setのため、外部から変更するためのメソッド)
//    /// </summary>
//    /// <param name="newType"></param>
//    public void SetType(TerrainType newType)
//    {
//        Type = newType;
//    }
//}


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

    //マップ作成用
    [SerializeField] private string[] _mapSequence;//Resoucesフォルダ以下のCSVファイルのパス
    [SerializeField] private GameObject _tilePrefab;//各タイル生成に使用するTileコンポーネントが付いたPrefab
    [SerializeField] private float _tileSize = 1.0f;//グリッドの1マスあたりのワールド座標でのサイズ
    [SerializeField] private Sprite[] _terrainSprite;//地形タイプに対応するスプライトを設定するための配列

    private int _currentMapIndex = 0;//現在ロードしているマップのインデックス
    private MapData _currentmapData;//MapDtaLoaderによって読み込まれるマップデータ

    private Dictionary<Vector2Int ,Tile> _tiles = new Dictionary<Vector2Int, Tile>();//生成された全てのTileオブジェクトとグリッド座標を管理する

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
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

        //_mapCsvFilesPath = new string "";

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
    /// マップデータに基づいてマップを生成する
    /// </summary>
    public void GenerateMap(string mapPath)
    {
        ClearMap();//既にマップが生成されている可能性を考慮して、一度クリアする

        //MapDataLoaderでCSVファイルからマップデータを読み込む
        _currentmapData = MapDataLoader.LoadMapDataFromCSV(mapPath);

        if(_currentmapData == null)
        {
            Debug.LogError("MapManager:マップデータの読み込みに失敗しました。マップ生成できません");
            return;
        }

        for(int y = 0; y < _currentmapData.Height; y++)
        {
            for(int x = 0; x < _currentmapData.Width; x++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);//現在のグリッド座標
                TerrainType terrainType = _currentmapData.GetTerrainType(gridPos);//その座標の地形タイプを取得

                Vector3 worldPos = GetWorldPosition(gridPos);//グリッド座標をワールド座標に変換

                GameObject tileGO = Instantiate(_tilePrefab,worldPos,Quaternion.identity, transform);

                //生成したGameObjectからTileコンポーネントを取得する
                Tile tile = tileGO.GetComponent<Tile>();
                if(tile == null)
                {
                    Debug.LogError($"TilePrefabにTileコンポーネントがアタッチされいません{_tilePrefab.name}");
                    Destroy(tileGO);
                    continue;
                }

                //取得したTileコンポーネントを初期化
                tile.Initialize(gridPos, terrainType);

                //地形タイプに応じたスプライトをTileに設定
                SetTileSprite(tile,terrainType);

                //生成・初期化が完了したTileオブジェクトを後で検索できるように
                _tiles.Add(gridPos,tile);
            }
        }
        Debug.Log($"MapManager:マップを生成しました({_currentmapData.Width}x{_currentmapData.Height})");
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
        if (tile == null)
        {
            return int.MinValue; //範囲外は移動不可
        }

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

    /// <summary>
    /// 指定されたタイルに、その地形タイプに応じたスプライトを設定する
    /// </summary>
    /// <param name="tile">スプライトを設定したいTileオブジェクト</param>
    /// <param name="type">そのタイルの地形タイプ</param>
    private void SetTileSprite(Tile tile, TerrainType type)
    {
        //地形のEnumの値をキャストして、_terrainSprites配列のインデックスとして使用
        int typeIndex = (int)type;

        //配列の範囲チェックとスプライトがIns@ectorで設定されているかの確認
        if(typeIndex >= 0 && typeIndex < _terrainSprite.Length && _terrainSprite[typeIndex] != null)
        {
            //TileクラスのSpriteメソッドを呼び出して、スプライトを設定
            tile.SetSprite(_terrainSprite[typeIndex]);
        }
        else
        {
            //スプライトが設定されていない場合やインデックスが不正な場合は警告を表示
            Debug.LogWarning($"MapManager:TerrainType'{type}'Terrain Sprite");
        }
    }

    /// <summary>
    /// グリッド座標をワールド座標に変換する
    /// </summary>
    /// <param name="gridPos">変換したいグリッド座標</param>
    /// <return>対応するワールド座標</return>
    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * _tileSize, gridPos.y * _tileSize, 0);
    }

    ///<summary>
    /// ワールド座標をグリッド座標に変換する
    /// </summary>
    /// <param name="worldPos">変換したいワールド座標</param>
    /// <return>対応するグリッド座標</return>
    public Vector2Int GetGridPositio(Vector3 worldPos)
    {
        //ワールド座標をタイルサイズで割り、四捨五入して整数グリッド座標にする
        int x = Mathf.RoundToInt(worldPos.x / _tileSize);
        int y = Mathf.RoundToInt(worldPos.y / _tileSize);
        return new Vector2Int(x, y);
    }

    ///<summary>
    /// 既存のマップを全てクリアする
    /// </summary>
    private void ClearMap()
    {
        foreach(var tileEntry in _tiles)
        {
            Destroy(tileEntry.Value.gameObject);//TileオブジェクトがアタッチされているGameObjectを破棄
        }
        _tiles.Clear();         //Dictionaryの中身をクリア
        _currentmapData = null; //読み込んだマップデータをクリア
        Debug.Log("MapManager:既存のマップをクリアしました");
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_mapSequence.Length > 0)
        {
            //GenerateMap(_mapSequence[_currentMapIndex]);
            GenerateMap(_mapSequence[0]);
        }
        else
        {
            Debug.LogError("MapManager:マップシーケンスが設定されていません");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
