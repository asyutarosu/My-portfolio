using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// ���j�b�g�̓������`����񋓌^
/// </summary>
public enum UnitType
{
    Infantry,//����
    Aquatic,//����
    Flying,//��s
    Cavalry,//�R�n
    Heavy,//�d��
    Archer,//�|
    Mountain//�R��
}


public enum EnemyAIType
{
    notEnemy,   //�G���j�b�g�ł͂Ȃ�
    Aggreeive,  //�ϋɓI�Ƀv���C���[��ǂ�
    Stationary, //����̃}�X�ɐN�������܂œ����Ȃ�
    Patrol,     //�p�g���[���^�F�����\��
    DefalutAI      //��Ƀv���C���[���j�b�g��_����{AI
}

public partial class Unit : MonoBehaviour
{
    [field:SerializeField]public string UnitId { get;private set; }//���j�b�g�̃��j�[�NID
    [field:SerializeField]public string UnitName { get; private set; }//���j�b�g��
    [field:SerializeField]public UnitType Type { get; private set; }//���j�b�g�^�C�v
    [field: SerializeField] protected FactionType _factionType = FactionType.Player;//�f�t�H���g�̓v���C���[
    public FactionType Faction => _factionType;
    

    /////
    [field: SerializeField] protected EnemyAIType _enemyAIType { get; private set; }//�G��AI�^�C�v
    public EnemyAIType EnemyAIType => _enemyAIType;

    [field:SerializeField]public int CurrentHP { get; private set; }//���݂�HP
    [field:SerializeField]public int MaxHP { get; private set; }//�ő�HP
    [field: SerializeField] public int BaseMovement { get; private set; }//��b�ړ���
    [field: SerializeField] public int CurrentMovementPoints { get; private set; }//���݂̈ړ���
    [field: SerializeField] public int AttackPower { get; private set; }//�U����
    [field: SerializeField] public int DefensePower { get; private set; }//�h���
    [field: SerializeField]public int Skill { get; private set; }//�Z

    [field: SerializeField]public int Speed { get; private set; }//����

    public bool IsAlive => CurrentHP > 0;//���j�b�g�̎��S����t���O

    //�U���͈�:������
    [SerializeField] public int _minAttackRange = 1;//�ŏ��U���˒�
    [SerializeField] public int _maxAttackRange = 2;//�ő�U���˒�


    [field:SerializeField]protected UnitData unitData { get; private set; }//���j�b�g�f�[�^�擾�p

    [field:SerializeField]public Weapon EquippedWeapon { get; private set; }//�������̕���
    [field: SerializeField] public Vector2Int CurrentGridPosition { get; protected set; }//�}�b�v��̌��݂̃O���b�h���W
    [field: SerializeField] public bool HasActedThisTurn { get; private set; }//���^�[���s���ς݂�

    [field: SerializeField] public int CurrentExperience { get; private set; }//���݂̌o���l
    [field:SerializeField]public int CurrentLevel { get; private set; }//���݂̃��x��

    public MyTile OccupyingTile { get; protected set; }

    //���j�b�g�̑I����Ԃ��Ǘ�
    private SpriteRenderer _spriteRenderer;
    //���j�b�g�̑I����ԂƔ�I�����(�f�o�b�O�p)
    [SerializeField] private Color _selectedColor = Color.blue;//�I����Ԃ̐F
    [SerializeField] private Color _defaultColor = Color.white;//��I����Ԃ̐F
    [SerializeField] private Color _actedColor = Color.gray;//�s���ς݂̏�Ԃ̐F

    //���j�b�g�̎��o�I���̂��߂̗v�f
    [SerializeField] private float _visualMoveSpeed = 5.0f;//���o�I�ړ����x
    


    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if( _spriteRenderer == null)
        {
            Debug.LogError($"{gameObject.name}:SpriteRenderer��������܂���");
        }

        //���f�[�^
        //UnitId = "NoId";

