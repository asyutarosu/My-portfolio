using UnityEngine;


//���j�b�g�̃^�C�v�����ʂ���Enum


public class PlayerUnit : Unit
{

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
