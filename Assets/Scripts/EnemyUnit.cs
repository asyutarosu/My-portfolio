using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class EnemyUnit : Unit
{
    //ScriptableObjectで管理
    //敵AIの行動タイプ
    //public enum EnemyAIType
    //{
    //    Aggreeive,  //積極的にプレイヤーを追う
    //    Stationary, //特定のマスに侵入されるまで動かない
    //    Patrol,     //パトロール型：実装予定
    //    Normal      //常にプレイヤーユニットを狙う
    //}

    //ScriptableObjectで管理
    //[SerializeField] private EnemyAIType _aiType = EnemyAIType.Aggreeive;//デフォルトのAIタイプ

    //攻撃範囲:Unit.csに移行2025/07
    //[SerializeField] private int _minAttackRange = 1;//最小攻撃射程
    //[SerializeField] private int _maxAttackRange = 2;//最大攻撃射程


    //デバッグ用
    //public Vector2Int EnemyCurrentGridPosition => MapManager.Instance.GetGridPositionFromWorld(transform.position);
    //private List<Vector2Int> currentAttackableTiles = new List<Vector2Int>(); // ★修正: ここでリストを初期化する

    private bool AImoveing = false;


    public override void Initialize(UnitData data)
    {
        //_currentPosition = initialGridPos;
        //_unitName = name;
        //Debug.Log($"PlayerUnit'{_unitName}'initialized at grid:{_currentPosition}");

        base.Initialize(data);

        _factionType = FactionType.Enemy;


        Debug.Log($"Enemy Unit '{UnitName}' (Type: {Type}, Faction: {Faction},AIType{EnemyAIType}) initialized at grid: {CurrentGridPosition}");
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

        Debug.LogWarning($"AIの状態{_enemyAIType}");

        //敵AIのタイプに応じて行動を分岐
        switch (_enemyAIType)
        {
            case EnemyAIType.Aggreeive:
                Debug.Log($"{name}が突撃します");
                yield return StartCoroutine(HandleAggressiveAI());
                break;
            case EnemyAIType.Stationary:
                Debug.Log($"{name}は待機しています");
                break;
            case EnemyAIType.DefalutAI:
                yield return StartCoroutine(HandleAggressiveAItest());
                break;
            case EnemyAIType.KimoAI:
                yield return DecideDefensiveAction();
                break;
            case EnemyAIType.Distance:
                yield return DistanceAI();
                break;
            //他のAIを追加
            default:
                Debug.Log($"{name}のAIタイプが未定義です:{_enemyAIType}");
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
        //現在地の取得
        Vector2Int targetPos = CurrentGridPosition;

        //プレイヤーユニットの検索
        //List<PlayerUnit> playerUnits = MapManager.Instance.GetAllPlayerUnits();
        List<PlayerUnit> playerUnits = TurnManager.Instance.PlayerUnits;

        if (playerUnits == null || playerUnits.Count == 0)
        {
            Debug.Log($"{name}:プレイヤーユニットが見つかりません");
            yield break;
        }
        Debug.LogWarning($"ユニット名{this.name}");


        //現在の攻撃範囲内にプレイヤーユニットがいるかチェック
        //現在位置からの攻撃範囲を計算
        //HashSet<Vector2Int> currentAttackableTiles = CalculateAttackRange(GetCurrentGridPostion());
        //bool playerInAttackRange = false;
        //PlayerUnit targetPlayerInAttackRange = null;

        //Vector2Int originalCurrentGridPosition2 = CurrentGridPosition;

        //Dictionary<Vector2Int, PathNodes> reachableNodes2 = DijkstraPathfinder.FindReachableNodes(originalCurrentGridPosition2, this);


        //foreach (PlayerUnit players in playerUnits)
        //{
        //    HashSet<Vector2Int> potentialEnemyMoveToAttackPositions2 = CalculateEnemyMoveToAttackPositions(players.GetCurrentGridPostion());


        //    foreach (Vector2Int attackPosCandidate in potentialEnemyMoveToAttackPositions2)
        //    {
        //        if (reachableNodes2.ContainsKey(attackPosCandidate) && !MapManager.Instance.IsTileOccupiedForStooping(attackPosCandidate, this))
        //        {
        //            Debug.LogWarning($"{name}: 攻撃範囲にいます");

        //            playerInAttackRange = true;
        //            targetPlayerInAttackRange = players;
        //            //break;//誰か1人でもいれば良い
        //        }

        //        if (currentAttackableTiles.Contains(players.GetCurrentGridPostion()))
        //        {
        //            Debug.LogWarning($"{name}: 攻撃範囲");

        //            playerInAttackRange = true;
        //            targetPlayerInAttackRange = players;
        //            //break;//誰か1人でもいれば良い
        //        }
        //    }

        //}




        //if (playerInAttackRange)
        //{
        //    //Debug.LogWarning($"{name}:プレイヤーユニット({targetPlayerInAttackRange.name})が攻撃範囲にいます。攻撃します");

        //    Debug.LogWarning($"{name}:プレイヤーユニット({targetPlayerInAttackRange.name})が攻撃範囲にいます。攻撃します");
        //    //攻撃ロジックを追加
        //    Debug.LogWarning($"{name}: 攻撃攻撃攻撃攻撃");


        //    yield return new WaitForSeconds(0.5f);
        //    yield break;//攻撃したら移動しない
        //}

        Debug.Log($"{name}: 攻撃範囲内にプレイヤーユニットがいません。最も近いプレイヤーユニットに向かって移動します。");

        //確認用
        Debug.Log($"{name}: 現在の移動力: {CurrentMovementPoints}"); // !!! ここで実際の移動力を確認 !!!


        //最も近いかつ移動コストが最小の位置にいるプレイヤーユニットへの移動目標を決定
        Vector2Int bestMoveTargetPos = Vector2Int.zero;
        List<Vector2Int> bestPath = null;
        int minCostToAttackPos = int.MaxValue; // 敵から攻撃可能位置までのコスト
        PlayerUnit targetedPlayer = null;
        int minManhattanDistanceToPlayer = int.MaxValue;//移動目標位置からプレイヤーまでのマンハッタン距離

        Vector2Int originalCurrentGridPosition = CurrentGridPosition;

        //一部の経路計算時に不具合があったため変更2025/07
        //Dictionary<Vector2Int, PathNodes> reachableNodes = DijkstraPathfinder.FindReachableNodes(GetCurrentGridPostion(), this);
        //Dictionary<Vector2Int, PathNodes> reachableNodes = DijkstraPathfinder.FindReachableNodes(originalCurrentGridPosition, this);

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> TestreachableNodes =
            DijkstraPathfinder.FindReachableTiles(this.CurrentGridPosition, this);

        //確認用
        Debug.Log($"{name}: DijkstraPathfinderが計算した到達可能マス数: {TestreachableNodes.Count}"); // 到達可能なマスの総数


        // 到達可能なマスとそのコストを全てログ出力
        Debug.Log($"{name}: --- 到達可能マス詳細 ---");
        foreach (var entry in TestreachableNodes.OrderBy(e => e.Value.Cost)) // コスト順にソートして見やすく
        {
            Debug.Log($"  Reachable Node: {entry.Key} (Cost: {entry.Value.Cost})");
        }
        Debug.Log($"{name}: ---------------------");


        foreach (PlayerUnit player in playerUnits)
        {
            Debug.Log($"  ターゲット候補のプレイヤー: {player.name} 位置: {player.GetCurrentGridPostion()}");

            HashSet<Vector2Int> potentialEnemyMoveToAttackPositions = CalculateEnemyMoveToAttackPositions(player.GetCurrentGridPostion());

            Debug.Log($"    プレイヤー({player.name})を攻撃可能なマス候補数 (敵が移動すべき位置): {potentialEnemyMoveToAttackPositions.Count}");

            // 敵が移動可能なマスの中から、このプレイヤーを攻撃できるマスを探す
            foreach (Vector2Int attackPosCandidate in potentialEnemyMoveToAttackPositions)
            {
                // 候補の攻撃位置が、敵の移動可能範囲内にあり、かつ空きマスであるかチェック
                if (TestreachableNodes.ContainsKey(attackPosCandidate) && !MapManager.Instance.IsTileOccupiedForStooping(attackPosCandidate, this)) {
                    int costToMoveToAttackPos = TestreachableNodes[attackPosCandidate].Cost;

                    int currentManhattanDistanceToPlayer =
                        Mathf.Abs(attackPosCandidate.x - player.GetCurrentGridPostion().x) + Mathf.Abs(attackPosCandidate.y - player.GetCurrentGridPostion().y);

                    Debug.Log($"    [有効な候補] 攻撃可能位置: {attackPosCandidate} (移動コスト: {costToMoveToAttackPos}, プレイヤーへの距離: {currentManhattanDistanceToPlayer})");

                    // 現在見つかっている最小コストより小さい場合、更新
                    if (costToMoveToAttackPos < minCostToAttackPos)
                    {
                        minCostToAttackPos = costToMoveToAttackPos;
                        bestMoveTargetPos = attackPosCandidate;
                        targetedPlayer = player; // このプレイヤーをターゲットにする
                        minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
                    }
                    //移動コストが同じであれば、プレイヤーに近い（マンハッタン距離が短い）ものを優先
                    else if (costToMoveToAttackPos == minCostToAttackPos)
                    {
                        if (currentManhattanDistanceToPlayer < minManhattanDistanceToPlayer)
                        {
                            minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
                            bestMoveTargetPos = attackPosCandidate;
                            targetedPlayer = player;
                        }
                    }
                }
                else
                {
                    //確認用
                    Debug.Log($"    [無効な候補] 攻撃可能位置: {attackPosCandidate} (理由: 移動範囲外または占有済み)");
                }
            }
        }

        // 最適な移動目標位置が見つかった場合
        //if(targetedPlayer != null && bestMoveTargetPos != Vector2Int.zero && minCostToAttackPos != int.MaxValue)
        if (targetedPlayer != null && minCostToAttackPos != int.MaxValue)
        {
            // 移動コストが現在の移動力以下であることを確認
            if (minCostToAttackPos <= CurrentMovementPoints)
            {
                //座標データの移動処理
                MyTile newTile = MapManager.Instance.GetTileAt(bestMoveTargetPos);
                if (newTile != null)
                {
                    MoveToGridPosition(bestMoveTargetPos, newTile); // 占有情報更新
                }

                // 最短経路を計算 (アニメーション用)
                //List<Vector2Int> pathForAnimation = DijkstraPathfinder.FindPath(originalCurrentGridPosition, bestMoveTargetPos, this);

                List<Vector2Int> AnimationPath = DijkstraPathfinder.GetPathToTarget(
                originalCurrentGridPosition,
                bestMoveTargetPos,
                this);

                //デバッグ用
                //foreach (Vector2Int animation in AnimationPath)
                //{
                //    Debug.LogWarning(animation);
                //}


                //if (pathForAnimation != null && pathForAnimation.Count > 1)
                //{
                //    string pathString = string.Join(" -> ", pathForAnimation.Select(p => $"({p.x},{p.y})"));
                //    Debug.Log($"{name}: 最終決定目標位置: {bestMoveTargetPos}");
                //    Debug.Log($"{name}: 最終決定経路: {pathString}");

                //    Debug.Log($"{name}: ({originalCurrentGridPosition.x},{originalCurrentGridPosition.y}) から ({bestMoveTargetPos.x},{bestMoveTargetPos.y}) へ移動します（対象プレイヤー: {targetedPlayer.name}）。");
                //    // ★アニメーションは Unit.AnimateMove を使用しているはずなので、以下に修正
                //    yield return StartCoroutine(AnimateMove(pathForAnimation));

                //    //確認用
                //    Debug.LogWarning($"{name}: 攻撃攻撃攻撃攻撃");

                //    //ここに攻撃処理
                //    BattleManager.Instance.ResolveBattle_ShogiBase(this, targetedPlayer);
                //}

                if (AnimationPath != null && AnimationPath.Count > 0)
                {
                    string pathString = string.Join(" -> ", AnimationPath.Select(p => $"({p.x},{p.y})"));
                    Debug.Log($"{name}: 最終決定目標位置: {bestMoveTargetPos}");
                    Debug.Log($"{name}: 最終決定経路: {pathString}");

                    Debug.Log($"{name}: ({originalCurrentGridPosition.x},{originalCurrentGridPosition.y}) から ({bestMoveTargetPos.x},{bestMoveTargetPos.y}) へ移動します（対象プレイヤー: {targetedPlayer.name}）。");
                    // ★アニメーションは Unit.AnimateMove を使用しているはずなので、以下に修正
                    yield return StartCoroutine(AnimateMove(AnimationPath));


                    //確認用
                    Debug.LogWarning($"{name}: 攻撃攻撃攻撃攻撃");

                    //ここに攻撃処理
                    BattleManager.Instance.ResolveBattle_ShogiBase(this, targetedPlayer);

                    //ゲームオーバー判定
                    TurnManager.Instance.CheckGameOver();

                }
                else
                {
                    Debug.Log($"{name}: 経路計算に失敗したか、既に最適な位置にいます。目標位置: {bestMoveTargetPos} (経路なし)");

                    //確認用
                    Debug.LogWarning($"{name}: ここで攻撃ここで攻撃");
                    //targetedPlayer.Die();
                    BattleManager.Instance.ResolveBattle_ShogiBase(this, targetedPlayer);
                    TurnManager.Instance.CheckGameOver();

                    yield return null; // 移動しないが行動は完了
                }
            }
            else
            {
                Debug.Log($"{name}: 目標位置 ({bestMoveTargetPos}) への移動コスト ({minCostToAttackPos}) が移動力 ({CurrentMovementPoints}) を超えています。移動しません。");
                yield return null; // 移動しないが行動は完了
            }

        }
        else
        {
            // 攻撃できる移動先が見つからなかったか、プレイヤーが範囲外の場合
            Debug.Log($"{name}: 攻撃できる移動先が見つからないか、全てのプレイヤーが移動範囲外です。移動しません。");
            yield return null; // 移動しないが行動は完了
        }





        //AIの行動ロジックの見直しにより変更2025/07
        //// 全てのプレイヤーユニットに対して、最適な移動先を探す
        //foreach (PlayerUnit player in playerUnits)
        //{
        //    //確認用
        //    Debug.Log($"  ターゲット候補のプレイヤー: {player.name} 位置: {player.GetCurrentGridPostion()}"); // ターゲット候補のプレイヤー情報をログ出力

        //    // そのプレイヤーを攻撃できるマス（敵が移動すべきターゲットマス）を計算
        //    //HashSet<Vector2Int> potentialAttackPositions = CalculateAttackRange(player.GetCurrentGridPostion());
        //    HashSet<Vector2Int> potentialAttackPositions = CalculateAttackRange(player.GetCurrentGridPostion());

        //    Debug.Log($"    プレイヤー({player.name})を攻撃可能なマス候補数 (攻撃範囲内): {potentialAttackPositions.Count}");


        //    // 敵が移動可能なマスの中から、このプレイヤーを攻撃できるマスを探す
        //    foreach (Vector2Int attackPosCandidate in potentialAttackPositions)
        //    {
        //        // 候補の攻撃位置が、敵の移動可能範囲内にあり、かつ空きマスであるかチェック
        //        if (reachableNodes.ContainsKey(attackPosCandidate))
        //        {
        //            int costToMoveToAttackPos = reachableNodes[attackPosCandidate].Cost;

        //            //AIの挙動に納得できないのでマンハッタン距離による移動経路計算を試験2025/07
        //            int currentManhattanDistanceToPlayer =
        //                Mathf.Abs(attackPosCandidate.x - player.GetCurrentGridPostion().x) + Mathf.Abs(attackPosCandidate.y - player.GetCurrentGridPostion().y);

        //            //確認用
        //            Debug.Log($"    [有効な候補] 攻撃可能位置: {attackPosCandidate} (移動コスト: {costToMoveToAttackPos}, プレイヤーへの距離: {currentManhattanDistanceToPlayer})");


        //            // 現在見つかっている最小コストより小さい場合、更新
        //            if (costToMoveToAttackPos < minCostToAttackPos)
        //            {
        //                minCostToAttackPos = costToMoveToAttackPos;
        //                bestMoveTargetPos = attackPosCandidate;
        //                targetedPlayer = player; // このプレイヤーをターゲットにする
        //                //マンハッタン
        //                minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
        //            }
        //            //移動コストが同じであれば、プレイヤーに近い（マンハッタン距離が短い）ものを優先
        //            else if (costToMoveToAttackPos == minCostToAttackPos)
        //            {
        //                if(currentManhattanDistanceToPlayer < minManhattanDistanceToPlayer)
        //                {
        //                    minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
        //                    bestMoveTargetPos = attackPosCandidate;
        //                    targetedPlayer =player;
        //                }
        //            }
        //            else
        //            {
        //                //確認用
        //                // 移動範囲外または占有済みのため到達できない候補マス
        //                Debug.Log($"    [無効な候補] 攻撃可能位置: {attackPosCandidate} (理由: 移動範囲外または占有済み)");
        //            }
        //        }
        //    }
        //}



        //// 最適な移動目標位置が見つかった場合
        //if (targetedPlayer != null && bestMoveTargetPos != Vector2Int.zero)
        //{
        //    // 最短経路を計算
        //    bestPath = DijkstraPathfinder.FindPath(GetCurrentGridPostion(), bestMoveTargetPos, this);

        //    // 経路が見つかり、現在の位置以外に移動先がある場合 (path.Count > 1 は開始地点も含まれるため)
        //    if (bestPath != null && bestPath.Count > 1)
        //    {
        //        //確認用
        //        string pathString = string.Join(" -> ", bestPath.Select(p => $"({p.x},{p.y})"));
        //        Debug.Log($"{name}: 最終決定目標位置: {bestMoveTargetPos}");
        //        Debug.Log($"{name}: 最終決定経路: {pathString}");



        //        Debug.Log($"{name}: ({GetCurrentGridPostion().x},{GetCurrentGridPostion().y}) から ({bestMoveTargetPos.x},{bestMoveTargetPos.y}) へ移動します（対象プレイヤー: {targetedPlayer.name}）。");
        //        yield return StartCoroutine(MapManager.Instance.SmoothMoveCoroutine(this, GetCurrentGridPostion(), bestMoveTargetPos, bestPath));
        //    }
        //    else
        //    {
        //        // このケースは通常発生しないはずですが、念のため
        //        Debug.Log($"{name}: 経路計算に失敗したか、既に最適な位置にいます。目標位置: {bestMoveTargetPos}");
        //    }
        //}
        //else
        //{
        //    // 攻撃できる移動先が見つからなかった場合
        //    Debug.Log($"{name}: 攻撃できる移動先が見つからないか、全てのプレイヤーが移動範囲外です。");
        //}


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
    /// 好戦的なAIの全体的な行動ロジック
    /// <summary>
    /// </summary>
    /// <returns></returns>
    private IEnumerator HandleAggressiveAItest()
    {
        PlayerUnit targetPlayer = GetClosestPlayerUnit();
        if (targetPlayer == null)
        {
            Debug.Log($"{UnitName}: ターゲットとなるプレイヤーユニットが見つかりません");
            yield break;
        }

        yield return EnemyAIbestMoveAttack(targetPlayer);


        //各AIのタイプによって移動の仕方を帰る::現段階では仮として一律同じにしている
        if (AImoveing)
        {
            yield return StartCoroutine(PerformAggressiveMoveToAttackRange(targetPlayer));
            Debug.LogWarning("yayayaay");
        }

        /////
        //DecideDefensiveAction();
    }

    

    /// <summary>
    /// きもそうなAIの行動
    /// </summary>
    /// <returns></returns>
    private IEnumerator DecideDefensiveAction()
    {
        //存在する敵ユニットの取得
        List<EnemyUnit> enemyUnits = TurnManager.Instance.SetAllEnemyUnits();

        //存在するプレイヤーユニットの取得
        List<PlayerUnit> playerUnits = TurnManager.Instance.SetAllPlayerUnit();

        //一番近いプレイヤーユニットの取得
        PlayerUnit nearplayerUnit = GetClosestPlayerUnit();

        if (enemyUnits != null && enemyUnits.Count > 0)
        {
            //最も近い敵ユニットの取得
            EnemyUnit targetUnit = GetClosestEnemyUnit();
            if(targetUnit != null)
            {
                //移動する先のマスを取得
                Vector2Int bestPos = FindBestDefensivePosition(targetUnit, nearplayerUnit);
                Vector2Int originalCurrentGridPosition = CurrentGridPosition;

                //AI行動
                List<Vector2Int> AnimationPath = DijkstraPathfinder.GetPathToTarget(
                originalCurrentGridPosition,
                bestPos,
                this);

                //攻撃目標のプレイヤーユニットを取得
                PlayerUnit targetPlayer = CanAttackPlayerUnit();
                if(targetPlayer != null)
                {
                    Debug.LogWarning("mooooooooooooa");
                    yield return EnemyAIbestMoveAttack(targetPlayer);
                }
                else
                {
                    if (AnimationPath != null && AnimationPath.Count > 0)
                    {
                        MyTile newTile = MapManager.Instance.GetTileAt(bestPos);
                        if (newTile != null)
                        {
                            MoveToGridPosition(bestPos, newTile); // 占有情報更新
                        }

                        //アニメーションは Unit.AnimateMove を使用しているはずなので、以下に修正
                        yield return StartCoroutine(AnimateMove(AnimationPath));

                    }
                }
            }
            //敵ユニットが自身のしかいない場合
            //基本AI挙動を行う
            else
            {
                //AI行動
                yield return HandleAggressiveAItest();
            }
        }
    }

    /// <summary>
    /// 安全な位置から遠距離攻撃するAI
    /// </summary>
    /// <returns></returns>
    private IEnumerator DistanceAI()
    {
        //一番近いプレイヤーユニットから順に３体の取得(移動先の取得のため）
        List<PlayerUnit> targetPlayers = GetThreeClosestPlayers();

        Vector2Int bestPos = new Vector2Int();

        //NotDoubleUnitAttackRange(targetPlayers);
        Vector2Int targetsPos = NotDoubleUnitAttackRange(targetPlayers);
        //取得した移動可能マスの候補からランダムに１マスを選択する（多少のランダム性を持たせることで動きを読みにくくする）
        //List<Vector2Int> targetsPosList = targetsPos.ToList();
        //int randomIndex = Random.Range(0, targetsPos.Count);

        Vector2Int? NullableBestPos = NotOneUnitAttackRange(targetPlayers[0]);

        if (targetPlayers != null && targetsPos != null)
        {
            bestPos = targetsPos;
        }

        //最も近い位置の攻撃可能なプレイヤーユニットを攻撃目標として取得
        PlayerUnit targetPlayer = CanAttackPlayerUnit();
        Vector2Int originalCurrentGridPosition = CurrentGridPosition;

        if (targetPlayer != null)
        {
            MyTile newTile = MapManager.Instance.GetTileAt(bestPos);
            if (newTile != null)
            {
                MoveToGridPosition(bestPos, newTile); // 占有情報更新
            }

            List<Vector2Int> AnimationPath = DijkstraPathfinder.GetPathToTarget(
                originalCurrentGridPosition,
                bestPos,
                this);

            if (AnimationPath != null && AnimationPath.Count > 0)
            {
                yield return StartCoroutine(AnimateMove(AnimationPath));

                //ここに攻撃処理
                BattleManager.Instance.ResolveBattle_ShogiBase(this, targetPlayer);

                //ゲームオーバー判定
                TurnManager.Instance.CheckGameOver();
            }
        }
        else
        {
            //ターゲットが攻撃範囲外ならターゲットに近づくように移動
            yield return StartCoroutine(PerformAggressiveMoveToAttackRange(targetPlayers[0]));
        }
    }

    ///プレイヤーユニットへの遠距離攻撃用
    /// ターゲットのプレイヤーユニットのから、攻撃を受けない位置で自身の移動可能マスを探す
    /// nullを返すためにNullable
    /// 
    /// 現段階の将棋ベースでの戦闘ではあまり意味をなさないかもしれない
    /// 動きとしては移動不可の地形などを挟んで攻撃をする際にターゲットの攻撃範囲が１のときに使える
    /// 
    private Vector2Int? NotOneUnitAttackRange(PlayerUnit targetPlayer)
    {
        Vector2Int? bestMoveTargetPos = Vector2Int.zero;
        int maxPlayerDistance = -1;

        HashSet<Vector2Int> attackableTiles = GetAttackRange(targetPlayer);

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableNodes =
            DijkstraPathfinder.FindReachableTiles(this.CurrentGridPosition, this);


        bool isMatch = false;
        foreach (var attackPosCandidate in attackableTiles)
        {
            if (reachableNodes.ContainsKey(attackPosCandidate))
            {
                isMatch = true;
                int distanceToPlayer = 
                    Mathf.Abs(attackPosCandidate.x - targetPlayer.CurrentGridPosition.x) +
                    Mathf.Abs(attackPosCandidate.y - targetPlayer.CurrentGridPosition.y);
                if(distanceToPlayer > maxPlayerDistance)
                {
                    maxPlayerDistance = distanceToPlayer;
                    bestMoveTargetPos = attackPosCandidate;
                }
            }
        }
        if (!isMatch)
        {
            bestMoveTargetPos = this.CurrentGridPosition;
        }
        return bestMoveTargetPos;
    }

    //////位置のみ
    /// ターゲットのプレイヤーユニットの以外の周囲2体までの攻撃範囲を取得し、
    /// ターゲットに攻撃可能でターゲット以外の周囲2体から攻撃を受けない位置を探す
    private Vector2Int NotDoubleUnitAttackRange(List<PlayerUnit> playerUnits)
    {
        Vector2Int bestPos = new Vector2Int();
        if(playerUnits == null || playerUnits.Count == 0)
        {
            return bestPos;
        }
        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableNodes =
            DijkstraPathfinder.FindReachableTiles(this.CurrentGridPosition, this);

        HashSet<Vector2Int> moveableTiles = new HashSet<Vector2Int>(reachableNodes.Keys.ToList());

        HashSet<Vector2Int> sharedTiles1 = new HashSet<Vector2Int>();
        HashSet<Vector2Int> sharedTiles2 = new HashSet<Vector2Int>();


        //ターゲット以外に1体いる場合
        if (playerUnits.Count == 2)
        {
            sharedTiles1 = GetAttackRange(playerUnits[1]);
            moveableTiles.ExceptWith(sharedTiles1);
        }
        //ターゲット以外に2体以上いる場合
        else if (playerUnits.Count == 3)
        {
            sharedTiles1 = GetAttackRange(playerUnits[1]);
            moveableTiles.ExceptWith(sharedTiles1);
            sharedTiles2 = GetAttackRange(playerUnits[2]);
            moveableTiles.ExceptWith(sharedTiles2);
        }

        //ターゲットへ攻撃可能な位置の計算
        HashSet<Vector2Int> potentialEnemyMoveToAttackPositions = CalculateEnemyMoveToAttackPositions(playerUnits[0].GetCurrentGridPostion());
        int maxPlayerDistance = -1;
        bool isMatch = false;
        foreach (Vector2Int attackPosCandidate in potentialEnemyMoveToAttackPositions)
        {
            //移動可能かつ安全な位置と攻撃可能な位置の一致を検索
            if (moveableTiles.Contains(attackPosCandidate))
            {
                isMatch = true;
                bestPos = attackPosCandidate;
                int distanceToPlayer =
                     Mathf.Abs(attackPosCandidate.x - playerUnits[0].CurrentGridPosition.x) +
                     Mathf.Abs(attackPosCandidate.y - playerUnits[0].CurrentGridPosition.y);
                if (distanceToPlayer > maxPlayerDistance)
                {
                    maxPlayerDistance = distanceToPlayer;
                    bestPos = attackPosCandidate;
                }
            }
        }

        //条件を満たしたマスがない場合は攻撃可能マスからランダムに取得する
        if (!isMatch)
        {
            List<Vector2Int> tileList = potentialEnemyMoveToAttackPositions.ToList();
            int ramdomIndex = Random.Range(0, tileList.Count);
            bestPos = tileList[ramdomIndex];
        }

        if (moveableTiles != null)
        {
            foreach (Vector2Int tilePos in moveableTiles)
            {
                Debug.LogWarning($"デバックログ：（{tilePos.x}：{tilePos.y}）");

            }
            Debug.LogWarning($"合計数：：{moveableTiles.Count}");
        }
        Debug.LogWarning($"目標位置：：{bestPos}");

        return bestPos;
    }

    ///プレイヤーユニットの攻撃範囲外のマスの取得
    private HashSet<Vector2Int> GetAttackRange(PlayerUnit targetPlayer)
    {

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableNodes =
            DijkstraPathfinder.FindReachableTiles(targetPlayer.CurrentGridPosition, targetPlayer);

        List<Vector2Int> moveableTiles = reachableNodes.Keys.ToList();

        HashSet<Vector2Int> attackableTiles = new HashSet<Vector2Int>();

        //攻撃範囲指定のマンハッタン距離方での実装(まだ各typeとの連携は未実装)
        //一部数値を仮として実装2025/06
        int minAttackRange = targetPlayer._minAttackRange;//最小射程
        int maxAttackRange = targetPlayer._maxAttackRange;//最大射程

        foreach (Vector2Int movePos in moveableTiles)
        {
            for (int x = -maxAttackRange; x <= maxAttackRange; x++)
            {
                for (int y = -maxAttackRange; y <= maxAttackRange; y++)
                {
                    //現在の移動可能タイル(movePos)からの相対座標
                    Vector2Int potentialAttackPos = movePos + new Vector2Int(x, y);

                    //マンハッタン距離計算
                    int distance = Mathf.Abs(x) + Mathf.Abs(y);

                    if (distance >= minAttackRange && distance <= maxAttackRange)
                    {
                        if (MapManager.Instance.IsValidGridPosition(potentialAttackPos))
                        {
                            attackableTiles.Add(potentialAttackPos);
                        }
                    }
                }
            }
        }
        Debug.LogWarning($"合計数合計数合計数：：{moveableTiles.Count}");

        return attackableTiles;
    }


    ///
    /// 
    /// 
    ///攻撃する目標のプレイヤーユニットを選定及び取得
    private PlayerUnit CanAttackPlayerUnit()
    {
        //プレイヤーユニットのリストを取得
        var allPlayerUnits = TurnManager.Instance.SetAllPlayerUnit();

        if (allPlayerUnits == null || allPlayerUnits.Count == 0)
        {
            return null; //リストが空の場合はnullを返す
        }

        //今後、対象を追加->AIタイプによってターゲットユニットを変えたい
        //現段階では一時的にフラグで管理
        bool test = true;
        bool aiTypeChange = true;


        PlayerUnit closestPlayer = null;
        int minDistance = 999;

        //プレイヤーユニットを距離でグループ化するための辞書
        Dictionary<int, List<PlayerUnit>> playersByDistance = new Dictionary<int, List<PlayerUnit>>();

        foreach (PlayerUnit player in allPlayerUnits)
        {
            if (player == null)
            {
                continue;
            }

            //敵ユニットからプレイヤーまでの距離を計算
            int dist = Mathf.Abs(CurrentGridPosition.x - player.CurrentGridPosition.x) +
                       Mathf.Abs(CurrentGridPosition.y - player.CurrentGridPosition.y);

            //距離をキーとして、プレイヤーユニットを辞書に追加
            if (!playersByDistance.ContainsKey(dist))
            {
                playersByDistance[dist] = new List<PlayerUnit>();
            }
            playersByDistance[dist].Add(player);

            //最小距離を更新
            if (dist < minDistance)
            {
                minDistance = dist;
                closestPlayer = player;
            }
        }

        ///////ToDo
        if (test)
        {

            //最も近いターゲットが見つかったが、それが移動力を超える距離である場合、
            //ターゲットをnullにする
            int CanAttackRange = this.BaseMovement + this._maxAttackRange;

            if (closestPlayer != null && minDistance > CanAttackRange)
            {
                Debug.LogWarning("最も近いプレイヤーは攻撃範囲を超えているため、ターゲットから外します。");
                return null;
            }
        }
        
        //-----------------------------------------------Aiの目標選定項目

        //対象の選定条件の追加：未完成
        if (!aiTypeChange)
        {
            //最小距離のグループが存在するか確認
            if (playersByDistance.ContainsKey(minDistance))
            {
                //最も近い距離にあるプレイヤーユニットのリストを取得
                List<PlayerUnit> closestPlayers = playersByDistance[minDistance];

                //そのリストの中から、MaxHPが一番高いユニットをLINQを使って取得
                closestPlayer = closestPlayers.OrderByDescending(p => p.MaxHP).FirstOrDefault();
            }
        }
        return closestPlayer;
    }


    ////////ToDo
    //敵AIのために追加：指定マスからの周囲2マスの取得
    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetUnit"></param>
    /// <returns>ユニットの周囲２マス目のマスを取得</returns>
    public List<Vector2Int> GetSurroundingTiles(Unit targetUnit)
    {
        

        //周囲マスを保持する
        List<Vector2Int> surroundingTiles = new List<Vector2Int>();

        //周囲のマスの範囲を指定する：今回は周囲１マスを除く＝周囲２マスを指定
        int minRange = 2;
        int maxRange = 2;

        Vector2Int currentPos = targetUnit.CurrentGridPosition;
        Debug.LogWarning($"ターゲットユニットの位置::{currentPos}");

        for (int x = -maxRange; x <= maxRange; x++)
        {
            for (int y = -maxRange; y <= maxRange; y++)
            {
                //現在の移動可能タイル(movePos)からの相対座標
                Vector2Int potentialAttackPos = currentPos + new Vector2Int(x, y);

                //マンハッタン距離計算
                int distance = Mathf.Abs(x) + Mathf.Abs(y);

                if (distance >= minRange && distance <= maxRange)
                {
                    if (MapManager.Instance.IsValidGridPosition(potentialAttackPos))
                    {
                        surroundingTiles.Add(potentialAttackPos);
                        Debug.LogWarning($"指定した周囲２マスの範囲：：{potentialAttackPos.x}::{potentialAttackPos.y}");
                    }
                }
            }
        }
        return surroundingTiles;
    }



    /// <summary>
    /// プレイヤーユニットの位置を加味して、最も遠い位置で敵ユニットの周囲２マス目の位置を取得
    /// </summary>
    /// <param name="targetenemyUnit"></param>
    /// <param name="playerUnits"></param>
    /// <returns></returns>
    public Vector2Int FindBestDefensivePosition(EnemyUnit targetenemyUnit, PlayerUnit nearPlayerUnit)
    {
        Vector2Int bestPos = this.CurrentGridPosition;
        int maxDistanceToPlayers = -1;

        //目標のユニットの周囲２マス目のマスを取得
        var surroundingTiles = GetSurroundingTiles(targetenemyUnit);

        //自身の移動可能マスを計算
        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableTiles =
            DijkstraPathfinder.FindReachableTiles(this.CurrentGridPosition, this);

        //自身の移動可能マスのうち他のユニットに占有されているかを検討
        List<Vector2Int> candidateTiles = new List<Vector2Int>();
        foreach (var tilePos in reachableTiles.Keys)
        {
            if (!MapManager.Instance.IsTileOccupiedForStooping(tilePos, this))
            {
                candidateTiles.Add(tilePos);
            }
        }

        if (candidateTiles.Count == 0)
        {
            Debug.LogWarning($"{UnitName}: 移動可能なマスがないため、移動しません");
            return this.CurrentGridPosition;
        }

        //candidateTiles.Contains(tilePos)の完全不一致を判定するフラグ
        bool _isMatchFound = false;


        //目標のユニットの周囲２マス目のマスと移動可能マスの一致を検討
        foreach (var tilePos in candidateTiles)
        {
            if (surroundingTiles.Contains(tilePos))
            {
                _isMatchFound = true;
                int minDistanceToPlayers = 999;
                Vector2Int nextPos = tilePos;

                //プレイヤーユニットから最も遠い位置を検討
                int dist = Mathf.Abs(tilePos.x - nearPlayerUnit.CurrentGridPosition.x) + Mathf.Abs(tilePos.y - nearPlayerUnit.CurrentGridPosition.y);
                if (dist < minDistanceToPlayers)
                {
                    minDistanceToPlayers = dist;
                }
                
                //より遠い位置であればbestPosを更新
                if (minDistanceToPlayers > maxDistanceToPlayers)
                {
                    maxDistanceToPlayers = minDistanceToPlayers;
                    bestPos = nextPos;
                }

            }

            //if(!_isMatchFound && candidateTiles.Contains(this.CurrentGridPosition))
            //{
            //    Debug.LogWarning("bbbbbbbbbbbbbbbb");
            //    bestPos = this.CurrentGridPosition;
            //    return bestPos;
            //}


            //周囲２マス目のマスと移動可能マスの一致が存在しない場合
            //周囲２マス目のマスのマスに到達できない場合は目標に向かって近づく
            //if (!_isMatchFound)
            //{
            //    Debug.LogWarning("ggggggggggggggggggg");
            //    int minDistanceToTarget = 999;

            //    foreach (Vector2Int moveCandidate in candidateTiles)
            //    {
            //        int dist = Mathf.Abs(moveCandidate.x - targetenemyUnit.CurrentGridPosition.x) +
            //            Mathf.Abs(moveCandidate.y - targetenemyUnit.CurrentGridPosition.y);

            //        if (dist >= _maxAttackRange && dist <= _maxAttackRange)
            //        {
            //            if (dist < minDistanceToTarget)
            //            {
            //                minDistanceToTarget = dist;
            //                bestPos = moveCandidate;
            //            }
            //        }
            //        else if (dist < minDistanceToTarget)
            //        {
            //            minDistanceToTarget = dist;
            //            bestPos = moveCandidate;
            //        }
            //    }

        }
        if (!_isMatchFound)
        {
            Debug.LogWarning("ggggggggggggggggggg");
            int minDistanceToTarget = 999;

            foreach (Vector2Int moveCandidate in candidateTiles)
            {
                int dist = Mathf.Abs(moveCandidate.x - targetenemyUnit.CurrentGridPosition.x) +
                    Mathf.Abs(moveCandidate.y - targetenemyUnit.CurrentGridPosition.y);

                if (dist >= _maxAttackRange && dist <= _maxAttackRange)
                {
                    if (dist < minDistanceToTarget)
                    {
                        minDistanceToTarget = dist;
                        bestPos = moveCandidate;
                    }
                }
                else if (dist < minDistanceToTarget)
                {
                    minDistanceToTarget = dist;
                    bestPos = moveCandidate;
                }
            }
        }
        Debug.LogWarning($"新規AI１号：BestPOS{bestPos.x}:{bestPos.y}");
        return bestPos;
    }

    /////////////
    /// <summary>
    /// /// 
    /// AIの基本的な攻撃までのロジック
    /// 手順
    /// 最も近いプレイヤーユニットを検索
    ///     攻撃可能：最小移動コストで攻撃
    ///     攻撃不可：AImoveing=false
    /// </summary>
    /// <param name="targetUnit"></param>
    /// <returns></returns>
    private IEnumerator EnemyAIbestMoveAttack(Unit targetUnit)
    {
        AImoveing = false;
        Debug.LogWarning("eeeeeeeee");
        if (targetUnit == null)
        {
            Debug.Log($"{UnitName}: ターゲットとなるユニットが見つかりません");
            yield break;
        }
        Vector2Int bestMoveTargetPos = Vector2Int.zero;
        int minMax = 999;
        int minCostToAttackPos = minMax; // 敵から攻撃可能位置までのコスト
        PlayerUnit targetedPlayer = null;
        int minManhattanDistanceToPlayer = 999;//移動目標位置からプレイヤーまでのマンハッタン距離

        Vector2Int originalCurrentGridPosition = CurrentGridPosition;

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> TestreachableNodes =
            DijkstraPathfinder.FindReachableTiles(this.CurrentGridPosition, this);

        //HashSet<Vector2Int> attackableTiles = CalculateAttackRange(targetUnit.CurrentGridPosition);

        HashSet<Vector2Int> potentialEnemyMoveToAttackPositions = CalculateEnemyMoveToAttackPositions(targetUnit.GetCurrentGridPostion());

        foreach (Vector2Int attackPosCandidate in potentialEnemyMoveToAttackPositions)
        {
            if (TestreachableNodes.ContainsKey(attackPosCandidate) && !MapManager.Instance.IsTileOccupiedForStooping(attackPosCandidate, this))
            {
                int costToMoveToAttackPos = TestreachableNodes[attackPosCandidate].Cost;

                int currentManhattanDistanceToPlayer =
               Mathf.Abs(attackPosCandidate.x - targetUnit.GetCurrentGridPostion().x) + Mathf.Abs(attackPosCandidate.y - targetUnit.GetCurrentGridPostion().y);


                // 現在見つかっている最小コストより小さい場合、更新
                if (costToMoveToAttackPos < minCostToAttackPos)
                {
                    minCostToAttackPos = costToMoveToAttackPos;
                    bestMoveTargetPos = attackPosCandidate;
                    minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
                    if(targetUnit.Faction == FactionType.Player)
                    {
                        targetedPlayer = targetUnit as PlayerUnit; // このプレイヤーをターゲットにする
                    }
                }
                //移動コストが同じであれば、プレイヤーに近い（マンハッタン距離が短い）ものを優先
                else if (costToMoveToAttackPos == minCostToAttackPos)
                {
                    if (currentManhattanDistanceToPlayer < minManhattanDistanceToPlayer)
                    {
                        minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
                        bestMoveTargetPos = attackPosCandidate;
                        if (targetUnit.Faction == FactionType.Player)
                        {
                            targetedPlayer = targetUnit as PlayerUnit; // このプレイヤーをターゲットにする
                        }
                    }
                }
            }
        }
        Debug.LogWarning($"bestPos::{bestMoveTargetPos.x} ,{bestMoveTargetPos.y}");

        if (targetedPlayer != null && minCostToAttackPos != minMax)
        {
            // 移動コストが現在の移動力以下であることを確認
            if (minCostToAttackPos <= CurrentMovementPoints)
            {
                MyTile newTile = MapManager.Instance.GetTileAt(bestMoveTargetPos);
                if (newTile != null)
                {
                    MoveToGridPosition(bestMoveTargetPos, newTile); // 占有情報更新
                }

                List<Vector2Int> AnimationPath = DijkstraPathfinder.GetPathToTarget(
                originalCurrentGridPosition,
                bestMoveTargetPos,
                this);

                Debug.LogWarning($"!!!!!!!!!!!!!!{AnimationPath.Count}");
                if (AnimationPath != null && AnimationPath.Count > 0)
                {
                    string pathString = string.Join(" -> ", AnimationPath.Select(p => $"({p.x},{p.y})"));
                    Debug.Log($"{name}: 最終決定目標位置: {bestMoveTargetPos}");
                    Debug.Log($"{name}: 最終決定経路: {pathString}");

                    Debug.Log($"{name}: ({originalCurrentGridPosition.x},{originalCurrentGridPosition.y}) から ({bestMoveTargetPos.x},{bestMoveTargetPos.y}) へ移動します（対象プレイヤー: {targetedPlayer.name}）。");
                    //アニメーションは Unit.AnimateMove を使用しているはずなので、以下に修正
                    yield return StartCoroutine(AnimateMove(AnimationPath));

                    //ここに攻撃処理
                    BattleManager.Instance.ResolveBattle_ShogiBase(this, targetedPlayer);

                    //ゲームオーバー判定
                    TurnManager.Instance.CheckGameOver();
                }
                else
                {
                    Debug.Log($"{name}: 経路計算に失敗したか、既に最適な位置にいます。目標位置: {bestMoveTargetPos} (経路なし)");

                    //確認用
                    Debug.LogWarning($"{name}: ここで攻撃ここで攻撃");
                    BattleManager.Instance.ResolveBattle_ShogiBase(this, targetedPlayer);

                    yield return null; // 移動しないが行動は完了
                }
            }
        }
        else
        {
            //Debug.LogWarning($"{UnitName}: ターゲット ({targetUnit.UnitName}) が攻撃範囲外です、移動を試みます");
            //yield return StartCoroutine(PerformAggressiveMoveToAttackRange(targetUnit as PlayerUnit));
            AImoveing = true;
            yield return null;
        }
        
    }


    /// <summary>
    /// 好戦的なAIの移動ロジック
    /// ターゲットプレイヤーに最も近づくように移動する
    /// 
    /// 追加：ターゲットユニットに最も近づくように移動するに変更
    /// 
    /// 
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <returns></returns>
    private IEnumerator PerformAggressiveMoveToAttackRange(Unit targetUnit)
    {
        //Dictionary<Vector2Int, PathNodes> reachableTiles = DijkstraPathfinder.FindReachableNodes(GetCurrentGridPostion(), this);

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableTiles =
            DijkstraPathfinder.FindReachableTiles(CurrentGridPosition, this);

        Vector2Int bestMovePos = CurrentGridPosition;
        Vector2Int originalCurrentGridPosition = CurrentGridPosition;
        int minDistanceToTarget = int.MaxValue;

        List<Vector2Int> candidateTiles = new List<Vector2Int>();
        foreach(var tilePos in reachableTiles.Keys)
        {
            if (!MapManager.Instance.IsTileOccupiedForStooping(tilePos, this))
            {
                candidateTiles.Add(tilePos);
            }
        }

        if(candidateTiles.Count == 0)
        {
            Debug.Log($"{UnitName}: 移動可能なマスがないため、移動しません");
            yield break;
        }

        Vector2Int targetPos = originalCurrentGridPosition;
        bool moved = false;

        foreach(Vector2Int moveCandidate in candidateTiles)
        {
            int dist = Mathf.Abs(moveCandidate.x - targetUnit.CurrentGridPosition.x) + 
                Mathf.Abs(moveCandidate.y - targetUnit.CurrentGridPosition.y);

            if(dist >= _minAttackRange && dist <= _maxAttackRange)
            {
                if(dist < minDistanceToTarget)
                {
                    minDistanceToTarget = dist;
                    bestMovePos = moveCandidate;
                }
            }
            else if(dist < minDistanceToTarget)
            {
                minDistanceToTarget = dist;
                bestMovePos = moveCandidate;
            }
        }
        
        targetPos = bestMovePos;
        Debug.Log($"{UnitName}: 移動型AIが目標地点を {targetPos} に決定しました");

        MyTile targetTile = MapManager.Instance.GetTileAt(targetPos);
        if(targetTile != null)
        {
            MoveToGridPosition(targetPos,targetTile);
        }

        List<Vector2Int> pathForAnimation = DijkstraPathfinder.FindPath(originalCurrentGridPosition, targetPos, this);
        if(pathForAnimation == null || pathForAnimation.Count == 0)
        {
            pathForAnimation = new List<Vector2Int>() { originalCurrentGridPosition };
        }

        yield return AnimateMove(pathForAnimation);
    }


    ///追加の敵AI：：常にプレイヤーユニットへ向かって範囲内であれば攻撃

    //敵ユニットの行動を制御するメインメソッド
    public void TakeTurnNomalAI(PlayerUnit targetPlayerUnit)
    {

   
    }

    

    /// <summary>
    /// 敵ユニットによる攻撃を実行する(未実装2025/07)
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator PerformAttack(PlayerUnit target)
    {
        Debug.Log($"{UnitName} が {target.UnitName} を攻撃！");
        yield return new WaitForSeconds(0.5f); 
        Debug.Log("攻撃しました");
    }

    /// <summary>
    /// 最も近いプレイヤーユニットを取得する
    /// </summary>
    /// <returns></returns>
    private PlayerUnit GetClosestPlayerUnit()
    {
        PlayerUnit closesetPlayer = null;
        int minDistance = int.MaxValue;

        foreach(PlayerUnit player in TurnManager.Instance.SetAllPlayerUnit())
        {
            if (player == null)
            {
                continue;
            }

            int dist = Mathf.Abs(CurrentGridPosition.x - player.CurrentGridPosition.x) + 
                Mathf.Abs(CurrentGridPosition.y - player.CurrentGridPosition.y);

            if (dist < minDistance)
            {
                minDistance = dist;
                closesetPlayer = player;
            }
        }
        return closesetPlayer;
    }

    /// <summary>
    /// 最も近いプレイヤーユニットから順に３体プレイヤーユニットを取得する
    /// </summary>
    /// <returns></returns>
    public List<PlayerUnit> GetThreeClosestPlayers()
    {
        var allPlayerUnits = TurnManager.Instance.SetAllPlayerUnit();

        //敵ユニットが存在しない、またはプレイヤーユニットの数が3未満の場合は、取得できる分だけ返す
        if(allPlayerUnits == null || allPlayerUnits.Count == 0)
        {
            return new List<PlayerUnit>();
        }

        //LINQを使用して、距離でソートし、上位3つのユニットを取得
        var closestPlayers = allPlayerUnits.OrderBy(player =>
        Mathf.Abs(CurrentGridPosition.x - player.CurrentGridPosition.x) +
        Mathf.Abs(CurrentGridPosition.y - player.CurrentGridPosition.y))
            .Take(3).ToList();//上位3つの要素を取得

        return closestPlayers;
    }

    //自身を除く最も近い敵ユニットを取得する
    private EnemyUnit GetClosestEnemyUnit()
    {
        EnemyUnit closesetEnemy = null;
        int minDistance = int.MaxValue;

        var allEnemyUnits = TurnManager.Instance.SetAllEnemyUnits();
        if(allEnemyUnits == null || allEnemyUnits.Count == 0)
        {
            Debug.Log("敵ユニットが存在しません");
            return null;
        }

        foreach (EnemyUnit enemyUnit in allEnemyUnits)
        {
            if(enemyUnit == this || enemyUnit == null)
            {
                continue;
            }

            int dist = Mathf.Abs(CurrentGridPosition.x - enemyUnit.CurrentGridPosition.x) +
                Mathf.Abs(CurrentGridPosition.y - enemyUnit.CurrentGridPosition.y);

            if (dist < minDistance)
            {
                minDistance = dist;
                closesetEnemy = enemyUnit;
            }
        }

        if(closesetEnemy == null)
        {
            Debug.Log("自分以外の敵ユニットが見つかりませんでした");
        }
        return closesetEnemy;
    }


    /// <summary>
    /// 指定された位置からの攻撃可能範囲のグリッド座標を計算する
    /// </summary>
    private HashSet<Vector2Int> CalculateAttackRange(Vector2Int centerPos)
    {
        HashSet<Vector2Int> attackableTiles = new  HashSet<Vector2Int>();
        
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


    /// <summary>
    /// 指定されたプレイヤーの位置から見て、この敵ユニットが攻撃可能なマスを計算する
    /// </summary>
    /// <param name="playerPos">ターゲットとなるプレイヤーのグリッド座標</param>
    /// <returns>敵ユニットが移動すべき攻撃可能位置のリスト（HashSetで重複なし）</returns>
    private HashSet<Vector2Int> CalculateEnemyMoveToAttackPositions(Vector2Int playerPos)
    {
        HashSet<Vector2Int> potentialEnemyAttackMovePositions = new HashSet<Vector2Int>();

        //プレイヤーの位置から見て、_minAttackRange から _maxAttackRange の範囲にあるマスを探す
        //敵ユニットがプレイヤーを攻撃するために移動できる候補のマスを逆算して計算する
        for(int x = -_maxAttackRange;x <= _maxAttackRange; x++)
        {
            for(int y = -_maxAttackRange;y <= _maxAttackRange; y++)
            {
                //プレイヤー位置からの相対座標で、敵ユニットが位置する可能性のあるマスを計算
                Vector2Int potentialEnemyPos = playerPos + new Vector2Int(x, y);

                //敵ユニットが potentialEnemyPos に移動した場合、プレイヤー (playerPos) との距離を計算
                int distance = Mathf.Abs(potentialEnemyPos.x - playerPos.x) +
                               Mathf.Abs(potentialEnemyPos.y - playerPos.y); // マンハッタン距離

                //敵ユニットの攻撃射程内かチェック
                if(distance >= _minAttackRange && distance <= _maxAttackRange)
                {
                    // この位置に敵ユニットが移動できれば、プレイヤーを攻撃可能
                    // ただし、MapManager.Instance.IsValidGridPosition のチェックは必須
                    if (MapManager.Instance.IsValidGridPosition(potentialEnemyPos))
                    {
                        // このマスは、敵ユニットが移動してプレイヤーを攻撃できる有効な候補である
                        potentialEnemyAttackMovePositions.Add(potentialEnemyPos);
                        Debug.LogWarning($"攻撃可能マス：{potentialEnemyPos.x}:::{potentialEnemyPos.y}");
                    }
                }
            }
        }
        return potentialEnemyAttackMovePositions;
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
