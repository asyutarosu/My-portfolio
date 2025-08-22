using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField]private MapManager _mapManager;

    [SerializeField]private GameManager _gameManager;

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


    //�V�n�����C���V�X�e���F�n�`
    [SerializeField] private float _terrainChangeChance = 1.0f;//�n�`�ω�����������m���i��������100���j
    [SerializeField]private float _tentimeidouChance = 1.0f;//��K�͂Ȓn�`�ω�����������m���i��������100���j


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

            if(unit.Faction == FactionType.Enemy)
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

        //�^�[���I�����ɕ\������Ă���n�C���C�g���N���A����
        MapManager.Instance.ClearAllHighlights();


        //���F�^�[���V�X�e���̊m�F�̂��߃^�[���I���̂�
        Debug.Log("�v���C���[�^�[���I��");
        StartEnemyTurn();
    }

    /// <summary>
    /// �S�Ẵv���C���[���j�b�g���s���ς݂��`�F�b�N���A�s���ς݂Ȃ�^�[�����I������
    /// </summary>
    public void CheckAllPlayerUnitActed()
    {
        //�v���C���[���j�b�g���S�ł̏ꍇ
        if(_playerUnits == null || _playerUnits.Count == 0)
        {
            Debug.Log("�v���C���[���j�b�g�����܂���B�Q�[���I�[�o�[�B");
            //���Ƃ��ēG�^�[���ֈڍs�i�Q�[���I�[�o�[�֘A������2025/07�j
            EndPlayerTurn();
            return;
        }

        bool allActed = true;
        foreach(PlayerUnit playerUnit in _playerUnits)
        {
            if (!playerUnit.HasActedThisTurn)
            {
                allActed = false;
                break;
            }
        }

        if (allActed)
        {
            Debug.Log("�S�Ẵv���C���[���j�b�g���s�����������܂����B�G�^�[���ֈڍs���܂��B");
            EndPlayerTurn();
        }
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
        Debug.Log("�G�^�[���J�n(�R���[�`������)");

        yield return new WaitForSeconds(_enemyTurnDelay);

        while (_currentEnemyUnitIndex < _enemyUnits.Count)
        {
            EnemyUnit currentEnemy = _enemyUnits[_currentEnemyUnitIndex];

            //�|���ꂽ�G���j�b�g�̓X�L�b�v
            //if(currentEnemy == null || currentEnemy.CurrentHP <= 0)
            //{
            //    continue;
            //}

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

        if(Random.value <= _terrainChangeChance)
        {
            Debug.LogWarning("�n�`�ω����������܂���");
            //TriggerTerrainChangeEvent();
        }

        //�m�F�p
        _mapManager.ChangeSpecificTerrain(TerrainType.Mountain, TerrainType.River, 4);
        _mapManager.ChangeAroundTerrain(TerrainType.Forest, TerrainType.Desert);

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


    /// <summary>
    /// �n�`�ύX�C�x���g���g���K�[����
    /// </summary>
    private void TriggerTerrainChangeEvent()
    {
        TerrainType newTerrainType = GetRandomTerrainType();

        int numTilesToChange = 0;
        if(Random.value < _tentimeidouChance)
        {
            //10�}�X�ȏ�̑�K�͂Ȓn�`�ω�
            numTilesToChange = Random.Range(10, _mapManager.GridSize.x * _mapManager.GridSize.y);
            Debug.Log($"��K�͂Ȓn�`�ω�������{ numTilesToChange}�}�X��{ newTerrainType}�ɕω������܂�");
            //Debug.LogWarning($"{ _mapManager.GridSize}");
        }
        else
        {
            //4�`7�}�X�̏��K�͂Ȓn�`�ω�
            numTilesToChange = Random.Range(4, 8);
            Debug.Log($"���K�͂Ȓn�`�ύX�C�x���g������{numTilesToChange}�}�X�� {newTerrainType} �ɕω������܂�");
        }

        //�ύX����^�C���̃O���b�h���W�������_���ɑI��
        List<Vector2Int> tileToChange = GetRandomGridPosition(numTilesToChange);

        _mapManager.ChangeMultipleTerrains(tileToChange,newTerrainType);
    }


    /// <summary>
    /// �����̃����_���ȃO���b�h���W���擾����w���p�[���\�b�h
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private List<Vector2Int> GetRandomGridPosition(int count)
    {
        List<Vector2Int> selectedTiles = new List<Vector2Int>();
        List<Vector2Int> allTiles = MapManager.Instance.GetAllGridPosition();

        //
        while(selectedTiles.Count < count && allTiles.Count > 0)
        {
            int randomIndex = Random.Range(0,allTiles.Count);
            Vector2Int randomPos = allTiles[randomIndex];

            selectedTiles.Add(randomPos);
            allTiles.RemoveAt(randomIndex);
        }
        return selectedTiles;
    }

    //�����_���Ȓn�`�^�C�v���擾����w���p�[���\�b�h
    private TerrainType GetRandomTerrainType()
    {
        TerrainType[] types = (TerrainType[])System.Enum.GetValues(typeof(TerrainType));
        //�f�o�b�O�p
        return types[4];
        //return types[Random.Range(0, types.Length)];
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
        if(_gameManager.CurrentBattlePhase == BattlePhase.BattleMain)
        {
            //���F�v���C���[���^�[���I���𖾎��I�ɍs�����߂̏���2025/07
            if (CurrnetTurnState == TurnState.PlayerTurn && Input.GetKeyDown(KeyCode.E))
            {
                EndPlayerTurn();
            }

            //�f�o�b�O�p�F�G�^�[���̏I�������鏈��
            if (CurrnetTurnState == TurnState.EnemyTurn && Input.GetKeyDown(KeyCode.K))
            {
                EndEnemyTurn();
            }
        }
    }
}
