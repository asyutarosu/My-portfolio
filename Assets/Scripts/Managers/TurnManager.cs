using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] private float _enemyTurnDelay = 1.0f;//敵のターン処理開始までの遅延時間

    //現在のターン状態
    public TurnState CurrnetTurnState { get; private set; }
    public int CurrentTurnNumber { get; private set; } = 1;

    //TurnManagerで管理するのユニットリスト
    private List<Unit> _allUnits;
    private List<PlayerUnit> _playerUnits;
    private List<EnemyUnit> _enemyUnits;

    //現在行動中の敵ユニットのインデックス
    private int _currentEnemyUnitIndex = 0;

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
        _allUnits = MapManager.Instance.GetAllUnits();
        _playerUnits = MapManager.Instance.GetAllPlayerUnits();
        _enemyUnits = MapManager.Instance.GetAllEnemyUnits();

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

    /// <summary>
    /// プレイヤーのターンを開始する
    /// </summary>
    public void StartPlayerTurn()
    {
        CurrnetTurnState = TurnState.PlayerTurn;
        Debug.Log("プレイヤーターン開始");

        //全てのプレイヤーユニットの行動状態をリセット
        foreach(Unit unit in _allUnits)
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
            Debug.Log("全てのプレイヤーユニットが行動を完了しました。敵ターンへ移行します。");
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
        //仮：プレイヤーがターン終了を明示的に行うための処理2025/07
        if(CurrnetTurnState == TurnState.PlayerTurn && Input.GetKeyDown(KeyCode.E))
        {
            EndPlayerTurn();
        }

        //デバッグ用：敵ターンの終了させる処理
        if(CurrnetTurnState == TurnState.EnemyTurn && Input.GetKeyDown(KeyCode.K))
        {
            EndEnemyTurn();
        } 
    }
}
