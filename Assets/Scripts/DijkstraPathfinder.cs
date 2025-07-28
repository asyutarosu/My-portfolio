using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// ダイクストラ法による経路探索ノード
/// </summary>
public class PathNodes
{
    public Vector2Int Position;//グリッド座標
    public int Cost;           //開始地点からの累積コスト
    public PathNodes Parent;    //親ノード（経路再構築用）

    public PathNodes(Vector2Int postion, int cost, PathNodes parent)
    {
        Position = postion;
        Cost = cost;
        Parent = parent;
    }
}

/// <summary>
/// 優先度キューで使用するノードのラッパー
/// </summary>
public struct PriorityQueueNode : System.IComparable<PriorityQueueNode>
{
    public PathNodes Node;
    public int Priority; // CostがそのままPriorityになる

    public PriorityQueueNode(PathNodes node, int priority)
    {
        Node = node;
        Priority = priority;
    }

    // IComparableインターフェースの実装
    public int CompareTo(PriorityQueueNode other)
    {
        // 優先度（コスト）が小さいほど優先される
        return Priority.CompareTo(other.Priority);
    }
}


/// <summary>
/// ダイクストラ法を用いて、開始地点から到達可能な全てのマスへの最短コストを計算する
/// </summary>
public static class DijkstraPathfinder
{
    //移動方向（上下左右）
    private static readonly Vector2Int[] _directions = new Vector2Int[]
    {
        new Vector2Int(0, 1), //上
        new Vector2Int(0, -1),//下
        new Vector2Int(1, 0), //右
        new Vector2Int(-1, 0) //左
    };

    /// <summary>
    /// ダイクストラ法による経路探索ノード
    /// </summary>
    public class PathNode
    {
        public Vector2Int Position;//グリッド座標
        public int Cost;           //開始地点からの累積コスト
        public PathNode Parent;    //親ノード（経路再構築用）

        public PathNode(Vector2Int postion, int cost, PathNode parent)
        {
            Position = postion;
            Cost = cost;
            Parent = parent;
        }
    }

    /// <summary>
    /// ダイクストラ法を用いて、開始地点から到達可能な全てのタイルとその最小移動コストを計算する
    /// </summary>
    /// <param name="satrtPos">開始グリッド座標</param>
    /// <param name="unit">移動するユニット</param>
    /// <returns>到達可能なタイルのグリッド座標とそれに対応するPathNodeのDictionary</returns>
    public static Dictionary<Vector2Int, PathNodes> FindReachableNodes(Vector2Int startPos, Unit unit)
    {
        var distances = new Dictionary<Vector2Int, int>();  //各タイルへの最小コスト
        var visited = new HashSet<Vector2Int>();            //訪問済みタイルを記録
        var nodes = new Dictionary<Vector2Int, PathNodes>(); //各グリッド位置に対応するPathNodesを保存

        //優先度キュー：（ノード、優先度）のペアを管理し、優先度（コスト）が最小のものを先頭に取得
        var priorityQueue = new PriorityQueue<PriorityQueueNode>();

        // 開始地点のノードを初期化
        PathNodes startNode = new PathNodes(startPos, 0, null);
        distances[startPos] = 0;
        nodes[startPos] = startNode;
        priorityQueue.Enqueue(new PriorityQueueNode(startNode,0));


        while (priorityQueue.Count > 0)
        {
            PriorityQueueNode pqNode = priorityQueue.Dequeue();
            PathNodes currentNode = pqNode.Node;
            int currentPriority = pqNode.Priority;


            // 既に訪問済みのノードはスキップ（より短い経路が見つかっているため）
            if (visited.Contains(currentNode.Position))
            {
                continue;
            }

            visited.Add(currentNode.Position);

            // ユニットの現在の移動力を超えるコストのノードはそれ以上探索しない
            // （ただし、FindPathで最大移動力以上の経路も探索したい場合はこの条件を調整する）
            if (currentNode.Cost > unit.CurrentMovementPoints)
            {
                continue;
            }

            // 四方向を探索
            foreach (Vector2Int direction in _directions)
            {
                Vector2Int neighborPos = currentNode.Position + direction;

                // マップ範囲外の座標はスキップ
                if (!MapManager.Instance.IsValidGridPosition(neighborPos))
                {
                    continue;
                }

                Tile neighborTile = MapManager.Instance.GetTileAt(neighborPos);
                if (neighborTile == null)
                {
                    continue;
                }

                // 既にユニットが占有しているタイルには移動できない（プレイヤーユニットまたは敵ユニット）
                if (!MapManager.Instance.IsTilePassableForUnit(neighborPos, unit))
                {
                    continue;
                }

                //メソッド追加による変更2025/07
                // 既にユニットが占有しているタイルには移動できない（プレイヤーユニットまたは敵ユニット）
                // 敵AIが味方ユニットを避けるためにここをチェック
                //if (neighborTile.OccupyingUnit != null) // 占有ユニットがいる場合
                //{
                //    // 占有ユニットが移動するユニットと同じ派閥の場合も移動できない（ただし、特定のAIでは許可する場合もあり）
                //    // ここではシンプルに、他のユニットがいる場合は移動できないとする
                //    continue;
                //}


                // 地形による移動コストを取得
                int movementCost = MapManager.Instance.GetMovementCost(neighborPos, unit.Type);
                if (movementCost == int.MaxValue) // 移動不可能な地形（コストが無限大の場合）
                {
                    continue;
                }

                int newCost = currentNode.Cost + movementCost;

                // 新しいコストが現在の最小コストより小さい場合、または未訪問の場合
                if (newCost <= unit.CurrentMovementPoints)
                {
                    if (!distances.ContainsKey(neighborPos) || newCost < distances[neighborPos])
                    {
                        distances[neighborPos] = newCost;
                        PathNodes neighborNode = new PathNodes(neighborPos, newCost, currentNode);
                        nodes[neighborPos] = neighborNode; // ノード情報を更新
                        priorityQueue.Enqueue(new PriorityQueueNode(neighborNode, newCost));
                    }
                }
            }
        }
        return nodes;
    }