        //UnitData����X�e�[�^�X��ǂݎ��
        if(unitData != null)
        {
            UnitId = unitData.UnitId;
            UnitName  = unitData.UnitName;
            Type = unitData.Type;
            _factionType = unitData.FactionType;
            _enemyAIType = unitData.EnemyAIType;
            CurrentHP = unitData.MaxHP;
            MaxHP = unitData.MaxHP;
            BaseMovement = unitData.BaseMovement;
            CurrentMovementPoints = BaseMovement;
            AttackPower = unitData.BaseAttackPower;
            DefensePower = unitData.BaseDefensePower;

            Debug.Log($"!!!!!::���j�b�g�̐���{_factionType},AI�̃^�C�v{_enemyAIType}");
            Debug.Log($"{UnitName} (ID: {UnitId}) �̃X�e�[�^�X��UnitData����ݒ肵�܂����B" +
                      $"HP: {CurrentHP}/{MaxHP}, �ړ���: {BaseMovement}, �U����: {AttackPower}, �h���: {DefensePower}");
        }
        else
        {
            Debug.LogWarning($"{this.name} �� UnitData �����蓖�Ă��Ă��܂���I�f�t�H���g�l���g�p���܂��B");
            UnitId = "0";
            UnitName= "0";
            MaxHP = 1;
            BaseMovement = 1;
        }

