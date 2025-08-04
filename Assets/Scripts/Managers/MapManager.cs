using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

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

    private int _currentMapIndex = 0;//現在ロードしているマップのインデックス
    private MapData _currentMapData;//MapDtaLoaderによって読み込まれるマップデータ

    private Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();//生成された全てのTileオブジェクトとグリッド座標を管理する

    //移動関連
    //ハイライト表示関連
    [SerializeField] private GameObject _movableHighlightPrefab;//移動用の青色ハイライトプレハブ
    [SerializeField] private GameObject _attackHighlightPrefab;//攻撃可能範囲用の赤色ハイライトプレハブ
    private Dictionary<Vector2Int,GameObject> _currentHighlights = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int,GameObject> _activeHighlights = new Dictionary<Vector2Int,GameObject>();
    [SerializeField] private LineRenderer _pathLinePrefab;//経路表示用のプレハブ
    private LineRenderer _currentPathLine;//現在表示されている経路ライン
    //確認用ハイライト
    [SerializeField] private GameObject _occupiedHighlightPrefab;//ユニット占有用のハイライト

    //ユニットの移動状態を管理するための変数
    private Vector2Int _originalUnitPositon = Vector2Int.zero;//移動前のグリッド座標
    private Vector2Int _currentPlannedMovePositon = Vector2Int.zero;//移動先のグリッド座標
    private bool _isMovingOrPlanning = false;//移動計画中または移動中かを示すフラグ
    private bool _isConfirmingMove = false;//移動確定待ち状態かどうかを示すフラグ

    //ユニットの移動キャンセルを管理する
    private bool _canceled = false;//一度でもキャンセルしたかを示すフラグ

    
    //PlayerUnit関連
    [SerializeField] private GameObject _playerUnitPrefab;
    private Unit _currentPlayerUnit;//現在のプレイヤーユニットの参照
    private Unit _selectedUnit;//選択中のプレイヤーユニット
    private PlayerUnit _selectedplayerUnit;

    //EnemyUnit関連
    [SerializeField] private GameObject _enemyUnitprefab;
    private Unit _currentEnemyUnit;//現在の敵ユニットの参照
    private Unit _selectedEnemyUnit;//選択中の敵ユニット

    //各のユニットのリスト(管理を一括化するか個別化するか検討中2025/07）
    //各ユニットリスト
    private List<PlayerUnit> _allPlayerUnits = new List<PlayerUnit>();
    private List<EnemyUnit> _allEnemyUnits = new List<EnemyUnit>();
    //2025/07仮として個別化で実装（一部一括用の処理も記載）
    private List<Unit> _allUnit = new List<Unit>();
    //private List<PlayerUnit> _playerUnit;
    //private List<EnemyUnit> _enemyUnit;

    // 新しく用意したユニットプレハブのフィールド
    [SerializeField] private PlayerUnit _player001Prefab;
    [SerializeField] private PlayerUnit _player002Prefab;
    [SerializeField] private EnemyUnit _enemy001Prefab;
    [SerializeField] private EnemyUnit _enemy002Prefab;


    //カメラ設定関連
    [SerializeField] private Camera _mainCamera;//メインカメラへの参照用
    [SerializeField] private float _cameraMoveSpeed = 5.0f;//カメラの追従速度

    //１タイルのワールド座標上のサイズ（カメラ移動の仮仕様2025/07）
    [SerializeField] private float _tileWorldSize = 1.0f;

    //ゲーム画面に固定表示するグリッド範囲
    private const int _visibleGridWidth = 16;//横16マス
    private const int _visibleGridHeight = 10;//縦9マス

    private Vector3 _cameraTargetPosition;//カメラの目標位置
    private const float _cameraFixedZPosition = -10f;//2DなのでZ軸は固定

    //[SerializeField]
    private Grid _tilemapGrid;//カメラ移動処理のために基準点グリッド座標(0,0)のワールド座標の取得用
    Vector2Int tileBasePos = new Vector2Int(-4, -3);


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

    /// <summary>
    /// マップのサイズに合わせてカメラの表示範囲を初期設定する
    /// </summary>
    private void InitializeCamera()
    {
        if(_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if(_mainCamera == null)
            {
                Debug.LogError("メインカメラが見つかりません。");
                return;
            }
        }

        // 表示するグリッドの横幅と縦幅を、固定値と実際のマップサイズの大きい方を参照する
        //float targetWidth = Mathf.Max(_visibleGridWidth, _currentMapData.Width);
        //float targetHeight = Mathf.Max(_visibleGridHeight, _currentMapData.Height);

        //_mainCamera.orthographicSize = (float)_visibleGridHeight / 2.0f;
        
        
        _mainCamera.orthographicSize = (_visibleGridWidth * _tileWorldSize) / (16f / 9f * 2f);



        //_mainCamera.orthographicSize = (_visibleGridWidth * _tileWorldSize) / (_mainCamera.aspect * 2.0f);



        ////float fixedZPosition = -10;
        //float cameraPosRevise = 0.5f;

        //// マップが固定表示範囲より小さい場合、カメラをマップの中央に設定
        //if (_currentMapData.Width <= _visibleGridWidth && _currentMapData.Height <= _visibleGridHeight)
        //{
        //    _cameraTargetGridPosition = new Vector2Int(_currentMapData.Width / 2,_currentMapData.Height / 2);
        //}
        //else // マップが固定表示範囲より大きい場合
        //{
        //    Debug.LogWarning("大きい");

        //    _cameraTargetGridPosition = new Vector2Int(_visibleGridWidth / 2, _visibleGridHeight / 2); // マップの(0,0)グリッドが画面の左下隅に表示されるように
        //}

        //Vector3 targetWorldPosition = GetWorldPositionFromGrid(_cameraTargetGridPosition);
        //_mainCamera.transform.position = new Vector3(targetWorldPosition.x,targetWorldPosition.y,-10);
        //Debug.LogWarning($"{_cameraTargetGridPosition.x},{_cameraTargetGridPosition.y}");

        //ワールド座標とグリッド座標とのすり合わせ
        //Vector3 tileBaseWorldPos = GetTileWorldPosition(Vector2Int.zero);
        //Vector2Int tileBasePos =  new Vector2Int(-4, -3);


        //float cameraCenterX = tileBaseWorldPos.x + ((float)_visibleGridWidth * _tileWorldSize) / 2.0f;
        //float cameraCenterY = tileBaseWorldPos.y + ((float)_visibleGridHeight * _tileWorldSize) / 2.0f;



        float camHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect; 

        float camHalfHeight = _mainCamera.orthographicSize;
        _cameraTargetPosition = new Vector3(
            camHalfWidth + tileBasePos.x,
            camHalfHeight + tileBasePos.y,
            _cameraFixedZPosition
        );

        //_cameraTargetPosition = new Vector3(
        //    cameraCenterX,
        //    cameraCenterY,
        //    _cameraFixedZPosition
        //);



        //// マップが固定表示範囲より小さい場合、カメラをマップの中央に設定
        //if (_currentMapData.Width <= _visibleGridWidth && _currentMapData.Height <= _visibleGridHeight)
        //{
        //    _cameraTargetPosition = new Vector3(
        //       ((float)_currentMapData.Width * _tileWorldSize) / 2.0f,
        //       ((float)_currentMapData.Height * _tileWorldSize) / 2.0f,
        //       _cameraFixedZPosition
        //   );
        //}
        //else // マップが固定表示範囲より大きい場合
        //{
        //    _cameraTargetPosition = new Vector3(
        //       camHalfWidth,  // カメラの中心を、画面の左端がワールド座標の0になる位置に設定
        //       camHalfHeight, // カメラの中心を、画面の下端がワールド座標の0になる位置に設定
        //       _cameraFixedZPosition
        //   );
        //}



        _mainCamera.transform.position = _cameraTargetPosition;






        //カメラの位置をマップの中心に設定
        //if (targetWidth > targetHeight)
        //{
        //    cameraTargetPosition = new Vector3(
        //        targetWidth / 2.0f * _tileWorldSize,
        //        targetHeight / 2.0f * _tileWorldSize,
        //        _mainCamera.transform.position.z);

        //    float mapCenterX = ((float)_currentMapData.Width * _tileWorldSize) / 2.0f - _tileWorldSize;
        //    float mapCenterY = ((float)_currentMapData.Height * _tileWorldSize) / 2.0f - _tileWorldSize;

        //    float camHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;
        //    float camHalfHeight = _mainCamera.orthographicSize;

        //    float clampedX = Mathf.Clamp(mapCenterX, camHalfWidth, (_currentMapData.Width * _tileWorldSize) - camHalfWidth);
        //    float clampedY = Mathf.Clamp(mapCenterY, camHalfHeight, (_currentMapData.Height * _tileWorldSize) - camHalfHeight);
        //}

        //Vector3 _cameraTargetPosition = new Vector3(clampedX - 1, clampedY - 1, _cameraFixedZPosition);

        //確認用
        //Debug.LogWarning($"メインカメラの初期座標は。X{_cameraTargetPosition.x},Y{_cameraTargetPosition.y},Z{_cameraTargetPosition.z}");
        //Debug.LogWarning($"マップの大きさ。X{_currentMapData.Width},Y{_currentMapData.Height}");


        // マップが固定表示範囲より小さい場合、カメラをマップの中央に配置
        //_mainCamera.transform.position = _cameraTargetPosition;

    }


    //仮仕様2025/07
    /// <summary>
    /// キーボード入力に応じてカメラの目標位置を更新する
    /// </summary>
    private void HandleCameraInput()
    {
        // マップが固定表示範囲より大きい場合のみ、カメラを移動させる
        if (_currentMapData.Width <= _visibleGridWidth && _currentMapData.Height <= _visibleGridHeight)
        {
            return;
        }

        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDirection.x += 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveDirection.x -= 1;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveDirection.y += 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDirection.y -= 1;
        }

        if(moveDirection != Vector3.zero)
        {
            _cameraTargetPosition += moveDirection * _tileWorldSize;

            _cameraTargetPosition.z = _cameraFixedZPosition;

            //カメラの追従範囲を計算
            float camHalfHeight = _mainCamera.orthographicSize;
            float camHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;

            Vector3 mapMinWorldPos = GetWorldPositionFromGrid(Vector2Int.zero);
            Vector3 mapMaxWorldPos = GetWorldPositionFromGrid(new Vector2Int(_currentMapData.Width - 1, _currentMapData.Height - 1));


            float minX = camHalfWidth + tileBasePos.x;
            float maxX = (_currentMapData.Width * _tileWorldSize) - camHalfWidth + tileBasePos.x;
            float minY = camHalfHeight + tileBasePos.y;
            float maxY = (_currentMapData.Height * _tileWorldSize) - camHalfHeight + tileBasePos.y;

            //float minX = mapMinWorldPos.x + camHalfWidth;
            //float maxX = mapMaxWorldPos.x + camHalfWidth;
            //float minY = mapMinWorldPos.y + camHalfHeight;
            //float maxY = mapMaxWorldPos.y + camHalfHeight;

            // マップが固定表示範囲より大きい場合、 maxX, maxYの計算はマップのワールド座標の右端を基準とする
            //if (_currentMapData.Width > _visibleGridWidth)
            //{
            //    maxX = mapMinWorldPos.x + (_currentMapData.Width * _tileWorldSize) - camHalfWidth;
            //}
            //if (_currentMapData.Height > _visibleGridHeight)
            //{
            //    maxY = mapMinWorldPos.y + (_currentMapData.Height * _tileWorldSize) - camHalfHeight;
            //}

            //目標位置をマップの端にクランプ
            _cameraTargetPosition.x = Mathf.Clamp(_cameraTargetPosition.x,minX, maxX);
            _cameraTargetPosition.y = Mathf.Clamp(_cameraTargetPosition.y,minY, maxY);

            Vector3 newPos = new Vector3(_cameraTargetPosition.x,_cameraTargetPosition.y, _cameraTargetPosition.z);


            //確認用
            //Debug.LogWarning($"現在のカメラの位置：X{_cameraTargetPosition.x},Y{_cameraTargetPosition.y}");
            //Debug.LogWarning($"newPos：X{newPos.x},Y{newPos.y}");

            _mainCamera.transform.position = newPos;
        }
    }

    /// <summary>
    /// カメラを目標位置まで移動させる
    /// </summary>
    private void MoveCameraToTarget()
    {
        Vector3 newPos = Vector3.Lerp(
            _mainCamera.transform.position,
            _cameraTargetPosition,
            _cameraMoveSpeed * Time.deltaTime
            );

        newPos.z = _cameraFixedZPosition;
        _mainCamera.transform.position = newPos;
    }


    /// <summary>
    /// グリッド座標からワールド座標を取得するヘルパーメソッド
    /// タイルの左下隅を基準にする
    /// </summary>
    /// <param name="gridPosition">グリッド座標</param>
    /// <returns>タイルの左下隅のワールド座標</returns>
    private Vector3 GetTileWorldPosition(Vector2Int gridPosition)
    {
        if (_tilemapGrid == null)
        {
            Debug.LogError("Tilemap Gridが設定されていません。");
            return Vector3.zero;
        }

        // CellToWorldメソッドはタイルの左下隅を返すため、そのまま使用する
        Vector3 worldPos = _tilemapGrid.CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));

        return worldPos;
    }



    //初期化処理
    public void Initialize()
    {
        //タイルリストとユニットリストのクリア
        ClaerExistingUnits();
        ClearMap();

        //デバッグ用
        _currentMapIndex = 0;

        string currentMapId = _mapSequence[_currentMapIndex];
        GenerateMap(currentMapId);

        PlaceEnemiesForCurrentMap(currentMapId);

        //_allPlayerUnits.Clear();
        //_allEnemyUnits.Clear();

        //複数のユニット配置2025/07

        //プレイヤーユニット
        //GameObject player1GO = Instantiate(_playerUnitPrefab,transform);
        //PlayerUnit player1 = player1GO.GetComponent<PlayerUnit>();
        //if (player1 != null)
        //{
        //    player1.name = "Player001";
        //    PlaceUnit(player1,new Vector2Int(0,4));
        //}
        //else
        //{
        //    Debug.LogError("Player1プレハブにPlayerUnitコンポーネントが見つかりません！");
        //}


        if (_player001Prefab != null)
        {
            PlayerUnit player001 = Instantiate(_player001Prefab,transform);
            PlaceUnit(player001, new Vector2Int(0, 0));
        }
        else 
        { 
            Debug.LogError("MapManager: _player001Prefabが割り当てられていません！"); 
        }

        if (_player002Prefab != null)
        {
            PlayerUnit player002 = Instantiate(_player002Prefab, transform);
            PlaceUnit(player002, new Vector2Int(0, 2));
        }
        else
        {
            Debug.LogError("MapManager: _player002Prefabが割り当てられていません！");
        }

        //敵ユニット
        //GameObject enemy1Go = Instantiate(_enemyUnitprefab, transform);
        //EnemyUnit enemy1 = enemy1Go.GetComponent<EnemyUnit>();
        //if (enemy1 != null)
        //{
        //    enemy1.name = "Enemy001";
        //    PlaceUnit(enemy1, new Vector2Int(5, 0));
        //}
        //else
        //{
        //    Debug.LogError("Enemy001プレハブにPlayerUnitコンポーネントが見つかりません！");
        //}


        //敵ユニットの生成処理を変更2025/07
        //if(_enemy001Prefab != null)
        //{
        //    EnemyUnit enemy001 = Instantiate(_enemy001Prefab, transform);
        //    PlaceUnit(enemy001, new Vector2Int(5, 0));
        //}
        //else
        //{
        //    Debug.LogError("MapManager: _enemy001Prefabが割り当てられていません！");
        //}

        //if(_enemy002Prefab != null)
        //{
        //    EnemyUnit enemy002 = Instantiate(_enemy002Prefab, transform);
        //    PlaceUnit(enemy002, new Vector2Int(4, 4));
        //}
        //else
        //{
        //    Debug.LogError("MapManager: _enemy002Prefabが割り当てられていません！");
        //}

        TurnManager.Instance.InitializeTurnManager();
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
    public void GenerateMap(string mapId)
    {
        ClearMap();//既にマップが生成されている可能性を考慮して、一度クリアする

        //MapDataLoaderでCSVファイルからマップデータを読み込む
        MapData mapData = MapDataLoader.LoadMapDataFromCSV(mapId);

        //_currentMapData = MapDataLoader.LoadMapDataFromCSV(mapId);

        if(mapData == null)
        {
            Debug.LogError("MapManager:マップデータの読み込みに失敗しました。マップ生成できません");
            return;
        }

        _currentMapData = mapData;
        

        for (int y = 0; y < _currentMapData.Height; y++)
        {
            for(int x = 0; x < _currentMapData.Width; x++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);//現在のグリッド座標
                TerrainType terrainType = _currentMapData.GetTerrainType(gridPos);//その座標の地形タイプを取得
                

                Vector3 worldPos = GetWorldPositionFromGrid(gridPos);//グリッド座標をワールド座標に変換

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
            return int.MaxValue; //範囲外は移動不可
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
    /// 指定された座標がマップの有効な範囲内にあるかチェックする
    /// </summary>
    /// <param name="gridPosition">チェックするグリッド座標</param>
    /// <return>有効な範囲内であればtrue、そうでなければfalse</return>
    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        if (_currentMapData == null)
        {
            Debug.LogError("MapDataがロードされていません。IsValidGridPositionを呼び出す前にMapDataを初期化してください。");
            return false;
        }

        return gridPosition.x >= 0 && gridPosition.x < _currentMapData.Width &&
            gridPosition.y >= 0 && gridPosition.y < _currentMapData.Height;
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
    public Vector3 GetWorldPositionFromGrid(Vector2Int gridPos)
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
    /// 全てのユニットのリストを取得する
    /// </summary>
    public List<Unit> GetAllUnits()
    {
        return _allUnit.OfType<Unit>().ToList();
    }

    /// <summary>
    /// 全てのプレイヤーユニットのリストを取得する
    /// </summary>
    public List<PlayerUnit> GetAllPlayerUnits()
    {
        return _allPlayerUnits.OfType<PlayerUnit>().ToList();
    }

    /// <summary>
    /// 全ての敵ユニットのリストを取得する
    /// </summary>
    public List<EnemyUnit> GetAllEnemyUnits()
    {
        return _allEnemyUnits.OfType<EnemyUnit>().ToList();
    }

    /// <summary>
    /// 全てのシーン内のユニットのクリア
    /// </summary>
    private void ClaerExistingUnits()
    {
        //現在シーンにある全てのユニットオブジェクトを削除
        foreach(var tileEntry in _tiles)
        {
            if(tileEntry.Value.OccupyingUnit != null)
            {
                Destroy(tileEntry.Value.OccupyingUnit.gameObject);
                tileEntry.Value.OccupyingUnit = null;
            }
        }
        _allUnit.Clear();
        _allPlayerUnits.Clear();
        _allPlayerUnits.Clear();
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
        GameObject unitGO = Instantiate(_playerUnitPrefab, GetWorldPositionFromGrid(initialPlayerGridPos), Quaternion.identity, transform);
        Unit playerUnit = unitGO.GetComponent<Unit>();

        if(playerUnit == null)
        {
            Debug.LogError($"MapManager:PlayerUnitPrefubにPlayerUnitコンポーネントがアタッチされていません:{_playerUnitPrefab.name}");
            Destroy(unitGO);
            return;
        }
        _currentPlayerUnit = playerUnit;

        UnitData dummyData = new UnitData();
        //dummyData.UnitId = "PLAYER001";
        //dummyData.UnitName = "none";
        //dummyData.Type = UnitType.Infantry;
        //dummyData.BaseMovement = 3;
        //dummyData.BaseAttackPower = 5;
        //dummyData.BaseDefensePower = 5;
        //dummyData.MaxHP = 5;
        //dummyData.BaseSkill = 5;
        //dummyData.BaseSpeed = 5;

        //プレイヤーユニットの初期化とワールド座標への配置
        _currentPlayerUnit.Initialize(dummyData);//ユニット名も渡す
        _currentPlayerUnit.UpdatePosition(initialPlayerGridPos);//ワールド座標を設定
        
        

        Debug.Log($"PlayerUnit'{_currentPlayerUnit.name}'placed at grid:{initialPlayerGridPos}");
    }

    /// <summary>
    /// 敵ユニットを初期位置に配置する
    /// </summary>
    private void PlaceEnemyUnitAtInitialPostiton()
    {
        //プロトタイプ用の仮初期座標
        Vector2Int initialEnemyGridPos = new Vector2Int(5, 0);


        //指定された座標がマップ範囲内か確認
        if (initialEnemyGridPos.x < 0 || initialEnemyGridPos.x >= _currentMapData.Width ||
            initialEnemyGridPos.y < 0 || initialEnemyGridPos.y >= _currentMapData.Height)
        {
            Debug.LogError($"MapManager:敵ユニットの初期配置座標({initialEnemyGridPos})がマップ範囲外です");
        }

        //敵ユニットが既に存在する場合は破棄(シーン遷移などで再生成する場合)
        if (_currentEnemyUnit != null)
        {
            Destroy(_currentEnemyUnit.gameObject);
        }

        //敵ユニットプレハブの生成
        GameObject enemyGO = Instantiate(_enemyUnitprefab, GetWorldPositionFromGrid(initialEnemyGridPos), Quaternion.identity, transform);
        Unit enemyUnit = enemyGO.GetComponent<Unit>();

        if (enemyUnit == null)
        {
            Debug.LogError($"MapManager:EnemyUnitPrefubにEnemyUnitコンポーネントがアタッチされていません:{_enemyUnitprefab.name}");
            Destroy(enemyGO);
            return;
        }
        _currentEnemyUnit = enemyUnit;

        UnitData dummyData = new UnitData();
        //dummyData.UnitId = "ENEMY001";
        //dummyData.UnitName = "one";
        //dummyData.Type = UnitType.Infantry;
        //dummyData.BaseMovement = 3;
        //dummyData.BaseAttackPower = 5;
        //dummyData.BaseDefensePower = 5;
        //dummyData.MaxHP = 5;
        //dummyData.BaseSkill = 5;
        //dummyData.BaseSpeed = 5;

        //敵ユニットの初期化とワールド座標への配置
        //_currentEnemyUnit.Initialize(dummyData);//ユニット名も渡す
        _currentEnemyUnit.UpdatePosition(initialEnemyGridPos);//ワールド座標を設定



        Debug.Log($"EnemyUnit'{_currentEnemyUnit.name}'placed at grid:{initialEnemyGridPos}");
    }

    /// <summary>
    /// ユニットの配置
    /// </summary>
    public void PlaceUnit(Unit unit, Vector2Int gridPos)
    {
        if (!_tiles.ContainsKey(gridPos))
        {
            Debug.LogError($"MapManager: グリッド座標 {gridPos} にタイルが存在しません。");
            return;
        }

        Tile targetTile = _tiles[gridPos];

        //配置先のタイルがすでに他のユニットに占有されていないかチェックし、すでに存在する場合は破棄
        if (targetTile.OccupyingUnit != null)
        {
            Debug.LogWarning($"MapManager: グリッド座標 {gridPos} は既にユニット {targetTile.OccupyingUnit.UnitName} によって占有されています。");
            Destroy(unit.gameObject);
            return;
        }

        //unit.SetGridPosition(gridPos);
        //GetTileAt(gridPos).OccupyingUnit = unit;


        unit.MoveToGridPosition(gridPos,targetTile);
        unit.transform.position = GetWorldPositionFromGrid(gridPos);


        if (unit is PlayerUnit playerUnit)
        {
            _allPlayerUnits.Add(playerUnit);
        }
        else if(unit is EnemyUnit enemyUnit)
        {
            _allEnemyUnits.Add(enemyUnit);
        }
    }
    
    /// <summary>
    /// 敵ユニット上マップに配置する（マップデータを参照）
    /// </summary>
    /// <param name="mapId"></param>
    public void PlaceEnemiesForCurrentMap(string mapId)
    {
        EnemyEncounterData encounterData = EnemyEncounterManager.Instance.GetEnemyEncounterData(mapId);
        if(encounterData == null)
        {
            Debug.LogWarning($"MapManager: マップ '{mapId}' の敵データが見つかりません。敵は配置されません。");
            return;
        }

        foreach(EnemyPlacement placement in encounterData.enemyPlacements)
        {
            if(placement.enemyPrefab != null)
            {
                EnemyUnit enemyInstance = Instantiate(placement.enemyPrefab,transform);
                PlaceUnit(enemyInstance,placement.gridPosition);
            }
            else
            {
                Debug.LogWarning($"MapManager: マップ '{mapId}' の敵配置で、Prefabがnullのものが含まれています。");
            }
        }
        Debug.Log($"MapManager:マップ'{mapId}'に敵ユニットを配置しました");
    }

    /// <summary>
    /// マウスクリックを処理する
    /// </summary>
    private void HandleMouseClick()
    {
        //仮：プレイヤーターンでなければ処理しない
        if(TurnManager.Instance != null && TurnManager.Instance.CurrnetTurnState != TurnState.PlayerTurn)
        {
            return;
        }


        //ユニットが移動計画中または移動中の場合は、新たなマウスクリック入力を受け付けない
        if (_isMovingOrPlanning)
        {
            Debug.Log("移動計画中または移動中のため、新しい入力を受け付けません");
            return;
        }

        //マウスのスクリーン座標をワールド座標に変換
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;//2DゲームなのでZは0に固定

        Debug.Log($"クリックされたワールド座標：{mouseWorldPos}");

        //ワールド座標をグリッド座標に変換
        Vector2Int clickedGridPos = GetGridPositionFromWorld(mouseWorldPos);

        Debug.Log($"クリックされたグリッド座標：{clickedGridPos}");

        //Ryacastを使ってクリックされたオブジェクトを検出
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        Tile _clickedTile = GetTileAt(clickedGridPos);
        //マップ範囲外の場合
        if(_clickedTile == null)
        {
            Debug.Log("マップ範囲外です");
            return;
        }


        //ユニットが未選択の場合
        if (_selectedUnit == null)
        {
            //クリックされたタイルにユニットがいるか、かつプレイヤーユニットか、かつ未行動か
            if (_clickedTile.OccupyingUnit != null &&
                _clickedTile.OccupyingUnit.Faction == FactionType.Player &&
                !_clickedTile.OccupyingUnit.HasActedThisTurn)
            {
                SelectUnit(_clickedTile.OccupyingUnit);
            }
            //敵ユニットの場合行動範囲予測を表示（未実装2025 / 07）
            else if (_clickedTile.OccupyingUnit != null &&
                _clickedTile.OccupyingUnit.Faction == FactionType.Enemy &&
                !_clickedTile.OccupyingUnit.HasActedThisTurn)
            {
                Debug.Log("敵ユニットです");
                SelectUnit(_clickedTile.OccupyingUnit);
            }
            else
            {
                //ユニットが未選択の状態でハイライトが出ないように
                ClearAllHighlights();
                //////ClearMovableRangeDisplay();
            }
        }
        //ユニットが選択済みの場合
        else
        {
            //選択中のユニットと同じユニットがクリックされたら選択解除
            if (_clickedTile.OccupyingUnit == _selectedUnit)
            {
                CancelMove();
            }
           //移動可能なタイルがクリックされたら移動計画
           else if (_currentHighlights.ContainsKey(clickedGridPos) && clickedGridPos != _selectedUnit.CurrentGridPosition)
            {
                if (!IsTileOccupiedForStooping(clickedGridPos,_selectedUnit))
                {
                    StartCoroutine(InitiateVisualMove(clickedGridPos));
                }
                else
                {
                    Debug.Log("MapManager: そのマスは他のユニットに占有されているため移動できません");
                    //CancelMove();
                    return;
                }
            }
           //他のユニットがクリックされたら、現在の選択を解除し。新しいユニットを選択
           else if(_clickedTile.OccupyingUnit != null && _clickedTile.OccupyingUnit.Faction == FactionType.Player && _clickedTile.OccupyingUnit != _selectedUnit)
            {
                CancelMove();//現在の選択を解除
                SelectUnit(_clickedTile.OccupyingUnit);//新しいユニットを選択
            }
           //移動範囲外の空のタイルや敵ユニットをクリックしたらキャンセル
            else
            {
                CancelMove();
            }
        }
        

        //処理を変更2025/07
        //if (hit.collider != null)
        //{
        //    //クリックされたのがプレイヤーユニットか確認
        //    PlayerUnit clickedUnit = hit.collider.GetComponent<PlayerUnit>();
        //    if(clickedUnit != null)
        //    {
        //        //ユニットがクリックされた場合はタイルクリック処理を行わない
        //        HandleUnitClick(clickedUnit);
        //        return;
        //    }

        //    //クリックされたのがタイルか確認
        //    //Tile clickedTile = hit.collider.GetComponent<Tile>();
        //    //if (clickedTile != null)
        //    //{
        //    //    HandleTileClick(clickedTile);
        //    //    return;
        //    //}

        //    Tile clickedTile = hit.collider.GetComponent<Tile>();


        //    if (clickedTile != null && _selectedUnit != null)
        //    {
                

        //        //元の位置と移動先を記録
        //        _originalUnitPositon = _selectedUnit.GetCurrentGridPostion();
        //        _currentPlannedMovePositon = clickedGridPos;

        //        //移動が計画されたので、フラグをtrueに設定
        //        _isMovingOrPlanning = true;

        //        //ユニットを移動先のタイルに一時的に移動させる
        //        //_selectedUnit.transform.position = GetWorldPositionFromGrid(_currentPlannedMovePositon);

        //        Debug.Log($"Moved unit to temporary position({_currentPlannedMovePositon.x},{_currentPlannedMovePositon.y}).Press Space to confirm, Q to cancel.");
        //    }

        //    //クリックされたのがタイルか確認
        //    //Tile clickedTile = hit.collider.GetComponent<Tile>();
        //    if (clickedTile != null)
        //    {
        //        HandleTileClick(clickedTile);
        //        return;
        //    }
        //}
        //else
        //{
        //    //何もクリックされなかった場合（マップ外など）
        //    Debug.Log("何もクリックされませんでした");
        //    //選択中のユニットがいれば非選択状態にする
        //    if(_selectedUnit != null)
        //    {
        //        _selectedUnit.SetSelected(false);
        //        _selectedUnit = null;
        //        _isMovingOrPlanning = false;
        //        Debug.Log("ユニットの選択を解除しました。");
        //    }
        //}



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


    /// <summary>
    /// 選択中のユニット取得用
    /// </summary>
    /// <param name="unit"></param>
    private void SelectUnit(Unit unit)
    {
        _selectedUnit = unit;
        _selectedUnit.SetSelected(true);
        _originalUnitPositon = unit.CurrentGridPosition;

        if(_selectedUnit.Faction == FactionType.Enemy)
        {
            Debug.Log($"選択中の敵ユニット情報:名前{_selectedUnit.name}");
            CalculateAndShowMovableRange(_selectedUnit);
            _selectedUnit = null;
            return;
        }

        //確認用
        Debug.Log($"選択中のユニット情報:名前{_selectedUnit.name}");
        CalculateAndShowMovableRange(_selectedUnit);
    }

    /// <summary>
    /// 経路計算から移動処理
    /// </summary>
    /// <param name="targetTile"></param>
    /// <param name="clickedPos"></param>
    /// 
    private void PlanMove(Tile targetTile ,Vector2Int clickedPos)
    {
        //クリックされたタイルが移動可能範囲内にあるかチェック
        if (_currentHighlights.ContainsKey(targetTile.GridPosition))
        {
            Debug.Log($"移動可能なタイル {targetTile.GridPosition} がクリックされました。");

            //経路を計算する
            List<Vector2Int> path = DijkstraPathfinder.GetPathToTarget(
                _selectedUnit.CurrentGridPosition,
                targetTile.GridPosition,
                _selectedUnit);

            if (path != null && path.Count > 0)
            {
                ShowPathLine(path);
                StartCoroutine(MoveUnitAlogPath(_selectedUnit, path));
            }
            else
            {
                Debug.LogWarning("経路が見つからないか、計算された経路が空です。");
            }
        }
        else
        {
            _isMovingOrPlanning = false;
            Debug.Log("移動不可能なタイルがクリックされました。");
        }

        _originalUnitPositon = _selectedUnit.GetCurrentGridPostion();
        _currentPlannedMovePositon = clickedPos;

        _isMovingOrPlanning = true;

        //_selectedUnit.SetSelected(false);
        //ClearAllHighlights();
        //ClearMovableRangeDisplay();
        //ClearPathLine();
        //_selectedUnit = null;
    }

    //引数の追加による変更2025/07
    ///// <summary>
    ///// タイルが他のユニットに占有されているかチェック
    ///// </summary>
    ///// <param name=""></param>
    ///// <returns></returns>
    //public bool IsTileOccupied(Vector2Int gridPos)
    //{
    //    Tile tile = GetTileAt(gridPos);
    //    return tile != null && tile.OccupyingUnit != null;
    //}


    ///PlanMove処理の変更に伴い名前変更2025/07
    private IEnumerator InitiateVisualMove(Vector2Int targetGridPos)
    {
        _currentPlannedMovePositon = targetGridPos;
        _isMovingOrPlanning= true;

        ClearPathLine();

        List<Vector2Int> path = DijkstraPathfinder.FindPath(_selectedUnit.CurrentGridPosition, targetGridPos, _selectedUnit);

        if(path != null && path.Count > 0)
        {
            _currentPathLine = Instantiate(_pathLinePrefab);
            _currentPathLine.positionCount = path.Count;
            for(int i = 0; i < path.Count; i++)
            {
                _currentPathLine.SetPosition(i,GetWorldPositionFromGrid(path[i]));
            }

            _currentPathLine.startWidth = 0.1f;//線の開始時の太さ
            _currentPathLine.endWidth = 0.1f;//線の終了時の太さ

            yield return _selectedUnit.AnimateMove(path);

            _isConfirmingMove = true;
            Debug.Log("移動が完了しました。スペースキーで確定、Qキーでキャンセルしてください。");
        }
        else
        {
            Debug.LogWarning("経路が見つかりませんでした。");
            CancelMove(); // 経路がない場合はキャンセル
        }
    }

    //タイルが他のユニットに占有されているかチェック (停止地点の判定用)
    public bool IsTileOccupiedForStooping(Vector2Int gridPos, Unit selectedUnit)
    {
        Tile tile = GetTileAt(gridPos);

        //確認用のため処理を追加
        if (tile == null)
        {
            Debug.LogError($"IsTileOccupiedForStopping: Tile at {gridPos} is null! This should not happen.");
            return true;// タイルがnullの場合は安全のため停止不可とみなす
        }
        string occupyingUnitName = (tile.OccupyingUnit != null) ? tile.OccupyingUnit.UnitName : "NONE";
        string selectedUnitName = (selectedUnit != null) ? selectedUnit.UnitName : "NONE";
        Debug.Log($"IsTileOccupiedForStopping Check: Tile {gridPos}, OccupyingUnit: {occupyingUnitName}, SelectedUnit: {selectedUnitName}");

        // タイルが存在し、かつ何らかのユニットが占有しており、それが選択中のユニット自身ではない場合
        bool occupiedByOther = tile.OccupyingUnit != null && tile.OccupyingUnit != selectedUnit;

        //デバッグログ
        Debug.Log($"IsTileOccupiedForStopping Result for {gridPos}: Occupied by other unit = {occupiedByOther}");
        return occupiedByOther;



        //return tile != null && tile.OccupyingUnit != null && tile.OccupyingUnit != _selectedUnit;
    }

    //特定のユニットが通過可能かを判定する
    public bool IsTilePassableForUnit(Vector2Int gridPos, Unit unitToCheck)
    {
        //範囲外及び無効なタイルは通過不可
        Tile tile = GetTileAt(gridPos);
        if (tile == null)
        {
            return false;
        }

        //ユニットが占有している場合
        if (tile.OccupyingUnit != null)
        {
            //占有しているのが自分自身の場合、通過可能
            if (tile.OccupyingUnit == unitToCheck)
            {
                return true;
            }
            // 占有しているのが敵ユニットの場合、通過不可
            else if (tile.OccupyingUnit.Faction != unitToCheck.Faction)
            {
                return false;
            }
            //占有しているのが味方ユニットの場合、通過可能
            else
            {
                return true;
            }
        }
        //ユニットが占有していない場合、通過可能
        return true;
    }

    //ユニットがクリックされた時の処理
    private void HandleUnitClick(Unit clickedUnit)
    {
        

        if(_selectedUnit == clickedUnit)
        {
            _selectedUnit.SetSelected(false);
            _selectedUnit = null;
            ClearAllHighlights();
            ClearMovableRangeDisplay();
            _isMovingOrPlanning=false;
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
            else
            {
                _isMovingOrPlanning = false;
                Debug.Log("移動不可能なタイルがクリックされました。");
            }

            //現段階では移動後選択状態を解除するようにする
            //_selectdUnit.SetSelected(false);
            //_selectdUnit = null;

            //移動範囲の表示のクリア
            //ClearMovableRangeDisplay();
            //ClearAllHighlights();
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

    //移動範囲のハイライト表示をクリアする
    private void ClearMovableRangeDisplay()
    {
        //全てのタイルをデフォルトに戻す
        foreach(var highlight in _currentHighlights.Values)
        {
            Destroy(highlight);
        }
        _currentHighlights.Clear();
    }

  
    //現在表示されている全てのハイライトをクリアする
    public void ClearAllHighlights()
    {
        foreach(var highlightGo in _activeHighlights.Values)
        {
            Destroy(highlightGo);
        }

        foreach (var highlight in _currentHighlights.Values)
        {
            Destroy(highlight);
        }

        _currentHighlights.Clear();

        _activeHighlights.Clear();
    }

    //特定のタイルをハイライト表示する
    public void HighlightTile(Vector2Int gridPosition, HighlightType type)
    {
        //
        if (_activeHighlights.ContainsKey(gridPosition))
        {
            Destroy(_activeHighlights[gridPosition]);
            _activeHighlights.Remove(gridPosition);
        }

        GameObject highlightPrefab = null;
        switch (type)
        {
            case HighlightType.Move:
                highlightPrefab = _movableHighlightPrefab;
                break;
            case HighlightType.Attack:
                highlightPrefab = _attackHighlightPrefab;
                break;
        }

        if(highlightPrefab != null)
        {
            Vector3 worldPos = MapManager.Instance.GetWorldPositionFromGrid(gridPosition);
            GameObject highlight = Instantiate(highlightPrefab, worldPos, Quaternion.identity);
            highlight.transform.SetParent(MapManager.Instance.GetTileAt(gridPosition).transform);
            _activeHighlights[gridPosition] = highlight;
        }
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
            worldPoints[i] = GetWorldPositionFromGrid(path[i]);
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

    //public void OnUnitSelected(Unit unit)
    //{
    //    ClearAllHighlights();//既存のハイライトをクリア

    //    Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableTiles =
    //        DijkstraPathfinder.FindReachableTiles(unit.CurrentGridPosition, unit);

    //    foreach (Vector2Int pos in reachableTiles.Keys)
    //    {
    //        HighlightTile(pos, HighlightType.Move);
    //    }

    //    //攻撃可能範囲を計算し、赤色でハイライト
    //    ShowAttackRangeHighlight(reachableTiles.Keys.ToList(),unit);

    //}

    /// <summary>
    /// 移動可能範囲を計算し、ハイライト表示する
    /// </summary>
    /// <param name="unit">移動するユニット</param>
    private void CalculateAndShowMovableRange(Unit unit)
    {
        //ClearMovableRangeDisplay();
        ClearAllHighlights();
        ClearPathLine();

        if (unit == null || _movableHighlightPrefab == null)
        {
            return;
        }

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableNodes =
            DijkstraPathfinder.FindReachableTiles(unit.CurrentGridPosition, unit);

        

        //計算された移動可能範囲のタイルをハイライト表示
        foreach(var entry in reachableNodes)
        {
            Vector2Int highlightPos = entry.Key;

            if(highlightPos == unit.CurrentGridPosition)
            {
                Debug.Log($"Highlighting current position: {highlightPos}");
                GameObject highlightGO = Instantiate(_movableHighlightPrefab, GetWorldPositionFromGrid(highlightPos), Quaternion.identity, transform);
                _currentHighlights.Add(highlightPos, highlightGO);
                continue;
            }

            if (!IsTileOccupiedForStooping(highlightPos,unit))
            {
                Debug.Log($"Highlighting movable (not occupied by other): {highlightPos}");
                GameObject highlightGO = Instantiate(_movableHighlightPrefab,GetWorldPositionFromGrid(highlightPos),Quaternion.identity, transform);
                _currentHighlights.Add(highlightPos,highlightGO);
            }
            else
            {
                Debug.Log($"NOT Highlighting movable (occupied by other): {highlightPos}");
                //デバッグ用
                if (_occupiedHighlightPrefab != null)
                {
                    GameObject occupiedHighlightGO = Instantiate(_occupiedHighlightPrefab, GetWorldPositionFromGrid(highlightPos), Quaternion.identity, transform);
                    _currentHighlights.Add(highlightPos, occupiedHighlightGO);
                }
            }
        }
        ShowAttackRangeHighlight(reachableNodes.Keys.ToList(), unit);

    }


    /// <summary>
    /// 攻撃可能範囲をハイライト表示する
    /// </summary>
    /// <param name="moveableTiles">移動可能なタイルリスト</param>
    private void ShowAttackRangeHighlight(List<Vector2Int> moveableTiles, Unit currentUnit)
    {
        //ClearAllHighlights();

        HashSet<Vector2Int> attackableTiles = new HashSet<Vector2Int>();
        Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0), };

        //攻撃範囲指定のマンハッタン距離方での実装(まだ各typeとの連携は未実装)
        //一部数値を仮として実装2025/06
        int minAttackRange = 2;//最小射程
        int maxAttackRange = 2;//最大射程

        foreach(Vector2Int movePos in moveableTiles)
        {
            for(int x = -maxAttackRange; x <= maxAttackRange; x++)
            {
                for(int y = -maxAttackRange; y <= maxAttackRange; y++)
                {
                    //現在の移動可能タイル(movePos)からの相対座標
                    Vector2Int potentialAttackPos = movePos + new Vector2Int(x, y);

                    //マンハッタン距離計算
                    int distance = Mathf.Abs(x) + Mathf.Abs(y);

                    if(distance >= minAttackRange && distance <= maxAttackRange)
                    {
                        if (IsValidGridPosition(potentialAttackPos))
                        {
                            attackableTiles.Add(potentialAttackPos);
                        }
                    }
                }
            }
        }

        //仮として隣接１マスのみ実装2025/06
        //各移動可能タイルから1マス隣接するタイルを攻撃可能範囲候補として追加
        foreach (Vector2Int movePos in moveableTiles)
        {
            foreach (Vector2Int dir in directions)
            {
                Vector2Int attackTargetPos = movePos + dir;
                //マップの範囲内か確認
                if (IsValidGridPosition(attackTargetPos))
                {
                    attackableTiles.Add(attackTargetPos);
                }
            }
        }

        //処理を変更のため削除
        ////敵ユニットが存在するタイルのみを赤色ハイライト
        //foreach (Vector2Int targetPos in attackableTiles)
        //{
        //    Tile targetTile = GetTileAt(targetPos);
        //    if (targetTile != null && targetTile.OccupyingUnit != null)
        //    {
        //        //ユニットが敵factionTypeであるか確認
        //        if (targetTile.OccupyingUnit.Faction == FactionType.Enemy)
        //        {
        //            HighlightTile(targetPos, HighlightType.Attack);
        //        }
        //    }
        //}

        //攻撃範囲のハイライト表示
        foreach(Vector2Int targetPos in attackableTiles)
        {
            if (!moveableTiles.Contains(targetPos))
            {
                //敵の有無を問わずハイライト
                Tile targetTile = GetTileAt(targetPos);
                if(targetTile != null)
                {
                    HighlightTile(targetPos,HighlightType.Attack);
                }
            }
        }
    }

    /// <summary>
    /// ユニットを経路に沿って移動させる
    /// </summary>
    /// <param name="unit">移動するユニット</param>
    /// <param name="path">移動経路のグリッド座標リスト</param>
    public System.Collections.IEnumerator MoveUnitAlogPath(Unit unit,List<Vector2Int> path)
    {
        //選択解除とハイライトクリアは移動開始時に行う2025/06
        //if(_selectedUnit != null)
        //{
        //    _selectedUnit.SetSelected(false);
        //    _selectedUnit = null;
        //}
        //ClearMovableRangeDisplay();


        float moveSpeed = 5.0f;//Unitの見た目の移動速度

        for(int i = 0; i < path.Count; i++)
        {
            Vector2Int targetGridPosInPath = path[i];
            Vector3 startWorldPos = unit.transform.position;
            Vector3 targetWorldPos = GetWorldPositionFromGrid(path[i]);
            Vector3 endWorldPos = GetWorldPositionFromGrid(targetGridPosInPath);
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

    public IEnumerator SmoothMoveCoroutine(Unit unit, Vector2Int startGridPos, Vector2Int endGridPos,List<Vector2Int> path)
    {
        if (path == null || path.Count <= 1) // パスが見つからない、または同じ位置にいる場合 (path.Count <= 1 は開始地点のみの場合)
        {
            Debug.LogWarning("移動パスが見つからないか、ユニットが既に目的地にいます。");
            // プレイヤーユニットの場合のみFinalizeMoveStateを呼び出す
            if (unit is PlayerUnit)
            {
                ConfirmMove();
            }
            yield break;
        }


        // 経路の各ポイントを辿って移動
        // 最初の要素は現在の位置なのでスキップ (path.Count > 1 を確認)
        for (int i = 1; i < path.Count; i++) // 最初の地点は既にいる場所なので、2番目の地点から移動を開始
        {
            Vector2Int targetGridPosInPath = path[i];
            Vector3 startWorldPos = unit.transform.position;
            Vector3 endWorldPos = GetWorldPositionFromGrid(targetGridPosInPath);
            float durationPerTile = 0.2f; // タイル1マスあたりの移動時間
            float elapsed = 0f;

            while (elapsed < durationPerTile)
            {
                unit.transform.position = Vector3.Lerp(startWorldPos, endWorldPos, elapsed / durationPerTile);
                elapsed += Time.deltaTime;
                yield return null;
            }
            unit.transform.position = endWorldPos; // 各タイルの中心に正確にスナップ

        }


        // 全ての移動が完了したら、最終的な状態を確定
        // ユニットのグリッド座標を更新
        unit.SetGridPosition(endGridPos);

        // ユニットの占有タイルを更新 (MapManagerのGetTileAtとOccupyingUnitプロパティを使用)
        Tile oldTile = GetTileAt(startGridPos);
        if (oldTile != null) oldTile.OccupyingUnit = null; // 元のタイルからユニットを解除

        Tile newTile = GetTileAt(endGridPos);
        if (newTile != null) newTile.OccupyingUnit = unit; // 新しいタイルにユニットを設定

        // プレイヤーユニットの場合のみFinalizeMoveStateを呼び出す
        if (unit is PlayerUnit)
        {
            ConfirmMove();
        }
        else // 敵ユニットの場合の移動完了処理 (必要に応じて追加)
        {
            // 敵ユニット固有の移動完了後の処理があればここに記述
            // 例: ターン終了をAIに通知するなど
        }
    }

    /// <summary>
    /// 移動を確定する
    /// </summary>
    private void ConfirmMove()
    {
        //if (_selectedUnit == null || _currentPlannedMovePositon == Vector2Int.zero || !_isMovingOrPlanning)
        //{
        //    return;
        //}

        //if (_selectedUnit == null)
        //{
        //    return;
        //}

        if(_selectedUnit == null || !_isMovingOrPlanning)
        {
            return;
        }


        Tile newTile = GetTileAt(_currentPlannedMovePositon);
        if(newTile != null)
        {
            _selectedUnit.MoveToGridPosition(_currentPlannedMovePositon, newTile);
            _selectedUnit.transform.position = GetWorldPositionFromGrid(_currentPlannedMovePositon);
        }

        _selectedUnit.SetActedThisTrun();
        ResetMoveState();
        Debug.Log("移動を確定しました。");


        //_isMovingOrPlanning = false;
        //_selectedUnit = null;
        //ClearAllHighlights();
        //ClearMovableRangeDisplay();
        //ClearPathLine();

        TurnManager.Instance.CheckAllPlayerUnitActed();

        //処理を変更するためコメントアウト2025/07
        //if(_selectedUnit != null && _currentPlannedMovePositon != Vector2Int.zero)
        //{
        //    //ユニットのグリッド座標とワールド座標を更新
        //    //_selectedUnit.SetGridPosition(_currentPlannedMovePositon);
        //    //_selectedUnit.transform.position = GetWorldPositionFromGrid(_currentPlannedMovePositon);

            
        //    //ユニットの行動を完了状態にする
        //    _selectedUnit.SetActionTaken(true);

        //    //ハイライトをクリアし、選択状態を解除
        //    ClearAllHighlights();
        //    ClearMovableRangeDisplay();
        //    _selectedUnit.SetSelected(false);
        //    _selectedUnit.SetActionTaken(true);
        //    _selectedUnit = null;

        //    //状態をリセット
        //    _currentPlannedMovePositon = Vector2Int.zero;
        //    _originalUnitPositon = Vector2Int.zero;
        //    _isMovingOrPlanning = false;

        //    Debug.Log("ユニットの移動と状態クリアを完了しました");
        //}
    }

    /// <summary>
    /// 移動をキャンセルし、ユニットを元の位置に戻す
    /// </summary>
    private void CancelMove()
    {

        if (_selectedUnit != null)
        {
            //_selectedUnit.SetSelected(false);
            _selectedUnit.transform.position = MapManager.Instance.GetWorldPositionFromGrid(_originalUnitPositon);
        }


        //if (_canceled == false)
        //{
        //    if (_isMovingOrPlanning == true)
        //    {
        //        Tile newTile = GetTileAt(_originalUnitPositon);
        //        Debug.Log($"Trueのとき{_originalUnitPositon}");
        //        if (newTile != null)
        //        {
        //            _selectedUnit.MoveToGridPosition(_originalUnitPositon, newTile);
        //        }
        //    }
        //    else if (_isMovingOrPlanning == false)
        //    {
        //        //ユニットを元のグリッド座標とワールド座標に戻す
        //        Tile newTile = GetTileAt(_currentPlannedMovePositon);
        //        Debug.Log($"Falseのとき{_currentPlannedMovePositon}");
        //        if (newTile != null)
        //        {
        //            _selectedUnit.MoveToGridPosition(_currentPlannedMovePositon, newTile);
        //        }
        //    }
        //    _canceled = true;
        //}
        //else
        //{
        //    Tile newTile = GetTileAt(_originalUnitPositon);
        //    Debug.Log($"Trueのとき{_originalUnitPositon}");
        //    if (newTile != null)
        //    {
        //        _selectedUnit.MoveToGridPosition(_originalUnitPositon, newTile);
        //    }
        //    _canceled = false;
        //}


        //_selectedUnit.SetGridPosition(_originalUnitPositon);
        //_selectedUnit.transform.position = GetWorldPositionFromGrid(_originalUnitPositon);

        //_isMovingOrPlanning = false;

        //_selectedUnit = null;
        //_originalUnitPositon= Vector2Int.zero;
        //_currentPlannedMovePositon = Vector2Int.zero;
        //ClearAllHighlights();
        //ClearMovableRangeDisplay();
        //ClearPathLine();

        ResetMoveState();
        Debug.Log("キャンセルされたので元の場所に戻します");


        //if (_selectedUnit != null && _currentPlannedMovePositon != Vector2Int.zero)
        //{
        //    //ユニットを元のグリッド座標とワールド座標に戻す
        //    _selectedUnit.SetGridPosition(_originalUnitPositon);
        //    _selectedUnit.transform.position = GetWorldPositionFromGrid(_originalUnitPositon);

        //    //ハイライトをクリアし、選択状態を解除
        //    ClearAllHighlights();
        //    ClearMovableRangeDisplay();
        //    _selectedUnit = null;

        //    //状態をリセット
        //    _currentPlannedMovePositon = Vector2Int.zero;
        //    _originalUnitPositon= Vector2Int.zero;
        //    _isMovingOrPlanning= false;

        //    Debug.Log("ユニットの移動がキャンセルされたので元の場所に戻します");
        //}
    }



    /// <summary>
    /// 移動状態をリセットするヘルパーメソッド
    /// </summary>
    private void ResetMoveState()
    {
        if(_selectedUnit != null)
        {
            _selectedUnit.SetSelected(false);
        }
        _selectedUnit = null;
        _currentPlannedMovePositon = Vector2Int.zero;
        _originalUnitPositon = Vector2Int.zero;
        _isMovingOrPlanning = false;
        _isConfirmingMove = false;



        ClearAllHighlights();
        //////ClearMovableRangeDisplay();
        ClearPathLine();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_mapSequence.Length > 0)
        {
            Initialize();
            InitializeCamera();
            //TurnManager.Instance.InitializeTurnManager();
            //GenerateMap(_mapSequence[_currentMapIndex]);
            //GenerateMap(_mapSequence[0]);
            //PlacePlayerUnitAtInitialPostiton();
            //PlaceEnemyUnitAtInitialPostiton();
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

        HandleCameraInput();
        //MoveCameraToTarget();


        //プレイヤーターン中のみ入力を受け付ける
        if (TurnManager.Instance != null && TurnManager.Instance.CurrnetTurnState != TurnState.PlayerTurn)
        {
            return;
        }

        //マウス操作を検知（左クリック2025 / 06）
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        if(_selectedUnit != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CancelMove();
            }
        }


        //if(_selectedUnit != null && _currentPlannedMovePositon != Vector2Int.zero)
        // 移動確定待ち状態の場合は、入力処理を制限する
        if (_isConfirmingMove)
        {
            if (_selectedUnit != null)
            {
                //仮：スペースキーで移動確定
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ConfirmMove();
                }
                //仮：Qキーでキャンセル
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    CancelMove();
                }
                //現在は以上の入力処理以外は受け付けない2025/07
                return;
            }
            
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
