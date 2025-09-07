using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class EnemyUnit : Unit
{
    //ScriptableObject�ŊǗ�
    //�GAI�̍s���^�C�v
    //public enum EnemyAIType
    //{
    //    Aggreeive,  //�ϋɓI�Ƀv���C���[��ǂ�
    //    Stationary, //����̃}�X�ɐN�������܂œ����Ȃ�
    //    Patrol,     //�p�g���[���^�F�����\��
    //    Normal      //��Ƀv���C���[���j�b�g��_��
    //}

    //ScriptableObject�ŊǗ�
    //[SerializeField] private EnemyAIType _aiType = EnemyAIType.Aggreeive;//�f�t�H���g��AI�^�C�v

    //�U���͈�:Unit.cs�Ɉڍs2025/07
    //[SerializeField] private int _minAttackRange = 1;//�ŏ��U���˒�
    //[SerializeField] private int _maxAttackRange = 2;//�ő�U���˒�


    //�f�o�b�O�p
    //public Vector2Int EnemyCurrentGridPosition => MapManager.Instance.GetGridPositionFromWorld(transform.position);
    //private List<Vector2Int> currentAttackableTiles = new List<Vector2Int>(); // ���C��: �����Ń��X�g������������



    public override void Initialize(UnitData data)
    {
        //_currentPosition = initialGridPos;
        //_unitName = name;
        //Debug.Log($"PlayerUnit'{_unitName}'initialized at grid:{_currentPosition}");

        base.Initialize(data);

        _factionType = FactionType.Enemy;


        Debug.Log($"Enemy Unit '{UnitName}' (Type: {Type}, Faction: {Faction},AIType{EnemyAIType}) initialized at grid: {CurrentGridPosition}");
    }

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// �GAI���s�������s����
    /// </summary>
    public IEnumerator PerformAIAction()
    {
        //���ɍs���ς݂Ȃ�X�L�b�v
        if (HasActedThisTurn)
        {
            Debug.Log($"{name}�͊��ɂ��̃^�[���s���ς݂ł�");
            yield break;
        }

        Debug.Log($"{name}���s�����J�n���܂�");

        Debug.LogWarning($"AI�̏��{_enemyAIType}");

        //�GAI�̃^�C�v�ɉ����čs���𕪊�
        switch (_enemyAIType)
        {
            case EnemyAIType.Aggreeive: 
                Debug.Log($"{name}���ˌ����܂�");
                yield return StartCoroutine(HandleAggressiveAI());
                break;
            case EnemyAIType.Stationary: 
                Debug.Log($"{name}�͑ҋ@���Ă��܂�");
                break;
            case EnemyAIType.DefalutAI:
                yield return StartCoroutine(HandleAggressiveAItest());
                break;
            //����AI��ǉ�
            default: 
                Debug.Log($"{name}��AI�^�C�v������`�ł�:{_enemyAIType}");
                break;
        }

        //�s��������������s���ς݃t���O���Z�b�g
        SetActionTaken(true);
        Debug.Log($"{name}�̍s�����������܂���");

    }


    /// <summary>
    /// �ϋɓI��AI�̍s�����W�b�N
    /// �E�}�b�v��ōs������G���j�b�g����ł��߂����ړ��R�X�g���ŏ��̈ʒu�ɂ���v���C���[���j�b�g�Ɍ������Ĉړ�����
    /// �E�G���j�b�g�̍U���\�͈͓��Ƀv���C���[���j�b�g�����Ȃ���΍s�����Ȃ�
    /// </summary>
    private IEnumerator HandleAggressiveAI()
    {
        //���ݒn�̎擾
        Vector2Int targetPos = CurrentGridPosition;

        //�v���C���[���j�b�g�̌���
        //List<PlayerUnit> playerUnits = MapManager.Instance.GetAllPlayerUnits();
        List<PlayerUnit> playerUnits = TurnManager.Instance.PlayerUnits;

        if (playerUnits == null || playerUnits.Count == 0)
        {
            Debug.Log($"{name}:�v���C���[���j�b�g��������܂���");
            yield break;
        }
        Debug.LogWarning($"���j�b�g��{this.name}");


        //���݂̍U���͈͓��Ƀv���C���[���j�b�g�����邩�`�F�b�N
        //���݈ʒu����̍U���͈͂��v�Z
        //HashSet<Vector2Int> currentAttackableTiles = CalculateAttackRange(GetCurrentGridPostion());
        //bool playerInAttackRange = false;
        //PlayerUnit targetPlayerInAttackRange = null;

        //Vector2Int originalCurrentGridPosition2 = CurrentGridPosition;

        //Dictionary<Vector2Int, PathNodes> reachableNodes2 = DijkstraPathfinder.FindReachableNodes(originalCurrentGridPosition2, this);


        //foreach (PlayerUnit players in playerUnits)
        //{
        //    HashSet<Vector2Int> potentialEnemyMoveToAttackPositions2 = CalculateEnemyMoveToAttackPositions(players.GetCurrentGridPostion());


        //    foreach (Vector2Int attackPosCandidate in potentialEnemyMoveToAttackPositions2)
        //    {
        //        if (reachableNodes2.ContainsKey(attackPosCandidate) && !MapManager.Instance.IsTileOccupiedForStooping(attackPosCandidate, this))
        //        {
        //            Debug.LogWarning($"{name}: �U���͈͂ɂ��܂�");

        //            playerInAttackRange = true;
        //            targetPlayerInAttackRange = players;
        //            //break;//�N��1�l�ł�����Ηǂ�
        //        }

        //        if (currentAttackableTiles.Contains(players.GetCurrentGridPostion()))
        //        {
        //            Debug.LogWarning($"{name}: �U���͈�");

        //            playerInAttackRange = true;
        //            targetPlayerInAttackRange = players;
        //            //break;//�N��1�l�ł�����Ηǂ�
        //        }
        //    }

        //}




        //if (playerInAttackRange)
        //{
        //    //Debug.LogWarning($"{name}:�v���C���[���j�b�g({targetPlayerInAttackRange.name})���U���͈͂ɂ��܂��B�U�����܂�");

        //    Debug.LogWarning($"{name}:�v���C���[���j�b�g({targetPlayerInAttackRange.name})���U���͈͂ɂ��܂��B�U�����܂�");
        //    //�U�����W�b�N��ǉ�
        //    Debug.LogWarning($"{name}: �U���U���U���U��");


        //    yield return new WaitForSeconds(0.5f);
        //    yield break;//�U��������ړ����Ȃ�
        //}

        Debug.Log($"{name}: �U���͈͓��Ƀv���C���[���j�b�g�����܂���B�ł��߂��v���C���[���j�b�g�Ɍ������Ĉړ����܂��B");

        //�m�F�p
        Debug.Log($"{name}: ���݂̈ړ���: {CurrentMovementPoints}"); // !!! �����Ŏ��ۂ̈ړ��͂��m�F !!!


        //�ł��߂����ړ��R�X�g���ŏ��̈ʒu�ɂ���v���C���[���j�b�g�ւ̈ړ��ڕW������
        Vector2Int bestMoveTargetPos = Vector2Int.zero;
        List<Vector2Int> bestPath = null;
        int minCostToAttackPos = int.MaxValue; // �G����U���\�ʒu�܂ł̃R�X�g
        PlayerUnit targetedPlayer = null;
        int minManhattanDistanceToPlayer = int.MaxValue;//�ړ��ڕW�ʒu����v���C���[�܂ł̃}���n�b�^������

        Vector2Int originalCurrentGridPosition = CurrentGridPosition;

        //�ꕔ�̌o�H�v�Z���ɕs������������ߕύX2025/07
        //Dictionary<Vector2Int, PathNodes> reachableNodes = DijkstraPathfinder.FindReachableNodes(GetCurrentGridPostion(), this);
        //Dictionary<Vector2Int, PathNodes> reachableNodes = DijkstraPathfinder.FindReachableNodes(originalCurrentGridPosition, this);

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> TestreachableNodes =
            DijkstraPathfinder.FindReachableTiles(this.CurrentGridPosition, this);

        //�m�F�p
        Debug.Log($"{name}: DijkstraPathfinder���v�Z�������B�\�}�X��: {TestreachableNodes.Count}"); // ���B�\�ȃ}�X�̑���


        // ���B�\�ȃ}�X�Ƃ��̃R�X�g��S�ă��O�o��
        Debug.Log($"{name}: --- ���B�\�}�X�ڍ� ---");
        foreach (var entry in TestreachableNodes.OrderBy(e => e.Value.Cost)) // �R�X�g���Ƀ\�[�g���Č��₷��
        {
            Debug.Log($"  Reachable Node: {entry.Key} (Cost: {entry.Value.Cost})");
        }
        Debug.Log($"{name}: ---------------------");


        foreach(PlayerUnit player in playerUnits)
        {
            Debug.Log($"  �^�[�Q�b�g���̃v���C���[: {player.name} �ʒu: {player.GetCurrentGridPostion()}");

            HashSet<Vector2Int> potentialEnemyMoveToAttackPositions = CalculateEnemyMoveToAttackPositions(player.GetCurrentGridPostion());

            Debug.Log($"    �v���C���[({player.name})���U���\�ȃ}�X��␔ (�G���ړ����ׂ��ʒu): {potentialEnemyMoveToAttackPositions.Count}");

            // �G���ړ��\�ȃ}�X�̒�����A���̃v���C���[���U���ł���}�X��T��
            foreach(Vector2Int attackPosCandidate in potentialEnemyMoveToAttackPositions)
            {
                // ���̍U���ʒu���A�G�̈ړ��\�͈͓��ɂ���A���󂫃}�X�ł��邩�`�F�b�N
                if (TestreachableNodes.ContainsKey(attackPosCandidate) && !MapManager.Instance.IsTileOccupiedForStooping(attackPosCandidate, this)){
                    int costToMoveToAttackPos = TestreachableNodes[attackPosCandidate].Cost;

                    int currentManhattanDistanceToPlayer =
                        Mathf.Abs(attackPosCandidate.x - player.GetCurrentGridPostion().x) + Mathf.Abs(attackPosCandidate.y - player.GetCurrentGridPostion().y);

                    Debug.Log($"    [�L���Ȍ��] �U���\�ʒu: {attackPosCandidate} (�ړ��R�X�g: {costToMoveToAttackPos}, �v���C���[�ւ̋���: {currentManhattanDistanceToPlayer})");

                    // ���݌������Ă���ŏ��R�X�g��菬�����ꍇ�A�X�V
                    if (costToMoveToAttackPos < minCostToAttackPos)
                    {
                        minCostToAttackPos = costToMoveToAttackPos;
                        bestMoveTargetPos = attackPosCandidate;
                        targetedPlayer = player; // ���̃v���C���[���^�[�Q�b�g�ɂ���
                        minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
                    }
                    //�ړ��R�X�g�������ł���΁A�v���C���[�ɋ߂��i�}���n�b�^���������Z���j���̂�D��
                    else if (costToMoveToAttackPos == minCostToAttackPos)
                    {
                        if (currentManhattanDistanceToPlayer < minManhattanDistanceToPlayer)
                        {
                            minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
                            bestMoveTargetPos = attackPosCandidate;
                            targetedPlayer = player;
                        }
                    }
                }
                else
                {
                    //�m�F�p
                    Debug.Log($"    [�����Ȍ��] �U���\�ʒu: {attackPosCandidate} (���R: �ړ��͈͊O�܂��͐�L�ς�)");
                }
            } 
        }

        // �œK�Ȉړ��ڕW�ʒu�����������ꍇ
        //if(targetedPlayer != null && bestMoveTargetPos != Vector2Int.zero && minCostToAttackPos != int.MaxValue)
        if (targetedPlayer != null && minCostToAttackPos != int.MaxValue)
        {
            // �ړ��R�X�g�����݂̈ړ��͈ȉ��ł��邱�Ƃ��m�F
            if (minCostToAttackPos <= CurrentMovementPoints)
            {
                //���W�f�[�^�̈ړ�����
                MyTile newTile = MapManager.Instance.GetTileAt(bestMoveTargetPos);
                if (newTile != null)
                {
                    MoveToGridPosition(bestMoveTargetPos, newTile); // ��L���X�V
                }

                // �ŒZ�o�H���v�Z (�A�j���[�V�����p)
                //List<Vector2Int> pathForAnimation = DijkstraPathfinder.FindPath(originalCurrentGridPosition, bestMoveTargetPos, this);

                List<Vector2Int> AnimationPath = DijkstraPathfinder.GetPathToTarget(
                originalCurrentGridPosition,
                bestMoveTargetPos,
                this);

                //�f�o�b�O�p
                //foreach (Vector2Int animation in AnimationPath)
                //{
                //    Debug.LogWarning(animation);
                //}


                //if (pathForAnimation != null && pathForAnimation.Count > 1)
                //{
                //    string pathString = string.Join(" -> ", pathForAnimation.Select(p => $"({p.x},{p.y})"));
                //    Debug.Log($"{name}: �ŏI����ڕW�ʒu: {bestMoveTargetPos}");
                //    Debug.Log($"{name}: �ŏI����o�H: {pathString}");

                //    Debug.Log($"{name}: ({originalCurrentGridPosition.x},{originalCurrentGridPosition.y}) ���� ({bestMoveTargetPos.x},{bestMoveTargetPos.y}) �ֈړ����܂��i�Ώۃv���C���[: {targetedPlayer.name}�j�B");
                //    // ���A�j���[�V������ Unit.AnimateMove ���g�p���Ă���͂��Ȃ̂ŁA�ȉ��ɏC��
                //    yield return StartCoroutine(AnimateMove(pathForAnimation));

                //    //�m�F�p
                //    Debug.LogWarning($"{name}: �U���U���U���U��");

                //    //�����ɍU������
                //    BattleManager.Instance.ResolveBattle_ShogiBase(this, targetedPlayer);
                //}

                if (AnimationPath != null && AnimationPath.Count > 0)
                {
                    string pathString = string.Join(" -> ", AnimationPath.Select(p => $"({p.x},{p.y})"));
                    Debug.Log($"{name}: �ŏI����ڕW�ʒu: {bestMoveTargetPos}");
                    Debug.Log($"{name}: �ŏI����o�H: {pathString}");

                    Debug.Log($"{name}: ({originalCurrentGridPosition.x},{originalCurrentGridPosition.y}) ���� ({bestMoveTargetPos.x},{bestMoveTargetPos.y}) �ֈړ����܂��i�Ώۃv���C���[: {targetedPlayer.name}�j�B");
                    // ���A�j���[�V������ Unit.AnimateMove ���g�p���Ă���͂��Ȃ̂ŁA�ȉ��ɏC��
                    yield return StartCoroutine(AnimateMove(AnimationPath));


                    //�m�F�p
                    Debug.LogWarning($"{name}: �U���U���U���U��");

                    //�����ɍU������
                    BattleManager.Instance.ResolveBattle_ShogiBase(this, targetedPlayer);

                    //�Q�[���I�[�o�[����
                    TurnManager.Instance.CheckGameOver();

                }
                else
                {
                    Debug.Log($"{name}: �o�H�v�Z�Ɏ��s�������A���ɍœK�Ȉʒu�ɂ��܂��B�ڕW�ʒu: {bestMoveTargetPos} (�o�H�Ȃ�)");

                    //�m�F�p
                    Debug.LogWarning($"{name}: �����ōU�������ōU��");
                    //targetedPlayer.Die();
                    BattleManager.Instance.ResolveBattle_ShogiBase(this, targetedPlayer);
                    TurnManager.Instance.CheckGameOver();

                    yield return null; // �ړ����Ȃ����s���͊���
                }
            }
            else
            {
                Debug.Log($"{name}: �ڕW�ʒu ({bestMoveTargetPos}) �ւ̈ړ��R�X�g ({minCostToAttackPos}) ���ړ��� ({CurrentMovementPoints}) �𒴂��Ă��܂��B�ړ����܂���B");
                yield return null; // �ړ����Ȃ����s���͊���
            }

        }
        else
        {
            // �U���ł���ړ��悪������Ȃ��������A�v���C���[���͈͊O�̏ꍇ
            Debug.Log($"{name}: �U���ł���ړ��悪������Ȃ����A�S�Ẵv���C���[���ړ��͈͊O�ł��B�ړ����܂���B");
            yield return null; // �ړ����Ȃ����s���͊���
        }





        //AI�̍s�����W�b�N�̌������ɂ��ύX2025/07
        //// �S�Ẵv���C���[���j�b�g�ɑ΂��āA�œK�Ȉړ����T��
        //foreach (PlayerUnit player in playerUnits)
        //{
        //    //�m�F�p
        //    Debug.Log($"  �^�[�Q�b�g���̃v���C���[: {player.name} �ʒu: {player.GetCurrentGridPostion()}"); // �^�[�Q�b�g���̃v���C���[�������O�o��

        //    // ���̃v���C���[���U���ł���}�X�i�G���ړ����ׂ��^�[�Q�b�g�}�X�j���v�Z
        //    //HashSet<Vector2Int> potentialAttackPositions = CalculateAttackRange(player.GetCurrentGridPostion());
        //    HashSet<Vector2Int> potentialAttackPositions = CalculateAttackRange(player.GetCurrentGridPostion());

        //    Debug.Log($"    �v���C���[({player.name})���U���\�ȃ}�X��␔ (�U���͈͓�): {potentialAttackPositions.Count}");


        //    // �G���ړ��\�ȃ}�X�̒�����A���̃v���C���[���U���ł���}�X��T��
        //    foreach (Vector2Int attackPosCandidate in potentialAttackPositions)
        //    {
        //        // ���̍U���ʒu���A�G�̈ړ��\�͈͓��ɂ���A���󂫃}�X�ł��邩�`�F�b�N
        //        if (reachableNodes.ContainsKey(attackPosCandidate))
        //        {
        //            int costToMoveToAttackPos = reachableNodes[attackPosCandidate].Cost;

        //            //AI�̋����ɔ[���ł��Ȃ��̂Ń}���n�b�^�������ɂ��ړ��o�H�v�Z������2025/07
        //            int currentManhattanDistanceToPlayer =
        //                Mathf.Abs(attackPosCandidate.x - player.GetCurrentGridPostion().x) + Mathf.Abs(attackPosCandidate.y - player.GetCurrentGridPostion().y);

        //            //�m�F�p
        //            Debug.Log($"    [�L���Ȍ��] �U���\�ʒu: {attackPosCandidate} (�ړ��R�X�g: {costToMoveToAttackPos}, �v���C���[�ւ̋���: {currentManhattanDistanceToPlayer})");

                    
        //            // ���݌������Ă���ŏ��R�X�g��菬�����ꍇ�A�X�V
        //            if (costToMoveToAttackPos < minCostToAttackPos)
        //            {
        //                minCostToAttackPos = costToMoveToAttackPos;
        //                bestMoveTargetPos = attackPosCandidate;
        //                targetedPlayer = player; // ���̃v���C���[���^�[�Q�b�g�ɂ���
        //                //�}���n�b�^��
        //                minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
        //            }
        //            //�ړ��R�X�g�������ł���΁A�v���C���[�ɋ߂��i�}���n�b�^���������Z���j���̂�D��
        //            else if (costToMoveToAttackPos == minCostToAttackPos)
        //            {
        //                if(currentManhattanDistanceToPlayer < minManhattanDistanceToPlayer)
        //                {
        //                    minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
        //                    bestMoveTargetPos = attackPosCandidate;
        //                    targetedPlayer =player;
        //                }
        //            }
        //            else
        //            {
        //                //�m�F�p
        //                // �ړ��͈͊O�܂��͐�L�ς݂̂��ߓ��B�ł��Ȃ����}�X
        //                Debug.Log($"    [�����Ȍ��] �U���\�ʒu: {attackPosCandidate} (���R: �ړ��͈͊O�܂��͐�L�ς�)");
        //            }
        //        }
        //    }
        //}

        

        //// �œK�Ȉړ��ڕW�ʒu�����������ꍇ
        //if (targetedPlayer != null && bestMoveTargetPos != Vector2Int.zero)
        //{
        //    // �ŒZ�o�H���v�Z
        //    bestPath = DijkstraPathfinder.FindPath(GetCurrentGridPostion(), bestMoveTargetPos, this);

        //    // �o�H��������A���݂̈ʒu�ȊO�Ɉړ��悪����ꍇ (path.Count > 1 �͊J�n�n�_���܂܂�邽��)
        //    if (bestPath != null && bestPath.Count > 1)
        //    {
        //        //�m�F�p
        //        string pathString = string.Join(" -> ", bestPath.Select(p => $"({p.x},{p.y})"));
        //        Debug.Log($"{name}: �ŏI����ڕW�ʒu: {bestMoveTargetPos}");
        //        Debug.Log($"{name}: �ŏI����o�H: {pathString}");

               

        //        Debug.Log($"{name}: ({GetCurrentGridPostion().x},{GetCurrentGridPostion().y}) ���� ({bestMoveTargetPos.x},{bestMoveTargetPos.y}) �ֈړ����܂��i�Ώۃv���C���[: {targetedPlayer.name}�j�B");
        //        yield return StartCoroutine(MapManager.Instance.SmoothMoveCoroutine(this, GetCurrentGridPostion(), bestMoveTargetPos, bestPath));
        //    }
        //    else
        //    {
        //        // ���̃P�[�X�͒ʏ픭�����Ȃ��͂��ł����A�O�̂���
        //        Debug.Log($"{name}: �o�H�v�Z�Ɏ��s�������A���ɍœK�Ȉʒu�ɂ��܂��B�ڕW�ʒu: {bestMoveTargetPos}");
        //    }
        //}
        //else
        //{
        //    // �U���ł���ړ��悪������Ȃ������ꍇ
        //    Debug.Log($"{name}: �U���ł���ړ��悪������Ȃ����A�S�Ẵv���C���[���ړ��͈͊O�ł��B");
        //}


        //���������܂��s���Ȃ��̂ň�x��蒼���̂��߃R�����g�A�E�g�i�ړ��̂ݎ����ς݃\�[�X�R�[�h�j

        //�G���j�b�g����ł��߂��v���C���[���j�b�g��������
        //PlayerUnit closestPlayer = null;
        //int minCostToPlayer = int.MaxValue;
        //int minCostToTargetPlayer = int.MaxValue;


        //foreach (PlayerUnit player in playerUnits)
        //{
        //    // DijkstraPathfinder.FindReachableNodes �Ŏ擾���� PathNode ���g���ăv���C���[�̈ʒu�܂ł̃R�X�g���擾
        //    if (reachableNodes.ContainsKey(player.GetCurrentGridPostion()))
        //    {
        //        int costToCurrentPlayerPos = reachableNodes[player.GetCurrentGridPostion()].Cost;

        //        if (costToCurrentPlayerPos < minCostToPlayer)
        //        {
        //            minCostToPlayer = costToCurrentPlayerPos;
        //            closestPlayer = player;
        //        }
        //    }
        //}

        //if (closestPlayer == null)
        //{
        //    Debug.Log($"{name}: �ڋ߂ł���v���C���[���j�b�g��������܂���B");
        //    yield break; // �ڋ߂ł���v���C���[�����Ȃ��̂ōs���I��
        //}



        //Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableNode =
        //    DijkstraPathfinder.FindReachableTiles(this.CurrentGridPosition, this);

        //// ���B�\�Ȋe�}�X (reachablePos) ����A�ł��߂��v���C���[���j�b�g��T��
        //foreach (PlayerUnit player in playerUnits)
        //{
        //    // �e�v���C���[���j�b�g�ɑ΂��āA���̃v���C���[���U���ł���A�G���j�b�g�̓��B�\�ȃ}�X��S�ă`�F�b�N
        //    HashSet<Vector2Int> playerAttackableFromTiles = CalculateAttackRange(player.GetCurrentGridPostion());

        //    //�G���j�b�g�̓��B�\�ȃ}�X�̒�����AplayerAttackableFromTiles�Əd������}�X��T��
        //    foreach (Vector2Int possibleMovePos in reachableNode.Keys)
        //    {
        //        //possibleMoveToPos ���� player ���U���ł��邩�܂��́ApossibleMoveToPos �� player �̍U���͈͓��ɂ��邩
        //        if (playerAttackableFromTiles.Contains(possibleMovePos))
        //        {
        //            int costToMoveToPos = reachableNode[possibleMovePos].Cost;

        //            //���݌������Ă���ŏ��R�X�g��菬�����ꍇ
        //            if (costToMoveToPos < minCostToTargetPlayer)
        //            {
        //                minCostToTargetPlayer -= costToMoveToPos;
        //                bestMoveTargetPos = possibleMovePos;
        //                bestPath = DijkstraPathfinder.FindPath(GetCurrentGridPostion(), bestMoveTargetPos, this);
        //            }
        //        }
        //    }
        //}

        //HashSet<Vector2Int> attackableFromTilesForClosestPlayer = CalculateAttackRange(closestPlayer.GetCurrentGridPostion());

        ////�G���ړ��\�ȃ}�X�̒�����A���̃v���C���[���U���ł���}�X��T��
        //foreach (Vector2Int possibleMoveToPos in reachableNodes.Keys) // reachableNodes�͊��Ɉړ��͂Ńt�B���^�����O����Ă���
        //{
        //    // possibleMoveToPos ���� closestPlayer ���U���ł��邩
        //    if (attackableFromTilesForClosestPlayer.Contains(possibleMoveToPos))
        //    {
        //        int costToMoveToPos = reachableNodes[possibleMoveToPos].Cost;

        //        // ���݌������Ă���ŏ��R�X�g��菬�����ꍇ�A�܂��̓R�X�g�ŏ���D��
        //        if (costToMoveToPos < minCostToAttackPos)
        //        {
        //            minCostToAttackPos = costToMoveToPos;
        //            bestMoveTargetPos = possibleMoveToPos;
        //            bestPath = DijkstraPathfinder.FindPath(GetCurrentGridPostion(), bestMoveTargetPos, this);
        //        }
        //    }
        //}



        ////�o�H��������A���݂̈ʒu�ȊO�Ɉړ��悪����ꍇ�i�J�n�n�_���܂߂āj
        //if (bestPath != null && bestPath.Count > 1)
        //{
        //    Debug.Log($"{name}: ({GetCurrentGridPostion().x},{GetCurrentGridPostion().y}) ���� ({bestMoveTargetPos.x},{bestMoveTargetPos.y}) �ֈړ����܂��B");
        //    yield return StartCoroutine(MapManager.Instance.MoveUnitAlogPath(this, bestPath));
        //    yield return StartCoroutine(MapManager.Instance.SmoothMoveCoroutine(this, GetCurrentGridPostion(), bestMoveTargetPos, bestPath));
        //}
        //else
        //{
        //    Debug.Log($"{name}: �ړ��\�ȃv���C���[���j�b�g��������Ȃ����A���ɍœK�Ȉʒu�ɂ��܂��B");
        //}

        //�U���͈͓��Ƀv���C���[���j�b�g�����Ȃ���Έړ����Ȃ�
        //
        //Debug.Log($"{name}: �U���͈͓��Ƀv���C���[���j�b�g�����܂���B�ړ����܂���B");
        //yield break;
    }

   /// <summary>
   /// �D��I��AI�̑S�̓I�ȍs�����W�b�N
   /// </summary>
   /// <returns></returns>
    private IEnumerator HandleAggressiveAItest()
    {
        PlayerUnit targetPlayer = GetClosestPlayerUnit();
        if(targetPlayer == null)
        {
            Debug.Log($"{UnitName}: �^�[�Q�b�g�ƂȂ�v���C���[���j�b�g��������܂���");
            yield break;
        }


        Vector2Int bestMoveTargetPos = Vector2Int.zero;
        List<Vector2Int> bestPath = null;
        int minMax = 999;
        int minCostToAttackPos = minMax; // �G����U���\�ʒu�܂ł̃R�X�g
        PlayerUnit targetedPlayer = null;
        int minManhattanDistanceToPlayer = 999;//�ړ��ڕW�ʒu����v���C���[�܂ł̃}���n�b�^������

        Vector2Int originalCurrentGridPosition = CurrentGridPosition;




        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> TestreachableNodes =
            DijkstraPathfinder.FindReachableTiles(this.CurrentGridPosition, this);

        HashSet<Vector2Int> attackableTiles = CalculateAttackRange(targetPlayer.CurrentGridPosition);

        HashSet<Vector2Int> potentialEnemyMoveToAttackPositions = CalculateEnemyMoveToAttackPositions(targetPlayer.GetCurrentGridPostion());

        foreach (Vector2Int attackPosCandidate in potentialEnemyMoveToAttackPositions)
        {
            if (TestreachableNodes.ContainsKey(attackPosCandidate) && !MapManager.Instance.IsTileOccupiedForStooping(attackPosCandidate, this))
            {
                int costToMoveToAttackPos = TestreachableNodes[attackPosCandidate].Cost;

                int currentManhattanDistanceToPlayer =
               Mathf.Abs(attackPosCandidate.x - targetPlayer.GetCurrentGridPostion().x) + Mathf.Abs(attackPosCandidate.y - targetPlayer.GetCurrentGridPostion().y);


                // ���݌������Ă���ŏ��R�X�g��菬�����ꍇ�A�X�V
                if (costToMoveToAttackPos < minCostToAttackPos)
                {
                    minCostToAttackPos = costToMoveToAttackPos;
                    bestMoveTargetPos = attackPosCandidate;
                    targetedPlayer = targetPlayer; // ���̃v���C���[���^�[�Q�b�g�ɂ���
                    minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
                }
                //�ړ��R�X�g�������ł���΁A�v���C���[�ɋ߂��i�}���n�b�^���������Z���j���̂�D��
                else if (costToMoveToAttackPos == minCostToAttackPos)
                {
                    if (currentManhattanDistanceToPlayer < minManhattanDistanceToPlayer)
                    {
                        minManhattanDistanceToPlayer = currentManhattanDistanceToPlayer;
                        bestMoveTargetPos = attackPosCandidate;
                        targetedPlayer = targetPlayer;
                    }
                }
            }
        }
        Debug.LogWarning($"bestPos::{bestMoveTargetPos.x} ,{bestMoveTargetPos.y}");

        if (targetedPlayer != null && minCostToAttackPos != minMax)
        {
            // �ړ��R�X�g�����݂̈ړ��͈ȉ��ł��邱�Ƃ��m�F
            if (minCostToAttackPos <= CurrentMovementPoints)
            {
                MyTile newTile = MapManager.Instance.GetTileAt(bestMoveTargetPos);
                if (newTile != null)
                {
                    MoveToGridPosition(bestMoveTargetPos, newTile); // ��L���X�V
                }

                List<Vector2Int> AnimationPath = DijkstraPathfinder.GetPathToTarget(
                originalCurrentGridPosition,
                bestMoveTargetPos,
                this);
                Debug.LogWarning($"!!!!!!!!!!!!!!{AnimationPath.Count}");
                if (AnimationPath != null && AnimationPath.Count > 0)
                {
                    string pathString = string.Join(" -> ", AnimationPath.Select(p => $"({p.x},{p.y})"));
                    Debug.Log($"{name}: �ŏI����ڕW�ʒu: {bestMoveTargetPos}");
                    Debug.Log($"{name}: �ŏI����o�H: {pathString}");

                    Debug.Log($"{name}: ({originalCurrentGridPosition.x},{originalCurrentGridPosition.y}) ���� ({bestMoveTargetPos.x},{bestMoveTargetPos.y}) �ֈړ����܂��i�Ώۃv���C���[: {targetedPlayer.name}�j�B");
                    //�A�j���[�V������ Unit.AnimateMove ���g�p���Ă���͂��Ȃ̂ŁA�ȉ��ɏC��
                    yield return StartCoroutine(AnimateMove(AnimationPath));

                    //�����ɍU������
                    BattleManager.Instance.ResolveBattle_ShogiBase(this, targetedPlayer);

                    //�Q�[���I�[�o�[����
                    TurnManager.Instance.CheckGameOver();
                }
                else
                {
                    Debug.Log($"{name}: �o�H�v�Z�Ɏ��s�������A���ɍœK�Ȉʒu�ɂ��܂��B�ڕW�ʒu: {bestMoveTargetPos} (�o�H�Ȃ�)");

                    //�m�F�p
                    Debug.LogWarning($"{name}: �����ōU�������ōU��");
                    BattleManager.Instance.ResolveBattle_ShogiBase(this, targetedPlayer);


                    yield return null; // �ړ����Ȃ����s���͊���
                }
            }


        }
        else
        {
            Debug.LogWarning($"{UnitName}: �^�[�Q�b�g ({targetPlayer.UnitName}) ���U���͈͊O�ł��A�ړ������݂܂�");
            yield return StartCoroutine(PerformAggressiveMoveToAttackRange(targetPlayer));
        }
        //Debug.LogWarning($"{UnitName}: �^�[�Q�b�g ({targetPlayer.UnitName}) ���U���͈͊O�ł��A�ړ������݂܂�");
        //yield return StartCoroutine(PerformAggressiveMoveToAttackRange(targetPlayer));




        //if (potentialEnemyMoveToAttackPositions.Contains(bestMoveTargetPos))
        //{
        //    Debug.LogWarning($"{UnitName}: �^�[�Q�b�g ({targetPlayer.UnitName}) ���U���͈͓��ɂ��܂��A�U�����܂�");

        //    //�����ɍU������
        //    //BattleManager.Instance.ResolveBattle_ShogiBase(this, targetPlayer);

        //    //�Q�[���I�[�o�[����
        //    //TurnManager.Instance.CheckGameOver();

        //    yield return StartCoroutine(PerformAttack(targetPlayer));
        //}
        //else
        //{
        //    Debug.LogWarning($"{UnitName}: �^�[�Q�b�g ({targetPlayer.UnitName}) ���U���͈͊O�ł��A�ړ������݂܂�");
        //    yield return StartCoroutine(PerformAggressiveMoveToAttackRange(targetPlayer));
        //}

        //yield break;
    }

    /// <summary>
    /// �D��I��AI�̈ړ����W�b�N
    /// �^�[�Q�b�g�v���C���[�ɍł��߂Â��悤�Ɉړ�����
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <returns></returns>
    private IEnumerator PerformAggressiveMoveToAttackRange(PlayerUnit targetPlayer)
    {
        //Dictionary<Vector2Int, PathNodes> reachableTiles = DijkstraPathfinder.FindReachableNodes(GetCurrentGridPostion(), this);

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableTiles =
            DijkstraPathfinder.FindReachableTiles(CurrentGridPosition, this);

        Vector2Int bestMovePos = CurrentGridPosition;
        Vector2Int originalCurrentGridPosition = CurrentGridPosition;
        int minDistanceToTarget = int.MaxValue;

        List<Vector2Int> candidateTiles = new List<Vector2Int>();
        foreach(var tilePos in reachableTiles.Keys)
        {
            if (!MapManager.Instance.IsTileOccupiedForStooping(tilePos, this))
            {
                candidateTiles.Add(tilePos);
            }
        }

        if(candidateTiles.Count == 0)
        {
            Debug.Log($"{UnitName}: �ړ��\�ȃ}�X���Ȃ����߁A�ړ����܂���");
            yield break;
        }

        Vector2Int targetPos = originalCurrentGridPosition;
        bool moved = false;

        foreach(Vector2Int moveCandidate in candidateTiles)
        {
            int dist = Mathf.Abs(moveCandidate.x - targetPlayer.CurrentGridPosition.x) + 
                Mathf.Abs(moveCandidate.y - targetPlayer.CurrentGridPosition.y);

            if(dist >= _minAttackRange && dist <= _maxAttackRange)
            {
                if(dist < minDistanceToTarget)
                {
                    minDistanceToTarget = dist;
                    bestMovePos = moveCandidate;
                }
            }
            else if(dist < minDistanceToTarget)
            {
                minDistanceToTarget = dist;
                bestMovePos = moveCandidate;
            }
        }
        
        targetPos = bestMovePos;
        Debug.Log($"{UnitName}: �ړ��^AI���ڕW�n�_�� {targetPos} �Ɍ��肵�܂���");

        MyTile targetTile = MapManager.Instance.GetTileAt(targetPos);
        if(targetTile != null)
        {
            MoveToGridPosition(targetPos,targetTile);
        }

        List<Vector2Int> pathForAnimation = DijkstraPathfinder.FindPath(originalCurrentGridPosition, targetPos, this);
        if(pathForAnimation == null || pathForAnimation.Count == 0)
        {
            pathForAnimation = new List<Vector2Int>() { originalCurrentGridPosition };
        }

        yield return AnimateMove(pathForAnimation);
    }


    ///�ǉ��̓GAI�F�F��Ƀv���C���[���j�b�g�֌������Ĕ͈͓��ł���΍U��

    //�G���j�b�g�̍s���𐧌䂷�郁�C�����\�b�h
    public void TakeTurnNomalAI(PlayerUnit targetPlayerUnit)
    {
        PlayerUnit targetPlayer = GetClosestPlayerUnit();

        if(targetPlayer != null)
        {
           
        }
    }

    

    /// <summary>
    /// �G���j�b�g�ɂ��U�������s����(������2025/07)
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator PerformAttack(PlayerUnit target)
    {
        Debug.Log($"{UnitName} �� {target.UnitName} ���U���I");
        yield return new WaitForSeconds(0.5f); 
        Debug.Log("�U�����܂���");
    }

    /// <summary>
    /// �ł��߂��v���C���[���j�b�g���擾����
    /// </summary>
    /// <returns></returns>
    private PlayerUnit GetClosestPlayerUnit()
    {
        PlayerUnit closesetPlayer = null;
        int minDistance = int.MaxValue;

        foreach(PlayerUnit player in TurnManager.Instance.SetAllPlayerUnit())
        {
            if (player == null)
            {
                continue;
            }

            int dist = Mathf.Abs(CurrentGridPosition.x - player.CurrentGridPosition.x) + 
                Mathf.Abs(CurrentGridPosition.y - player.CurrentGridPosition.y);

            if (dist < minDistance)
            {
                minDistance = dist;
                closesetPlayer = player;
            }
        }
        return closesetPlayer;
    }


    /// <summary>
    /// �w�肳�ꂽ�ʒu����̍U���\�͈͂̃O���b�h���W���v�Z����
    /// </summary>
    private HashSet<Vector2Int> CalculateAttackRange(Vector2Int centerPos)
    {
        HashSet<Vector2Int> attackableTiles = new  HashSet<Vector2Int>();
        
        //�m�F�p
        Debug.Log($"--- CalculateAttackRange for center: {centerPos} (Min:{_minAttackRange}, Max:{_maxAttackRange}) ---");

        for (int x = -_maxAttackRange;x <= _maxAttackRange; x++)
        {
            for(int y = -_maxAttackRange;y <= _maxAttackRange; y++)
            {
                Vector2Int potentialAttackPos = centerPos + new Vector2Int(x, y);
                int distance = Mathf.Abs(x) + Mathf.Abs(y);//�}���n�b�^��

                if(distance >= _minAttackRange && distance <= _maxAttackRange)
                {
                    if (MapManager.Instance.IsValidGridPosition(potentialAttackPos))
                    {
                        attackableTiles.Add(potentialAttackPos);
                        //�m�F�p
                        Debug.Log($"  [AttackRange] Valid: {potentialAttackPos} (Distance: {distance})");
                    }
                    else
                    {
                        //�m�F�p
                        Debug.Log($"  [AttackRange] Invalid Grid Pos: {potentialAttackPos} (Distance: {distance})");
                    }
                }
            }
        }
        //�m�F�p
        Debug.Log($"--- CalculateAttackRange End. Total Valid Attackable Tiles: {attackableTiles.Count} ---");

        return attackableTiles;
    }


    /// <summary>
    /// �w�肳�ꂽ�v���C���[�̈ʒu���猩�āA���̓G���j�b�g���U���\�ȃ}�X���v�Z����
    /// </summary>
    /// <param name="playerPos">�^�[�Q�b�g�ƂȂ�v���C���[�̃O���b�h���W</param>
    /// <returns>�G���j�b�g���ړ����ׂ��U���\�ʒu�̃��X�g�iHashSet�ŏd���Ȃ��j</returns>
    private HashSet<Vector2Int> CalculateEnemyMoveToAttackPositions(Vector2Int playerPos)
    {
        HashSet<Vector2Int> potentialEnemyAttackMovePositions = new HashSet<Vector2Int>();

        //�v���C���[�̈ʒu���猩�āA_minAttackRange ���� _maxAttackRange �͈̔͂ɂ���}�X��T��
        //�G���j�b�g���v���C���[���U�����邽�߂Ɉړ��ł�����̃}�X���t�Z���Čv�Z����
        for(int x = -_maxAttackRange;x <= _maxAttackRange; x++)
        {
            for(int y = -_maxAttackRange;y <= _maxAttackRange; y++)
            {
                //�v���C���[�ʒu����̑��΍��W�ŁA�G���j�b�g���ʒu����\���̂���}�X���v�Z
                Vector2Int potentialEnemyPos = playerPos + new Vector2Int(x, y);

                //�G���j�b�g�� potentialEnemyPos �Ɉړ������ꍇ�A�v���C���[ (playerPos) �Ƃ̋������v�Z
                int distance = Mathf.Abs(potentialEnemyPos.x - playerPos.x) +
                               Mathf.Abs(potentialEnemyPos.y - playerPos.y); // �}���n�b�^������

                //�G���j�b�g�̍U���˒������`�F�b�N
                if(distance >= _minAttackRange && distance <= _maxAttackRange)
                {
                    // ���̈ʒu�ɓG���j�b�g���ړ��ł���΁A�v���C���[���U���\
                    // �������AMapManager.Instance.IsValidGridPosition �̃`�F�b�N�͕K�{
                    if (MapManager.Instance.IsValidGridPosition(potentialEnemyPos))
                    {
                        // ���̃}�X�́A�G���j�b�g���ړ����ăv���C���[���U���ł���L���Ȍ��ł���
                        potentialEnemyAttackMovePositions.Add(potentialEnemyPos);
                        Debug.LogWarning($"�U���\�}�X�F{potentialEnemyPos.x}:::{potentialEnemyPos.y}");
                    }
                }
            }
        }
        return potentialEnemyAttackMovePositions;
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
