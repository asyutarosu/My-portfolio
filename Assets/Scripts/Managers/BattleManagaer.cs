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

    //[SerializeField] private TurnManager _turnManager;

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

    /// <summary>
    /// �d�l�ύX�̂��ߏ����x�[�X�̐퓬����������2025/07
    /// </summary>
    /// <param name="attacker">�U�����̃��j�b�g</param>
    /// <param name="target">�h�q���̃��j�b�g</param>
    public void ResolveBattle_ShogiBase(Unit attacker, Unit target)
    {

        HashSet<Vector2Int> attackableTiles = CalculateUnitMoveToAttackPositions(target.GetCurrentGridPostion(), attacker);



        Debug.LogWarning($"{attacker.gameObject.name}��{target.gameObject.name}�ɍU���I");
        Debug.LogWarning($"{attacker.GetCurrentGridPostion()}");


        if (attackableTiles.Contains(attacker.GetCurrentGridPostion()))
        {
            Debug.LogWarning($"�U�������F��");
            TurnManager.Instance.RemoveSpecificUnit(target);
            //���i�K�ł͏����x�[�X�̂��ߕ��G�ȏ������Ȃ��ꌂ�œ|��
            target.TakeDamage(10);
        }
        // �U��������ɍU���������߁A�����|��
        //target.Die();
    }

    /// <summary>
    /// ���j�b�g�Ԃ̐퓬�����i�X�e�[�^�X�x�[�X�F�d�l�ύX�ɔ��������̌�����2025/07�j
    /// </summary>
    /// <param name="attacker">�U�����̃��j�b�g</param>
    /// <param name="target">�h�q���̃��j�b�g</param>
    public void ResolveBattleSystem(Unit attacker, Unit target)
    {

    }





    /// <summary>
    /// �w�肳�ꂽ���j�b�g�̈ʒu���猩�āA�U�����郆�j�b�g���U���\�ȃ}�X���v�Z����
    /// </summary>
    /// <param name="targetPos">�^�[�Q�b�g�ƂȂ郆�j�b�g�̃O���b�h���W</param>
    /// <returns>�U�����郆�j�b�g���ړ����ׂ��U���\�ʒu�̃��X�g�iHashSet�ŏd���Ȃ��j</returns>
    private HashSet<Vector2Int> CalculateUnitMoveToAttackPositions(Vector2Int targetPos,Unit unit)
    {
        HashSet<Vector2Int> potentialUnitAttackMovePositions = new HashSet<Vector2Int>();


        int _maxAttackRange = unit._maxAttackRange;
        int _minAttackRange = unit._minAttackRange;

        //�v���C���[�̈ʒu���猩�āA_minAttackRange ���� _maxAttackRange �͈̔͂ɂ���}�X��T��
        //�G���j�b�g���v���C���[���U�����邽�߂Ɉړ��ł�����̃}�X���t�Z���Čv�Z����
        for (int x = -_maxAttackRange; x <= _maxAttackRange; x++)
        {
            for (int y = -_maxAttackRange; y <= _maxAttackRange; y++)
            {
                //�v���C���[�ʒu����̑��΍��W�ŁA�G���j�b�g���ʒu����\���̂���}�X���v�Z
                Vector2Int potentialEnemyPos = targetPos + new Vector2Int(x, y);

                //�G���j�b�g�� potentialEnemyPos �Ɉړ������ꍇ�A�v���C���[ (playerPos) �Ƃ̋������v�Z
                int distance = Mathf.Abs(potentialEnemyPos.x - targetPos.x) +
                               Mathf.Abs(potentialEnemyPos.y - targetPos.y); // �}���n�b�^������

                //�G���j�b�g�̍U���˒������`�F�b�N
                if (distance >= _minAttackRange && distance <= _maxAttackRange)
                {
                    // ���̈ʒu�ɓG���j�b�g���ړ��ł���΁A�v���C���[���U���\
                    // �������AMapManager.Instance.IsValidGridPosition �̃`�F�b�N�͕K�{
                    if (MapManager.Instance.IsValidGridPosition(potentialEnemyPos))
                    {
                        // ���̃}�X�́A�G���j�b�g���ړ����ăv���C���[���U���ł���L���Ȍ��ł���
                        potentialUnitAttackMovePositions.Add(potentialEnemyPos);
                    }
                }
            }
        }

        //�m�F�p
        //foreach(Vector2Int ShowAttackPos in potentialUnitAttackMovePositions)
        //{
        //    Debug.LogWarning($"{ShowAttackPos}");

        //}

        Debug.LogWarning($"{potentialUnitAttackMovePositions.Count}");

        return potentialUnitAttackMovePositions;
    }







    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="attackedUnit"></param>
    ///// <param name="targetUnit"></param>
    //public void CandidateAttackedPos(Unit attackedUnit, Unit targetUnit,FactionType fation)
    //{
    //    Vector2Int CurrentGridPosition = Vector2Int.zero;
    //    Vector2Int targetPos = CurrentGridPosition;
    //    Vector2Int originalCurrentGridPosition = CurrentGridPosition;

    //    List<PlayerUnit> playerUnits = MapManager.Instance.GetAllPlayerUnits();
    //    Dictionary<Vector2Int, PathNodes> reachableNodes = DijkstraPathfinder.FindReachableNodes(originalCurrentGridPosition, this);

    //    int minCostToAttackPos = int.MaxValue; // �G����U���\�ʒu�܂ł̃R�X�g


    //    foreach (PlayerUnit player in playerUnits)
    //    {
    //        Debug.Log($"  �^�[�Q�b�g���̃v���C���[: {player.name} �ʒu: {player.GetCurrentGridPostion()}");

    //        HashSet<Vector2Int> potentialEnemyMoveToAttackPositions = CalculateUnitMoveToAttackPositions(player.GetCurrentGridPostion(),player);

    //        Debug.Log($"    �v���C���[({player.name})���U���\�ȃ}�X��␔ (�G���ړ����ׂ��ʒu): {potentialEnemyMoveToAttackPositions.Count}");

    //        // �G���ړ��\�ȃ}�X�̒�����A���̃v���C���[���U���ł���}�X��T��
    //        foreach (Vector2Int attackPosCandidate in potentialEnemyMoveToAttackPositions)
    //        {
    //            // ���̍U���ʒu���A�G�̈ړ��\�͈͓��ɂ���A���󂫃}�X�ł��邩�`�F�b�N
    //            if (reachableNodes.ContainsKey(attackPosCandidate) && !MapManager.Instance.IsTileOccupiedForStooping(attackPosCandidate, this))
    //            {
    //                int costToMoveToAttackPos = reachableNodes[attackPosCandidate].Cost;

    //                int currentManhattanDistanceToPlayer =
    //                    Mathf.Abs(attackPosCandidate.x - player.GetCurrentGridPostion().x) + Mathf.Abs(attackPosCandidate.y - player.GetCurrentGridPostion().y);

    //                Debug.Log($"    [�L���Ȍ��] �U���\�ʒu: {attackPosCandidate} (�ړ��R�X�g: {costToMoveToAttackPos}, �v���C���[�ւ̋���: {currentManhattanDistanceToPlayer})");

    //                // ���݌������Ă���ŏ��R�X�g��菬�����ꍇ�A�X�V
    //                if (costToMoveToAttackPos < minCostToAttackPos)
    //                {
    //                    //minCostToAttackPos = costToMoveToAttackPos;
    //                    //bestMoveTargetPos = attackPosCandidate;
    //                    //targetedPlayer = player; // ���̃v���C���[���^�[�Q�b�g�ɂ���
    //                    //minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
    //                }
    //                //�ړ��R�X�g�������ł���΁A�v���C���[�ɋ߂��i�}���n�b�^���������Z���j���̂�D��
    //                else if (costToMoveToAttackPos == minCostToAttackPos)
    //                {
    //                    if (currentManhattanDistanceToPlayer < minManhattanDistanceToPlayer)
    //                    {
    //                        //minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
    //                        //bestMoveTargetPos = attackPosCandidate;
    //                        //targetedPlayer = player;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                //�m�F�p
    //                Debug.Log($"    [�����Ȍ��] �U���\�ʒu: {attackPosCandidate} (���R: �ړ��͈͊O�܂��͐�L�ς�)");
    //            }
    //        }
    //    }
    //}











    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    /// <summary>
    /// �퓬���J�n����
    /// </summary>
    //public void StartBattle()
    //{
    //    Debug.Log("BattleManager:�퓬���J�n���܂�");
    //    _cuurrentTurn = 1;

    //    //�ŏ����i�K�Ƃ��Ď蓮�ŉ��̃��j�b�g��z�u
    //    _allUnits = new List<Unit>();
    //    //���̃v���C���[���j�b�g
    //    GameObject playerUnitObj = new GameObject("PlayerUnit");
    //    playerUnitObj.transform.position = new Vector3(0, 0, 0);
    //    Unit playerUnit = playerUnitObj.AddComponent<Unit>();
    //    playerUnit.Initialize(new UnitData { UnitId = "PLAYER001", UnitName = "�E��", BaseHP = 10, BaseMovement = 5 });
    //    playerUnit.UpdatePosition(new Vector2Int(0, 0));
    //    _allUnits.Add(playerUnit);
    //    //���̓G���j�b�g
    //    GameObject enemyUnitObj = new GameObject("EnemyUnit");
    //    enemyUnitObj.transform.position = new Vector3(5, 5, 0);
    //    Unit enemyUnit = enemyUnitObj.AddComponent<Unit>();
    //    enemyUnit.Initialize(new UnitData { UnitId = "ENEMY001", UnitName = "��ʕ��m", BaseHP = 1, BaseMovement = 3 });
    //    enemyUnit.UpdatePosition(new Vector2Int(3, 3));
    //    _allUnits.Add(enemyUnit);

    //    StartTurn();//�ŏ��̃^�[���J�n
    //}

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
            GameManager.Instance.ChangeState(GameState.StageClear);
            return;
        }
        if (CheckLoseCondition())
        {
            Debug.Log("BattleManager:�s�k�����B��");
            GameManager.Instance.ChangeState(GameState.GameOver);
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
