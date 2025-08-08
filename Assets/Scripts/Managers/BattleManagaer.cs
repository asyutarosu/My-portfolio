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

    /// <summary>
    /// 仕様変更のため将棋ベースの戦闘処理を実装2025/07
    /// </summary>
    /// <param name="attacker">攻撃側のユニット</param>
    /// <param name="target">防衛側のユニット</param>
    public void ResolveBattle_ShogiBase(Unit attacker, Unit target)
    {

        HashSet<Vector2Int> attackableTiles = CalculateUnitMoveToAttackPositions(target.GetCurrentGridPostion(), attacker);



        Debug.LogWarning($"{attacker.gameObject.name}が{target.gameObject.name}に攻撃！");
        Debug.LogWarning($"{attacker.GetCurrentGridPostion()}");

        if (attackableTiles.Contains(attacker.GetCurrentGridPostion()))
        {
            Debug.LogWarning($"攻撃許可が認証");
            target.Die();
        }
        // 攻撃側が先に攻撃したため、相手を倒す
        //target.Die();
    }

    /// <summary>
    /// ユニット間の戦闘処理（ステータスベース：仕様変更に伴い実装の見送り2025/07）
    /// </summary>
    /// <param name="attacker">攻撃側のユニット</param>
    /// <param name="target">防衛側のユニット</param>
    public void ResolveBattleSystem(Unit attacker, Unit target)
    {

    }





    /// <summary>
    /// 指定されたユニットの位置から見て、攻撃するユニットが攻撃可能なマスを計算する
    /// </summary>
    /// <param name="targetPos">ターゲットとなるユニットのグリッド座標</param>
    /// <returns>攻撃するユニットが移動すべき攻撃可能位置のリスト（HashSetで重複なし）</returns>
    private HashSet<Vector2Int> CalculateUnitMoveToAttackPositions(Vector2Int targetPos,Unit unit)
    {
        HashSet<Vector2Int> potentialUnitAttackMovePositions = new HashSet<Vector2Int>();


        int _maxAttackRange = unit._maxAttackRange;
        int _minAttackRange = unit._minAttackRange;

        //プレイヤーの位置から見て、_minAttackRange から _maxAttackRange の範囲にあるマスを探す
        //敵ユニットがプレイヤーを攻撃するために移動できる候補のマスを逆算して計算する
        for (int x = -_maxAttackRange; x <= _maxAttackRange; x++)
        {
            for (int y = -_maxAttackRange; y <= _maxAttackRange; y++)
            {
                //プレイヤー位置からの相対座標で、敵ユニットが位置する可能性のあるマスを計算
                Vector2Int potentialEnemyPos = targetPos + new Vector2Int(x, y);

                //敵ユニットが potentialEnemyPos に移動した場合、プレイヤー (playerPos) との距離を計算
                int distance = Mathf.Abs(potentialEnemyPos.x - targetPos.x) +
                               Mathf.Abs(potentialEnemyPos.y - targetPos.y); // マンハッタン距離

                //敵ユニットの攻撃射程内かチェック
                if (distance >= _minAttackRange && distance <= _maxAttackRange)
                {
                    // この位置に敵ユニットが移動できれば、プレイヤーを攻撃可能
                    // ただし、MapManager.Instance.IsValidGridPosition のチェックは必須
                    if (MapManager.Instance.IsValidGridPosition(potentialEnemyPos))
                    {
                        // このマスは、敵ユニットが移動してプレイヤーを攻撃できる有効な候補である
                        potentialUnitAttackMovePositions.Add(potentialEnemyPos);
                    }
                }
            }
        }

        //確認用
        //foreach(Vector2Int ShowAttackPos in potentialUnitAttackMovePositions)
        //{
        //    Debug.LogWarning($"{ShowAttackPos}");

        //}

        Debug.LogWarning($"{potentialUnitAttackMovePositions.Count}");

        return potentialUnitAttackMovePositions;
    }







    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="attackedUnit"></param>
    ///// <param name="targetUnit"></param>
    //public void CandidateAttackedPos(Unit attackedUnit, Unit targetUnit,FactionType fation)
    //{
    //    Vector2Int CurrentGridPosition = Vector2Int.zero;
    //    Vector2Int targetPos = CurrentGridPosition;
    //    Vector2Int originalCurrentGridPosition = CurrentGridPosition;

    //    List<PlayerUnit> playerUnits = MapManager.Instance.GetAllPlayerUnits();
    //    Dictionary<Vector2Int, PathNodes> reachableNodes = DijkstraPathfinder.FindReachableNodes(originalCurrentGridPosition, this);

    //    int minCostToAttackPos = int.MaxValue; // 敵から攻撃可能位置までのコスト


    //    foreach (PlayerUnit player in playerUnits)
    //    {
    //        Debug.Log($"  ターゲット候補のプレイヤー: {player.name} 位置: {player.GetCurrentGridPostion()}");

    //        HashSet<Vector2Int> potentialEnemyMoveToAttackPositions = CalculateUnitMoveToAttackPositions(player.GetCurrentGridPostion(),player);

    //        Debug.Log($"    プレイヤー({player.name})を攻撃可能なマス候補数 (敵が移動すべき位置): {potentialEnemyMoveToAttackPositions.Count}");

    //        // 敵が移動可能なマスの中から、このプレイヤーを攻撃できるマスを探す
    //        foreach (Vector2Int attackPosCandidate in potentialEnemyMoveToAttackPositions)
    //        {
    //            // 候補の攻撃位置が、敵の移動可能範囲内にあり、かつ空きマスであるかチェック
    //            if (reachableNodes.ContainsKey(attackPosCandidate) && !MapManager.Instance.IsTileOccupiedForStooping(attackPosCandidate, this))
    //            {
    //                int costToMoveToAttackPos = reachableNodes[attackPosCandidate].Cost;

    //                int currentManhattanDistanceToPlayer =
    //                    Mathf.Abs(attackPosCandidate.x - player.GetCurrentGridPostion().x) + Mathf.Abs(attackPosCandidate.y - player.GetCurrentGridPostion().y);

    //                Debug.Log($"    [有効な候補] 攻撃可能位置: {attackPosCandidate} (移動コスト: {costToMoveToAttackPos}, プレイヤーへの距離: {currentManhattanDistanceToPlayer})");

    //                // 現在見つかっている最小コストより小さい場合、更新
    //                if (costToMoveToAttackPos < minCostToAttackPos)
    //                {
    //                    //minCostToAttackPos = costToMoveToAttackPos;
    //                    //bestMoveTargetPos = attackPosCandidate;
    //                    //targetedPlayer = player; // このプレイヤーをターゲットにする
    //                    //minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
    //                }
    //                //移動コストが同じであれば、プレイヤーに近い（マンハッタン距離が短い）ものを優先
    //                else if (costToMoveToAttackPos == minCostToAttackPos)
    //                {
    //                    if (currentManhattanDistanceToPlayer < minManhattanDistanceToPlayer)
    //                    {
    //                        //minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
    //                        //bestMoveTargetPos = attackPosCandidate;
    //                        //targetedPlayer = player;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                //確認用
    //                Debug.Log($"    [無効な候補] 攻撃可能位置: {attackPosCandidate} (理由: 移動範囲外または占有済み)");
    //            }
    //        }
    //    }
    //}











    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    /// <summary>
    /// 戦闘を開始する
    /// </summary>
    //public void StartBattle()
    //{
    //    Debug.Log("BattleManager:戦闘を開始します");
    //    _cuurrentTurn = 1;

    //    //最初期段階として手動で仮のユニットを配置
    //    _allUnits = new List<Unit>();
    //    //仮のプレイヤーユニット
    //    GameObject playerUnitObj = new GameObject("PlayerUnit");
    //    playerUnitObj.transform.position = new Vector3(0, 0, 0);
    //    Unit playerUnit = playerUnitObj.AddComponent<Unit>();
    //    playerUnit.Initialize(new UnitData { UnitId = "PLAYER001", UnitName = "勇者", BaseHP = 10, BaseMovement = 5 });
    //    playerUnit.UpdatePosition(new Vector2Int(0, 0));
    //    _allUnits.Add(playerUnit);
    //    //仮の敵ユニット
    //    GameObject enemyUnitObj = new GameObject("EnemyUnit");
    //    enemyUnitObj.transform.position = new Vector3(5, 5, 0);
    //    Unit enemyUnit = enemyUnitObj.AddComponent<Unit>();
    //    enemyUnit.Initialize(new UnitData { UnitId = "ENEMY001", UnitName = "一般兵士", BaseHP = 1, BaseMovement = 3 });
    //    enemyUnit.UpdatePosition(new Vector2Int(3, 3));
    //    _allUnits.Add(enemyUnit);

    //    StartTurn();//最初のターン開始
    //}

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
            GameManager.Instance.ChangeState(GameState.StageClear);
            return;
        }
        if (CheckLoseCondition())
        {
            Debug.Log("BattleManager:敗北条件達成");
            GameManager.Instance.ChangeState(GameState.GameOver);
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
