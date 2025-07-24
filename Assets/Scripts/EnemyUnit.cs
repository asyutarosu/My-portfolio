using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class EnemyUnit : Unit
{

    //敵AIの行動タイプ
    public enum EnemyAIType
    {
        Aggreeive,  //積極的にプレイヤーを追う
        Stationary, //特定のマスに侵入されるまで動かない
        Patrol      //パトロール型：実装予定
    }

    [SerializeField] private EnemyAIType _aiType = EnemyAIType.Aggreeive;//デフォルトのAIタイプ

    //攻撃範囲
    [SerializeField] private int _minAttackRange = 1;//最小攻撃射程
    [SerializeField] private int _maxAttackRange = 2;//最大攻撃射程
    public override void Initialize(UnitData data)
    {
        //_currentPosition = initialGridPos;
        //_unitName = name;
        //Debug.Log($"PlayerUnit'{_unitName}'initialized at grid:{_currentPosition}");

        base.Initialize(data);

        _factionType = FactionType.Enemy;
        Debug.Log($"Enemy Unit '{UnitName}' (Type: {Type}, Faction: {Faction}) initialized at grid: {CurrentGridPosition}");
    }

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 敵AIが行動を実行する
    /// </summary>
    public IEnumerator PerformAIAction()
    {
        //既に行動済みならスキップ
        if (HasActedThisTurn)
        {
            Debug.Log($"{name}は既にこのターン行動済みです");
            yield break;
        }

        Debug.Log($"{name}が行動を開始します");

        //敵AIのタイプに応じて行動を分岐
        switch (_aiType)
        {
            case EnemyAIType.Aggreeive: 
                Debug.Log($"{name}が突撃します");
                yield return StartCoroutine(HandleAggressiveAI());
                break;
            case EnemyAIType.Stationary: 
                Debug.Log($"{name}は待機しています");
                break;
            //他のAIを追加
            default: 
                Debug.Log($"{name}のAIタイプが未定義です:{_aiType}");
                break;
        }

        //行動が完了したら行動済みフラグをセット
        SetActionTaken(true);
        Debug.Log($"{name}の行動が完了しました");
    }


    /// <summary>
    /// 積極的なAIの行動ロジック
    /// ・マップ上で行動する敵ユニットから最も近いかつ移動コストが最小の位置にいるプレイヤーユニットに向かって移動する
    /// ・敵ユニットの攻撃可能範囲内にプレイヤーユニットがいなければ行動しない
    /// </summary>
    private IEnumerator HandleAggressiveAI()
    {
        //プレイヤーユニットの検索
        List<PlayerUnit> playerUnits = MapManager.Instance.GetAllPlayerUnits();

        if(playerUnits == null || playerUnits.Count == 0)
        {
            Debug.Log($"{name}:プレイヤーユニットが見つかりません");
            yield break;
        }

        //現在の攻撃範囲内にプレイヤーユニットがいるかチェック
        //現在位置からの攻撃範囲を計算
        HashSet<Vector2Int> currentAttackableTiles = CalculateAttackRange(GetCurrentGridPostion());
        bool playerInAttackRange = false;
        PlayerUnit targetPlayerInAttackRange = null;

        foreach(PlayerUnit player in playerUnits)
        {
            if (currentAttackableTiles.Contains(player.GetCurrentGridPostion()))
            {
                playerInAttackRange = true;
                targetPlayerInAttackRange = player;
                break;//誰か1人でもいれば良い
            }
        }

        if (playerInAttackRange)
        {
            Debug.Log($"{name}:プレイヤーユニット({targetPlayerInAttackRange.name})が攻撃範囲にいます。攻撃します");
            //攻撃ロジックを追加
            yield return new WaitForSeconds(0.5f);
            yield break;//攻撃したら移動しない
        }

        Debug.Log($"{name}: 攻撃範囲内にプレイヤーユニットがいません。最も近いプレイヤーユニットに向かって移動します。");

        //確認用
        Debug.Log($"{name}: 現在の移動力: {CurrentMovementPoints}"); // !!! ここで実際の移動力を確認 !!!


        //最も近いかつ移動コストが最小の位置にいるプレイヤーユニットへの移動目標を決定
        Vector2Int bestMoveTargetPos = Vector2Int.zero;
        List<Vector2Int> bestPath = null;
        int minCostToAttackPos = int.MaxValue; // 敵から攻撃可能位置までのコスト
        PlayerUnit targetedPlayer = null;
        int minManhattanDistanceToPlayer = int.MaxValue;//移動目標位置からプレイヤーまでのマンハッタン距離


        Dictionary<Vector2Int, PathNodes> reachableNodes = DijkstraPathfinder.FindReachableNodes(GetCurrentGridPostion(), this);

        //確認用
        Debug.Log($"{name}: DijkstraPathfinderが計算した到達可能マス数: {reachableNodes.Count}"); // 到達可能なマスの総数

        // 到達可能なマスとそのコストを全てログ出力
        Debug.Log($"{name}: --- 到達可能マス詳細 ---");
        foreach (var entry in reachableNodes.OrderBy(e => e.Value.Cost)) // コスト順にソートして見やすく
        {
            Debug.Log($"  Reachable Node: {entry.Key} (Cost: {entry.Value.Cost})");
        }
        Debug.Log($"{name}: ---------------------");

        // 全てのプレイヤーユニットに対して、最適な移動先を探す
        foreach (PlayerUnit player in playerUnits)
        {
            //確認用
            Debug.Log($"  ターゲット候補のプレイヤー: {player.name} 位置: {player.GetCurrentGridPostion()}"); // ターゲット候補のプレイヤー情報をログ出力


            // そのプレイヤーを攻撃できるマス（敵が移動すべきターゲットマス）を計算
            HashSet<Vector2Int> potentialAttackPositions = CalculateAttackRange(player.GetCurrentGridPostion());

            Debug.Log($"    プレイヤー({player.name})を攻撃可能なマス候補数 (攻撃範囲内): {potentialAttackPositions.Count}");


            // 敵が移動可能なマスの中から、このプレイヤーを攻撃できるマスを探す
            foreach (Vector2Int attackPosCandidate in potentialAttackPositions)
            {
                // 候補の攻撃位置が、敵の移動可能範囲内にあり、かつ空きマスであるかチェック
                if (reachableNodes.ContainsKey(attackPosCandidate))
                {
                    int costToMoveToAttackPos = reachableNodes[attackPosCandidate].Cost;

                    //AIの挙動に納得できないのでマンハッタン距離による移動経路計算を試験2025/07
                    int currentManhattanDistanceToPlayer =
                        Mathf.Abs(attackPosCandidate.x - player.GetCurrentGridPostion().x) + Mathf.Abs(attackPosCandidate.y - player.GetCurrentGridPostion().y);

                    //確認用
                    Debug.Log($"    [有効な候補] 攻撃可能位置: {attackPosCandidate} (移動コスト: {costToMoveToAttackPos}, プレイヤーへの距離: {currentManhattanDistanceToPlayer})");


                    // 現在見つかっている最小コストより小さい場合、更新
                    if (costToMoveToAttackPos < minCostToAttackPos)
                    {
                        minCostToAttackPos = costToMoveToAttackPos;
                        bestMoveTargetPos = attackPosCandidate;
                        targetedPlayer = player; // このプレイヤーをターゲットにする
                        //マンハッタン
                        minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
                    }
                    //移動コストが同じであれば、プレイヤーに近い（マンハッタン距離が短い）ものを優先
                    else if (costToMoveToAttackPos == minCostToAttackPos)
                    {
                        if(currentManhattanDistanceToPlayer < minManhattanDistanceToPlayer)
                        {
                            minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
                            bestMoveTargetPos = attackPosCandidate;
                            targetedPlayer =player;
                        }
                    }
                    else
                    {
                        //確認用
                        // 移動範囲外または占有済みのため到達できない候補マス
                        Debug.Log($"    [無効な候補] 攻撃可能位置: {attackPosCandidate} (理由: 移動範囲外または占有済み)");
                    }
                }
            }
        }

        // 最適な移動目標位置が見つかった場合
        if (targetedPlayer != null && bestMoveTargetPos != Vector2Int.zero)
        {
            // 最短経路を計算
            bestPath = DijkstraPathfinder.FindPath(GetCurrentGridPostion(), bestMoveTargetPos, this);

            // 経路が見つかり、現在の位置以外に移動先がある場合 (path.Count > 1 は開始地点も含まれるため)
            if (bestPath != null && bestPath.Count > 1)
            {
                //確認用
                string pathString = string.Join(" -> ", bestPath.Select(p => $"({p.x},{p.y})"));
                Debug.Log($"{name}: 最終決定目標位置: {bestMoveTargetPos}");
                Debug.Log($"{name}: 最終決定経路: {pathString}");

                Debug.Log($"{name}: ({GetCurrentGridPostion().x},{GetCurrentGridPostion().y}) から ({bestMoveTargetPos.x},{bestMoveTargetPos.y}) へ移動します（対象プレイヤー: {targetedPlayer.name}）。");
                yield return StartCoroutine(MapManager.Instance.SmoothMoveCoroutine(this, GetCurrentGridPostion(), bestMoveTargetPos, bestPath));
            }
            else
            {
                // このケースは通常発生しないはずですが、念のため
                Debug.Log($"{name}: 経路計算に失敗したか、既に最適な位置にいます。目標位置: {bestMoveTargetPos}");
            }
        }
        else
        {
            // 攻撃できる移動先が見つからなかった場合
            Debug.Log($"{name}: 攻撃できる移動先が見つからないか、全てのプレイヤーが移動範囲外です。");
        }


        //処理がうまく行かないので一度作り直しのためコメントアウト（移動のみ実装済みソースコード）

        //敵ユニットから最も近いプレイヤーユニットを見つける
        //PlayerUnit closestPlayer = null;
        //int minCostToPlayer = int.MaxValue;
        //int minCostToTargetPlayer = int.MaxValue;


        //foreach (PlayerUnit player in playerUnits)
        //{
        //    // DijkstraPathfinder.FindReachableNodes で取得した PathNode を使ってプレイヤーの位置までのコストを取得
        //    if (reachableNodes.ContainsKey(player.GetCurrentGridPostion()))
        //    {
        //        int costToCurrentPlayerPos = reachableNodes[player.GetCurrentGridPostion()].Cost;

        //        if (costToCurrentPlayerPos < minCostToPlayer)
        //        {
        //            minCostToPlayer = costToCurrentPlayerPos;
        //            closestPlayer = player;
        //        }
        //    }
        //}

        //if (closestPlayer == null)
        //{
        //    Debug.Log($"{name}: 接近できるプレイヤーユニットが見つかりません。");
        //    yield break; // 接近できるプレイヤーがいないので行動終了
        //}



        //Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableNode =
        //    DijkstraPathfinder.FindReachableTiles(this.CurrentGridPosition, this);

        //// 到達可能な各マス (reachablePos) から、最も近いプレイヤーユニットを探す
        //foreach (PlayerUnit player in playerUnits)
        //{
        //    // 各プレイヤーユニットに対して、そのプレイヤーを攻撃できる、敵ユニットの到達可能なマスを全てチェック
        //    HashSet<Vector2Int> playerAttackableFromTiles = CalculateAttackRange(player.GetCurrentGridPostion());

        //    //敵ユニットの到達可能なマスの中から、playerAttackableFromTilesと重複するマスを探す
        //    foreach (Vector2Int possibleMovePos in reachableNode.Keys)
        //    {
        //        //possibleMoveToPos から player を攻撃できるかまたは、possibleMoveToPos が player の攻撃範囲内にあるか
        //        if (playerAttackableFromTiles.Contains(possibleMovePos))
        //        {
        //            int costToMoveToPos = reachableNode[possibleMovePos].Cost;

        //            //現在見つかっている最小コストより小さい場合
        //            if (costToMoveToPos < minCostToTargetPlayer)
        //            {
        //                minCostToTargetPlayer -= costToMoveToPos;
        //                bestMoveTargetPos = possibleMovePos;
        //                bestPath = DijkstraPathfinder.FindPath(GetCurrentGridPostion(), bestMoveTargetPos, this);
        //            }
        //        }
        //    }
        //}

        //HashSet<Vector2Int> attackableFromTilesForClosestPlayer = CalculateAttackRange(closestPlayer.GetCurrentGridPostion());

        ////敵が移動可能なマスの中から、このプレイヤーを攻撃できるマスを探す
        //foreach (Vector2Int possibleMoveToPos in reachableNodes.Keys) // reachableNodesは既に移動力でフィルタリングされている
        //{
        //    // possibleMoveToPos から closestPlayer を攻撃できるか
        //    if (attackableFromTilesForClosestPlayer.Contains(possibleMoveToPos))
        //    {
        //        int costToMoveToPos = reachableNodes[possibleMoveToPos].Cost;

        //        // 現在見つかっている最小コストより小さい場合、まずはコスト最小を優先
        //        if (costToMoveToPos < minCostToAttackPos)
        //        {
        //            minCostToAttackPos = costToMoveToPos;
        //            bestMoveTargetPos = possibleMoveToPos;
        //            bestPath = DijkstraPathfinder.FindPath(GetCurrentGridPostion(), bestMoveTargetPos, this);
        //        }
        //    }
        //}



        ////経路が見つかり、現在の位置以外に移動先がある場合（開始地点を含めて）
        //if (bestPath != null && bestPath.Count > 1)
        //{
        //    Debug.Log($"{name}: ({GetCurrentGridPostion().x},{GetCurrentGridPostion().y}) から ({bestMoveTargetPos.x},{bestMoveTargetPos.y}) へ移動します。");
        //    yield return StartCoroutine(MapManager.Instance.MoveUnitAlogPath(this, bestPath));
        //    yield return StartCoroutine(MapManager.Instance.SmoothMoveCoroutine(this, GetCurrentGridPostion(), bestMoveTargetPos, bestPath));
        //}
        //else
        //{
        //    Debug.Log($"{name}: 移動可能なプレイヤーユニットが見つからないか、既に最適な位置にいます。");
        //}

        //攻撃範囲内にプレイヤーユニットがいなければ移動しない
        //
        //Debug.Log($"{name}: 攻撃範囲内にプレイヤーユニットがいません。移動しません。");
        //yield break;
    }

    /// <summary>
    /// 指定された位置からの攻撃可能範囲のグリッド座標を計算する
    /// </summary>
    private HashSet<Vector2Int> CalculateAttackRange(Vector2Int centerPos)
    {
        HashSet<Vector2Int> attackableTiles = new HashSet<Vector2Int>();
        
        //確認用
        Debug.Log($"--- CalculateAttackRange for center: {centerPos} (Min:{_minAttackRange}, Max:{_maxAttackRange}) ---");

        for (int x = -_maxAttackRange;x <= _maxAttackRange; x++)
        {
            for(int y = -_maxAttackRange;y <= _maxAttackRange; y++)
            {
                Vector2Int potentialAttackPos = centerPos + new Vector2Int(x, y);
                int distance = Mathf.Abs(x) + Mathf.Abs(y);//マンハッタン

                if(distance >= _minAttackRange && distance <= _maxAttackRange)
                {
                    if (MapManager.Instance.IsValidGridPosition(potentialAttackPos))
                    {
                        attackableTiles.Add(potentialAttackPos);
                        //確認用
                        Debug.Log($"  [AttackRange] Valid: {potentialAttackPos} (Distance: {distance})");
                    }
                    else
                    {
                        //確認用
                        Debug.Log($"  [AttackRange] Invalid Grid Pos: {potentialAttackPos} (Distance: {distance})");
                    }
                }
            }
        }
        //確認用
        Debug.Log($"--- CalculateAttackRange End. Total Valid Attackable Tiles: {attackableTiles.Count} ---");

        return attackableTiles;
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
