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

    //攻撃範囲:Unit.csに移行2025/07
    //[SerializeField] private int _minAttackRange = 1;//最小攻撃射程
    //[SerializeField] private int _maxAttackRange = 2;//最大攻撃射程


    //デバッグ用
    //public Vector2Int EnemyCurrentGridPosition => MapManager.Instance.GetGridPositionFromWorld(transform.position);
    //private List<Vector2Int> currentAttackableTiles = new List<Vector2Int>(); // ★修正: ここでリストを初期化する



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
        //現在地の取得
        Vector2Int targetPos = CurrentGridPosition;

        //プレイヤーユニットの検索
        List<PlayerUnit> playerUnits = MapManager.Instance.GetAllPlayerUnits();

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


        foreach(PlayerUnit player in playerUnits)
        {
            Debug.Log($"  ターゲット候補のプレイヤー: {player.name} 位置: {player.GetCurrentGridPostion()}");

            HashSet<Vector2Int> potentialEnemyMoveToAttackPositions = CalculateEnemyMoveToAttackPositions(player.GetCurrentGridPostion());

            Debug.Log($"    プレイヤー({player.name})を攻撃可能なマス候補数 (敵が移動すべき位置): {potentialEnemyMoveToAttackPositions.Count}");

            // 敵が移動可能なマスの中から、このプレイヤーを攻撃できるマスを探す
            foreach(Vector2Int attackPosCandidate in potentialEnemyMoveToAttackPositions)
            {
                // 候補の攻撃位置が、敵の移動可能範囲内にあり、かつ空きマスであるかチェック
                if (TestreachableNodes.ContainsKey(attackPosCandidate) && !MapManager.Instance.IsTileOccupiedForStooping(attackPosCandidate, this)){
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
        if(targetedPlayer != null && minCostToAttackPos != int.MaxValue)
            {
            // 移動コストが現在の移動力以下であることを確認
            if(minCostToAttackPos <= CurrentMovementPoints)
            {
                //座標データの移動処理
                Tile newTile = MapManager.Instance.GetTileAt(bestMoveTargetPos);
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

                foreach (Vector2Int animation in AnimationPath)
                {
                    Debug.LogWarning(animation);
                }

                
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
                }
                else
                {
                    Debug.Log($"{name}: 経路計算に失敗したか、既に最適な位置にいます。目標位置: {bestMoveTargetPos} (経路なし)");
                    
                    //確認用
                    Debug.LogWarning($"{name}: ここで攻撃ここで攻撃");
                    targetedPlayer.Die();

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
   /// </summary>
   /// <returns></returns>
    private IEnumerator HandleAggressiveAItest()
    {
        PlayerUnit targetPlayer = GetClosestPlayerUnit();
        if(targetPlayer == null)
        {
            Debug.Log($"{UnitName}: ターゲットとなるプレイヤーユニットが見つかりません");
            yield break;
        }

        HashSet<Vector2Int> attackableTiles = CalculateAttackRange(targetPlayer.CurrentGridPosition);

        if (attackableTiles.Contains(targetPlayer.CurrentGridPosition))
        {
            Debug.LogWarning($"{UnitName}: ターゲット ({targetPlayer.UnitName}) が攻撃範囲内にいます、攻撃します");
            yield return StartCoroutine(PerformAttack(targetPlayer));
        }
        else
        {
            Debug.Log($"{UnitName}: ターゲット ({targetPlayer.UnitName}) が攻撃範囲外です、移動を試みます");
            yield return StartCoroutine(PerformAggressiveMoveToAttackRange(targetPlayer)); 
        }

        yield break;
    }

    /// <summary>
    /// 好戦的なAIの移動ロジック
    /// ターゲットプレイヤーに最も近づくように移動する
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <returns></returns>
    private IEnumerator PerformAggressiveMoveToAttackRange(PlayerUnit targetPlayer)
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
            int dist = Mathf.Abs(moveCandidate.x - targetPlayer.CurrentGridPosition.x) + 
                Mathf.Abs(moveCandidate.y - targetPlayer.CurrentGridPosition.y);

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

        Tile targetTile = MapManager.Instance.GetTileAt(targetPos);
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

        foreach(PlayerUnit player in MapManager.Instance.GetAllPlayerUnits())
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
