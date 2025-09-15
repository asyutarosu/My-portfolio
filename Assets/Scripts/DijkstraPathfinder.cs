using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// �_�C�N�X�g���@�ɂ��o�H�T���m�[�h
/// </summary>
public class PathNodes
{
    public Vector2Int Position;//�O���b�h���W
    public int Cost;           //�J�n�n�_����̗ݐσR�X�g
    public PathNodes Parent;    //�e�m�[�h�i�o�H�č\�z�p�j

    public PathNodes(Vector2Int postion, int cost, PathNodes parent)
    {
        Position = postion;
        Cost = cost;
        Parent = parent;
    }
}

/// <summary>
/// �D��x�L���[�Ŏg�p����m�[�h�̃��b�p�[
/// </summary>
public struct PriorityQueueNode : System.IComparable<PriorityQueueNode>
{
    public PathNodes Node;
    public int Priority; // Cost�����̂܂�Priority�ɂȂ�

    public PriorityQueueNode(PathNodes node, int priority)
    {
        Node = node;
        Priority = priority;
    }

    // IComparable�C���^�[�t�F�[�X�̎���
    public int CompareTo(PriorityQueueNode other)
    {
        // �D��x�i�R�X�g�j���������قǗD�悳���
        return Priority.CompareTo(other.Priority);
    }
}


/// <summary>
/// �_�C�N�X�g���@��p���āA�J�n�n�_���瓞�B�\�ȑS�Ẵ}�X�ւ̍ŒZ�R�X�g���v�Z����
/// </summary>
public static class DijkstraPathfinder
{
    //�ړ������i�㉺���E�j
    private static readonly Vector2Int[] _directions = new Vector2Int[]
    {
        new Vector2Int(0, 1), //��
        new Vector2Int(0, -1),//��
        new Vector2Int(1, 0), //�E
        new Vector2Int(-1, 0) //��
    };

    /// <summary>
    /// �_�C�N�X�g���@�ɂ��o�H�T���m�[�h
    /// </summary>
    public class PathNode
    {
        public Vector2Int Position;//�O���b�h���W
        public int Cost;           //�J�n�n�_����̗ݐσR�X�g
        public PathNode Parent;    //�e�m�[�h�i�o�H�č\�z�p�j

        public PathNode(Vector2Int postion, int cost, PathNode parent)
        {
            Position = postion;
            Cost = cost;
            Parent = parent;
        }
    }

