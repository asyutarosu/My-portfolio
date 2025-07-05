using UnityEngine;
using System.Collections.Generic;


/// Tiles.csへ移行（済み）2025/06
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

    //オフセットフィールド
    [SerializeField] private Vector3 _offset = Vector3.zero;

    [field: SerializeField] private Vector2Int _gridSize;//マップのグリッドサイズ
    public Vector2Int GridSize => _gridSize;
    [SerializeField]private Tile[,] _tileGrid;//各グリッドの情報を格納する2次元配列(Inspector表示不可のため[SerializeField]は無効)

    //マップ作成用
    [SerializeField] private string[] _mapSequence;//Resoucesフォルダ以下のCSVファイルのパス
    [SerializeField] private GameObject _tilePrefab;//各タイル生成に使用するTileコンポーネントが付いたPrefab
    [SerializeField] private float _tileSize = 1.0f;//グリッドの1マスあたりのワールド座標でのサイズ
    [SerializeField] private Sprite[] _terrainSprite;//地形タイプに対応するスプライトを設定するための配列

    [SerializeField] private TerrainCost[] _terrainCosts;

    //移動関連
    [SerializeField] private GameObject _movableHighlightPrefab;
    private Dictionary<Vector2Int,GameObject> _currentHighlights = new Dictionary<Vector2Int, GameObject>();
    [SerializeField] private LineRenderer _pathLinePrefab;//経路表示用のプレハブ
    private LineRenderer _currentPathLine;//現在表示されている経路ライン
    


    private int _currentMapIndex = 0;//現在ロードしているマップのインデックス
    private MapData _currentMapData;//MapDtaLoaderによって読み込まれるマップデータ

    private Dictionary<Vector2Int ,Tile> _tiles = new Dictionary<Vector2Int, Tile>();//生成された全てのTileオブジェクトとグリッド座標を管理する

    //PlayerUnit関連
    [SerializeField] private GameObject _playerUnitPrefab;
    private Unit _currentPlayerUnit;//現在のプレイヤーユニットの参照
    private Unit _selectedUnit;//選択中のプレイヤーユニット

    [System.Serializable]public class TerrainCost
    {
        public TerrainType terrainType;
        public int cost;
    }

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

    //GenerateMapとして処理を統合2025/06
    /// <summary>
    /// ステージIDに基づいてマップデータをロードし、TileGridを生成する
    /// </summary>
    /// <param name="stageId">ロードするステージID</param>
    //public void LoadMap(int stageId)
    //{

    //    Debug.Log($"MapManager:ステージ{stageId}のマップをロードします");

    //    //_mapCsvFilesPath = new string "";

    //    //仮マップを生成
    //    _gridSize = new Vector2Int(10, 10);
    //    _tileGrid = new Tile[_gridSize.x, _gridSize.y];

    //    for (int y = 0; y < _gridSize.y; y++)
    //    {
    //        for (int x = 0; x < _gridSize.x; x++)
    //        {
    //            //地形タイプを設定するロジック（マップデータからの読み込み）
    //            //現在は仮のマップを生成する　2025/06
    //            TerrainType type = TerrainType.Plain;
    //            if(x == 5 && y == 5) { type = TerrainType.Forest; }//仮の森
    //            if(x== 0 || y == 0 || x == _gridSize.x -1 || y == _gridSize.y -1) { type = TerrainType.Water; }//仮の水場　外周
    //            if(x % 2 == 0 && y % 2 == 0) { type = TerrainType.Desert; }//仮の砂漠
    //            _tileGrid[y, x] = new Tile(new Vector2Int(x, y), type);
    //        }
    //    }
    //    Debug.Log($"MapManager:マップサイズ{_gridSize.x}×{_gridSize.y}で生成");

    //    //仮：タイルの視覚的に表示する処理関係
    //}

    /// <summary>
    /// マップデータに基づいてマップを生成する
    /// </summary>
    public void GenerateMap(string mapPath)
    {
        ClearMap();//既にマップが生成されている可能性を考慮して、一度クリアする

        //MapDataLoaderでCSVファイルからマップデータを読み込む
        _currentMapData = MapDataLoader.LoadMapDataFromCSV(mapPath);

        if(_currentMapData == null)
        {
            Debug.LogError("MapManager:マップデータの読み込みに失敗しました。マップ生成できません");
            return;
        }

        for(int y = 0; y < _currentMapData.Height; y++)
        {
            for(int x = 0; x < _currentMapData.Width; x++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);//現在のグリッド座標
                TerrainType terrainType = _currentMapData.GetTerrainType(gridPos);//その座標の地形タイプを取得
                

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
                tile.Initialize(gridPos, terrainType,false);

                //地形タイプに応じたスプライトをTileに設定
                SetTileSprite(tile,terrainType);

                //生成・初期化が完了したTileオブジェクトを後で検索できるように
                _tiles.Add(gridPos,tile);
            }
        }
        Debug.Log($"MapManager:マップを生成しました({_currentMapData.Width}x{_currentMapData.Height})");

        //PlayerUnitの初期配置
        //PlacePlayerUnitAtInitialPostiton();
    }


    /// <summary>
    /// 指定されたグリッド座標のタイル情報を取得する
    /// </summary>
    /// <param name="position">グリッド座標</param>
    /// <return>Tileオブジェクト、範囲外ならnull</return>
    public Tile GetTileAt(Vector2Int position)
    {
        if(_tiles.TryGetValue(position, out Tile tile))
        {
            return tile;
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
        switch (tile.TerrainType)
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
            Debug.Log($"MapManager:{position}の地形を{tile.TerrainType}から{newType}変化しました");
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
        float x = gridPos.x * _tileSize + _offset.x + (_tileSize / 2.0f);
        float y = gridPos.y * _tileSize + _offset.y + (_tileSize / 2.0f);
        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// ワールド座標をグリッド座標に変換
    /// </summary>
    /// <param name="worldPos">変換したいワールド座標</param>
    /// <returns>対応するグリッド座標</returns>
    public Vector2Int GetGridPositionFromWorld(Vector3 worldPos)
    {
        //オフセットとタイルサイズを考慮してグリッド座標を計算
        //int gridX = Mathf.FloorToInt((worldPos.x - _offset.x) / _tileSize);
        //int gridY = Mathf.FloorToInt((worldPos.y - _offset.y) / _tileSize);

        float x = (worldPos.x - _offset.x) / _tileSize;
        float y = (worldPos.y - _offset.y) / _tileSize;

        //return new Vector2Int(gridX, gridY);

        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }




    //オフセットによる座標管理に変更のため削除
    ///<summary>
    /// ワールド座標をグリッド座標に変換する
    /// </summary>
    /// <param name="worldPos">変換したいワールド座標</param>
    /// <return>対応するグリッド座標</return>
    //public Vector2Int GetGridPosition(Vector3 worldPos)
    //{
    //    //ワールド座標をタイルサイズで割り、四捨五入して整数グリッド座標にする
    //    int x = Mathf.RoundToInt(worldPos.x / _tileSize);
    //    int y = Mathf.RoundToInt(worldPos.y / _tileSize);
    //    return new Vector2Int(x, y);
    //}


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
        _currentMapData = null; //読み込んだマップデータをクリア
        Debug.Log("MapManager:既存のマップをクリアしました");
    }

    /// <summary>
    /// プレイヤーユニットを初期位置に配置する
    /// </summary>
    private void PlacePlayerUnitAtInitialPostiton()
    {
        //プロトタイプ用の仮初期座標
        Vector2Int initialPlayerGridPos = new Vector2Int(0, 0);


        //指定された座標がマップ範囲内か確認
        if(initialPlayerGridPos.x < 0 || initialPlayerGridPos.x >= _currentMapData.Width ||
            initialPlayerGridPos.y < 0 || initialPlayerGridPos.y >= _currentMapData.Height)
        {
            Debug.LogError($"MapManager:プレイヤーユニットの初期配置座標({initialPlayerGridPos})がマップ範囲外です");
        }

        //プレイヤーユニットが既に存在する場合は破棄(シーン遷移などで再生成する場合)
        if(_currentPlayerUnit != null)
        {
            Destroy(_currentPlayerUnit.gameObject);
        }

        //プレイヤーユニットプレハブの生成
        GameObject unitGO = Instantiate(_playerUnitPrefab, GetWorldPosition(initialPlayerGridPos), Quaternion.identity, transform);
        Unit playerUnit = unitGO.GetComponent<Unit>();

        if(playerUnit == null)
        {
            Debug.LogError($"MapManager:PlayerUnitPrefubにPlayerUnitコンポーネントがアタッチされていません:{_playerUnitPrefab.name}");
            Destroy(unitGO);
            return;
        }
        _currentPlayerUnit = playerUnit;

        UnitData dummyData = new UnitData();
        dummyData.UnitId = "PLAYER001";
        dummyData.UnitName = "none";
        dummyData.Type = UnitType.Infantry;
        dummyData.BaseMovement = 5;
        dummyData.BaseAttackPower = 5;
        dummyData.BaseDefensePower = 5;
        dummyData.BaseHP = 5;
        dummyData.BaseSkill = 5;
        dummyData.BaseSpeed = 5;

        //プレイヤーユニットの初期化とワールド座標への配置
        _currentPlayerUnit.Initialize(dummyData);//ユニット名も渡す
        _currentPlayerUnit.UpdatePosition(initialPlayerGridPos);//ワールド座標を設定
        
        

        Debug.Log($"PlayerUnit'{_currentPlayerUnit.name}'placed at grid:{initialPlayerGridPos}");
    }

    private void HandleMouseClick()
    {
        //マウスのスクリーン座標をワールド座標に変換
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;//2DゲームなのでZは0に固定

        Debug.Log($"クリックされたワールド座標：{mouseWorldPos}");

        //ワールド座標をグリッド座標に変換
        Vector2Int clickedGridPos = GetGridPositionFromWorld(mouseWorldPos);

        Debug.Log($"クリックされたグリッド座標：{clickedGridPos}");

        //Ryacastを使ってクリックされたオブジェクトを検出
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if(hit.collider != null)
        {
            //クリックされたのがプレイヤーユニットか確認
            PlayerUnit clickedUnit = hit.collider.GetComponent<PlayerUnit>();
            if(clickedUnit != null)
            {
                //ユニットがクリックされた場合はタイルクリック処理を行わない
                HandleUnitClick(clickedUnit);
                return;
            }

            //クリックされたのがタイルか確認
            Tile clickedTile = hit.collider.GetComponent<Tile>();
            if(clickedTile != null)
            {
                HandleTileClick(clickedTile);
                return;
            }
        }
        else
        {
            //何もクリックされなかった場合（マップ外など）
            Debug.Log("何もクリックされませんでした");
            //選択中のユニットがいれば非選択状態にする
            if(_selectedUnit != null)
            {
                _selectedUnit.SetSelected(false);
                _selectedUnit = null;
                Debug.Log("ユニットの選択を解除しました。");
            }
        }

        ////クリックされたグリッド座標がマップ範囲内かチェック
        //if(_currentMapData == null || clickedGridPos.x < 0 || clickedGridPos.x >= _currentMapData.Width ||
        //    clickedGridPos.y < 0 || clickedGridPos.y >= _currentMapData.Height)
        //{
        //    Debug.Log("マップ範囲外がクリックされました");
        //    return;
        //}

        //プレイヤーユニットの移動処理の導入に伴い変更2025/06
        ////クリックされたタイルを取得
        //if(_tiles.TryGetValue(clickedGridPos, out Tile clickedTile))
        //{
        //    Debug.Log($"クリックされたタイル：{clickedTile.GridPosition},地形：{clickedTile.TerrainType}ゴール：{clickedTile}");

        //    
        //    //仮の移動処理（クリックなどの事前処理の確認のため
        //    //現在は、クリックされたタイルへプレイヤーユニットを移動させるのみ
        //    //if(_currentPlayerUnit != null)
        //    //{
        //    //    //仮の移動処理
        //    //    _currentPlayerUnit.SetGridPosition(clickedGridPos);
        //    //    _currentPlayerUnit.transform.position = GetWorldPosition(clickedGridPos);
        //    //    Debug.Log($"PlayerUnit moved to:{clickedGridPos}");
        //    //}
        //}
        //else
        //{
        //    Debug.LogWarning($"グリッド座標{clickedGridPos}に対応するタイルが見つかりません");
        //}
    }

    //ユニットがクリックされた時の処理
    private void HandleUnitClick(Unit clickedUnit)
    {
        if(_selectedUnit == clickedUnit)
        {
            _selectedUnit.SetSelected(false);
            _selectedUnit = null;
            ClearMovableRangeDisplay();
            Debug.Log("ユニットの選択を解除しました");
            return;
        }

        if(_selectedUnit != null)
        {
            _selectedUnit.SetSelected(false);//前に選択していたユニットを非選択状態にする
        }

        _selectedUnit = clickedUnit;//新しくクリックされたユニットを選択状態にする
        _selectedUnit.SetSelected(true);
        Debug.Log($"PlayerUnit'{_selectedUnit.name}'(Grid:{_selectedUnit.CurrentGridPosition}が選択されました");

        //移動範囲処理
        //ShowMovableRange(_selectdUnit);

        CalculateAndShowMovableRange(_selectedUnit);
    }

    //タイルがクリックされた時の処理
    private void HandleTileClick(Tile clickedTile)
    {
        Debug.Log($"クリックされたタイル：{clickedTile.GridPosition},地形：{clickedTile.TerrainType}");
        
        if(_selectedUnit != null)
        {
            //クリックされたタイルが移動可能範囲内にあるかチェック
            if (_currentHighlights.ContainsKey(clickedTile.GridPosition))
            {
                Debug.Log($"移動可能なタイル {clickedTile.GridPosition} がクリックされました。");

                //経路を計算する
                List<Vector2Int> path = DijkstraPathfinder.GetPathToTarget(
                    _selectedUnit.CurrentGridPosition,
                    clickedTile.GridPosition,
                    _selectedUnit);

                if(path != null && path.Count > 0)
                {
                    ShowPathLine(path);
                    StartCoroutine(MoveUnitAlogPath(_selectedUnit, path));
                }
                else
                {
                    Debug.LogWarning("経路が見つからないか、計算された経路が空です。");
                }
            }
            Debug.Log("移動不可能なタイルがクリックされました。");

            //現段階では移動後選択状態を解除するようにする
            //_selectdUnit.SetSelected(false);
            //_selectdUnit = null;

            //移動範囲の表示のクリア
            //ClearMovableRangeDisplay();
        }
        else
        {
            Debug.Log("ユニットが選択されていません");
        }
    }

    //移動可能範囲を表示する
    //private void ShowMovableRange(Unit unit)
    //{
    //    //現段階ではテスト用としてユニットの周囲のタイルをハイライト表示
    //    Debug.Log($"ユニットの移動範囲を表示。移動力{unit.GetMoveRange()}");

    //    //仮：現在地から周囲1マスをハイライト表示
    //    Vector2Int cuurentPos = unit.GetCurrentGridPostion();
    //    for(int y = -1; y <= 1; y++)
    //    {
    //        for(int x = -1; x <= 1; x++)
    //        {
    //            Vector2Int cheakPos = new Vector2Int(cuurentPos.x + x , cuurentPos.y + y);
    //            if(_tiles.TryGetValue(cheakPos, out Tile tile))
    //            {
    //                if(tile.GetComponent<SpriteRenderer>() != null)
    //                {
    //                    tile.GetComponent<SpriteRenderer>().color = Color.blue;
    //                }
    //            }
    //        }
    //    }
    //}

    //移動範囲の表示をクリアする
    private void ClearMovableRangeDisplay()
    {
        //全てのタイルをデフォルトに戻す
        foreach(var highlight in _currentHighlights.Values)
        {
            Destroy(highlight);
        }
        _currentHighlights.Clear();
    }

    /// <summary>
    /// 経路ラインを表示する
    /// </summary>
    /// <param name="path">経路のグリッド座標リスト</param>
    private void ShowPathLine(List<Vector2Int> path)
    {
        ClearPathLine();//既存のラインをクリア

        if(path == null || path.Count < 2)
        {
            return;
        }

        if(_pathLinePrefab == null)
        {
            Debug.LogWarning("Path Line Prefubが設定されていません");
            return;
        }

        _currentPathLine = Instantiate(_pathLinePrefab, Vector3.zero, Quaternion.identity);
        _currentPathLine.transform.SetParent(transform);//MapManagerの子にする

        //経路のグリッド座標をワールド座標のリストに変換
        Vector3[] worldPoints = new Vector3[path.Count];
        for(int i = 0; i < path.Count; i++)
        {
            worldPoints[i] = GetWorldPosition(path[i]);
        }
        
        //LineRendererに頂点数を設定し、経路のワールド座標をセット
        _currentPathLine.positionCount = worldPoints.Length;
        _currentPathLine.SetPositions(worldPoints);

        //LineRendererの表示設定
        _currentPathLine.startWidth = 0.1f;//線の開始時の太さ
        _currentPathLine.endWidth = 0.1f;//線の終了時の太さ
        _currentPathLine.material = new Material(Shader.Find("Sprites/Default"));//マテリアル
        _currentPathLine.startColor = Color.white;//線の開始時の色
        _currentPathLine.endColor = Color.white;//線の終了時の色
        _currentPathLine.sortingLayerName = "Foreground"; // 表示レイヤーを設定（任意、最前面に表示したい場合）
        _currentPathLine.sortingOrder = 10; // 表示順序を設定（任意）

        Debug.Log($"経路ラインを表示：経路長：{path.Count}");
    }
    
    /// <summary>
    /// 経路ラインをクリアする
    /// </summary>
    private void ClearPathLine()
    {
        if(_currentPathLine != null)
        {
            Destroy(_currentPathLine.gameObject);
            _currentPathLine = null;
        }
    }

    /// <summary>
    /// 移動可能範囲を計算し、ハイライト表示する
    /// </summary>
    /// <param name="unit">移動するユニット</param>
    private void CalculateAndShowMovableRange(Unit unit)
    {
        ClearMovableRangeDisplay();
        ClearPathLine();

        if(unit == null || _movableHighlightPrefab == null)
        {
            return;
        }

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableNodes =
            DijkstraPathfinder.FindReachableTiles(unit.CurrentGridPosition, unit);

        //計算された移動可能範囲のタイルをハイライト表示
        foreach(var entry in reachableNodes)
        {
            Vector2Int highlightPos = entry.Key;

            GameObject highlightGO = Instantiate(_movableHighlightPrefab, GetWorldPosition(highlightPos), Quaternion.identity, transform);
            _currentHighlights.Add(highlightPos,highlightGO);
        }
    } 

    /// <summary>
    /// ユニットを経路に沿って移動させる
    /// </summary>
    /// <param name="unit">移動するユニット</param>
    /// <param name="path">移動経路のグリッド座標リスト</param>
    private System.Collections.IEnumerator MoveUnitAlogPath(Unit unit,List<Vector2Int> path)
    {
        //選択解除とハイライトクリアは移動開始時に行う2025/06
        if(_selectedUnit != null)
        {
            _selectedUnit.SetSelected(false);
            _selectedUnit = null;
        }
        ClearMovableRangeDisplay();


        float moveSpeed = 5.0f;//Unitの見た目の移動速度

        for(int i = 0; i < path.Count; i++)
        {
            Vector3 startWorldPos = unit.transform.position;
            Vector3 targetWorldPos = GetWorldPosition(path[i]);
            float distance = Vector3.Distance(startWorldPos, targetWorldPos);
            float duration = distance / moveSpeed;
            float elapsed = 0f;

            //各タイルへ向けて移動
            while(elapsed < duration)
            {
                unit.transform.position = Vector3.Lerp(startWorldPos, targetWorldPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;//1フレーム待つ
            }
            unit.transform.position = targetWorldPos;//確実に目標地点に到達させる

            
            unit.UpdatePosition(path[i]);
        }
        ClearPathLine();//現在は移動完了でクリア
        Debug.Log("ユニットの移動が完了しました");
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_mapSequence.Length > 0)
        {
            //GenerateMap(_mapSequence[_currentMapIndex]);
            GenerateMap(_mapSequence[0]);
            PlacePlayerUnitAtInitialPostiton();
        }
        else
        {
            Debug.LogError("MapManager:マップシーケンスが設定されていません");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2Int clickedGridPos = GetGridPositionFromWorld(mouseWorldPos);
        
        
        //マウス操作を検知（左クリック2025 / 06）
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        //Tile clickedTile = GetTileAt(clickedGridPos);
        //if(clickedTile == null)
        //{
        //    return;
        //}

        ////ユニットがクリックされた場合
        //if(clickedTile.OccupyingUnit != null)
        //{
        //    HandleUnitClick(clickedTile.OccupyingUnit);
        //}
        ////タイルがクリックされた場合
        //else
        //{
        //    HandleTileClick(clickedTile);
        //}
    }
}
