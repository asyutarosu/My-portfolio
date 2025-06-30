using UnityEngine;
using System.Collections.Generic;
using System.Linq;//LINQを使用するため

/// <summary>
/// 戦闘システムを統括するシングルトンクラス
/// </summary>
public partial class BattleManager : MonoBehaviour
{
    private static BattleManager _instanse;
    public static BattleManager Instance
    { 
        get 
        { 
            if(_instanse == null)
            {
                _instanse = FindAnyObjectByType<BattleManager>();
                if(_instanse == null)
                {
                    GameObject singletonObject = new GameObject("BatteleManager");
                    _instanse = singletonObject.AddComponent<BattleManager>();
                }
            }
            return _instanse; 
        } 
    }

    [SerializeField] private int _cuurrentTurn;//現在のターン数
    public int CurrentTurn => _cuurrentTurn;
    [SerializeField] private Unit _activeUnit;//現在行動中のユニット
    [SerializeField] private List<Unit> _allUnits;//マップ上の全てのユニットのリスト

    private void Awake()
    {
        if(_instanse != null && _instanse != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instanse = this;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    /// <summary>
    /// 戦闘を開始する
    /// </summary>
    public void StartBattle()
    {
        Debug.Log("BattleManager:戦闘を開始します");
        _cuurrentTurn = 1;

        //最初期段階として手動で仮のユニットを配置
        _allUnits = new List<Unit>();
        //仮のプレイヤーユニット
        GameObject playerUnitObj = new GameObject("PlayerUnit");
        playerUnitObj.transform.position = new Vector3(0, 0, 0);
        Unit playerUnit = playerUnitObj.AddComponent<Unit>();
        playerUnit.Initialize(new UnitData { UnitId = "PLAYER001", UnitName = "勇者", BaseHP = 10, BaseMovement = 5 });
        playerUnit.UpdatePosition(new Vector2Int(0, 0));
        _allUnits.Add(playerUnit);
        //仮の敵ユニット
        GameObject enemyUnitObj = new GameObject("EnemyUnit");
        enemyUnitObj.transform.position = new Vector3(5, 5, 0);
        Unit enemyUnit = enemyUnitObj.AddComponent<Unit>();
        enemyUnit.Initialize(new UnitData { UnitId = "ENEMY001", UnitName = "一般兵士", BaseHP = 1, BaseMovement = 3 });
        enemyUnit.UpdatePosition(new Vector2Int(3, 3));
        _allUnits.Add(enemyUnit);

        StartTurn();//最初のターン開始
    }

    /// <summary>
    /// ターンを開始する
    /// </summary>
    public void StartTurn()
    {
        Debug.Log($"BattleManager:ターン{CurrentTurn}を開始します");
        //全ユニットの行動済みフラグをリセットする
        foreach(var unit in _allUnits)
        {
            unit.ResetAction();
            unit.ResetMovementPoints();//移動ポイントをリセット
        }

        //基本的にプレイヤー側から行動
        _activeUnit = _allUnits.FirstOrDefault(u => u.UnitId == "PLAYER001");
        if(_activeUnit == null)
        {
            //プレイヤーユニットがいなければ敵ユニットなど次の行動可能なユニットを設定
            _activeUnit = _allUnits.FirstOrDefault(u => !u.HasActedThisTurn);
        }

        if(_activeUnit != null)
        {
            Debug.Log($"BattleManager:{_activeUnit.UnitId}:{_activeUnit.UnitName}の行動開始");
        }
        else
        {
            Debug.Log("BattleManager:行動可能なユニットがいません");
            EndTurn();
        }
        
    }

    /// <summary>
    /// ターンを終了し、次のターンの処理を開始する
    /// </summary>
    public void EndTurn()
    {
        Debug.Log($"BattleManager:ターン{CurrentTurn}を終了します");
        _cuurrentTurn++;

        //勝利・敗北条件のチェック
        if (CheckWinConditon())
        {
            Debug.Log("BattleManager:勝利条件達成");
            GameManager.Instance.ChangePhase(GamePhase.StageClear);
            return;
        }
        if (CheckLoseCondition())
        {
            Debug.Log("BattleManager:敗北条件達成");
            GameManager.Instance.ChangePhase(GamePhase.GameOver);
            return;
        }

        //不確定要素イベント発生の判定

        StartTurn();
    }

    ///<summary>
    /// ユニットをターゲット位置へ移動させる
    /// </summary>
    /// <param name="unit">移動させるユニット</param>
    /// <param name="targetPosition">目標グリッド座標</param>
    public void MoveUnit(Unit unit, Vector2Int targetPosition)
    {
        Debug.Log($"BattleManager:{unit.UnitId}:{unit.UnitName}を{targetPosition}へ移動します");
        //ダイクストラ法を使用して経路とコストを計算する
    }


    /// <summary>
    /// 勝利条件をチェックする
    /// 敵総大将の撃破または、特定拠点の制圧（仮の勝利条件：敵の全滅）
    /// </summary>
    /// <return>勝利条件を満たしていれば　true</return>
    private bool CheckWinConditon()
    {
        //最初期段階のため敵の全滅を勝利条件とする
        return _allUnits.Count(unit => unit.UnitId.StartsWith("ENEMY") && unit.CurrentHP > 0) == 0;
    }

    /// <summary>
    /// 敗北条件をチェックする
    /// 自軍の総大将の撃破
    /// </summary>
    /// <return>敗北条件を満たしていれば　ture</return>
    private bool CheckLoseCondition()
    {
        return _allUnits.Count(unit => unit.UnitId.StartsWith("PLAYER001") && unit.CurrentHP > 0) == 0;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