    /// <summary>
    /// 開始地点から目的地点までの最短経路をリストで返す。
    /// （FindReachableNodesで取得した親ノード情報を使用）
    /// </summary>
    /// <param name="startPos">開始グリッド座標</param>
    /// <param name="targetPos">目的グリッド座標</param>
    /// <param name="unit">移動するユニット</param>
    /// <returns>最短経路のグリッド座標リスト。経路が見つからない場合はnull。</returns>
    public static List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos, Unit unit)
    {
        // 親ノード情報を含む、開始地点から到達可能な全てのノードを取得
        Dictionary<Vector2Int, PathNodes> reachableNodes = FindReachableNodes(startPos, unit);

        // 目的地点のノードが存在するか確認
        if (!reachableNodes.ContainsKey(targetPos))
        {
            return null; // 目的地点に到達できない
        }

        // 経路を再構築
        List<Vector2Int> path = new List<Vector2Int>();
        PathNodes currentNode = reachableNodes[targetPos];

        // 逆順に親を辿って経路を構築
        // 目標地点から開始地点までさかのぼるため、開始地点のノードもパスに含まれる
        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse(); // 経路を正しい順序にする（開始地点から目的地点へ）

        // 最初の要素は開始地点なので、移動パスとしては不要な場合が多い。
        // UnityのPathFollowerなどによっては含める場合もあるため、ここでは残す。
        // もし必要なければ path.RemoveAt(0); を追加して、現在の位置は含めないようにする。
        return path;
    }


    public class PriorityQueue<T> where T : System.IComparable<T>
    {
        private List<T> heap;

        public int Count => heap.Count;

        public PriorityQueue()
        {
            heap = new List<T>();
        }

        public void Enqueue(T item)
        {
            heap.Add(item);
            int currentIndex = heap.Count - 1;
            while(currentIndex > 0)
            {
                int parentIndex = (currentIndex - 1) / 2;
                if(heap[currentIndex].CompareTo(heap[parentIndex]) < 0)
                {
                    currentIndex = parentIndex;

                }
                else
                {
                    break;
                }
            }
        }

        public T Dequeue()
        {
            if(heap.Count == 0)
            {
                throw new System.InvalidOperationException("キューは空です");
            }

            T item = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            HeapifyDown(0);
            return item;
        }

        private void HeapifyDown(int currentIndex)
        {
            int leftChildIndex;
            int rightChildIndex;
            int smallestChildIndex;

            while (true)
            {
                leftChildIndex = 2 * currentIndex + 1;
                rightChildIndex = 2 * currentIndex + 2;
                smallestChildIndex = currentIndex;//最小の子のインデックスを仮定

                //左の子が存在し、それが現在の最小より優先度が高い場合
                if (leftChildIndex < heap.Count && heap[leftChildIndex].CompareTo(heap[smallestChildIndex]) < 0)
                {
                    smallestChildIndex = leftChildIndex;
                }

                //右の子が存在し、それが現在の最小より優先度が高い場合
                if (rightChildIndex < heap.Count && heap[rightChildIndex].CompareTo(heap[smallestChildIndex]) < 0)
                {
                    smallestChildIndex = rightChildIndex;
                }

                // もし最小の子が現在の位置と異なる場合、交換して下に辿る
                if (smallestChildIndex != currentIndex)
                {
                    //Swap(currentIndex, smallestChildIndex);
                    currentIndex = smallestChildIndex; // 子の位置に移動し、さらに下に辿る
                }
                else
                {
                    break; // ヒープのプロパティが満たされた
                }
            }
        }

        /// <summary>
        /// リスト内の2つの要素を交換するメソッド
        /// </summary>
        private void Swap(int index1, int index2)
        {
            T temp  = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = temp;
        }
    }

    /// <summary>
    /// ダイクストラ法を用いて、開始地点から到達可能な全てのタイルとその最小移動コストを計算する
    /// </summary>
    /// <param name="satrtPos">開始グリッド座標</param>
    /// <param name="unit">移動するユニット</param>
    /// <returns>到達可能なタイルの座標と最小移動コスト</returns>
    public static Dictionary<Vector2Int,PathNode> FindReachableTiles(Vector2Int startPos, Unit unit)
    {
        Dictionary<Vector2Int,PathNode> visiteNodes = new Dictionary<Vector2Int,PathNode>();//各タイルへの最小コストと経路情報を保持する

        PathNodePriorityQueue frontier = new PathNodePriorityQueue();//探索するノードの優先度キュー

        //開始ノードを作成し、初期化
        PathNode startNode = new PathNode(startPos, 0, null);
        frontier.Enqueue(startNode, 0);
        visiteNodes[startPos] = startNode;

        //優先度キューが空になるまで探索を続ける
        while(frontier.Count > 0)
        {
            //最もコストの低いノードを取り出す
            PathNode current = frontier.Dequeue();

            //取り出したノードのコストが、既に記録されているそのノードへの最小コストよりも大きい場合はスキップ
            if (current.Cost > visiteNodes[current.Position].Cost)
            {
                continue;
            }

            //ユニットの最大移動力よりコストが高ければ、それ以上この経路を探索しない
            if (current.Cost > unit.CurrentMovementPoints)
            {
                continue;
            }

            //隣接ノードを上下左右4方向で探索
            foreach(var dir in _directions)
            {
                Vector2Int nextPos = current.Position + dir;
                Tile nextTile = MapManager.Instance.GetTileAt(nextPos);

                //範囲外またはユニットが占有しているマス
                if(nextTile == null || (nextTile.OccupyingUnit != null && nextTile.OccupyingUnit.Faction != unit.Faction))
                {
                    continue;
                }

                int movementCost = MapManager.Instance.GetMovementCost(nextPos, unit.Type);

                //移動不可な地形
                if (movementCost == int.MaxValue)
                {
                    continue;
                }

                int newCost = current.Cost + movementCost;

                //新しいコストがユニットの最大移動力以内であること
                //かつ、
                //そのnextPosがまだ探索されていない(visitedNodesに含まれていない)か、
                //または、新しいコストがこれまでに記録されたnextPosへの最小コストよりも小さい場合
                if(newCost <= unit.CurrentMovementPoints && (!visiteNodes.ContainsKey(nextPos) || newCost < visiteNodes[nextPos].Cost))
                {
                    //visitedNodesを新しい最小コストと経路情報で更新
                    PathNode nextNode = new PathNode(nextPos, newCost, current);
                    visiteNodes[nextPos] = nextNode;

                    //優先度キューに新しいノードを追加
                    frontier.Enqueue(nextNode, newCost);
                }
            }
        }
        return visiteNodes;
    }

    

    /// <summary>
    /// 計算された到達可能なタイル情報から、目標地点への最短経路を再構築して返す
    /// </summary>
    /// <param name="startPos">開始グリッド座標</param>
    /// <param name="target">目標グリッド座標</param>
    /// <param name="unit">移動するユニット</param>
    /// <returns>最短経路のグリッド座標リスト（開始地点と目標地点を含む</returns>
    public static List<Vector2Int> GetPathToTarget(Vector2Int startPos, Vector2Int targetPos ,Unit unit)
    {
        Dictionary<Vector2Int, PathNode> allReachableNodes = FindReachableTiles(startPos, unit);

        if (!allReachableNodes.ContainsKey(targetPos))
        {
            Debug.Log($"経路が見つかりませんでした：{startPos}から{targetPos}");
            return null;
        }

        List<Vector2Int> path = new List<Vector2Int>();

        PathNode current = allReachableNodes[targetPos];
        while(current != null)
        {
            path.Add(current.Position);
            current = current.Parent;
        }

        path.Reverse();

        Debug.Log($"経路が見つかりました：{startPos}から{targetPos}({path.Count}マス)");
        return path;
    }

    

    /// <summary>
    /// PathNodeオブジェクトを優先度付きで管理するための優先度キュー
    /// ヒープベースでの作成
    /// </summary>
    private class PathNodePriorityQueue
    {
        private List<(PathNode node, int priority)> heap = new List<(PathNode, int)>();

        public int Count => heap.Count;

        public PathNodePriorityQueue()
        {
            heap = new List<(PathNode node, int priority)>();
        }

        /// <summary>
        /// ノードを優先度付きでキューに追加する
        /// </summary>
        /// <param name="node">追加するPathNode</param>
        /// <param name="priority">ノードの優先度</param>
        public void Enqueue(PathNode node, int priority)
        {
            heap.Add((node,priority));//リストの末尾に追加
            HeapifyUp(heap.Count - 1);//ヒープのプロパティを維持するために上に辿る

            int currentIndex = heap.Count - 1;
            int parentIndex;

            while(currentIndex >= 0)
            {
                parentIndex = (currentIndex -1) / 2;
                if(heap[currentIndex].priority < heap[parentIndex].priority)
                {
                    Swap(currentIndex, parentIndex);
                    currentIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 最も優先度が高いノードを取り出す
        /// </summary>
        /// <returns>取り出されたPathNode</returns>
        /// <exception cref="System.InvalidOperationException">キューが空の場合</exception>
        public PathNode Dequeue()
        {
            if(heap.Count == 0)
            {
                throw new System.InvalidOperationException("PriorityQueue is empty");
            }

            //最も優先度の高いルート結果
            //(PathNode node, int priority) result = heap[0];

            //最後の要素をルートに移動し、元のルートを削除
            PathNode result = heap[0].node;
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            HeapifyDown(0);

            if(heap.Count > 0)
            {
                HeapifyDown(0);//新しいルートからヒープのプロパティを維持するために下に辿る
            }
            return result;
        }

        /// <summary>
        /// 要素をリストの末尾に追加した後、ヒープのプロパティを維持するために親と比較しながら上に移動させる
        /// </summary>
        private void HeapifyUp(int currentIndex)
        {
            while (currentIndex > 0)
            {
                int parentIndex = (currentIndex - 1) / 2;//親のインデックスを計算

                //現在の要素の優先度が親の優先度より高い場合
                if (heap[currentIndex].priority < heap[parentIndex].priority)
                {
                    //親と子を交換
                    Swap(currentIndex, parentIndex);
                }
                else
                {
                    break;//ヒープのプロパティが満たされた
                }
            }
        }

        /// <summary>
        /// ルート要素を削除した後、ヒープのプロパティを維持するために子と比較しながら下に移動させる
        /// </summary>
        private void HeapifyDown(int currentIndex)
        {
            int leftChildIndex;
            int rightChildIndex;
            int smallestChildIndex;

            while (true)
            {
                leftChildIndex = 2 * currentIndex + 1;
                rightChildIndex = 2 * currentIndex + 2;
                smallestChildIndex = currentIndex;//最小の子のインデックスを仮定

                //左の子が存在し、それが現在の最小より優先度が高い場合
                if(leftChildIndex < heap.Count && heap[leftChildIndex].priority < heap[smallestChildIndex].priority)
                {
                    smallestChildIndex = leftChildIndex;
                }

                //右の子が存在し、それが現在の最小より優先度が高い場合
                if(rightChildIndex < heap.Count && heap[rightChildIndex].priority < heap[smallestChildIndex].priority)
                {
                    smallestChildIndex = rightChildIndex;
                }

                //もし最小の子が現在の位置と異なる場合、交換して下に辿る
                if(smallestChildIndex != currentIndex)
                {
                    Swap(currentIndex, smallestChildIndex);
                    currentIndex = smallestChildIndex;//子の位置に移動し、さらに下に辿る
                }
                else
                {
                    break;//ヒープのプロパティが満たされた
                }
            }
        }

        /// <summary>
        /// リスト内の2つの要素を交換するメソッド
        /// </summary>
        private void Swap(int index1, int index2)
        {
            (PathNode node, int priority) temp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = temp;
        }
    }
}
