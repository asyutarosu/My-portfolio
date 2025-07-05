using UnityEngine;
using System.Collections.Generic;
using System.Linq;




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
                Tile nextTile = MapManager.Instance.GetTileAt(nextPos);

                //�͈͊O�܂��̓��j�b�g����L���Ă���}�X
                if(nextTile == null || (nextTile.OccupyingUnit != null && nextTile.OccupyingUnit.Faction != unit.Faction))
                {
                    continue;
                }

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
}
