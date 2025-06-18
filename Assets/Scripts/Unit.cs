using UnityEngine;
using System.Collections.Generic;

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

public partial class Unit : MonoBehaviour
{
    [field:SerializeField]public string UnitId { get;private set; }//���j�b�g�̃��j�[�NID
    [field:SerializeField]public string UnitName { get; private set; }//���j�b�g��
    [field:SerializeField]public UnitType Type { get; private set; }//���j�b�g�^�C�v

    [field:SerializeField]public int CurrentHP { get; private set; }//���݂�HP
    [field:SerializeField]public int MaxHP { get; private set; }//�ő�HP
    [field: SerializeField] public int BaseMovement { get; private set; }//��b�ړ���
    [field: SerializeField] public int CurrentMovementPoints { get; private set; }//���݂̈ړ���
    [field: SerializeField] public int AttackPower { get; private set; }//�U����
    [field: SerializeField] public int DefensePower { get; private set; }//�h���
    [field: SerializeField]public int Skill { get; private set; }//�Z

    [field: SerializeField]public int Speed { get; private set; }//����



    [field:SerializeField]public Weapon EquippedWeapon { get; private set; }//�������̕���
    [field: SerializeField] public Vector2Int CurrentGridPosition { get; private set; }//�}�b�v��̌��݂̃O���b�h���W
    [field: SerializeField] public bool HasActedThisTurn { get; private set; }//���^�[���s���ς݂�

    [field: SerializeField] public int CurrentExperience { get; private set; }//���݂̌o���l
    [field:SerializeField]public int CurrentLevel { get; private set; }//���݂̃��x��

    private void Awake()
    {
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
    public void Initialized(UnitData data)
    {
        UnitId = data.UnitId;
        UnitName = data.UnitName;
        Type = data.Type;
        MaxHP = data.BaseHP;
        CurrentHP = MaxHP;
        BaseMovement = data.BaseMovement;
        //CurrentMovementPoints = BaseMovement;
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
    public void UpdatePosition(Vector2Int vector2Int){
        //MapManager�ɏ����̎w������\��
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
        if(CurrentHP <= 0)
        {
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