    /// <summary>
    /// �_�C�N�X�g���@��p���āA�J�n�n�_���瓞�B�\�ȑS�Ẵ^�C���Ƃ��̍ŏ��ړ��R�X�g���v�Z����
    /// </summary>
    /// <param name="satrtPos">�J�n�O���b�h���W</param>
    /// <param name="unit">�ړ����郆�j�b�g</param>
    /// <returns>���B�\�ȃ^�C���̃O���b�h���W�Ƃ���ɑΉ�����PathNode��Dictionary</returns>
    public static Dictionary<Vector2Int, PathNodes> FindReachableNodes(Vector2Int startPos, Unit unit)
    {
        var distances = new Dictionary<Vector2Int, int>();  //�e�^�C���ւ̍ŏ��R�X�g
        var visited = new HashSet<Vector2Int>();            //�K��ς݃^�C�����L�^
        var nodes = new Dictionary<Vector2Int, PathNodes>(); //�e�O���b�h�ʒu�ɑΉ�����PathNodes��ۑ�

        //�D��x�L���[�F�i�m�[�h�A�D��x�j�̃y�A���Ǘ����A�D��x�i�R�X�g�j���ŏ��̂��̂�擪�Ɏ擾
        var priorityQueue = new PriorityQueue<PriorityQueueNode>();

        // �J�n�n�_�̃m�[�h��������
        PathNodes startNode = new PathNodes(startPos, 0, null);
        distances[startPos] = 0;
        nodes[startPos] = startNode;
        priorityQueue.Enqueue(new PriorityQueueNode(startNode,0));


        while (priorityQueue.Count > 0)
        {
            PriorityQueueNode pqNode = priorityQueue.Dequeue();
            PathNodes currentNode = pqNode.Node;
            int currentPriority = pqNode.Priority;


            // ���ɖK��ς݂̃m�[�h�̓X�L�b�v�i���Z���o�H���������Ă��邽�߁j
            if (visited.Contains(currentNode.Position))
            {
                continue;
            }

            visited.Add(currentNode.Position);

            // ���j�b�g�̌��݂̈ړ��͂𒴂���R�X�g�̃m�[�h�͂���ȏ�T�����Ȃ�
            // �i�������AFindPath�ōő�ړ��͈ȏ�̌o�H���T���������ꍇ�͂��̏����𒲐�����j
            if (currentNode.Cost > unit.CurrentMovementPoints)
            {
                continue;
            }

            // �l������T��
            foreach (Vector2Int direction in _directions)
            {
                Vector2Int neighborPos = currentNode.Position + direction;

                // �}�b�v�͈͊O�̍��W�̓X�L�b�v
                if (!MapManager.Instance.IsValidGridPosition(neighborPos))
                {
                    continue;
                }

                MyTile neighborTile = MapManager.Instance.GetTileAt(neighborPos);
                if (neighborTile == null)
                {
                    continue;
                }

                // ���Ƀ��j�b�g����L���Ă���^�C���ɂ͈ړ��ł��Ȃ��i�v���C���[���j�b�g�܂��͓G���j�b�g�j
                if (!MapManager.Instance.IsTilePassableForUnit(neighborPos, unit))
                {
                    continue;
                }

                //���\�b�h�ǉ��ɂ��ύX2025/07
                // ���Ƀ��j�b�g����L���Ă���^�C���ɂ͈ړ��ł��Ȃ��i�v���C���[���j�b�g�܂��͓G���j�b�g�j
                // �GAI���������j�b�g������邽�߂ɂ������`�F�b�N
                //if (neighborTile.OccupyingUnit != null) // ��L���j�b�g������ꍇ
                //{
                //    // ��L���j�b�g���ړ����郆�j�b�g�Ɠ����h���̏ꍇ���ړ��ł��Ȃ��i�������A�����AI�ł͋�����ꍇ������j
                //    // �����ł̓V���v���ɁA���̃��j�b�g������ꍇ�͈ړ��ł��Ȃ��Ƃ���
                //    continue;
                //}


                // �n�`�ɂ��ړ��R�X�g���擾
                //int movementCost = MapManager.Instance.GetMovementCost(neighborPos, unit.Type);
                int movementCost = MapManager.Instance.GetMovementCost(neighborPos, unit.Type);
                if (movementCost == int.MaxValue) // �ړ��s�\�Ȓn�`�i�R�X�g��������̏ꍇ�j
                {
                    continue;
                }

                int newCost = currentNode.Cost + movementCost;

                // �V�����R�X�g�����݂̍ŏ��R�X�g��菬�����ꍇ�A�܂��͖��K��̏ꍇ
                if (newCost <= unit.CurrentMovementPoints)
                {
                    if (!distances.ContainsKey(neighborPos) || newCost < distances[neighborPos])
                    {
                        distances[neighborPos] = newCost;
                        PathNodes neighborNode = new PathNodes(neighborPos, newCost, currentNode);
                        nodes[neighborPos] = neighborNode; // �m�[�h�����X�V
                        priorityQueue.Enqueue(new PriorityQueueNode(neighborNode, newCost));
                    }
                }
            }
        }
        return nodes;
    }