        //�f�o�b�O�p
        if (string.IsNullOrEmpty(UnitId))
        {
            Debug.LogWarning($"Unit:{gameObject.name}��UnitData������������Ă��܂���");
        }
    }

    /// <summary>
    /// ���j�b�g������������
    /// (DataManager���烍�[�h���ꂽUnitData���g�p)
    /// </summary>
    /// <param name="data">���j�b�g�̃}�X�^�[�f�[�^</param>
    public virtual void Initialize(UnitData data)
    {
        UnitId = data.UnitId;
        UnitName = data.UnitName;
        Type = data.Type;
        MaxHP = data.MaxHP;
        CurrentHP = MaxHP;
        BaseMovement = data.BaseMovement;
        CurrentMovementPoints = BaseMovement;
        AttackPower = data.BaseAttackPower;
        DefensePower = data.BaseDefensePower;
        Skill = data.BaseSkill;
        Speed = data.BaseSpeed;

        CurrentExperience = 0;
        CurrentLevel = 1;
        HasActedThisTurn = false;



        //����̏����������Ȃ�
        //���f�[�^2025/06
        EquippedWeapon = new Weapon("SWORD001","������",1,1,100);
    }

    /// <summary>
    /// ���j�b�g�̃O���b�h���W���X�V����
    /// (BatteManager����Ă΂��)
    /// </summary>
    /// <param name="newPosition">�V�����O���b�h���W</param>
    public void UpdatePosition(Vector2Int newPosition){
        //MapManager�ɏ����̎w������\��
        CurrentGridPosition = newPosition;
        UpdateOccupyingTile();
    }

    /// <summary>
    /// �ړ��͂������
    /// </summary>
    /// <param name="cost">�����ړ���</param>
    public void ConsumeMovementPoints(int cost)
    {
        CurrentMovementPoints -= cost;
        if (CurrentMovementPoints < 0)
        {
            CurrentMovementPoints = 0;
        }
        Debug.Log($"�ړ��͂�����܂����B�c��F{CurrentMovementPoints}");
    }

    /// <summary>
    /// ���j�b�g���w�肳�ꂽ�R�X�g�ňړ��\������
    /// </summary>
    /// <param name="cost">�ړ��ɕK�v�ȃR�X�g</param>
    /// <returns>�ړ��\�Ȃ�true</returns>
    public bool CanMove(int cost)
    {
        return CurrentMovementPoints >= cost && !HasActedThisTurn;
    }

    /// <summary>
    /// �_���[�W����
    /// </summary>
    /// <param name="damage">�󂯂�_���[�W</param>
    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        Debug.LogWarning($"���j�b�g��{damage}�_���[�W���󂯂�");
        if(CurrentHP <= 0)
        {
            Debug.LogWarning("���j�b�g��HP���O�ɂȂ�܂���");
            CurrentHP = 0;
            Die();
        }
    }


    /// <summary>
    /// ���j�b�g�̎��S����
    /// </summary>
    public void Die()
    {
        Debug.Log($"{UnitId}{UnitName}�͓|�ꂽ");

        //BattleManager�֎w��������\��
        OnDestroy();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �o���l���l�����A���x���A�b�v������s��
    /// </summary>
    /// <param name="exp">�l���o���l</param>
    public void GainExperience(int exp)
    {
        CurrentExperience += exp;
        Debug.Log($"{UnitId}{UnitName}��{exp}�o���l���l���B���݌o���l�F{CurrentExperience}");
        if(CurrentExperience >= 100)
        {
            CurrentExperience = 0;
            LevelUp();
        }

        //�A�����x���A�b�v�̔���̃��W�b�N�i�v�Ē��j
        /*int requiredExpForNextLevel = CalulateRequiredExp(CurrentLevel + 1);
        while(CurrentExperience >= requiredExpForNextLevel)
        {
            LevelUp();
            requiredExpForNextLevel = CalulateRequiredExp(CurrentLevel + 1);
        }
        */
    }

    /// <summary>
    /// ���x���A�b�v����
    /// </summary>
    private void LevelUp()
    {
        CurrentLevel++;
        //���̃X�e�[�^�X����
        MaxHP += 1;
        AttackPower += 1;
        DefensePower += 1;
        Skill += 1;
        Speed += 1;

        Debug.Log($"{UnitId}{UnitName}�����x���A�b�v�I���x��{CurrentLevel}�ɂȂ�܂���");
    }

    /// <summary>
    /// ���̃��x���ɕK�v�Ȍo���l���v�Z����(�v�Ē�)
    /// </summary>
    /// <param name="level">�v�Z�Ώۂ̃��x��</param>
    /// <return>�K�v�Ȍo���l</return>
    //private int CalculateRequiredExp(int level)
    //{
    //    return 0;
    //}


    /// <summary>
    /// �^�[���J�n���Ɉړ��͂ƍs���ς݃t���O�����Z�b�g����
    /// </summary>
    public void ResetAction()
    {
        HasActedThisTurn = false;
        CurrentMovementPoints = BaseMovement;
        SetSelected(false);
    }

    /// <summary>
    /// ���j�b�g���s���������������Ƃ�ݒ肷��
    /// </summary>
    public void SetActedThisTrun()
    {
        HasActedThisTurn |= true;
        CurrentMovementPoints = 0;
        SetSelected(false );
        UpdateVisualColor();
    }

    /// <summary>
    /// �s���ς݃t���O��ݒ肷��
    /// </summary>
    /// <param name="acted">�s���ς݂�</param>
    public void SetActionTaken(bool acted)
    {
        HasActedThisTurn = acted;
    }

    /// <summary>
    /// �ړ��|�C���g�������l�Ƀ��Z�b�g����
    /// </summary>
    public void ResetMovementPoints()
    {
        CurrentMovementPoints = BaseMovement;
    }

    //��L�^�C�����X�V����
    protected virtual void UpdateOccupyingTile()
    {
        if (MapManager.Instance != null)
        {
            //�Â��^�C�����烆�j�b�g������
            if (OccupyingTile != null)
            {
                OccupyingTile.OccupyingUnit = null;
            }

            //�V�����^�C����ݒ肵�A���j�b�g���L
            OccupyingTile = MapManager.Instance.GetTileAt(CurrentGridPosition);
            if (OccupyingTile != null)
            {
                OccupyingTile.OccupyingUnit = this;//���̃��j�b�g���^�C�����L
            }
        }
    }

    //�f�[�^�X�V�悤�ɕύX2025/07
    //�v���C���[���ړ����m�肵�����̂݌Ăяo�����
    //���j�b�g���ړ������������ۂɁA�V�����^�C�����L���A�Â��^�C������������
    public void MoveToGridPosition(Vector2Int newGridPos, MyTile newTile)
    {
        //�Â��^�C������Q�Ƃ�����
        if(OccupyingTile != null && OccupyingTile.OccupyingUnit == this)
        {
            OccupyingTile.OccupyingUnit = null;
        }

        CurrentGridPosition = newGridPos;
        //���o�I���̍X�V�͕ʂ̃��\�b�h�őΉ�
        //transform.position = MapManager.Instance.GetWorldPositionFromGrid(newGridPos);

        //�V�����^�C����ݒ肵�A��L����
        OccupyingTile = newTile;
        if(OccupyingTile != null)
        {
            OccupyingTile.OccupyingUnit = this;
        }
    }

    //���o�I����S�����郁�\�b�h
    /// <summary>
    /// ���j�b�g�����o�I�Ƀp�X�ɉ����Ĉړ�������B�f�[�^��̈ʒu�͕ύX���Ȃ�
    /// </summary>
    /// <param name="path">�ړ��o�H�̃O���b�h���W���X�g</param>
    public IEnumerator AnimateMove(List<Vector2Int> path)
    {
        if(path == null || path.Count == 0)
        {
            yield break;
        }

        // ���j�b�g�̌��݂̎��o�I�Ȉʒu���A�p�X�̊J�n�_�ɐݒ�
        transform.position = MapManager.Instance.GetWorldPositionFromGrid(path[0]);

        for(int i = 1; i < path.Count; i++)
        {
            Vector3 startPos = MapManager.Instance.GetWorldPositionFromGrid(path[i - 1]);
            Vector3 targetPos = MapManager.Instance.GetWorldPositionFromGrid(path[i]);
            float journeyLength = Vector3.Distance(startPos, targetPos);
            float startTime = Time.time;

            while(Vector3.Distance(transform.position, targetPos)> 0.01f)
            {
                float distCovered = (Time.time - startTime) * _visualMoveSpeed;
                float fractionOfJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(startPos, targetPos, fractionOfJourney);
                yield return null;
            }
            transform.position = targetPos;
        }
        Debug.Log($"{UnitName} �̎��o�I�Ȉړ����������܂����B");
    }



    /// <summary>
    /// ���j�b�g�̌��݂̃O���b�h���W���X�V����
    /// </summary>
    /// <param name="newGridPos">�V�����O���b�h���W</param>
    public void SetGridPosition(Vector2Int newGridPos)
    {
        CurrentGridPosition = newGridPos;
    }

    /// <summary>
    /// ���j�b�g�̌��݂̃O���b�h���W���擾����
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetCurrentGridPostion()
    {
        return CurrentGridPosition;
    }

    /// <summary>
    /// ���j�b�g�̈ړ��͈͂��擾����
    /// </summary>
    public int GetMoveRange()
    {
        return CurrentMovementPoints;
    }

    //���j�b�g�̍s����Ԃ̐F��ݒ肷��
    public void SetSelected(bool isSelected)
    {
        if (_spriteRenderer != null)
        {
            //_spriteRenderer.color = isSelected ? _selectedColor : _defaultColor;

            if (isSelected)
            {
                _spriteRenderer.color = _selectedColor;
            }
            else if (HasActedThisTurn)
            {
                _spriteRenderer.color = _actedColor;
            }
            else
            {
                _spriteRenderer.color = _defaultColor;
            }
        }
    }

    //���j�b�g�̐F���X�V����
    private void UpdateVisualColor()
    {
        if(_spriteRenderer != null)
        {
            if(HasActedThisTurn)
                _spriteRenderer.color = _actedColor;
        }
        else
        {
            _spriteRenderer.color= _defaultColor;
        }
    }

    protected void OnDestroy()
    {
        //���j�b�g���j�󂳂��Ƃ��ɁA��L���Ă����^�C������Q�Ƃ�����
        if (OccupyingTile != null && OccupyingTile.OccupyingUnit == this)
        {
            OccupyingTile.OccupyingUnit = null;
        }
    }

    //�f�o�b�O�p�F�}�E�X�Ń��j�b�g�̍��W�m�F�p
    protected virtual void OnMouseEnter()
    {
        Debug.Log($"Unit:{UnitName},GridPos:{CurrentGridPosition}");
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
