using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//���j�b�g�̃^�C�v�����ʂ���Enum


public class PlayerUnit : Unit
{
    [SerializeField] private PlayerUnit _playerUnit;//�Ώۂ̃v���C���[���j�b�g
    private MapManager _tileHighlighter;//MapManager�̃n�C���C�g�֘A�ւ̎Q��

    public static PlayerUnit Instance;

    //public bool IsAlive => CurrentHP > 0;

    //[SerializeField] private int _moveRange = 3;//�ړ��͈�
    //[SerializeField] private int _currentMovementPoints = 3;

    //private Vector2Int _currentPosition;//���݂̃O���b�h���W

    //�v���g�^�C�v�p�ŊȈՓI�ȏ��݂̂��L��2025/06
    //private string _unitName;//���j�b�g�̖��O
    //[SerializeField]private UnitType _unitType = UnitType.Infantry;
    //[SerializeField] private FactionType _factionUnitType = FactionType.Player;

    //�v���g�^�C�v�p�ŊȈՓI�ȏ��݂̂��L��2025/06
    //public int CurretHP { get; private set; } = 10;//����HP


    //���j�b�g�̑I����Ԃ��Ǘ�
    //private SpriteRenderer _spriteRenderer;
    //���j�b�g�̑I����ԂƔ�I�����
    //[SerializeField] private Color _selectedColor = Color.blue;//�I����Ԃ̐F
    //[SerializeField] private Color _defaultColor = Color.white;//��I����Ԃ̐F

    //�O������ړ��͂ƃ^�C�v�̎Q�Ɨp
    //public int MoveRange => _moveRange;
    //public int CurrentMovementPoints => _currentMovementPoints;
    //public UnitType Type => _unitType;

    //���j�b�g����L���Ă���^�C���ւ̎Q��
    //public Tile OccupyingTile { get; private set; }

    /// <summary>
    /// �v���C���[���j�b�g��������
    /// </summary>
    /// <param name="initialGridPos">�����z�u�����O���b�h���W</param>
    /// <param name="name">���j�b�g�̖��O</param>
    public override void Initialize(UnitData data)
    {
        //_currentPosition = initialGridPos;
        //_unitName = name;
        //Debug.Log($"PlayerUnit'{_unitName}'initialized at grid:{_currentPosition}");

        base.Initialize(data);

        _factionType = FactionType.Player;
        Debug.Log($"Player Unit '{UnitName}' (Type: {Type}, Faction: {Faction}) initialized at grid: {CurrentGridPosition}");
    }

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// ���j�b�g�̌��݂̃O���b�h���W���X�V����
    /// </summary>
    /// <param name="newGridPos">�V�����O���b�h���W</param>
    //public void SetGridPosition(Vector2Int newGridPos)
    //{
    //    _currentPosition = newGridPos;
    //}

    /// <summary>
    /// ���j�b�g�̌��݂̃O���b�h���W���擾����
    /// </summary>
    /// <returns></returns>
    //public Vector2Int GetCurrentGridPostion()
    //{
    //    return _currentPosition;
    //}

    /// <summary>
    /// ���j�b�g�̈ړ��͈͂��擾����
    /// </summary>
    //public int GetMoveRange()
    //{
    //    return _moveRange;
    //}



    //�v���g�^�C�v�p�Ō����_�ł͈ړ��݂̂��L��2025/06
    //public void Attack(PlayerUnit target)
    //{
    //    //�U�����W�b�N
    //}

    //���j�b�g�̑I����Ԃ�ݒ肷��
    //public void SetSelected(bool isSelected)
    //{
    //    if(_spriteRenderer != null)
    //    {
    //        _spriteRenderer.color = isSelected ? _selectedColor : _defaultColor;
    //    }
    //}

    //private void UpdateOccupyingTile()
    //{
    //    if(MapManager.Instance != null)
    //    {
    //        //�Â��^�C�����烆�j�b�g������
    //        if(OccupyingTile != null)
    //        {
    //            OccupyingTile.OccupyingUnit = null;
    //        }
    //        //�V�����^�C����ݒ肵�A���j�b�g���L
    //        OccupyingTile = MapManager.Instance.GetTileAt(_currentPosition);
    //        if(OccupyingTile != null)
    //        {
    //            //OccupyingTile.OccupyingUnit = this;//���̃��j�b�g���^�C�����L
    //        }
    //    }
    //}

    //private void OnDestroy()
    //{
    //    //���j�b�g���j�󂳂��Ƃ��ɁA��L���Ă����^�C������Q�Ƃ�����
    //    if(OccupyingTile != null && OccupyingTile.OccupyingUnit == this)
    //    {
    //        OccupyingTile.OccupyingUnit = null;
    //    }
    //}


    //�f�o�b�O�p�F�}�E�X�Ń��j�b�g�̍��W�m�F�p
    //private void OnMouseEnter()
    //{
    //    Debug.Log($"Unit:{_unitName},GridPos:{_currentPosition}");
    //}