    /// <summary>
    /// �J�n�n�_����ړI�n�_�܂ł̍ŒZ�o�H�����X�g�ŕԂ��B
    /// �iFindReachableNodes�Ŏ擾�����e�m�[�h�����g�p�j
    /// </summary>
    /// <param name="startPos">�J�n�O���b�h���W</param>
    /// <param name="targetPos">�ړI�O���b�h���W</param>
    /// <param name="unit">�ړ����郆�j�b�g</param>
    /// <returns>�ŒZ�o�H�̃O���b�h���W���X�g�B�o�H��������Ȃ��ꍇ��null�B</returns>
    public static List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos, Unit unit)
    {
        // �e�m�[�h�����܂ށA�J�n�n�_���瓞�B�\�ȑS�Ẵm�[�h���擾
        Dictionary<Vector2Int, PathNodes> reachableNodes = FindReachableNodes(startPos, unit);

        // �ړI�n�_�̃m�[�h�����݂��邩�m�F
        if (!reachableNodes.ContainsKey(targetPos))
        {
            return null; // �ړI�n�_�ɓ��B�ł��Ȃ�
        }

        // �o�H���č\�z
        List<Vector2Int> path = new List<Vector2Int>();
        PathNodes currentNode = reachableNodes[targetPos];

        // �t���ɐe��H���Čo�H���\�z
        // �ڕW�n�_����J�n�n�_�܂ł����̂ڂ邽�߁A�J�n�n�_�̃m�[�h���p�X�Ɋ܂܂��
        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse(); // �o�H�𐳂��������ɂ���i�J�n�n�_����ړI�n�_�ցj

        // �ŏ��̗v�f�͊J�n�n�_�Ȃ̂ŁA�ړ��p�X�Ƃ��Ă͕s�v�ȏꍇ�������B
        // Unity��PathFollower�Ȃǂɂ���Ă͊܂߂�ꍇ�����邽�߁A�����ł͎c���B
        // �����K�v�Ȃ���� path.RemoveAt(0); ��ǉ����āA���݂̈ʒu�͊܂߂Ȃ��悤�ɂ���B
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
                throw new System.InvalidOperationException("�L���[�͋�ł�");
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
                smallestChildIndex = currentIndex;//�ŏ��̎q�̃C���f�b�N�X������

                //���̎q�����݂��A���ꂪ���݂̍ŏ����D��x�������ꍇ
                if (leftChildIndex < heap.Count && heap[leftChildIndex].CompareTo(heap[smallestChildIndex]) < 0)
                {
                    smallestChildIndex = leftChildIndex;
                }

                //�E�̎q�����݂��A���ꂪ���݂̍ŏ����D��x�������ꍇ
                if (rightChildIndex < heap.Count && heap[rightChildIndex].CompareTo(heap[smallestChildIndex]) < 0)
                {
                    smallestChildIndex = rightChildIndex;
                }

                // �����ŏ��̎q�����݂̈ʒu�ƈقȂ�ꍇ�A�������ĉ��ɒH��
                if (smallestChildIndex != currentIndex)
                {
                    //Swap(currentIndex, smallestChildIndex);
                    currentIndex = smallestChildIndex; // �q�̈ʒu�Ɉړ����A����ɉ��ɒH��
                }
                else
                {
                    break; // �q�[�v�̃v���p�e�B���������ꂽ
                }
            }
        }

        /// <summary>
        /// ���X�g����2�̗v�f���������郁�\�b�h
        /// </summary>
        private void Swap(int index1, int index2)
        {
            T temp  = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = temp;
        }
    }





    /// <summary>
    /// �_�C�N�X�g���@��p���āA�J�n�n�_���瓞�B�\�ȑS�Ẵ^�C���Ƃ��̍ŏ��ړ��R�X�g���v�Z����
    /// </summary>
    /// <param name="satrtPos">�J�n�O���b�h���W</param>
    /// <param name="unit">�ړ����郆�j�b�g</param>
    /// <returns>���B�\�ȃ^�C���̍��W�ƍŏ��ړ��R�X�g</returns>
    public static Dictionary<Vector2Int,PathNode> FindReachableTiles(Vector2Int startPos, Unit unit)
    {
        Dictionary<Vector2Int,PathNode> visiteNodes = new Dictionary<Vector2Int,PathNode>();//�e�^�C���ւ̍ŏ��R�X�g�ƌo�H����ێ�����

        PathNodePriorityQueue frontier = new PathNodePriorityQueue();//�T������m�[�h�̗D��x�L���[

        //�J�n�m�[�h���쐬���A������
        PathNode startNode = new PathNode(startPos, 0, null);
        frontier.Enqueue(startNode, 0);
        visiteNodes[startPos] = startNode;


        //�D��x�L���[����ɂȂ�܂ŒT���𑱂���
        while(frontier.Count > 0)
        {
            //�ł��R�X�g�̒Ⴂ�m�[�h�����o��
            PathNode current = frontier.Dequeue();

            //���o�����m�[�h�̃R�X�g���A���ɋL�^����Ă��邻�̃m�[�h�ւ̍ŏ��R�X�g�����傫���ꍇ�̓X�L�b�v
            if (current.Cost > visiteNodes[current.Position].Cost)
            {
                continue;
            }

            //���j�b�g�̍ő�ړ��͂��R�X�g��������΁A����ȏケ�̌o�H��T�����Ȃ�
            if (current.Cost > unit.CurrentMovementPoints)
            {
                continue;
            }

            //�אڃm�[�h���㉺���E4�����ŒT��
            foreach(var dir in _directions)
            {
                Vector2Int nextPos = current.Position + dir;
                MyTile nextTile = MapManager.Instance.GetTileAt(nextPos);

                //�͈͊O�܂��̓��j�b�g����L���Ă���}�X
                if(nextTile == null || (nextTile.OccupyingUnit != null && nextTile.OccupyingUnit.Faction != unit.Faction))
                {
                    continue;
                }

                //int movementCost = MapManager.Instance.GetMovementCost(nextPos, unit.Type);
                int movementCost = MapManager.Instance.GetMovementCost(nextPos, unit.Type);

                //�ړ��s�Ȓn�`
                if (movementCost == int.MaxValue)
                {
                    continue;
                }

                int newCost = current.Cost + movementCost;

                //�V�����R�X�g�����j�b�g�̍ő�ړ��͈ȓ��ł��邱��
                //���A
                //����nextPos���܂��T������Ă��Ȃ�(visitedNodes�Ɋ܂܂�Ă��Ȃ�)���A
                //�܂��́A�V�����R�X�g������܂łɋL�^���ꂽnextPos�ւ̍ŏ��R�X�g�����������ꍇ
                if(newCost <= unit.CurrentMovementPoints && (!visiteNodes.ContainsKey(nextPos) || newCost < visiteNodes[nextPos].Cost))
                {
                    //visitedNodes��V�����ŏ��R�X�g�ƌo�H���ōX�V
                    PathNode nextNode = new PathNode(nextPos, newCost, current);
                    visiteNodes[nextPos] = nextNode;

                    //�D��x�L���[�ɐV�����m�[�h��ǉ�
                    frontier.Enqueue(nextNode, newCost);
                }
            }
        }
        return visiteNodes;
    }

    

    /// <summary>
    /// �v�Z���ꂽ���B�\�ȃ^�C����񂩂�A�ڕW�n�_�ւ̍ŒZ�o�H���č\�z���ĕԂ�
    /// </summary>
    /// <param name="startPos">�J�n�O���b�h���W</param>
    /// <param name="target">�ڕW�O���b�h���W</param>
    /// <param name="unit">�ړ����郆�j�b�g</param>
    /// <returns>�ŒZ�o�H�̃O���b�h���W���X�g�i�J�n�n�_�ƖڕW�n�_���܂�</returns>
    public static List<Vector2Int> GetPathToTarget(Vector2Int startPos, Vector2Int targetPos ,Unit unit)
    {
        Dictionary<Vector2Int, PathNode> allReachableNodes = FindReachableTiles(startPos, unit);

        if (!allReachableNodes.ContainsKey(targetPos))
        {
            Debug.Log($"�o�H��������܂���ł����F{startPos}����{targetPos}");
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

        Debug.Log($"�o�H��������܂����F{startPos}����{targetPos}({path.Count}�}�X)");
        return path;
    }

    

    /// <summary>
    /// PathNode�I�u�W�F�N�g��D��x�t���ŊǗ����邽�߂̗D��x�L���[
    /// �q�[�v�x�[�X�ł̍쐬
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
        /// �m�[�h��D��x�t���ŃL���[�ɒǉ�����
        /// </summary>
        /// <param name="node">�ǉ�����PathNode</param>
        /// <param name="priority">�m�[�h�̗D��x</param>
        public void Enqueue(PathNode node, int priority)
        {
            heap.Add((node,priority));//���X�g�̖����ɒǉ�
            HeapifyUp(heap.Count - 1);//�q�[�v�̃v���p�e�B���ێ����邽�߂ɏ�ɒH��

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
        /// �ł��D��x�������m�[�h�����o��
        /// </summary>
        /// <returns>���o���ꂽPathNode</returns>
        /// <exception cref="System.InvalidOperationException">�L���[����̏ꍇ</exception>
        public PathNode Dequeue()
        {
            if(heap.Count == 0)
            {
                throw new System.InvalidOperationException("PriorityQueue is empty");
            }

            //�ł��D��x�̍������[�g����
            //(PathNode node, int priority) result = heap[0];

            //�Ō�̗v�f�����[�g�Ɉړ����A���̃��[�g���폜
            PathNode result = heap[0].node;
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            HeapifyDown(0);

            if(heap.Count > 0)
            {
                HeapifyDown(0);//�V�������[�g����q�[�v�̃v���p�e�B���ێ����邽�߂ɉ��ɒH��
            }
            return result;
        }

        /// <summary>
        /// �v�f�����X�g�̖����ɒǉ�������A�q�[�v�̃v���p�e�B���ێ����邽�߂ɐe�Ɣ�r���Ȃ����Ɉړ�������
        /// </summary>
        private void HeapifyUp(int currentIndex)
        {
            while (currentIndex > 0)
            {
                int parentIndex = (currentIndex - 1) / 2;//�e�̃C���f�b�N�X���v�Z

                //���݂̗v�f�̗D��x���e�̗D��x��荂���ꍇ
                if (heap[currentIndex].priority < heap[parentIndex].priority)
                {
                    //�e�Ǝq������
                    Swap(currentIndex, parentIndex);
                }
                else
                {
                    break;//�q�[�v�̃v���p�e�B���������ꂽ
                }
            }
        }

        /// <summary>
        /// ���[�g�v�f���폜������A�q�[�v�̃v���p�e�B���ێ����邽�߂Ɏq�Ɣ�r���Ȃ��牺�Ɉړ�������
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
                smallestChildIndex = currentIndex;//�ŏ��̎q�̃C���f�b�N�X������

                //���̎q�����݂��A���ꂪ���݂̍ŏ����D��x�������ꍇ
                if(leftChildIndex < heap.Count && heap[leftChildIndex].priority < heap[smallestChildIndex].priority)
                {
                    smallestChildIndex = leftChildIndex;
                }

                //�E�̎q�����݂��A���ꂪ���݂̍ŏ����D��x�������ꍇ
                if(rightChildIndex < heap.Count && heap[rightChildIndex].priority < heap[smallestChildIndex].priority)
                {
                    smallestChildIndex = rightChildIndex;
                }

                //�����ŏ��̎q�����݂̈ʒu�ƈقȂ�ꍇ�A�������ĉ��ɒH��
                if(smallestChildIndex != currentIndex)
                {
                    Swap(currentIndex, smallestChildIndex);
                    currentIndex = smallestChildIndex;//�q�̈ʒu�Ɉړ����A����ɉ��ɒH��
                }
                else
                {
                    break;//�q�[�v�̃v���p�e�B���������ꂽ
                }
            }
        }

        /// <summary>
        /// ���X�g����2�̗v�f���������郁�\�b�h
        /// </summary>
        private void Swap(int index1, int index2)
        {
            (PathNode node, int priority) temp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = temp;
        }
    }

    /////////////////////////////////�GAI�̏����p�t�H�[�}���X�̌���̂���A-Star�𓱓�
    
    ///
    /// <summary>
    /// A-Star�@��p���āA�J�n�n�_���瓞�B�\�ȑS�Ẵ^�C���Ƃ��̍ŏ��ړ��R�X�g���v�Z����
    /// </summary>
    /// <param name="startPos">�J�n�O���b�h���W</param>
    /// <param name="unit">�ړ����郆�j�b�g</param>
    /// <param name="targetPos">�ڕW�O���b�h���W (A-Star�@�ł͒T���̍œK���ɗ��p)</param>
    /// <returns>���B�\�ȃ^�C���̍��W�ƍŏ��ړ��R�X�g</returns>
    public static Dictionary<Vector2Int, PathNode> FindReachableTiles_Astar(Vector2Int startPos, Vector2Int targetPos, Unit unit)
    {
        Dictionary<Vector2Int, PathNode> visitedNodes = new Dictionary<Vector2Int, PathNode>();
        PathNodePriorityQueue frontier = new PathNodePriorityQueue();

        PathNode startNode = new PathNode(startPos, 0, null);
        // �D��x�ɂ� g(n) + h(n) ���g�p
        int startPriority = startNode.Cost + GetManhattanDistance_Astar(startPos, targetPos);
        frontier.Enqueue(startNode, startPriority);
        visitedNodes[startPos] = startNode;

        while (frontier.Count > 0)
        {
            PathNode current = frontier.Dequeue();

            if (current.Cost > visitedNodes[current.Position].Cost)
            {
                continue;
            }

            if (current.Cost > unit.CurrentMovementPoints)
            {
                continue;
            }

            foreach (var dir in _directions)
            {
                Vector2Int nextPos = current.Position + dir;
                MyTile nextTile = MapManager.Instance.GetTileAt(nextPos);

                if (nextTile == null || (nextTile.OccupyingUnit != null && nextTile.OccupyingUnit.Faction != unit.Faction))
                {
                    continue;
                }

                int movementCost = MapManager.Instance.GetMovementCost(nextPos, unit.Type);

                if (movementCost == int.MaxValue)
                {
                    continue;
                }

                int newCost = current.Cost + movementCost;

                // �V�����R�X�g�����j�b�g�̍ő�ړ��͈ȓ��ł��邱��
                if (newCost <= unit.CurrentMovementPoints)
                {
                    if (!visitedNodes.ContainsKey(nextPos) || newCost < visitedNodes[nextPos].Cost)
                    {
                        PathNode nextNode = new PathNode(nextPos, newCost, current);
                        visitedNodes[nextPos] = nextNode;

                        // A-Star�̗D��x���v�Z
                        int heuristicCost = GetManhattanDistance_Astar(nextPos, targetPos);
                        int newPriority = newCost + heuristicCost;

                        frontier.Enqueue(nextNode, newPriority);
                    }
                }
            }
        }
        return visitedNodes;
    }

    /// <summary>
    /// �}���n�b�^���������v�Z����q���[���X�e�B�b�N�֐�
    /// </summary>
    private static int GetManhattanDistance_Astar(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }
}
