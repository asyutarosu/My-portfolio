using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField]private MapManager _mapManager;

    [SerializeField]private GameManager _gameManager;

    [SerializeField] private float _enemyTurnDelay = 1.0f;//敵のターン処理開始までの遅延時間

    //現在のターン状態
    public TurnState CurrnetTurnState { get; private set; }
    public int CurrentTurnNumber { get; private set; } = 1;

    //TurnManagerで管理するのユニットリスト
    private List<Unit> _allUnits;
    private List<PlayerUnit> _playerUnits;
    //private List<Unit> _playerUnits;
    private List<EnemyUnit> _enemyUnits;

    //現在行動中の敵ユニットのインデックス
    private int _currentEnemyUnitIndex = 0;


    //天地鳴動メインシステム：地形
    [SerializeField] private float _terrainChangeChance = 1.0f;//地形変化が発生する確率（仮実装は100％）
    [SerializeField]private float _tentimeidouChance = 1.0f;//大規模な地形変化が発生する確率（仮実装は100％）


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    //初期化処理
    public void InitializeTurnManager()
    {
        CurrnetTurnState = TurnState.PreGame;
        CurrentTurnNumber = 1;

        //ユニットリストを取得
        _playerUnits = MapManager.Instance.GetAllPlayerUnits();
        _enemyUnits = MapManager.Instance.GetAllEnemyUnits();
        _allUnits = MapManager.Instance.GetAllUnits();


        //確認用
        if (_playerUnits == null)
        {
            Debug.LogError("TurnManager: _playerUnitsがMapManagerから取得できませんでした！");
        }
        if (_enemyUnits == null)
        {
            Debug.LogError("TurnManager: _enemyUnitsがMapManagerから取得できませんでした！");
        }
        Debug.Log($"TurnManager: プレイヤーユニット数: {_playerUnits?.Count ?? 0}, 敵ユニット数: {_enemyUnits?.Count ?? 0}");

        Debug.LogWarning($"マップに存在する全てのユニットの数：{_allUnits.Count}");
        Debug.LogWarning($"マップに存在する全てのプレイヤーユニットの数：{_playerUnits.Count}");
        Debug.LogWarning($"マップに存在する全ての敵ユニットの数：{_enemyUnits.Count}");

        Debug.Log("TurnManager:ゲーム初期化完了");

        StartPlayerTurn();//プレイヤーターンから開始
    }

    /// <summary>
    /// 全てのユニットのリストを更新する
    /// </summary>
    public void UpdateUnitList()
    {
        _allUnits = FindObjectsOfType<Unit>().ToList();
    }


    //全てユニットを取得する
    public void GetAllUnitsTurnManager()
    {
        //シーン内のすべてのUnitコンポーネントを検索
        _allUnits = FindObjectsOfType<Unit>().ToList();
        Debug.LogWarning($"マップに存在する全てのユニットの数：{_allUnits.Count}");
    }

    //プレイヤーユニットをリストに追加する公開メソッド
    public void AddPlayerUnit(PlayerUnit unit)
    {
        if (unit != null && !_playerUnits.Contains(unit))
        {
            _playerUnits.Add(unit);
            Debug.Log($"ユニットが_playerUnitsに追加されました。現在のユニット数: {_playerUnits.Count}");
        }
    }

    //ユニットをリストから削除する
    public void RemoveSpecificUnit(Unit unitToRemove)
    {
        //対象がPlayerUnitのとき
        if(unitToRemove.Faction == FactionType.Player)
        {
            //UnitをPlayerUnitにキャストしてUnit型へ
            PlayerUnit playerUnitToRemove = unitToRemove as PlayerUnit;

            //キャストが成功し、かつプレイヤーユニットのリストに存在するか確認
            if (playerUnitToRemove != null && _playerUnits.Contains(playerUnitToRemove))
            {
                //プレイヤーユニットのリストから削除
                _playerUnits.Remove(playerUnitToRemove);
                Debug.Log($"プレイヤーユニット {playerUnitToRemove.name} をリストから削除しました。");
            }
        }

        //対象がEnemyUnitのとき
        if (unitToRemove.Faction == FactionType.Enemy)
        {
            //UnitをEnemyUnitにキャストしてUnit型へ
            EnemyUnit enemyUnitToRemove = unitToRemove as EnemyUnit;

            //キャストが成功し、かつ敵ユニットのリストに存在するか確認
            if (enemyUnitToRemove != null && _enemyUnits.Contains(enemyUnitToRemove))
            {
                //敵ユニットのリストから削除
                _enemyUnits.Remove(enemyUnitToRemove);
                Debug.Log($"プレイヤーユニット {enemyUnitToRemove.name} をリストから削除しました。");
            }
        }
        

        if (_allUnits.Contains(unitToRemove))
        {
            _allUnits.Remove(unitToRemove);
            Debug.Log($"ユニット {unitToRemove.name} をリストから削除しました。");
        }
        else
        {
            Debug.LogWarning("削除しようとしたユニットはリスト内に見つかりませんでした。");
        }
    }

    /// <summary>
    /// プレイヤーのターンを開始する
    /// </summary>
    public void StartPlayerTurn()
    {

        CurrnetTurnState = TurnState.PlayerTurn;
        Debug.Log("プレイヤーターン開始");
        Debug.LogWarning($"プレイヤーの数：{_playerUnits.Count}");

        //全てのプレイヤーユニットの行動状態をリセット
        foreach (Unit unit in _allUnits)
        {
            if(unit.Faction == FactionType.Player)
            {
                unit.ResetAction();
            }

            if(unit.Faction == FactionType.Enemy)
            {
                unit.ResetAction();
            }
        }

        foreach(var unit in _playerUnits)
        {
            unit.SetActionTaken(false);
        }

        //Ui更新などを追加2025/07
    }

    /// <summary>
    /// プレイヤーのターン終了する
    /// </summary>
    public void EndPlayerTurn()
    {
        if (CurrnetTurnState != TurnState.PlayerTurn)
        {
            Debug.LogWarning("TurnManager: プレイヤーターン以外でEndPlayerTurnが呼び出されました。");
            return;
        }
        //全てのプレイヤーユニットが行動済み、またはプレイヤー操作による終了のチェックを追加2025/07

        //ターン終了時に表示されているハイライトをクリアする
        MapManager.Instance.ClearAllHighlights();


        //仮：ターンシステムの確認のためターン終了のみ
        Debug.Log("プレイヤーターン終了");
        StartEnemyTurn();
    }

    /// <summary>
    /// 全てのプレイヤーユニットが行動済みかチェックし、行動済みならターンを終了する
    /// </summary>
    public void CheckAllPlayerUnitActed()
    {
        //プレイヤーユニットが全滅の場合
        if(_playerUnits == null || _playerUnits.Count == 0)
        {
            Debug.Log("プレイヤーユニットがいません。ゲームオーバー。");
            //仮として敵ターンへ移行（ゲームオーバー関連未実装2025/07）
            EndPlayerTurn();
            return;
        }

        bool allActed = true;
        foreach(PlayerUnit playerUnit in _playerUnits)
        {
            if (!playerUnit.HasActedThisTurn)
            {
                allActed = false;
                break;
            }
        }

        if (allActed)
        {
            Debug.LogWarning("全てのプレイヤーユニットが行動を完了しました。敵ターンへ移行します。");
            Debug.LogWarning($"プレイヤーの数{_playerUnits.Count}");
            EndPlayerTurn();
        }
    }

    /// <summary>
    /// 敵のターンを開始する
    /// </summary>
    private void StartEnemyTurn()
    {
        CurrnetTurnState = TurnState.EnemyTurn;
        Debug.Log("敵ターン開始");

        _currentEnemyUnitIndex = 0;//敵ユニットの行動順は固定2025/07

        //全ての敵ユニットの行動状態をリセット
        foreach (Unit unit in _allUnits)
        {
            if (unit.Faction == FactionType.Enemy)
            {
                unit.ResetAction();
            }
        }

        foreach(var unit in _enemyUnits)
        {
            unit.SetActionTaken(false);
        }

        //敵の行動ロジックを遅延させて開始
        StartCoroutine(EnemyTurnRoutine());
    }

    /// <summary>
    /// 敵ターンのコルーチン処理
    /// </summary>
    private IEnumerator EnemyTurnRoutine()
    {
        Debug.Log("敵ターン開始(コルーチン処理)");

        yield return new WaitForSeconds(_enemyTurnDelay);

        while (_currentEnemyUnitIndex < _enemyUnits.Count)
        {
            EnemyUnit currentEnemy = _enemyUnits[_currentEnemyUnitIndex];

            //倒された敵ユニットはスキップ
            //if(currentEnemy == null || currentEnemy.CurrentHP <= 0)
            //{
            //    continue;
            //}

            //行動済みの敵ユニットはスキップ
            if (currentEnemy.HasActedThisTurn)
            {
                _currentEnemyUnitIndex++;
                continue;
            }

            Debug.Log($"敵ユニット {currentEnemy.name} が行動します。");
            yield return StartCoroutine(currentEnemy.PerformAIAction()); // 敵AIの行動を実行
            
            // 行動が完了したら、次の敵へ
            _currentEnemyUnitIndex++;
            yield return new WaitForSeconds(0.5f); // 各敵ユニットの行動間に少し間を置く

        }


        ////敵ＡＩの行動ロジックを追加する2025/07
        //Debug.Log("敵ユニットが行動中(AI起動)");
        //foreach(Unit unit in _allUnit)
        //{
        //    if(unit.Faction == FactionType.Enemy)
        //    {
        //        //敵ユニットに行動させるメソッドを呼び出しておく
        //        unit.SetActionTaken(false);
        //    }
        //}

        //Debug.Log("敵ユニットが移動中");

        ////敵ユニットのみをフィルタリング
        //List<EnemyUnit> enemyUnits = _allUnit.OfType<EnemyUnit>().ToList();

        //foreach (EnemyUnit enemyUnit in enemyUnits)
        //{
        //    //敵ユニットのAI行動を実行し、完了を待つ
        //    yield return StartCoroutine(enemyUnit.PerformAIAction());
        //    yield return new WaitForSeconds(0.2f);//各敵ユニットの行動間に短い間隔
        //}

        //yield return new WaitForSeconds(_enemyTurnDelay);

        EndEnemyTurn();
    }

    /// <summary>
    /// 敵のターンを終了する
    /// </summary>
    private void EndEnemyTurn()
    {
        Debug.Log("敵ターン終了");
        CurrentTurnNumber++;//ターン経過（プレイヤーターン→敵ターン終了で１サイクル）

        if(Random.value <= _terrainChangeChance)
        {
            Debug.LogWarning("地形変化が発生しました");
            //TriggerTerrainChangeEvent();
        }

        //確認用
        _mapManager.ChangeSpecificTerrain(TerrainType.Mountain, TerrainType.River, 4);
        _mapManager.ChangeAroundTerrain(TerrainType.Forest, TerrainType.Desert);

        //次のプレイヤーターンを開始
        StartPlayerTurn();
    }

    //ゲームオーバー処理
    public void SetGameOver()
    {
        CurrnetTurnState = TurnState.GameOver;
        Debug.Log("--- ゲームオーバー ---");
    }

    //その他のターン状態への遷移メソッド（未定2025/07）
    public void SetTurnState(TurnState newState)
    {
        CurrnetTurnState = newState;
    }


    /// <summary>
    /// 地形変更イベントをトリガーする
    /// </summary>
    private void TriggerTerrainChangeEvent()
    {
        TerrainType newTerrainType = GetRandomTerrainType();

        int numTilesToChange = 0;
        if(Random.value < _tentimeidouChance)
        {
            //10マス以上の大規模な地形変化
            numTilesToChange = Random.Range(10, _mapManager.GridSize.x * _mapManager.GridSize.y);
            Debug.Log($"大規模な地形変化が発生{ numTilesToChange}マスを{ newTerrainType}に変化させます");
            //Debug.LogWarning($"{ _mapManager.GridSize}");
        }
        else
        {
            //4～7マスの小規模な地形変化
            numTilesToChange = Random.Range(4, 8);
            Debug.Log($"小規模な地形変更イベントが発生{numTilesToChange}マスを {newTerrainType} に変化させます");
        }

        //変更するタイルのグリッド座標をランダムに選ぶ
        List<Vector2Int> tileToChange = GetRandomGridPosition(numTilesToChange);

        _mapManager.ChangeMultipleTerrains(tileToChange,newTerrainType);
    }


    /// <summary>
    /// 複数のランダムなグリッド座標を取得するヘルパーメソッド
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private List<Vector2Int> GetRandomGridPosition(int count)
    {
        List<Vector2Int> selectedTiles = new List<Vector2Int>();
        List<Vector2Int> allTiles = MapManager.Instance.GetAllGridPosition();

        //
        while(selectedTiles.Count < count && allTiles.Count > 0)
        {
            int randomIndex = Random.Range(0,allTiles.Count);
            Vector2Int randomPos = allTiles[randomIndex];

            selectedTiles.Add(randomPos);
            allTiles.RemoveAt(randomIndex);
        }
        return selectedTiles;
    }

    //ランダムな地形タイプを取得するヘルパーメソッド
    private TerrainType GetRandomTerrainType()
    {
        TerrainType[] types = (TerrainType[])System.Enum.GetValues(typeof(TerrainType));
        //デバッグ用
        return types[4];
        //return types[Random.Range(0, types.Length)];
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //初期化処理としてシーン上の全てのユニットを検索してリストに追加
        _allUnits = FindObjectsOfType<Unit>().ToList();

        //最初のターン開始
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_gameManager.CurrentBattlePhase == BattlePhase.BattleMain)
        {
            //仮：プレイヤーがターン終了を明示的に行うための処理2025/07
            if (CurrnetTurnState == TurnState.PlayerTurn && Input.GetKeyDown(KeyCode.E))
            {
                EndPlayerTurn();
            }

            //デバッグ用：敵ターンの終了させる処理
            if (CurrnetTurnState == TurnState.EnemyTurn && Input.GetKeyDown(KeyCode.K))
            {
                EndEnemyTurn();
            }
        }
    }
}