    //���j�b�g���I�����ꂽ�ꍇ�ɌĂяo��
    public void OnUnitSelected()
    {
        _tileHighlighter.ClearAllHighlights();//�����̃n�C���C�g���N���A

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableTiles = DijkstraPathfinder.FindReachableTiles(
            _playerUnit.GetCurrentGridPostion(),_playerUnit);

        foreach (Vector2Int pos in reachableTiles.Keys)
        {
            _tileHighlighter.HighlightTile(pos, HighlightType.Move);
        }

        //�U���\�͈͂��v�Z���A�ԐF�Ńn�C���C�g
        ShowAttackRangeHighlight(reachableTiles.Keys.ToList());

        //�m�F
        Debug.LogError("�Ă΂ꂽ��I");
    }

    public void OnUnitActionCompleted()
    {
        _tileHighlighter.ClearAllHighlights();//�S�Ẵn�C���C�g���N���A
    }

    /// <summary>
    /// �U���\�͈͂��n�C���C�g�\������
    /// </summary>
    /// <param name="moveableTiles">�ړ��\�ȃ^�C�����X�g</param>
    private void ShowAttackRangeHighlight(List<Vector2Int> moveableTiles)
    {
        HashSet<Vector2Int> attackableTiles = new HashSet<Vector2Int>();
        Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0), };

        //�e�ړ��\�^�C������1�}�X�אڂ���^�C�����U���\�͈͌��Ƃ��Ēǉ�
        foreach (Vector2Int movePos in moveableTiles)
        {
            foreach(Vector2Int dir in directions)
            {
                Vector2Int attackTargetPos = movePos + dir;
                //�}�b�v�͈͓̔����m�F
                if (MapManager.Instance.IsValidGridPosition(attackTargetPos))
                {
                    attackableTiles.Add(attackTargetPos);
                }
            }
        }

        //�G���j�b�g�����݂���^�C���݂̂�ԐF�n�C���C�g
        foreach(Vector2Int targetPos in attackableTiles)
        {
            MyTile targetTile = MapManager.Instance.GetTileAt(targetPos);
            if(targetTile != null && targetTile.OccupyingUnit != null)
            {
                //���j�b�g���GfactionType�ł��邩�m�F
                if(targetTile.OccupyingUnit.Faction == FactionType.Enemy)
                {
                    _tileHighlighter.HighlightTile(targetPos,HighlightType.Attack);
                }
            }
        }
    }


    /// <summary>
    /// �U���\�͈͂��n�C���C�g�\������
    /// </summary>
    /// <param name="moveableTiles">�ړ��\�ȃ^�C�����X�g</param>
    public void AttackRangeHighlight(Vector2Int currentPos, Unit currentUnit)
    {
        //ClearAllHighlights();

        HashSet<Vector2Int> attackableTiles = new HashSet<Vector2Int>();
        Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0), };

        //�U���͈͎w��̃}���n�b�^���������ł̎���(�܂��etype�Ƃ̘A�g�͖�����)
        //�ꕔ���l�����Ƃ��Ď���2025/06
        int minAttackRange = 2;//�ŏ��˒�
        int maxAttackRange = 2;//�ő�˒�


        for (int x = -maxAttackRange; x <= maxAttackRange; x++)
        {
            for (int y = -maxAttackRange; y <= maxAttackRange; y++)
            {
                //���݂̈ړ��\�^�C��(movePos)����̑��΍��W
                Vector2Int potentialAttackPos = currentPos + new Vector2Int(x, y);

                //�}���n�b�^�������v�Z
                int distance = Mathf.Abs(x) + Mathf.Abs(y);

                if (distance >= minAttackRange && distance <= maxAttackRange)
                {
                    if (MapManager.Instance.IsValidGridPosition(potentialAttackPos))
                    {
                        attackableTiles.Add(potentialAttackPos);
                        Debug.LogWarning($"�e�X�g�p{potentialAttackPos}");
                    }
                }
            }
        }
    }


    /// <summary>
    /// �ł��߂��G���j�b�g���擾����
    /// </summary>
    /// <returns></returns>
    private EnemyUnit GetClosestEnemyUnit()
    {
        EnemyUnit closesetEnemy = null;
        int minDistance = int.MaxValue;

        foreach (EnemyUnit enemy in MapManager.Instance.GetAllEnemyUnits())
        {
            if (enemy == null)
            {
                continue;
            }

            int dist = Mathf.Abs(CurrentGridPosition.x - enemy.CurrentGridPosition.x) +
                Mathf.Abs(CurrentGridPosition.y - enemy.CurrentGridPosition.y);

            if (dist < minDistance)
            {
                minDistance = dist;
                closesetEnemy = enemy;
            }
        }
        return closesetEnemy;
    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tileHighlighter = FindObjectOfType<MapManager>();//�V�[������MapManager���擾
        if( _tileHighlighter == null)
        {
            Debug.LogError("MapManager��������܂���");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
