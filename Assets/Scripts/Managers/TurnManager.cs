using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] private float _enemyTurnDelay = 1.0f;//�G�̃^�[�������J�n�܂ł̒x������

    //���݂̃^�[�����
    public TurnState CurrnetTurnState { get; private set; }
    public int CurrentTurnNumber { get; private set; } = 1;

    //TurnManager�ŊǗ�����̃��j�b�g���X�g
    private List<Unit> _allUnits;
    private List<PlayerUnit> _playerUnits;
    private List<EnemyUnit> _enemyUnits;

    //���ݍs�����̓G���j�b�g�̃C���f�b�N�X
    private int _currentEnemyUnitIndex = 0;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    //����������
    public void InitializeTurnManager()
    {
        CurrnetTurnState = TurnState.PreGame;
        CurrentTurnNumber = 1;

        //���j�b�g���X�g���擾
        _allUnits = MapManager.Instance.GetAllUnits();
        _playerUnits = MapManager.Instance.GetAllPlayerUnits();
        _enemyUnits = MapManager.Instance.GetAllEnemyUnits();

        //�m�F�p
        if (_playerUnits == null)
        {
            Debug.LogError("TurnManager: _playerUnits��MapManager����擾�ł��܂���ł����I");
        }
        if (_enemyUnits == null)
        {
            Debug.LogError("TurnManager: _enemyUnits��MapManager����擾�ł��܂���ł����I");
        }
        Debug.Log($"TurnManager: �v���C���[���j�b�g��: {_playerUnits?.Count ?? 0}, �G���j�b�g��: {_enemyUnits?.Count ?? 0}");


        Debug.Log("TurnManager:�Q�[������������");
        StartPlayerTurn();//�v���C���[�^�[������J�n
    }

    /// <summary>
    /// �S�Ẵ��j�b�g�̃��X�g���X�V����
    /// </summary>
    public void UpdateUnitList()
    {
        _allUnits = FindObjectsOfType<Unit>().ToList();
    }

    /// <summary>
    /// �v���C���[�̃^�[�����J�n����
    /// </summary>
    public void StartPlayerTurn()
    {
        CurrnetTurnState = TurnState.PlayerTurn;
        Debug.Log("�v���C���[�^�[���J�n");

        //�S�Ẵv���C���[���j�b�g�̍s����Ԃ����Z�b�g
        foreach(Unit unit in _allUnits)
        {
            if(unit.Faction == FactionType.Player)
            {
                unit.ResetAction();
            }
        }

        foreach(var unit in _playerUnits)
        {
            unit.SetActionTaken(false);
        }

        //Ui�X�V�Ȃǂ�ǉ�2025/07
    }

    /// <summary>
    /// �v���C���[�̃^�[���I������
    /// </summary>
    public void EndPlayerTurn()
    {
        if (CurrnetTurnState != TurnState.PlayerTurn)
        {
            Debug.LogWarning("TurnManager: �v���C���[�^�[���ȊO��EndPlayerTurn���Ăяo����܂����B");
            return;
        }
        //�S�Ẵv���C���[���j�b�g���s���ς݁A�܂��̓v���C���[����ɂ��I���̃`�F�b�N��ǉ�2025/07

        //���F�^�[���V�X�e���̊m�F�̂��߃^�[���I���̂�
        Debug.Log("�v���C���[�^�[���I��");
        StartEnemyTurn();
    }

    /// <summary>
    /// �G�̃^�[�����J�n����
    /// </summary>
    private void StartEnemyTurn()
    {
        CurrnetTurnState = TurnState.EnemyTurn;
        Debug.Log("�G�^�[���J�n");

        _currentEnemyUnitIndex = 0;//�G���j�b�g�̍s�����͌Œ�2025/07

        //�S�Ă̓G���j�b�g�̍s����Ԃ����Z�b�g
        foreach (Unit unit in _allUnits)
        {
            if (unit.Faction == FactionType.Enemy)
            {
                unit.ResetAction();
            }
        }

        foreach(var unit in _enemyUnits)
        {
            unit.SetActionTaken(false);
        }

        //�G�̍s�����W�b�N��x�������ĊJ�n
        StartCoroutine(EnemyTurnRoutine());
    }

    /// <summary>
    /// �G�^�[���̃R���[�`������
    /// </summary>
    private IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(_enemyTurnDelay);

        while (_currentEnemyUnitIndex < _enemyUnits.Count)
        {
            EnemyUnit currentEnemy = _enemyUnits[_currentEnemyUnitIndex];

            //�s���ς݂̓G���j�b�g�̓X�L�b�v
            if (currentEnemy.HasActedThisTurn)
            {
                _currentEnemyUnitIndex++;
                continue;
            }

            Debug.Log($"�G���j�b�g {currentEnemy.name} ���s�����܂��B");
            yield return StartCoroutine(currentEnemy.PerformAIAction()); // �GAI�̍s�������s
            
            // �s��������������A���̓G��
            _currentEnemyUnitIndex++;
            yield return new WaitForSeconds(0.5f); // �e�G���j�b�g�̍s���Ԃɏ����Ԃ�u��

        }


        ////�G�`�h�̍s�����W�b�N��ǉ�����2025/07
        //Debug.Log("�G���j�b�g���s����(AI�N��)");
        //foreach(Unit unit in _allUnit)
        //{
        //    if(unit.Faction == FactionType.Enemy)
        //    {
        //        //�G���j�b�g�ɍs�������郁�\�b�h���Ăяo���Ă���
        //        unit.SetActionTaken(false);
        //    }
        //}

        //Debug.Log("�G���j�b�g���ړ���");

        ////�G���j�b�g�݂̂��t�B���^�����O
        //List<EnemyUnit> enemyUnits = _allUnit.OfType<EnemyUnit>().ToList();

        //foreach (EnemyUnit enemyUnit in enemyUnits)
        //{
        //    //�G���j�b�g��AI�s�������s���A������҂�
        //    yield return StartCoroutine(enemyUnit.PerformAIAction());
        //    yield return new WaitForSeconds(0.2f);//�e�G���j�b�g�̍s���ԂɒZ���Ԋu
        //}

        //yield return new WaitForSeconds(_enemyTurnDelay);

        EndEnemyTurn();
    }

    /// <summary>
    /// �G�̃^�[�����I������
    /// </summary>
    private void EndEnemyTurn()
    {
        Debug.Log("�G�^�[���I��");
        CurrentTurnNumber++;//�^�[���o�߁i�v���C���[�^�[�����G�^�[���I���łP�T�C�N���j

        //���̃v���C���[�^�[�����J�n
        StartPlayerTurn();
    }

    //�Q�[���I�[�o�[����
    public void SetGameOver()
    {
        CurrnetTurnState = TurnState.GameOver;
        Debug.Log("--- �Q�[���I�[�o�[ ---");
    }

    //���̑��̃^�[����Ԃւ̑J�ڃ��\�b�h�i����2025/07�j
    public void SetTurnState(TurnState newState)
    {
        CurrnetTurnState = newState;
    }

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //�����������Ƃ��ăV�[����̑S�Ẵ��j�b�g���������ă��X�g�ɒǉ�
        _allUnits = FindObjectsOfType<Unit>().ToList();

        //�ŏ��̃^�[���J�n
        
    }

    // Update is called once per frame
    void Update()
    {
        //���F�v���C���[���^�[���I���𖾎��I�ɍs�����߂̏���2025/07
        if(CurrnetTurnState == TurnState.PlayerTurn && Input.GetKeyDown(KeyCode.E))
        {
            EndPlayerTurn();
        }

        //�f�o�b�O�p�F�G�^�[���̏I�������鏈��
        if(CurrnetTurnState == TurnState.EnemyTurn && Input.GetKeyDown(KeyCode.K))
        {
            EndEnemyTurn();
        } 
    }
}
