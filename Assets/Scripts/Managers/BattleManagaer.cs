using UnityEngine;
using System.Collections.Generic;
using System.Linq;//LINQ���g�p���邽��

/// <summary>
/// �퓬�V�X�e���𓝊�����V���O���g���N���X
/// </summary>
public partial class BattleManager : MonoBehaviour
{
    private static BattleManager _instanse;
    public static BattleManager Instance
    { 
        get 
        { 
            if(_instanse == null)
            {
                _instanse = FindAnyObjectByType<BattleManager>();
                if(_instanse == null)
                {
                    GameObject singletonObject = new GameObject("BatteleManager");
                    _instanse = singletonObject.AddComponent<BattleManager>();
                }
            }
            return _instanse; 
        } 
    }

    [SerializeField] private int _cuurrentTurn;//���݂̃^�[����
    public int CurrentTurn => _cuurrentTurn;
    [SerializeField] private Unit _activeUnit;//���ݍs�����̃��j�b�g
    [SerializeField] private List<Unit> _allUnits;//�}�b�v��̑S�Ẵ��j�b�g�̃��X�g

    private void Awake()
    {
        if(_instanse != null && _instanse != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instanse = this;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    /// <summary>
    /// �퓬���J�n����
    /// </summary>
    public void StartBattle()
    {
        Debug.Log("BattleManager:�퓬���J�n���܂�");
        _cuurrentTurn = 1;

        //�ŏ����i�K�Ƃ��Ď蓮�ŉ��̃��j�b�g��z�u
        _allUnits = new List<Unit>();
        //���̃v���C���[���j�b�g
        GameObject playerUnitObj = new GameObject("PlayerUnit");
        playerUnitObj.transform.position = new Vector3(0, 0, 0);
        Unit playerUnit = playerUnitObj.AddComponent<Unit>();
        playerUnit.Initialize(new UnitData { UnitId = "PLAYER001", UnitName = "�E��", BaseHP = 10, BaseMovement = 5 });
        playerUnit.UpdatePosition(new Vector2Int(0, 0));
        _allUnits.Add(playerUnit);
        //���̓G���j�b�g
        GameObject enemyUnitObj = new GameObject("EnemyUnit");
        enemyUnitObj.transform.position = new Vector3(5, 5, 0);
        Unit enemyUnit = enemyUnitObj.AddComponent<Unit>();
        enemyUnit.Initialize(new UnitData { UnitId = "ENEMY001", UnitName = "��ʕ��m", BaseHP = 1, BaseMovement = 3 });
        enemyUnit.UpdatePosition(new Vector2Int(3, 3));
        _allUnits.Add(enemyUnit);

        StartTurn();//�ŏ��̃^�[���J�n
    }

    /// <summary>
    /// �^�[�����J�n����
    /// </summary>
    public void StartTurn()
    {
        Debug.Log($"BattleManager:�^�[��{CurrentTurn}���J�n���܂�");
        //�S���j�b�g�̍s���ς݃t���O�����Z�b�g����
        foreach(var unit in _allUnits)
        {
            unit.ResetAction();
            unit.ResetMovementPoints();//�ړ��|�C���g�����Z�b�g
        }

        //��{�I�Ƀv���C���[������s��
        _activeUnit = _allUnits.FirstOrDefault(u => u.UnitId == "PLAYER001");
        if(_activeUnit == null)
        {
            //�v���C���[���j�b�g�����Ȃ���ΓG���j�b�g�Ȃǎ��̍s���\�ȃ��j�b�g��ݒ�
            _activeUnit = _allUnits.FirstOrDefault(u => !u.HasActedThisTurn);
        }

        if(_activeUnit != null)
        {
            Debug.Log($"BattleManager:{_activeUnit.UnitId}:{_activeUnit.UnitName}�̍s���J�n");
        }
        else
        {
            Debug.Log("BattleManager:�s���\�ȃ��j�b�g�����܂���");
            EndTurn();
        }
        
    }

    /// <summary>
    /// �^�[�����I�����A���̃^�[���̏������J�n����
    /// </summary>
    public void EndTurn()
    {
        Debug.Log($"BattleManager:�^�[��{CurrentTurn}���I�����܂�");
        _cuurrentTurn++;

        //�����E�s�k�����̃`�F�b�N
        if (CheckWinConditon())
        {
            Debug.Log("BattleManager:���������B��");
            GameManager.Instance.ChangePhase(GamePhase.StageClear);
            return;
        }
        if (CheckLoseCondition())
        {
            Debug.Log("BattleManager:�s�k�����B��");
            GameManager.Instance.ChangePhase(GamePhase.GameOver);
            return;
        }

        //�s�m��v�f�C�x���g�����̔���

        StartTurn();
    }

    ///<summary>
    /// ���j�b�g���^�[�Q�b�g�ʒu�ֈړ�������
    /// </summary>
    /// <param name="unit">�ړ������郆�j�b�g</param>
    /// <param name="targetPosition">�ڕW�O���b�h���W</param>
    public void MoveUnit(Unit unit, Vector2Int targetPosition)
    {
        Debug.Log($"BattleManager:{unit.UnitId}:{unit.UnitName}��{targetPosition}�ֈړ����܂�");
        //�_�C�N�X�g���@���g�p���Čo�H�ƃR�X�g���v�Z����
    }


    /// <summary>
    /// �����������`�F�b�N����
    /// �G���叫�̌��j�܂��́A���苒�_�̐����i���̏��������F�G�̑S�Łj
    /// </summary>
    /// <return>���������𖞂����Ă���΁@true</return>
    private bool CheckWinConditon()
    {
        //�ŏ����i�K�̂��ߓG�̑S�ł����������Ƃ���
        return _allUnits.Count(unit => unit.UnitId.StartsWith("ENEMY") && unit.CurrentHP > 0) == 0;
    }

    /// <summary>
    /// �s�k�������`�F�b�N����
    /// ���R�̑��叫�̌��j
    /// </summary>
    /// <return>�s�k�����𖞂����Ă���΁@ture</return>
    private bool CheckLoseCondition()
    {
        return _allUnits.Count(unit => unit.UnitId.StartsWith("PLAYER001") && unit.CurrentHP > 0) == 0;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
