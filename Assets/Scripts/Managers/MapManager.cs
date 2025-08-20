using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.Tilemaps;

/// Tiles.cs�ֈڍs�i�ς݁j2025/06
/// < summary >
/// �}�b�v��̊e�O���b�h�̏���ێ�����N���X
/// </ summary >
//public partial class Tile
//{
//    [field: SerializeField] public Vector2Int GridPosition { get; private set; }//�O���b�h�̍��W
//    [field: SerializeField] public TerrainType Type { get; private set; }//�n�`�̎��
//    [field: SerializeField] public Unit OccupyingUnit { get; private set; }//���̃O���b�h�ɑ��݂��郆�j�b�g

//    //�R���X�g���N�^
//    public Tile(Vector2Int position, TerrainType type)
//    {
//        GridPosition = position;
//        Type = type;
//        OccupyingUnit = null;//������Ԃł̓��j�b�g�͂��Ȃ�
//    }

//    /// <summary>
//    /// ���̃^�C���Ƀ��j�b�g��ݒ肷��
//    /// </summary>
//    /// <param name="unit">�ݒ肷�郆�j�b�g</param>
//    public void SetOccupyingUnit(Unit unit)
//    {
//        OccupyingUnit = unit;
//    }

//    /// <summary>
//    /// ���̃^�C���̖h��{�[�i�X���擾
//    /// </summary>
//    /// c<return>�h��{�[�i�X</return>
//    public int GetDefenseBonus()
//    {
//        switch (Type)
//        {
//            case TerrainType.Forest: return 1;
//            case TerrainType.Mountain: return 1;
//            case TerrainType.Desert: return 1;
//            case TerrainType.Snow: return 1;
//            //case TerrainType.Flooded: return -1;
//            //case TerrainType.Landslide: return -1;
//            default: return 0;
//        }
//    }

//    /// <summary>
//    /// ���̃^�C���̉���{�[�i�X���擾
//    /// </summary>
//    /// c<return>����{�[�i�X</return>
//    public int GetEvadeBonus()
//    {
//        switch (Type)
//        {
//            case TerrainType.Forest: return 10;
//            case TerrainType.Mountain: return 10;
//            //case TerrainType.Desert: return -10;
//            //case TerrainType.Snow: return -10;
//            //case TerrainType.Flooded: return -10;
//            //case TerrainType.Landslide: return -10;
//            default: return 0;
//        }
//    }

//    /// <summary>
//    /// �^�C���̒n�`�^�C�v��ݒ肷��
//    /// (Type�v���p�e�B��private�@set�̂��߁A�O������ύX���邽�߂̃��\�b�h)
//    /// </summary>
//    /// <param name="newType"></param>
//    public void SetType(TerrainType newType)
//    {
//        Type = newType;
//    }
//}

////
[System.Serializable]
public class TileMapping
{
    public TerrainType terrainType;
    public TileBase tileBase;
}


/// <summary>
/// �Q�[���}�b�v�̐����A�Ǘ��A�n�`���ʂȂǂ������V���O���g���N���X
/// </summary>
public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MapManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("MapManager");
                    _instance = singletonObject.AddComponent<MapManager>();
                }
            }
            return _instance;
        }
    }

    //�I�t�Z�b�g�t�B�[���h
    [SerializeField] private Vector3 _offset = Vector3.zero;

    [field: SerializeField] private Vector2Int _gridSize;//�}�b�v�̃O���b�h�T�C�Y
    public Vector2Int GridSize => _gridSize;

    [SerializeField] private MyTile[,] _tileGrid;//�e�O���b�h�̏����i�[����2�����z��(Inspector�\���s�̂���[SerializeField]�͖���)

    //�}�b�v�쐬�p
    [SerializeField] private string[] _mapSequence;//Resouces�t�H���_�ȉ���CSV�t�@�C���̃p�X
    [SerializeField] private GameObject _tilePrefab;//�e�^�C�������Ɏg�p����MyTile�R���|�[�l���g���t����Prefab
    [SerializeField] private float _tileSize = 1.0f;//�O���b�h��1�}�X������̃��[���h���W�ł̃T�C�Y
    [SerializeField] private Sprite[] _terrainSprite;//�n�`�^�C�v�ɑΉ�����X�v���C�g��ݒ肷�邽�߂̔z��

    [SerializeField] private TerrainCost[] _terrainCosts;

    private int _currentMapIndex = 0;//���݃��[�h���Ă���}�b�v�̃C���f�b�N�X
    private MapData _currentMapData;//MapDtaLoader�ɂ���ēǂݍ��܂��}�b�v�f�[�^

    private Dictionary<Vector2Int, MyTile> _tileData = new Dictionary<Vector2Int, MyTile>();//�������ꂽ�S�Ă�Tile�I�u�W�F�N�g�ƃO���b�h���W���Ǘ�����

    //�V�n���V�X�e���̒n�`�ω��֘A
    [SerializeField] private List<TileMapping> _terrainTileList;
    [SerializeField] private Dictionary<TerrainType, TileBase> _terrainTiles = new Dictionary<TerrainType, TileBase>();
    //�ꎞ�I�ɃR�����g�A�E�g
    //[SerializeField] private Tilemap _groundTilemap;


    //Tilemap�֘A
    [SerializeField] private Tilemap _generateTilemap;
    private Vector2Int _tilemapGridSize;
    private BoundsInt _tilemapBounds;
    private Dictionary<Vector2Int, Tile> _tileDataFromTilemap = new Dictionary<Vector2Int, Tile>();
    private Dictionary<Vector2Int, MyTile> _tileDataFromTilemapTest = new Dictionary<Vector2Int, MyTile>();
    [SerializeField] private GameObject _tilePrefabTilemap;//�e�^�C�������Ɏg�p����MyTile�R���|�[�l���g���t����Prefab

    //�ړ��֘A
    //�n�C���C�g�\���֘A
    [SerializeField] private GameObject _movableHighlightPrefab;//�ړ��p�̐F�n�C���C�g�v���n�u
    [SerializeField] private GameObject _attackHighlightPrefab;//�U���\�͈͗p�̐ԐF�n�C���C�g�v���n�u
    private Dictionary<Vector2Int, GameObject> _currentHighlights = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, GameObject> _activeHighlights = new Dictionary<Vector2Int, GameObject>();
    [SerializeField] private LineRenderer _pathLinePrefab;//�o�H�\���p�̃v���n�u
    private LineRenderer _currentPathLine;//���ݕ\������Ă���o�H���C��
    //�m�F�p�n�C���C�g
    [SerializeField] private GameObject _occupiedHighlightPrefab;//���j�b�g��L�p�̃n�C���C�g

    //���j�b�g�̈ړ���Ԃ��Ǘ����邽�߂̕ϐ�
    private Vector2Int _originalUnitPositon = Vector2Int.zero;//�ړ��O�̃O���b�h���W
    private Vector2Int _currentPlannedMovePositon = Vector2Int.zero;//�ړ���̃O���b�h���W
    private bool _isMovingOrPlanning = false;//�ړ��v�撆�܂��͈ړ������������t���O
    private bool _isConfirmingMove = false;//�ړ��m��҂���Ԃ��ǂ����������t���O
    private MyTile _originalTile;

    //���j�b�g�̈ړ��L�����Z�����Ǘ�����
    private bool _canceled = false;//��x�ł��L�����Z���������������t���O

    //���j�b�g�̍U���֘A
    private bool _isAttacking = false;//�U���J�n��Ԃ��������t���O


    //PlayerUnit�֘A
    [SerializeField] private GameObject _playerUnitPrefab;
    private Unit _currentPlayerUnit;//���݂̃v���C���[���j�b�g�̎Q��
    private Unit _selectedUnit;//�I�𒆂̃v���C���[���j�b�g
    private PlayerUnit _selectedplayerUnit;

    //EnemyUnit�֘A
    [SerializeField] private GameObject _enemyUnitprefab;
    private Unit _currentEnemyUnit;//���݂̓G���j�b�g�̎Q��
    private Unit _selectedEnemyUnit;//�I�𒆂̓G���j�b�g

    //�e�̃��j�b�g�̃��X�g(�Ǘ����ꊇ�����邩�ʉ����邩������2025/07�j
    //�e���j�b�g���X�g
    private List<PlayerUnit> _allPlayerUnits = new List<PlayerUnit>();
    private List<EnemyUnit> _allEnemyUnits = new List<EnemyUnit>();
    //2025/07���Ƃ��Čʉ��Ŏ����i�ꕔ�ꊇ�p�̏������L�ځj
    private List<Unit> _allUnit = new List<Unit>();
    //private List<PlayerUnit> _playerUnit;
    //private List<EnemyUnit> _enemyUnit;

    // �V�����p�ӂ������j�b�g�v���n�u�̃t�B�[���h
    [SerializeField] private PlayerUnit _player001Prefab;
    [SerializeField] private PlayerUnit _player002Prefab;
    [SerializeField] private EnemyUnit _enemy001Prefab;
    [SerializeField] private EnemyUnit _enemy002Prefab;


    //�J�����ݒ�֘A
    [SerializeField] private Camera _mainCamera;//���C���J�����ւ̎Q�Ɨp
    [SerializeField] private float _cameraMoveSpeed = 5.0f;//�J�����̒Ǐ]���x

    //�P�^�C���̃��[���h���W��̃T�C�Y�i�J�����ړ��̉��d�l2025/07�j
    [SerializeField] private float _tileWorldSize = 1.0f;

    //�Q�[����ʂɌŒ�\������O���b�h�͈�
    private const int _visibleGridWidth = 16;//��16�}�X
    private const int _visibleGridHeight = 10;//�c9�}�X

    private Vector3 _cameraTargetPosition;//�J�����̖ڕW�ʒu
    private const float _cameraFixedZPosition = -10f;//2D�Ȃ̂�Z���͌Œ�

    //[SerializeField]
    private Grid _tilemapGrid;//�J�����ړ������̂��߂Ɋ�_�O���b�h���W(0,0)�̃��[���h���W�̎擾�p
    Vector2Int tileBasePos = new Vector2Int(-4, -3);


    //�퓬�t�F�C�Y�Ǘ�
    [SerializeField] private GameManager _gameManager;
    private bool _isInBattleDeployment = true;// �퓬�����t�F�C�Y���ǂ����̃t���O���ȍ~�����t�F�C�Y�Ƃ���



    [System.Serializable] public class TerrainCost
    {
        public TerrainType terrainType;
        public int cost;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }

        foreach(var mapping in _terrainTileList)
        {
            if (!_terrainTiles.ContainsKey(mapping.terrainType))
            {
                _terrainTiles.Add(mapping.terrainType, mapping.tileBase);
            }
        }
    }


    //�����t�F�C�Y�p�̃��\�b�h
    // �}�E�X���͂Ń^�C�������擾���邽�߂̏���
    private void HandleMouseInputInBattlePreparation()
    {
        // ���[���h���W����O���b�h���W���擾
        Vector3 mouseworldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseworldPos.z = 0;
        Vector2Int clickedgridPos = GetGridPositionFromWorld(mouseworldPos);

        //ToDo->GetTileAt----GetTileFromTIlemap
        //�m�F�̂��߈ꎞ�I�Ɉړ�
        MyTile _clickedTile = GetTileAt(clickedgridPos);

        //Tilemap
        //MyTile _clickedTileTilemap = GetTileAtFromTilemap(clickedgridPos);
        //if (_clickedTileTilemap != null)
        //{
        //    Debug.LogWarning($"�퓬�����t�F�C�Y: �O���b�h���W({clickedgridPos.x}, {clickedgridPos.y})�̃^�C��{_clickedTileTilemap.TerrainType}���N���b�N���܂����B");
        //}
        //else if (_clickedTile == null)
        //{
        //    Debug.Log("�}�b�v�͈͊O�ł�");
        //    return;
        //}


        //�O���b�h���W�̃^�C�������擾
        //MyTile _clickedTile = GetTileAt(clickedgridPos);
        if (_clickedTile != null)
        {
            Debug.Log($"�퓬�����t�F�C�Y: �O���b�h���W({clickedgridPos.x}, {clickedgridPos.y})�̃^�C��{_clickedTile.TerrainType}���N���b�N���܂����B");
        }
        else if (_clickedTile == null)
        {
            Debug.Log("�}�b�v�͈͊O�ł�");
            return;
        }



        //���j�b�g�����I���̏ꍇ
        if (_selectedUnit == null)
        {
            //�N���b�N���ꂽ�^�C���Ƀ��j�b�g�����邩�A���v���C���[���j�b�g���A�����s����
            if (_clickedTile.OccupyingUnit != null &&
                _clickedTile.OccupyingUnit.Faction == FactionType.Player &&
                !_clickedTile.OccupyingUnit.HasActedThisTurn)
            {
                SelectUnit(_clickedTile.OccupyingUnit);
                _originalTile = _clickedTile;
            }
            //�G���j�b�g�̏ꍇ�s���͈͗\����\���i������2025 / 07�j
            else if (_clickedTile.OccupyingUnit != null &&
                _clickedTile.OccupyingUnit.Faction == FactionType.Enemy &&
                !_clickedTile.OccupyingUnit.HasActedThisTurn)
            {
                Debug.Log("�G���j�b�g�ł�");
                SelectUnit(_clickedTile.OccupyingUnit);
            }
            else
            {
                //���j�b�g�����I���̏�ԂŃn�C���C�g���o�Ȃ��悤��
                ClearAllHighlights();
                //////ClearMovableRangeDisplay();
                Debug.LogWarning("!!!");
            }
        }
        else
        {
            //�I�𒆂̃��j�b�g�Ɠ������j�b�g���N���b�N���ꂽ��I������
            if (_clickedTile.OccupyingUnit == _selectedUnit)
            {
                CancelMove();
            }
            //���̃v���C���[���j�b�g���N���b�N���ꂽ��A���݂̑I�����������B�V�������j�b�g��I��
            else if (_clickedTile.OccupyingUnit != null && _clickedTile.OccupyingUnit.Faction == FactionType.Player && _clickedTile.OccupyingUnit != _selectedUnit)
            {
                CancelMove();//���݂̑I��������
                SelectUnit(_clickedTile.OccupyingUnit);//�V�������j�b�g��I��
            }

            //�ړ��͈͊O�̋�̃^�C����G���j�b�g���N���b�N������L�����Z��
            else
            {
                CancelMove();
            }
        }
        
    }


    /// <summary>
    /// �}�b�v�̃T�C�Y�ɍ��킹�ăJ�����̕\���͈͂������ݒ肷��
    /// </summary>
    private void InitializeCamera()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogError("���C���J������������܂���B");
                return;
            }
        }


        // �\������O���b�h�̉����Əc�����A�Œ�l�Ǝ��ۂ̃}�b�v�T�C�Y�̑傫�������Q�Ƃ���
        //float targetWidth = Mathf.Max(_visibleGridWidth, _currentMapData.Width);
        //float targetHeight = Mathf.Max(_visibleGridHeight, _currentMapData.Height);

        //_mainCamera.orthographicSize = (float)_visibleGridHeight / 2.0f;


        _mainCamera.orthographicSize = (_visibleGridWidth * _tileWorldSize) / (16f / 9f * 2f);



        //_mainCamera.orthographicSize = (_visibleGridWidth * _tileWorldSize) / (_mainCamera.aspect * 2.0f);



        ////float fixedZPosition = -10;
        //float cameraPosRevise = 0.5f;

        //// �}�b�v���Œ�\���͈͂�菬�����ꍇ�A�J�������}�b�v�̒����ɐݒ�
        //if (_currentMapData.Width <= _visibleGridWidth && _currentMapData.Height <= _visibleGridHeight)
        //{
        //    _cameraTargetGridPosition = new Vector2Int(_currentMapData.Width / 2,_currentMapData.Height / 2);
        //}
        //else // �}�b�v���Œ�\���͈͂��傫���ꍇ
        //{
        //    Debug.LogWarning("�傫��");

        //    _cameraTargetGridPosition = new Vector2Int(_visibleGridWidth / 2, _visibleGridHeight / 2); // �}�b�v��(0,0)�O���b�h����ʂ̍������ɕ\�������悤��
        //}

        //Vector3 targetWorldPosition = GetWorldPositionFromGrid(_cameraTargetGridPosition);
        //_mainCamera.transform.position = new Vector3(targetWorldPosition.x,targetWorldPosition.y,-10);
        //Debug.LogWarning($"{_cameraTargetGridPosition.x},{_cameraTargetGridPosition.y}");

        //���[���h���W�ƃO���b�h���W�Ƃ̂��荇�킹
        //Vector3 tileBaseWorldPos = GetTileWorldPosition(Vector2Int.zero);
        //Vector2Int tileBasePos =  new Vector2Int(-4, -3);


        //float cameraCenterX = tileBaseWorldPos.x + ((float)_visibleGridWidth * _tileWorldSize) / 2.0f;
        //float cameraCenterY = tileBaseWorldPos.y + ((float)_visibleGridHeight * _tileWorldSize) / 2.0f;



        float camHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;

        float camHalfHeight = _mainCamera.orthographicSize;
        _cameraTargetPosition = new Vector3(
            camHalfWidth + tileBasePos.x,
            camHalfHeight + tileBasePos.y,
            _cameraFixedZPosition
        );

        //_cameraTargetPosition = new Vector3(
        //    cameraCenterX,
        //    cameraCenterY,
        //    _cameraFixedZPosition
        //);



        //// �}�b�v���Œ�\���͈͂�菬�����ꍇ�A�J�������}�b�v�̒����ɐݒ�
        //if (_currentMapData.Width <= _visibleGridWidth && _currentMapData.Height <= _visibleGridHeight)
        //{
        //    _cameraTargetPosition = new Vector3(
        //       ((float)_currentMapData.Width * _tileWorldSize) / 2.0f,
        //       ((float)_currentMapData.Height * _tileWorldSize) / 2.0f,
        //       _cameraFixedZPosition
        //   );
        //}
        //else // �}�b�v���Œ�\���͈͂��傫���ꍇ
        //{
        //    _cameraTargetPosition = new Vector3(
        //       camHalfWidth,  // �J�����̒��S���A��ʂ̍��[�����[���h���W��0�ɂȂ�ʒu�ɐݒ�
        //       camHalfHeight, // �J�����̒��S���A��ʂ̉��[�����[���h���W��0�ɂȂ�ʒu�ɐݒ�
        //       _cameraFixedZPosition
        //   );
        //}



        _mainCamera.transform.position = _cameraTargetPosition;






        //�J�����̈ʒu���}�b�v�̒��S�ɐݒ�
        //if (targetWidth > targetHeight)
        //{
        //    cameraTargetPosition = new Vector3(
        //        targetWidth / 2.0f * _tileWorldSize,
        //        targetHeight / 2.0f * _tileWorldSize,
        //        _mainCamera.transform.position.z);

        //    float mapCenterX = ((float)_currentMapData.Width * _tileWorldSize) / 2.0f - _tileWorldSize;
        //    float mapCenterY = ((float)_currentMapData.Height * _tileWorldSize) / 2.0f - _tileWorldSize;

        //    float camHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;
        //    float camHalfHeight = _mainCamera.orthographicSize;

        //    float clampedX = Mathf.Clamp(mapCenterX, camHalfWidth, (_currentMapData.Width * _tileWorldSize) - camHalfWidth);
        //    float clampedY = Mathf.Clamp(mapCenterY, camHalfHeight, (_currentMapData.Height * _tileWorldSize) - camHalfHeight);
        //}

        //Vector3 _cameraTargetPosition = new Vector3(clampedX - 1, clampedY - 1, _cameraFixedZPosition);

        //�m�F�p
        //Debug.LogWarning($"���C���J�����̏������W�́BX{_cameraTargetPosition.x},Y{_cameraTargetPosition.y},Z{_cameraTargetPosition.z}");
        //Debug.LogWarning($"�}�b�v�̑傫���BX{_currentMapData.Width},Y{_currentMapData.Height}");


        // �}�b�v���Œ�\���͈͂�菬�����ꍇ�A�J�������}�b�v�̒����ɔz�u
        //_mainCamera.transform.position = _cameraTargetPosition;

    }


    //���d�l2025/07
    /// <summary>
    /// �L�[�{�[�h���͂ɉ����ăJ�����̖ڕW�ʒu���X�V����
    /// </summary>
    private void HandleCameraInput()
    {
        //ToDo->GetTileAt----GetTileFromTIlemap
        if (_tilemapGridSize.x <= _visibleGridWidth && _tilemapGridSize.y <= _visibleGridHeight)
        // �}�b�v���Œ�\���͈͂��傫���ꍇ�̂݁A�J�������ړ�������
        // if (_currentMapData.Width <= _visibleGridWidth && _currentMapData.Height <= _visibleGridHeight)
        {
            return;
        }

        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDirection.x += 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveDirection.x -= 1;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveDirection.y += 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDirection.y -= 1;
        }

        if (moveDirection != Vector3.zero)
        {
            _cameraTargetPosition += moveDirection * _tileWorldSize;

            _cameraTargetPosition.z = _cameraFixedZPosition;

            //�J�����̒Ǐ]�͈͂��v�Z
            float camHalfHeight = _mainCamera.orthographicSize;
            float camHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;

            Vector3 mapMinWorldPos = GetWorldPositionFromGrid(Vector2Int.zero);
            Vector3 mapMaxWorldPos = GetWorldPositionFromGrid(new Vector2Int(_currentMapData.Width - 1, _currentMapData.Height - 1));


            float minX = camHalfWidth + tileBasePos.x;
            //ToDo->GetTileAt----GetTileFromTIlemap
            //float maxX = (_currentMapData.Width * _tileWorldSize) - camHalfWidth + tileBasePos.x;
            float maxX = (_tilemapGridSize.x * _tileWorldSize) - camHalfWidth + tileBasePos.x;
            float minY = camHalfHeight + tileBasePos.y;
            //ToDo->GetTileAt----GetTileFromTIlemap
            //float maxY = (_currentMapData.Height * _tileWorldSize) - camHalfHeight + tileBasePos.y;
            float maxY = (_tilemapGridSize.y * _tileWorldSize) - camHalfHeight + tileBasePos.y;

            //float minX = mapMinWorldPos.x + camHalfWidth;
            //float maxX = mapMaxWorldPos.x + camHalfWidth;
            //float minY = mapMinWorldPos.y + camHalfHeight;
            //float maxY = mapMaxWorldPos.y + camHalfHeight;

            // �}�b�v���Œ�\���͈͂��傫���ꍇ�A maxX, maxY�̌v�Z�̓}�b�v�̃��[���h���W�̉E�[����Ƃ���
            //if (_currentMapData.Width > _visibleGridWidth)
            //{
            //    maxX = mapMinWorldPos.x + (_currentMapData.Width * _tileWorldSize) - camHalfWidth;
            //}
            //if (_currentMapData.Height > _visibleGridHeight)
            //{
            //    maxY = mapMinWorldPos.y + (_currentMapData.Height * _tileWorldSize) - camHalfHeight;
            //}

            //�ڕW�ʒu���}�b�v�̒[�ɃN�����v
            _cameraTargetPosition.x = Mathf.Clamp(_cameraTargetPosition.x, minX, maxX);
            _cameraTargetPosition.y = Mathf.Clamp(_cameraTargetPosition.y, minY, maxY);

            Vector3 newPos = new Vector3(_cameraTargetPosition.x, _cameraTargetPosition.y, _cameraTargetPosition.z);


            //�m�F�p
            //Debug.LogWarning($"���݂̃J�����̈ʒu�FX{_cameraTargetPosition.x},Y{_cameraTargetPosition.y}");
            //Debug.LogWarning($"newPos�FX{newPos.x},Y{newPos.y}");

            _mainCamera.transform.position = newPos;
        }
    }

    /// <summary>
    /// �J������ڕW�ʒu�܂ňړ�������
    /// </summary>
    private void MoveCameraToTarget()
    {
        Vector3 newPos = Vector3.Lerp(
            _mainCamera.transform.position,
            _cameraTargetPosition,
            _cameraMoveSpeed * Time.deltaTime
            );

        newPos.z = _cameraFixedZPosition;
        _mainCamera.transform.position = newPos;
    }


    /// <summary>
    /// �O���b�h���W���烏�[���h���W���擾����w���p�[���\�b�h
    /// �^�C���̍���������ɂ���
    /// </summary>
    /// <param name="gridPosition">�O���b�h���W</param>
    /// <returns>�^�C���̍������̃��[���h���W</returns>
    private Vector3 GetTileWorldPosition(Vector2Int gridPosition)
    {
        if (_tilemapGrid == null)
        {
            Debug.LogError("Tilemap Grid���ݒ肳��Ă��܂���B");
            return Vector3.zero;
        }

        // CellToWorld���\�b�h�̓^�C���̍�������Ԃ����߁A���̂܂܎g�p����
        Vector3 worldPos = _tilemapGrid.CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));

        return worldPos;
    }



    //����������
    public void Initialize()
    {
        Debug.LogWarning("�����t�F�C�Y����n�܂�܂�");


        //�^�C�����X�g�ƃ��j�b�g���X�g�̃N���A
        ClaerExistingUnits();
        ClearMap();

        //�f�o�b�O�p
        _currentMapIndex = 0;

        //string currentMapId = _mapSequence[_currentMapIndex];
        //GenerateMap(currentMapId);


        GenerateMapFromTilemap();


        //ToDo->�ꎞ�I�ɃR�����g�A�E�g
        PlaceEnemiesForCurrentMap("Maps/map_00");



        //_allPlayerUnits.Clear();
        //_allEnemyUnits.Clear();

        //�����̃��j�b�g�z�u2025/07

        //�v���C���[���j�b�g
        //GameObject player1GO = Instantiate(_playerUnitPrefab,transform);
        //PlayerUnit player1 = player1GO.GetComponent<PlayerUnit>();
        //if (player1 != null)
        //{
        //    player1.name = "Player001";
        //    PlaceUnit(player1,new Vector2Int(0,4));
        //}
        //else
        //{
        //    Debug.LogError("Player1�v���n�u��PlayerUnit�R���|�[�l���g��������܂���I");
        //}


        if (_player001Prefab != null)
        {
            PlayerUnit player001 = Instantiate(_player001Prefab, transform);
            PlaceUnit(player001, new Vector2Int(5, 0));
        }
        else
        {
            Debug.LogError("MapManager: _player001Prefab�����蓖�Ă��Ă��܂���I");
        }

        //if (_player002Prefab != null)
        //{
        //    PlayerUnit player002 = Instantiate(_player002Prefab, transform);
        //    PlaceUnit(player002, new Vector2Int(0, 2));
        //}
        //else
        //{
        //    Debug.LogError("MapManager: _player002Prefab�����蓖�Ă��Ă��܂���I");
        //}

        //�G���j�b�g
        //GameObject enemy1Go = Instantiate(_enemyUnitprefab, transform);
        //EnemyUnit enemy1 = enemy1Go.GetComponent<EnemyUnit>();
        //if (enemy1 != null)
        //{
        //    enemy1.name = "Enemy001";
        //    PlaceUnit(enemy1, new Vector2Int(5, 0));
        //}
        //else
        //{
        //    Debug.LogError("Enemy001�v���n�u��PlayerUnit�R���|�[�l���g��������܂���I");
        //}


        //�G���j�b�g�̐���������ύX2025/07
        //if(_enemy001Prefab != null)
        //{
        //    EnemyUnit enemy001 = Instantiate(_enemy001Prefab, transform);
        //    PlaceUnit(enemy001, new Vector2Int(5, 0));
        //}
        //else
        //{
        //    Debug.LogError("MapManager: _enemy001Prefab�����蓖�Ă��Ă��܂���I");
        //}

        //if(_enemy002Prefab != null)
        //{
        //    EnemyUnit enemy002 = Instantiate(_enemy002Prefab, transform);
        //    PlaceUnit(enemy002, new Vector2Int(4, 4));
        //}
        //else
        //{
        //    Debug.LogError("MapManager: _enemy002Prefab�����蓖�Ă��Ă��܂���I");
        //}

        TurnManager.Instance.InitializeTurnManager();
    }

    //GenerateMap�Ƃ��ď����𓝍�2025/06
    /// <summary>
    /// �X�e�[�WID�Ɋ�Â��ă}�b�v�f�[�^�����[�h���ATileGrid�𐶐�����
    /// </summary>
    /// <param name="stageId">���[�h����X�e�[�WID</param>
    //public void LoadMap(int stageId)
    //{

    //    Debug.Log($"MapManager:�X�e�[�W{stageId}�̃}�b�v�����[�h���܂�");

    //    //_mapCsvFilesPath = new string "";

    //    //���}�b�v�𐶐�
    //    _gridSize = new Vector2Int(10, 10);
    //    _tileGrid = new Tile[_gridSize.x, _gridSize.y];

    //    for (int y = 0; y < _gridSize.y; y++)
    //    {
    //        for (int x = 0; x < _gridSize.x; x++)
    //        {
    //            //�n�`�^�C�v��ݒ肷�郍�W�b�N�i�}�b�v�f�[�^����̓ǂݍ��݁j
    //            //���݂͉��̃}�b�v�𐶐�����@2025/06
    //            TerrainType type = TerrainType.Plain;
    //            if(x == 5 && y == 5) { type = TerrainType.Forest; }//���̐X
    //            if(x== 0 || y == 0 || x == _gridSize.x -1 || y == _gridSize.y -1) { type = TerrainType.Water; }//���̐���@�O��
    //            if(x % 2 == 0 && y % 2 == 0) { type = TerrainType.Desert; }//���̍���
    //            _tileGrid[y, x] = new Tile(new Vector2Int(x, y), type);
    //        }
    //    }
    //    Debug.Log($"MapManager:�}�b�v�T�C�Y{_gridSize.x}�~{_gridSize.y}�Ő���");

    //    //���F�^�C���̎��o�I�ɕ\�����鏈���֌W
    //}

    /// <summary>
    /// �}�b�v�f�[�^�Ɋ�Â��ă}�b�v�𐶐�����
    /// </summary>
    public void GenerateMap(string mapId)
    {
        ClearMap();//���Ƀ}�b�v����������Ă���\�����l�����āA��x�N���A����

        //MapDataLoader��CSV�t�@�C������}�b�v�f�[�^��ǂݍ���
        MapData mapData = MapDataLoader.LoadMapDataFromCSV(mapId);

        //_currentMapData = MapDataLoader.LoadMapDataFromCSV(mapId);

        if (mapData == null)
        {
            Debug.LogError("MapManager:�}�b�v�f�[�^�̓ǂݍ��݂Ɏ��s���܂����B�}�b�v�����ł��܂���");
            return;
        }

        _currentMapData = mapData;

        _gridSize = new Vector2Int(_currentMapData.Width,_currentMapData.Height);


        for (int y = 0; y < _currentMapData.Height; y++)
        {
            for (int x = 0; x < _currentMapData.Width; x++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);//���݂̃O���b�h���W
                TerrainType terrainType = _currentMapData.GetTerrainType(gridPos);//���̍��W�̒n�`�^�C�v���擾


                Vector3 worldPos = GetWorldPositionFromGrid(gridPos);//�O���b�h���W�����[���h���W�ɕϊ�

                GameObject tileGO = Instantiate(_tilePrefab, worldPos, Quaternion.identity, transform);

                //��������GameObject����Tile�R���|�[�l���g���擾����
                MyTile tile = tileGO.GetComponent<MyTile>();
                if (tile == null)
                {
                    Debug.LogError($"TilePrefab��Tile�R���|�[�l���g���A�^�b�`���ꂢ�܂���{_tilePrefab.name}");
                    Destroy(tileGO);
                    continue;
                }

                //�擾����Tile�R���|�[�l���g��������
                tile.Initialize(gridPos, terrainType, false);

                //�n�`�^�C�v�ɉ������X�v���C�g��Tile�ɐݒ�
                SetTileSprite(tile, terrainType);

                //�����E����������������Tile�I�u�W�F�N�g����Ō����ł���悤��
                _tileData.Add(gridPos, tile);
            }
        }
        Debug.Log($"MapManager:�}�b�v�𐶐����܂���({_currentMapData.Width}x{_currentMapData.Height})");

        //PlayerUnit�̏����z�u
        //PlacePlayerUnitAtInitialPostiton();
    }


    //////ToDo->GetTileAt�𖼑O�ύX���ATilemap�p�̃��\�b�h�ɕύX
    /// <summary>
    /// �w�肳�ꂽ�O���b�h���W�̃^�C�������擾����
    /// </summary>
    /// <param name="position">�O���b�h���W</param>
    /// <return>Tile�I�u�W�F�N�g�A�͈͊O�Ȃ�null</return>
    //public MyTile GetTileAt(Vector2Int position)
    //{
    //    if (_tileData.TryGetValue(position, out MyTile tile))
    //    {
    //        return tile;
    //    }
    //    return null;
    //}

    /// <summary>
    /// �w��O���b�h�ƃ��j�b�g�^�C�v�ɉ������ړ��R�X�g��Ԃ�
    /// </summary>
    /// <param name="positon">�O���b�h���W</param>
    /// <param name="unitType">���j�b�g�^�C�v</param>
    /// <return>�ړ��R�X�g</return>
    public int GetMovementCost(Vector2Int position, UnitType unitType)
    {
        MyTile tile = GetTileAt(position);
        if (tile == null)
        {
            return int.MaxValue; //�͈͊O�͈ړ��s��
        }

        //�n�`�ƃ��j�b�g�^�C�v�ɉ������ړ��R�X�g�̃��W�b�N
        switch (tile.TerrainType)
        {
            case TerrainType.Plain://���n
                return 1;
            case TerrainType.Forest://�X
                return 2;
            case TerrainType.Mountain://�R
                if (unitType == UnitType.Flying) { return 1; }//��s���j�b�g�̓R�X�g��
                if (unitType == UnitType.Mountain) { return 1; }//�R�����j�b�g�̓R�X�g��
                return 3;
            case TerrainType.Water://����
                if (unitType == UnitType.Flying) { return 1; }//��s���j�b�g�̓R�X�g��
                if (unitType == UnitType.Aquatic) { return 1; }//�������j�b�g�̓R�X�g��
                return 4;
            case TerrainType.Desert://����
                if (unitType == UnitType.Flying) { return 1; }//��s���j�b�g�̓R�X�g��
                return 2;
            case TerrainType.Snow://�ϐ�
                if (unitType == UnitType.Flying) { return 1; }//��s���j�b�g�̓R�X�g��
                return 2;
            case TerrainType.River://��
                if (unitType == UnitType.Flying) { return 1; }//��s���j�b�g�̓R�X�g��
                if (unitType == UnitType.Aquatic) { return 1; }//�������j�b�g�̓R�X�g��
                return int.MinValue;//���̃��j�b�g�͈ړ��s��
            case TerrainType.Flooded://���Q
                if (unitType == UnitType.Flying) { return 1; }//��s���j�b�g�̓R�X�g��
                if (unitType == UnitType.Aquatic) { return 1; }//�������j�b�g�̓R�X�g��
                return 4;
            case TerrainType.Landslide://�y��
                if (unitType == UnitType.Flying) { return 1; }//��s���j�b�g�̓R�X�g��
                return 3;
            case TerrainType.Paved://�ܑ�
                return 1;
            default:
                return 1;
        }
    }

    /// <summary>
    /// ����̃O���b�h�̒n�`�^�C�v��ύX����(�s�m��v�f)
    /// </summary>
    /// <param name="postion">�O���b�h���W</param>
    /// <param name="newType">�V�����n�`�^�C�v</param>
    public void ChangeEventTerrain(Vector2Int gridPosition, TerrainType newType)
    {
        MyTile tile = GetTileAt(gridPosition);
        if (tile != null)
        {
            //�����ڂ̕ω��̂��ߎ��o�I�������w��
            //tile.SetType(newType);
            //SetTileSprite(tile, newType);
            //Debug.Log($"MapManager:{gridPosition}�̒n�`��{tile.TerrainType}����{newType}�ω����܂���");

            //���s��Dictionary����TileBase���擾
            if (_terrainTiles.TryGetValue(newType, out TileBase tileBase))
            {
                _generateTilemap.SetTile(new Vector3Int(gridPosition.x, gridPosition.y, 0), tileBase);
            }
        }
    }

    /// <summary>
    /// �����̃O���b�h���W�̒n�`����x�ɕύX����
    /// </summary>
    /// <param name="gridPosition">�ύX����^�C���̃O���b�h���W���X�g</param>
    /// <param name="newType">�ύX��̒n�`�^�C�v</param>
    public void ChangeMultipleTerrains(List<Vector2Int> gridPositions,TerrainType newType)
    {
        foreach(Vector2Int gridPos in gridPositions)
        {
            ChangeEventTerrain(gridPos, newType);
        }
        Debug.Log($"{gridPositions.Count}�}�X�̒n�`�ύX���������܂����B");
    }

    /// <summary>
    /// �w�肵��TerrainType�����^�C���݂̂��A�ʂ̒n�`�^�C�v�ɕω�������
    /// </summary>
    /// <param name="targetType"></param>
    /// <param name="newType"></param>
    /// <param name="changeCount"></param>
    public void ChangeSpecificTerrain(TerrainType targetType, TerrainType newType,int changeCount)
    {
        List<Vector2Int> targetTiles = new List<Vector2Int>();

        //�}�b�v��̑S�^�C�����X�L�������āA�����ɍ����^�C����T��
        //ToDo->_tileData---_tileDataTilemapTest
        foreach (var tilePair in _tileDataFromTilemapTest)
        {
            if(tilePair.Value.TerrainType == targetType)
            {
                targetTiles.Add(tilePair.Key);
            }
        }

        //�����_���ɕω�������^�C����I��
        List<Vector2Int> tilesToChange = new List<Vector2Int>();
        if(targetTiles.Count > 0)
        {
            for(int i = 0;i < changeCount && targetTiles.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, targetTiles.Count);
                tilesToChange.Add(targetTiles[randomIndex]);
                targetTiles.RemoveAt(randomIndex);
            }
        }

        ChangeMultipleTerrains(tilesToChange, newType);
    }

    /// <summary>
    /// �w�肵��TerrainType�����^�C���̎���1�}�X��ω�������
    /// </summary>
    /// <param name="centerType"></param>
    /// <param name="newType"></param>
    public void ChangeAroundTerrain(TerrainType centerType,TerrainType newType)
    {
        List<Vector2Int> tileToChange = new List<Vector2Int>();

        //���S�ƂȂ�^�C�������ׂČ�����
        List<Vector2Int> centerTile = new List<Vector2Int>();
        //ToDo->_tileData---_tileDataTilemapTest
        foreach (var tilePair in _tileDataFromTilemapTest)
        {
            if(tilePair.Value.TerrainType == centerType)
            {
                centerTile.Add(tilePair.Key);
            }
        }

        //���������S�^�C���̎���1�}�X��T��
        foreach (Vector2Int centerPos in centerTile)
        {
            //���͂�8�}�X���`�F�b�N
            for (int y = -1;y <= 1; y++)
            {
                for(int x = -1;x <= 1; x++)
                {
                    // ���S�^�C���̓X�L�b�v
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    Vector2Int surroundingPos = new Vector2Int(centerPos.x + x, centerPos.y + y);

                    //�}�b�v�͈͓̔����`�F�b�N
                    if (IsValidGridPosition(surroundingPos))
                    {
                        MyTile surroundingTile = GetTileAt(surroundingPos);
                        //���S�^�C���Ɠ���TerrainType�ł͂Ȃ��ꍇ�̂ݒǉ�
                        if (surroundingTile != null && surroundingTile.TerrainType != centerType)
                        {
                            //�d���������
                            if (!tileToChange.Contains(surroundingPos))
                            {
                                tileToChange.Add(surroundingPos);
                            }
                        }
                    }
                }
            }
        }

        ChangeMultipleTerrains(tileToChange,newType);
    }
    

    /// <summary>
    /// �}�b�v��̃^�C���̃O���b�h���W���擾����
    /// </summary>
    /// <returns></returns>
    public List<Vector2Int> GetAllGridPosition()
    {
        //ToDo->_tileData---_tileDataTilemapTest
        return new List<Vector2Int>(_tileDataFromTilemapTest.Keys);
    }



    /// <summary>
    /// �w�肳�ꂽ���W���}�b�v�̗L���Ȕ͈͓��ɂ��邩�`�F�b�N����
    /// </summary>
    /// <param name="gridPosition">�`�F�b�N����O���b�h���W</param>
    /// <return>�L���Ȕ͈͓��ł����true�A�����łȂ����false</return>
    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        //ToDo->_currentMapData----�r��
        //if (_currentMapData == null)
        //{
        //    Debug.LogError("MapData�����[�h����Ă��܂���BIsValidGridPosition���Ăяo���O��MapData�����������Ă��������B");
        //    return false;
        //}

        //return gridPosition.x >= 0 && gridPosition.x < _currentMapData.Width &&
        //    gridPosition.y >= 0 && gridPosition.y < _currentMapData.Height;

        //ToDo->_currentMapData����Tilemap�̃T�C�Y���g�p����
        return gridPosition.x >= 0 && gridPosition.x < _tilemapGridSize.x &&
            gridPosition.y >= 0 && gridPosition.y < _tilemapGridSize.y;
    }



    /// <summary>
    /// Tile�N���X��Type���Z�b�g����
    /// </summary>
    /// <param name="positon"></param>
    /// <param name="newType"></param>
    public void SetTileType(Vector2Int positon, TerrainType newType)
    {
        MyTile tile = GetTileAt(positon);
        if (tile != null)
        {
            tile.SetType(newType);
        }
    }

    /// <summary>
    /// �w�肳�ꂽ�^�C���ɁA���̒n�`�^�C�v�ɉ������X�v���C�g��ݒ肷��
    /// </summary>
    /// <param name="tile">�X�v���C�g��ݒ肵����Tile�I�u�W�F�N�g</param>
    /// <param name="type">���̃^�C���̒n�`�^�C�v</param>
    private void SetTileSprite(MyTile tile, TerrainType type)
    {
        //�n�`��Enum�̒l���L���X�g���āA_terrainSprites�z��̃C���f�b�N�X�Ƃ��Ďg�p
        int typeIndex = (int)type;

        //�z��͈̔̓`�F�b�N�ƃX�v���C�g��Inspector�Őݒ肳��Ă��邩�̊m�F
        if (typeIndex >= 0 && typeIndex < _terrainSprite.Length && _terrainSprite[typeIndex] != null)
        {
            //Tile�N���X��Sprite���\�b�h���Ăяo���āA�X�v���C�g��ݒ�
            tile.SetSprite(_terrainSprite[typeIndex]);
        }
        else
        {
            //�X�v���C�g���ݒ肳��Ă��Ȃ��ꍇ��C���f�b�N�X���s���ȏꍇ�͌x����\��
            Debug.LogWarning($"MapManager:TerrainType'{type}'Terrain Sprite");
        }
    }

    /// <summary>
    /// �O���b�h���W�����[���h���W�ɕϊ�����
    /// </summary>
    /// <param name="gridPos">�ϊ��������O���b�h���W</param>
    /// <return>�Ή����郏�[���h���W</return>
    public Vector3 GetWorldPositionFromGrid(Vector2Int gridPos)
    {
        float x = gridPos.x * _tileSize + _offset.x + (_tileSize / 2.0f);
        float y = gridPos.y * _tileSize + _offset.y + (_tileSize / 2.0f);
        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// ���[���h���W���O���b�h���W�ɕϊ�
    /// </summary>
    /// <param name="worldPos">�ϊ����������[���h���W</param>
    /// <returns>�Ή�����O���b�h���W</returns>
    public Vector2Int GetGridPositionFromWorld(Vector3 worldPos)
    {
        //�I�t�Z�b�g�ƃ^�C���T�C�Y���l�����ăO���b�h���W���v�Z
        //int gridX = Mathf.FloorToInt((worldPos.x - _offset.x) / _tileSize);
        //int gridY = Mathf.FloorToInt((worldPos.y - _offset.y) / _tileSize);

        float x = (worldPos.x - _offset.x) / _tileSize;
        float y = (worldPos.y - _offset.y) / _tileSize;

        //return new Vector2Int(gridX, gridY);

        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }




    //�I�t�Z�b�g�ɂ����W�Ǘ��ɕύX�̂��ߍ폜
    ///<summary>
    /// ���[���h���W���O���b�h���W�ɕϊ�����
    /// </summary>
    /// <param name="worldPos">�ϊ����������[���h���W</param>
    /// <return>�Ή�����O���b�h���W</return>
    //public Vector2Int GetGridPosition(Vector3 worldPos)
    //{
    //    //���[���h���W���^�C���T�C�Y�Ŋ���A�l�̌ܓ����Đ����O���b�h���W�ɂ���
    //    int x = Mathf.RoundToInt(worldPos.x / _tileSize);
    //    int y = Mathf.RoundToInt(worldPos.y / _tileSize);
    //    return new Vector2Int(x, y);
    //}


    ///<summary>
    /// �����̃}�b�v��S�ăN���A����
    /// </summary>
    private void ClearMap()
    {
        foreach (var tileEntry in _tileData)
        {
            Destroy(tileEntry.Value.gameObject);//Tile�I�u�W�F�N�g���A�^�b�`����Ă���GameObject��j��
        }
        _tileData.Clear();         //Dictionary�̒��g���N���A
        _currentMapData = null; //�ǂݍ��񂾃}�b�v�f�[�^���N���A
        Debug.Log("MapManager:�����̃}�b�v���N���A���܂���");
    }

    /// <summary>
    /// �S�Ẵ��j�b�g�̃��X�g���擾����
    /// </summary>
    public List<Unit> GetAllUnits()
    {
        return _allUnit.OfType<Unit>().ToList();
    }

    /// <summary>
    /// �S�Ẵv���C���[���j�b�g�̃��X�g���擾����
    /// </summary>
    public List<PlayerUnit> GetAllPlayerUnits()
    {
        return _allPlayerUnits.OfType<PlayerUnit>().ToList();
    }

    /// <summary>
    /// �S�Ă̓G���j�b�g�̃��X�g���擾����
    /// </summary>
    public List<EnemyUnit> GetAllEnemyUnits()
    {
        return _allEnemyUnits.OfType<EnemyUnit>().ToList();
    }

    /// <summary>
    /// �S�ẴV�[�����̃��j�b�g�̃N���A
    /// </summary>
    private void ClaerExistingUnits()
    {

        //ToDo->_tileData---_tileDataTilemapTest
        //���݃V�[���ɂ���S�Ẵ��j�b�g�I�u�W�F�N�g���폜
        foreach (var tileEntry in _tileDataFromTilemapTest)
        {
            if (tileEntry.Value.OccupyingUnit != null)
            {
                Destroy(tileEntry.Value.OccupyingUnit.gameObject);
                tileEntry.Value.OccupyingUnit = null;
            }
        }
        _allUnit.Clear();
        _allPlayerUnits.Clear();
        _allPlayerUnits.Clear();
    }


    /// <summary>
    /// �v���C���[���j�b�g�������ʒu�ɔz�u����
    /// </summary>
    private void PlacePlayerUnitAtInitialPostiton()
    {
        //�v���g�^�C�v�p�̉��������W
        Vector2Int initialPlayerGridPos = new Vector2Int(0, 0);


        //�w�肳�ꂽ���W���}�b�v�͈͓����m�F
        if (initialPlayerGridPos.x < 0 || initialPlayerGridPos.x >= _currentMapData.Width ||
            initialPlayerGridPos.y < 0 || initialPlayerGridPos.y >= _currentMapData.Height)
        {
            Debug.LogError($"MapManager:�v���C���[���j�b�g�̏����z�u���W({initialPlayerGridPos})���}�b�v�͈͊O�ł�");
        }

        //�v���C���[���j�b�g�����ɑ��݂���ꍇ�͔j��(�V�[���J�ڂȂǂōĐ�������ꍇ)
        if (_currentPlayerUnit != null)
        {
            Destroy(_currentPlayerUnit.gameObject);
        }

        //�v���C���[���j�b�g�v���n�u�̐���
        GameObject unitGO = Instantiate(_playerUnitPrefab, GetWorldPositionFromGrid(initialPlayerGridPos), Quaternion.identity, transform);
        Unit playerUnit = unitGO.GetComponent<Unit>();

        if (playerUnit == null)
        {
            Debug.LogError($"MapManager:PlayerUnitPrefub��PlayerUnit�R���|�[�l���g���A�^�b�`����Ă��܂���:{_playerUnitPrefab.name}");
            Destroy(unitGO);
            return;
        }
        _currentPlayerUnit = playerUnit;

        UnitData dummyData = new UnitData();
        //dummyData.UnitId = "PLAYER001";
        //dummyData.UnitName = "none";
        //dummyData.Type = UnitType.Infantry;
        //dummyData.BaseMovement = 3;
        //dummyData.BaseAttackPower = 5;
        //dummyData.BaseDefensePower = 5;
        //dummyData.MaxHP = 5;
        //dummyData.BaseSkill = 5;
        //dummyData.BaseSpeed = 5;

        //�v���C���[���j�b�g�̏������ƃ��[���h���W�ւ̔z�u
        _currentPlayerUnit.Initialize(dummyData);//���j�b�g�����n��
        _currentPlayerUnit.UpdatePosition(initialPlayerGridPos);//���[���h���W��ݒ�



        Debug.Log($"PlayerUnit'{_currentPlayerUnit.name}'placed at grid:{initialPlayerGridPos}");
    }

    /// <summary>
    /// �G���j�b�g�������ʒu�ɔz�u����
    /// </summary>
    private void PlaceEnemyUnitAtInitialPostiton()
    {
        //�v���g�^�C�v�p�̉��������W
        Vector2Int initialEnemyGridPos = new Vector2Int(5, 0);


        //�w�肳�ꂽ���W���}�b�v�͈͓����m�F
        if (initialEnemyGridPos.x < 0 || initialEnemyGridPos.x >= _currentMapData.Width ||
            initialEnemyGridPos.y < 0 || initialEnemyGridPos.y >= _currentMapData.Height)
        {
            Debug.LogError($"MapManager:�G���j�b�g�̏����z�u���W({initialEnemyGridPos})���}�b�v�͈͊O�ł�");
        }

        //�G���j�b�g�����ɑ��݂���ꍇ�͔j��(�V�[���J�ڂȂǂōĐ�������ꍇ)
        if (_currentEnemyUnit != null)
        {
            Destroy(_currentEnemyUnit.gameObject);
        }

        //�G���j�b�g�v���n�u�̐���
        GameObject enemyGO = Instantiate(_enemyUnitprefab, GetWorldPositionFromGrid(initialEnemyGridPos), Quaternion.identity, transform);
        Unit enemyUnit = enemyGO.GetComponent<Unit>();

        if (enemyUnit == null)
        {
            Debug.LogError($"MapManager:EnemyUnitPrefub��EnemyUnit�R���|�[�l���g���A�^�b�`����Ă��܂���:{_enemyUnitprefab.name}");
            Destroy(enemyGO);
            return;
        }
        _currentEnemyUnit = enemyUnit;

        UnitData dummyData = new UnitData();
        //dummyData.UnitId = "ENEMY001";
        //dummyData.UnitName = "one";
        //dummyData.Type = UnitType.Infantry;
        //dummyData.BaseMovement = 3;
        //dummyData.BaseAttackPower = 5;
        //dummyData.BaseDefensePower = 5;
        //dummyData.MaxHP = 5;
        //dummyData.BaseSkill = 5;
        //dummyData.BaseSpeed = 5;

        //�G���j�b�g�̏������ƃ��[���h���W�ւ̔z�u
        //_currentEnemyUnit.Initialize(dummyData);//���j�b�g�����n��
        _currentEnemyUnit.UpdatePosition(initialEnemyGridPos);//���[���h���W��ݒ�



        Debug.Log($"EnemyUnit'{_currentEnemyUnit.name}'placed at grid:{initialEnemyGridPos}");
    }

    /// <summary>
    /// ���j�b�g�̔z�u
    /// </summary>
    public void PlaceUnit(Unit unit, Vector2Int gridPos)
    {

        //ToDo->_tileData----_tileDataTilemapTest
        if (!_tileDataFromTilemapTest.ContainsKey(gridPos))
        {
            Debug.LogError($"MapManager: �O���b�h���W {gridPos} �Ƀ^�C�������݂��܂���B");
            return;
        }

        //ToDo->_tileData----_tileDataTilemapTest
        MyTile targetTile = _tileDataFromTilemapTest[gridPos];

        //�z�u��̃^�C�������łɑ��̃��j�b�g�ɐ�L����Ă��Ȃ����`�F�b�N���A���łɑ��݂���ꍇ�͔j��
        if (targetTile.OccupyingUnit != null)
        {
            Debug.LogWarning($"MapManager: �O���b�h���W {gridPos} �͊��Ƀ��j�b�g {targetTile.OccupyingUnit.UnitName} �ɂ���Đ�L����Ă��܂��B");
            Destroy(unit.gameObject);
            return;
        }

        //unit.SetGridPosition(gridPos);
        //GetTileAt(gridPos).OccupyingUnit = unit;


        unit.MoveToGridPosition(gridPos, targetTile);
        unit.transform.position = GetWorldPositionFromGrid(gridPos);


        if (unit is PlayerUnit playerUnit)
        {
            _allPlayerUnits.Add(playerUnit);
        }
        else if (unit is EnemyUnit enemyUnit)
        {
            _allEnemyUnits.Add(enemyUnit);
        }
    }

    /// <summary>
    /// �G���j�b�g��}�b�v�ɔz�u����i�}�b�v�f�[�^���Q�Ɓj
    /// </summary>
    /// <param name="mapId"></param>
    public void PlaceEnemiesForCurrentMap(string mapId)
    {
        EnemyEncounterData encounterData = EnemyEncounterManager.Instance.GetEnemyEncounterData(mapId);
        if (encounterData == null)
        {
            Debug.LogWarning($"MapManager: �}�b�v '{mapId}' �̓G�f�[�^��������܂���B�G�͔z�u����܂���B");
            return;
        }

        foreach (EnemyPlacement placement in encounterData.enemyPlacements)
        {
            if (placement.enemyPrefab != null)
            {
                EnemyUnit enemyInstance = Instantiate(placement.enemyPrefab, transform);
                PlaceUnit(enemyInstance, placement.gridPosition);
            }
            else
            {
                Debug.LogWarning($"MapManager: �}�b�v '{mapId}' �̓G�z�u�ŁAPrefab��null�̂��̂��܂܂�Ă��܂��B");
            }
        }
        Debug.Log($"MapManager:�}�b�v'{mapId}'�ɓG���j�b�g��z�u���܂���");
    }

    /// <summary>
    /// �}�E�X�N���b�N����������
    /// </summary>
    private void HandleMouseClick()
    {
        //���F�v���C���[�^�[���łȂ���Ώ������Ȃ�
        if (TurnManager.Instance != null && TurnManager.Instance.CurrnetTurnState != TurnState.PlayerTurn)
        {
            return;
        }

        //�U���s����Ԃ̏ꍇ�͓G���j�b�g�̑I���̂ݎ�t��
        if (_isAttacking)
        {
            Debug.LogWarning("�U����Ԃł�");

        }
        //���j�b�g���ړ��v�撆�܂��͈ړ����̏ꍇ�́A�V���ȃ}�E�X�N���b�N���͂��󂯕t���Ȃ�
        else if (_isMovingOrPlanning)
        {
            Debug.Log("�ړ��v�撆�܂��͈ړ����̂��߁A�V�������͂��󂯕t���܂���");
            return;
        }
        

        //�}�E�X�̃X�N���[�����W�����[���h���W�ɕϊ�
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;//2D�Q�[���Ȃ̂�Z��0�ɌŒ�

        Debug.Log($"�N���b�N���ꂽ���[���h���W�F{mouseWorldPos}");

        //���[���h���W���O���b�h���W�ɕϊ�
        Vector2Int clickedGridPos = GetGridPositionFromWorld(mouseWorldPos);

        Debug.Log($"�N���b�N���ꂽ�O���b�h���W�F{clickedGridPos}");

        //Ryacast���g���ăN���b�N���ꂽ�I�u�W�F�N�g�����o
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        MyTile _clickedTile = GetTileAt(clickedGridPos);
        //�}�b�v�͈͊O�̏ꍇ
        if (_clickedTile == null)
        {
            Debug.Log("�}�b�v�͈͊O�ł�");
            return;
        }



        //���j�b�g�����I���̏ꍇ
        if (_selectedUnit == null)
        {
            //�N���b�N���ꂽ�^�C���Ƀ��j�b�g�����邩�A���v���C���[���j�b�g���A�����s����
            if (_clickedTile.OccupyingUnit != null &&
                _clickedTile.OccupyingUnit.Faction == FactionType.Player &&
                !_clickedTile.OccupyingUnit.HasActedThisTurn)
            {
                SelectUnit(_clickedTile.OccupyingUnit);
                _originalTile = _clickedTile;
            }
            //�G���j�b�g�̏ꍇ�s���͈͗\����\���i������2025 / 07�j
            else if (_clickedTile.OccupyingUnit != null &&
                _clickedTile.OccupyingUnit.Faction == FactionType.Enemy &&
                !_clickedTile.OccupyingUnit.HasActedThisTurn)
            {
                Debug.Log("�G���j�b�g�ł�");
                SelectUnit(_clickedTile.OccupyingUnit);
            }
            else
            {
                //���j�b�g�����I���̏�ԂŃn�C���C�g���o�Ȃ��悤��
                ClearAllHighlights();
                //////ClearMovableRangeDisplay();
            }
        }
        //���j�b�g���I���ς݂̏ꍇ
        else
        {
            //�I�𒆂̃��j�b�g�Ɠ������j�b�g���N���b�N���ꂽ��I������
            if (_clickedTile.OccupyingUnit == _selectedUnit)
            {
                CancelMove();
            }
            //�ړ��\�ȃ^�C�����N���b�N���ꂽ��ړ��v��
            else if (_currentHighlights.ContainsKey(clickedGridPos) && clickedGridPos != _selectedUnit.CurrentGridPosition)
            {
                if (!IsTileOccupiedForStooping(clickedGridPos, _selectedUnit))
                {
                    StartCoroutine(InitiateVisualMove(clickedGridPos,_clickedTile));
                }
                else
                {
                    Debug.Log("MapManager: ���̃}�X�͑��̃��j�b�g�ɐ�L����Ă��邽�߈ړ��ł��܂���");
                    //CancelMove();
                    return;
                }
            }

            else if (_isAttacking)
            {
                if (_clickedTile.OccupyingUnit != null &&_clickedTile.OccupyingUnit.Faction == FactionType.Enemy)
                {
                    Debug.Log($"�v���C���[�̍U���F�U�����j�b�g{_selectedUnit.name}�ڕW���j�b�g{_clickedTile.OccupyingUnit.name}");
                    _isAttacking = false;
                    BattleManager.Instance.ResolveBattle_ShogiBase(_selectedUnit, _clickedTile.OccupyingUnit);
                    _selectedUnit.SetActedThisTrun();
                    ResetMoveState();
                }
                //�U���ł���G���j�b�g�ȊO�̏ꍇ�͍U����Ԃ̂�
                else
                {
                    Debug.LogWarning("�G���j�b�g�ł͂���܂���");
                    Debug.LogWarning($"�I���W�i���|�W�V�����F{_originalUnitPositon}�N���b�N�|�W�V�����F{_originalTile}");
                    //_selectedUnit.transform.position = MapManager.Instance.GetWorldPositionFromGrid(_originalUnitPositon);

                    _selectedUnit.MoveToGridPosition(_originalUnitPositon, _originalTile);
                    //_selectedUnit.transform.position = GetWorldPositionFromGrid(_currentPlannedMovePositon);
                    //ResetMoveState();
                    CancelMove();
                }
            }

            //���̃v���C���[���j�b�g���N���b�N���ꂽ��A���݂̑I�����������B�V�������j�b�g��I��
            else if (_clickedTile.OccupyingUnit != null && _clickedTile.OccupyingUnit.Faction == FactionType.Player && _clickedTile.OccupyingUnit != _selectedUnit)
            {
                CancelMove();//���݂̑I��������
                SelectUnit(_clickedTile.OccupyingUnit);//�V�������j�b�g��I��
            }
            
            //�ړ��͈͊O�̋�̃^�C����G���j�b�g���N���b�N������L�����Z��
            else
            {
                CancelMove();
            }
        }


        //������ύX2025/07
        //if (hit.collider != null)
        //{
        //    //�N���b�N���ꂽ�̂��v���C���[���j�b�g���m�F
        //    PlayerUnit clickedUnit = hit.collider.GetComponent<PlayerUnit>();
        //    if(clickedUnit != null)
        //    {
        //        //���j�b�g���N���b�N���ꂽ�ꍇ�̓^�C���N���b�N�������s��Ȃ�
        //        HandleUnitClick(clickedUnit);
        //        return;
        //    }

        //    //�N���b�N���ꂽ�̂��^�C�����m�F
        //    //Tile clickedTile = hit.collider.GetComponent<Tile>();
        //    //if (clickedTile != null)
        //    //{
        //    //    HandleTileClick(clickedTile);
        //    //    return;
        //    //}

        //    Tile clickedTile = hit.collider.GetComponent<Tile>();


        //    if (clickedTile != null && _selectedUnit != null)
        //    {


        //        //���̈ʒu�ƈړ�����L�^
        //        _originalUnitPositon = _selectedUnit.GetCurrentGridPostion();
        //        _currentPlannedMovePositon = clickedGridPos;

        //        //�ړ����v�悳�ꂽ�̂ŁA�t���O��true�ɐݒ�
        //        _isMovingOrPlanning = true;

        //        //���j�b�g���ړ���̃^�C���Ɉꎞ�I�Ɉړ�������
        //        //_selectedUnit.transform.position = GetWorldPositionFromGrid(_currentPlannedMovePositon);

        //        Debug.Log($"Moved unit to temporary position({_currentPlannedMovePositon.x},{_currentPlannedMovePositon.y}).Press Space to confirm, Q to cancel.");
        //    }

        //    //�N���b�N���ꂽ�̂��^�C�����m�F
        //    //Tile clickedTile = hit.collider.GetComponent<Tile>();
        //    if (clickedTile != null)
        //    {
        //        HandleTileClick(clickedTile);
        //        return;
        //    }
        //}
        //else
        //{
        //    //�����N���b�N����Ȃ������ꍇ�i�}�b�v�O�Ȃǁj
        //    Debug.Log("�����N���b�N����܂���ł���");
        //    //�I�𒆂̃��j�b�g������Δ�I����Ԃɂ���
        //    if(_selectedUnit != null)
        //    {
        //        _selectedUnit.SetSelected(false);
        //        _selectedUnit = null;
        //        _isMovingOrPlanning = false;
        //        Debug.Log("���j�b�g�̑I�����������܂����B");
        //    }
        //}



        ////�N���b�N���ꂽ�O���b�h���W���}�b�v�͈͓����`�F�b�N
        //if(_currentMapData == null || clickedGridPos.x < 0 || clickedGridPos.x >= _currentMapData.Width ||
        //    clickedGridPos.y < 0 || clickedGridPos.y >= _currentMapData.Height)
        //{
        //    Debug.Log("�}�b�v�͈͊O���N���b�N����܂���");
        //    return;
        //}

        //�v���C���[���j�b�g�̈ړ������̓����ɔ����ύX2025/06
        ////�N���b�N���ꂽ�^�C�����擾
        //if(_tiles.TryGetValue(clickedGridPos, out Tile clickedTile))
        //{
        //    Debug.Log($"�N���b�N���ꂽ�^�C���F{clickedTile.GridPosition},�n�`�F{clickedTile.TerrainType}�S�[���F{clickedTile}");

        //    
        //    //���̈ړ������i�N���b�N�Ȃǂ̎��O�����̊m�F�̂���
        //    //���݂́A�N���b�N���ꂽ�^�C���փv���C���[���j�b�g���ړ�������̂�
        //    //if(_currentPlayerUnit != null)
        //    //{
        //    //    //���̈ړ�����
        //    //    _currentPlayerUnit.SetGridPosition(clickedGridPos);
        //    //    _currentPlayerUnit.transform.position = GetWorldPosition(clickedGridPos);
        //    //    Debug.Log($"PlayerUnit moved to:{clickedGridPos}");
        //    //}
        //}
        //else
        //{
        //    Debug.LogWarning($"�O���b�h���W{clickedGridPos}�ɑΉ�����^�C����������܂���");
        //}
    }


    /// <summary>
    /// �I�𒆂̃��j�b�g�擾�p
    /// </summary>
    /// <param name="unit"></param>
    private void SelectUnit(Unit unit)
    {
        _selectedUnit = unit;
        _selectedUnit.SetSelected(true);
        _originalUnitPositon = unit.CurrentGridPosition;

        if (_selectedUnit.Faction == FactionType.Enemy)
        {
            Debug.Log($"�I�𒆂̓G���j�b�g���:���O{_selectedUnit.name}");
            CalculateAndShowMovableRange(_selectedUnit);
            if(_gameManager.CurrentBattlePhase == BattlePhase.BattleMain)
            {
                _selectedUnit = null;
            }
            return;
        }

        //�m�F�p
        Debug.Log($"�I�𒆂̃��j�b�g���:���O{_selectedUnit.name}");
        CalculateAndShowMovableRange(_selectedUnit);
    }

    /// <summary>
    /// �o�H�v�Z����ړ�����
    /// </summary>
    /// <param name="targetTile"></param>
    /// <param name="clickedPos"></param>
    /// 
    private void PlanMove(MyTile targetTile, Vector2Int clickedPos)
    {
        //�N���b�N���ꂽ�^�C�����ړ��\�͈͓��ɂ��邩�`�F�b�N
        if (_currentHighlights.ContainsKey(targetTile.GridPosition))
        {
            Debug.Log($"�ړ��\�ȃ^�C�� {targetTile.GridPosition} ���N���b�N����܂����B");

            //�o�H���v�Z����
            List<Vector2Int> path = DijkstraPathfinder.GetPathToTarget(
                _selectedUnit.CurrentGridPosition,
                targetTile.GridPosition,
                _selectedUnit);

            if (path != null && path.Count > 0)
            {
                ShowPathLine(path);
                StartCoroutine(MoveUnitAlogPath(_selectedUnit, path));
            }
            else
            {
                Debug.LogWarning("�o�H��������Ȃ����A�v�Z���ꂽ�o�H����ł��B");
            }
        }
        else
        {
            _isMovingOrPlanning = false;
            Debug.Log("�ړ��s�\�ȃ^�C�����N���b�N����܂����B");
        }

        _originalUnitPositon = _selectedUnit.GetCurrentGridPostion();
        _currentPlannedMovePositon = clickedPos;

        _isMovingOrPlanning = true;

        //_selectedUnit.SetSelected(false);
        //ClearAllHighlights();
        //ClearMovableRangeDisplay();
        //ClearPathLine();
        //_selectedUnit = null;
    }

    //�����̒ǉ��ɂ��ύX2025/07
    ///// <summary>
    ///// �^�C�������̃��j�b�g�ɐ�L����Ă��邩�`�F�b�N
    ///// </summary>
    ///// <param name=""></param>
    ///// <returns></returns>
    //public bool IsTileOccupied(Vector2Int gridPos)
    //{
    //    Tile tile = GetTileAt(gridPos);
    //    return tile != null && tile.OccupyingUnit != null;
    //}


    ///PlanMove�����̕ύX�ɔ������O�ύX2025/07
    private IEnumerator InitiateVisualMove(Vector2Int targetGridPos,MyTile targetTile)
    {
        _currentPlannedMovePositon = targetGridPos;
        _isMovingOrPlanning = true;

        ClearPathLine();

        //�ꕔ�̌o�H�v�Z�ɖ�肪���������ߕύX2025/07
        //List<Vector2Int> path = DijkstraPathfinder.FindPath(_selectedUnit.CurrentGridPosition, targetGridPos, _selectedUnit);

        List<Vector2Int> reacheleTiles = DijkstraPathfinder.GetPathToTarget(
                _selectedUnit.CurrentGridPosition,
                targetTile.GridPosition,
                _selectedUnit);

       


        if (reacheleTiles != null && reacheleTiles.Count > 0)
        {
            _currentPathLine = Instantiate(_pathLinePrefab);
            _currentPathLine.positionCount = reacheleTiles.Count;
            for (int i = 0; i < reacheleTiles.Count; i++)
            {
                _currentPathLine.SetPosition(i, GetWorldPositionFromGrid(reacheleTiles[i]));
            }

            _currentPathLine.startWidth = 0.1f;//���̊J�n���̑���
            _currentPathLine.endWidth = 0.1f;//���̏I�����̑���

            yield return _selectedUnit.AnimateMove(reacheleTiles);

            _isConfirmingMove = true;
            Debug.Log("�ړ����������܂����B�X�y�[�X�L�[�Ŋm��AQ�L�[�ŃL�����Z�����Ă��������B");

        }
        else
        {
            Debug.LogWarning("�o�H��������܂���ł����B");
            CancelMove(); // �o�H���Ȃ��ꍇ�̓L�����Z��
        }

        //�o�H�v�Z�ɕs������������ߕύX2025/07
        //if (path != null && path.Count > 0)
        //{
        //    _currentPathLine = Instantiate(_pathLinePrefab);
        //    _currentPathLine.positionCount = path.Count;
        //    for (int i = 0; i < path.Count; i++)
        //    {
        //        _currentPathLine.SetPosition(i, GetWorldPositionFromGrid(path[i]));
        //    }

        //    _currentPathLine.startWidth = 0.1f;//���̊J�n���̑���
        //    _currentPathLine.endWidth = 0.1f;//���̏I�����̑���

        //    yield return _selectedUnit.AnimateMove(path);

        //    _isConfirmingMove = true;
        //    Debug.Log("�ړ����������܂����B�X�y�[�X�L�[�Ŋm��AQ�L�[�ŃL�����Z�����Ă��������B");

        //}
        //else
        //{
        //    Debug.LogWarning("�o�H��������܂���ł����B");
        //    CancelMove(); // �o�H���Ȃ��ꍇ�̓L�����Z��
        //}
    }

    //�^�C�������̃��j�b�g�ɐ�L����Ă��邩�`�F�b�N (��~�n�_�̔���p)
    public bool IsTileOccupiedForStooping(Vector2Int gridPos, Unit selectedUnit)
    {
        //ToDo->GetTileAt---GetTileAtFromTilemap
        MyTile tile = GetTileAt(gridPos);

        //�m�F�p�̂��ߏ�����ǉ�
        if (tile == null)
        {
            Debug.LogError($"IsTileOccupiedForStopping: Tile at {gridPos} is null! This should not happen.");
            return true;// �^�C����null�̏ꍇ�͈��S�̂��ߒ�~�s�Ƃ݂Ȃ�
        }
        string occupyingUnitName = (tile.OccupyingUnit != null) ? tile.OccupyingUnit.UnitName : "NONE";
        string selectedUnitName = (selectedUnit != null) ? selectedUnit.UnitName : "NONE";
        Debug.Log($"IsTileOccupiedForStopping Check: Tile {gridPos}, OccupyingUnit: {occupyingUnitName}, SelectedUnit: {selectedUnitName}");

        // �^�C�������݂��A�����炩�̃��j�b�g����L���Ă���A���ꂪ�I�𒆂̃��j�b�g���g�ł͂Ȃ��ꍇ
        bool occupiedByOther = tile.OccupyingUnit != null && tile.OccupyingUnit != selectedUnit;

        //�f�o�b�O���O
        Debug.Log($"IsTileOccupiedForStopping Result for {gridPos}: Occupied by other unit = {occupiedByOther}");
        return occupiedByOther;



        //return tile != null && tile.OccupyingUnit != null && tile.OccupyingUnit != _selectedUnit;
    }

    //����̃��j�b�g���ʉ߉\���𔻒肷��
    public bool IsTilePassableForUnit(Vector2Int gridPos, Unit unitToCheck)
    {
        //�͈͊O�y�і����ȃ^�C���͒ʉߕs��
        MyTile tile = GetTileAt(gridPos);
        if (tile == null)
        {
            return false;
        }

        //���j�b�g����L���Ă���ꍇ
        if (tile.OccupyingUnit != null)
        {
            //��L���Ă���̂��������g�̏ꍇ�A�ʉ߉\
            if (tile.OccupyingUnit == unitToCheck)
            {
                return true;
            }
            // ��L���Ă���̂��G���j�b�g�̏ꍇ�A�ʉߕs��
            else if (tile.OccupyingUnit.Faction != unitToCheck.Faction)
            {
                return false;
            }
            //��L���Ă���̂��������j�b�g�̏ꍇ�A�ʉ߉\
            else
            {
                return true;
            }
        }
        //���j�b�g����L���Ă��Ȃ��ꍇ�A�ʉ߉\
        return true;
    }

    //���j�b�g���N���b�N���ꂽ���̏���
    private void HandleUnitClick(Unit clickedUnit)
    {


        if (_selectedUnit == clickedUnit)
        {
            _selectedUnit.SetSelected(false);
            _selectedUnit = null;
            ClearAllHighlights();
            ClearMovableRangeDisplay();
            _isMovingOrPlanning = false;
            Debug.Log("���j�b�g�̑I�����������܂���");
            return;
        }

        if (_selectedUnit != null)
        {
            _selectedUnit.SetSelected(false);//�O�ɑI�����Ă������j�b�g���I����Ԃɂ���
        }

        _selectedUnit = clickedUnit;//�V�����N���b�N���ꂽ���j�b�g��I����Ԃɂ���
        _selectedUnit.SetSelected(true);
        Debug.Log($"PlayerUnit'{_selectedUnit.name}'(Grid:{_selectedUnit.CurrentGridPosition}���I������܂���");

        //�ړ��͈͏���
        //ShowMovableRange(_selectdUnit);

        CalculateAndShowMovableRange(_selectedUnit);

    }

    //�^�C�����N���b�N���ꂽ���̏���
    private void HandleTileClick(MyTile clickedTile)
    {
        Debug.Log($"�N���b�N���ꂽ�^�C���F{clickedTile.GridPosition},�n�`�F{clickedTile.TerrainType}");

        if (_selectedUnit != null)
        {
            //�N���b�N���ꂽ�^�C�����ړ��\�͈͓��ɂ��邩�`�F�b�N
            if (_currentHighlights.ContainsKey(clickedTile.GridPosition))
            {
                Debug.Log($"�ړ��\�ȃ^�C�� {clickedTile.GridPosition} ���N���b�N����܂����B");

                //�o�H���v�Z����
                List<Vector2Int> path = DijkstraPathfinder.GetPathToTarget(
                    _selectedUnit.CurrentGridPosition,
                    clickedTile.GridPosition,
                    _selectedUnit);

                if (path != null && path.Count > 0)
                {
                    ShowPathLine(path);
                    StartCoroutine(MoveUnitAlogPath(_selectedUnit, path));
                }
                else
                {
                    Debug.LogWarning("�o�H��������Ȃ����A�v�Z���ꂽ�o�H����ł��B");
                }
            }
            else
            {
                _isMovingOrPlanning = false;
                Debug.Log("�ړ��s�\�ȃ^�C�����N���b�N����܂����B");
            }

            //���i�K�ł͈ړ���I����Ԃ���������悤�ɂ���
            //_selectdUnit.SetSelected(false);
            //_selectdUnit = null;

            //�ړ��͈͂̕\���̃N���A
            //ClearMovableRangeDisplay();
            //ClearAllHighlights();
        }
        else
        {
            Debug.Log("���j�b�g���I������Ă��܂���");
        }
    }

    //�ړ��\�͈͂�\������
    //private void ShowMovableRange(Unit unit)
    //{
    //    //���i�K�ł̓e�X�g�p�Ƃ��ă��j�b�g�̎��͂̃^�C�����n�C���C�g�\��
    //    Debug.Log($"���j�b�g�̈ړ��͈͂�\���B�ړ���{unit.GetMoveRange()}");

    //    //���F���ݒn�������1�}�X���n�C���C�g�\��
    //    Vector2Int cuurentPos = unit.GetCurrentGridPostion();
    //    for(int y = -1; y <= 1; y++)
    //    {
    //        for(int x = -1; x <= 1; x++)
    //        {
    //            Vector2Int cheakPos = new Vector2Int(cuurentPos.x + x , cuurentPos.y + y);
    //            if(_tiles.TryGetValue(cheakPos, out Tile tile))
    //            {
    //                if(tile.GetComponent<SpriteRenderer>() != null)
    //                {
    //                    tile.GetComponent<SpriteRenderer>().color = Color.blue;
    //                }
    //            }
    //        }
    //    }
    //}

    //�ړ��͈͂̃n�C���C�g�\�����N���A����
    private void ClearMovableRangeDisplay()
    {
        //�S�Ẵ^�C�����f�t�H���g�ɖ߂�
        foreach (var highlight in _currentHighlights.Values)
        {
            Destroy(highlight);
        }
        _currentHighlights.Clear();
    }


    //���ݕ\������Ă���S�Ẵn�C���C�g���N���A����
    public void ClearAllHighlights()
    {
        foreach (var highlightGo in _activeHighlights.Values)
        {
            Destroy(highlightGo);
        }

        foreach (var highlight in _currentHighlights.Values)
        {
            Destroy(highlight);
        }

        _currentHighlights.Clear();

        _activeHighlights.Clear();
    }

    //����̃^�C�����n�C���C�g�\������
    public void HighlightTile(Vector2Int gridPosition, HighlightType type)
    {
        //
        if (_activeHighlights.ContainsKey(gridPosition))
        {
            Destroy(_activeHighlights[gridPosition]);
            _activeHighlights.Remove(gridPosition);
        }

        GameObject highlightPrefab = null;
        switch (type)
        {
            case HighlightType.Move:
                highlightPrefab = _movableHighlightPrefab;
                break;
            case HighlightType.Attack:
                highlightPrefab = _attackHighlightPrefab;
                break;
        }

        if (highlightPrefab != null)
        {
            Vector3 worldPos = MapManager.Instance.GetWorldPositionFromGrid(gridPosition);
            GameObject highlight = Instantiate(highlightPrefab, worldPos, Quaternion.identity);
            highlight.transform.SetParent(MapManager.Instance.GetTileAt(gridPosition).transform);
            _activeHighlights[gridPosition] = highlight;
        }
    }

    /// <summary>
    /// �o�H���C����\������
    /// </summary>
    /// <param name="path">�o�H�̃O���b�h���W���X�g</param>
    private void ShowPathLine(List<Vector2Int> path)
    {
        ClearPathLine();//�����̃��C�����N���A

        if (path == null || path.Count < 2)
        {
            return;
        }

        if (_pathLinePrefab == null)
        {
            Debug.LogWarning("Path Line Prefub���ݒ肳��Ă��܂���");
            return;
        }

        _currentPathLine = Instantiate(_pathLinePrefab, Vector3.zero, Quaternion.identity);
        _currentPathLine.transform.SetParent(transform);//MapManager�̎q�ɂ���

        //�o�H�̃O���b�h���W�����[���h���W�̃��X�g�ɕϊ�
        Vector3[] worldPoints = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            worldPoints[i] = GetWorldPositionFromGrid(path[i]);
        }

        //LineRenderer�ɒ��_����ݒ肵�A�o�H�̃��[���h���W���Z�b�g
        _currentPathLine.positionCount = worldPoints.Length;
        _currentPathLine.SetPositions(worldPoints);

        //LineRenderer�̕\���ݒ�
        _currentPathLine.startWidth = 0.1f;//���̊J�n���̑���
        _currentPathLine.endWidth = 0.1f;//���̏I�����̑���
        _currentPathLine.material = new Material(Shader.Find("Sprites/Default"));//�}�e���A��
        _currentPathLine.startColor = Color.white;//���̊J�n���̐F
        _currentPathLine.endColor = Color.white;//���̏I�����̐F
        _currentPathLine.sortingLayerName = "Foreground"; // �\�����C���[��ݒ�i�C�ӁA�őO�ʂɕ\���������ꍇ�j
        _currentPathLine.sortingOrder = 10; // �\��������ݒ�i�C�Ӂj

        Debug.Log($"�o�H���C����\���F�o�H���F{path.Count}");
    }

    /// <summary>
    /// �o�H���C�����N���A����
    /// </summary>
    private void ClearPathLine()
    {
        if (_currentPathLine != null)
        {
            Destroy(_currentPathLine.gameObject);
            _currentPathLine = null;
        }
    }

    //public void OnUnitSelected(Unit unit)
    //{
    //    ClearAllHighlights();//�����̃n�C���C�g���N���A

    //    Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableTiles =
    //        DijkstraPathfinder.FindReachableTiles(unit.CurrentGridPosition, unit);

    //    foreach (Vector2Int pos in reachableTiles.Keys)
    //    {
    //        HighlightTile(pos, HighlightType.Move);
    //    }

    //    //�U���\�͈͂��v�Z���A�ԐF�Ńn�C���C�g
    //    ShowAttackRangeHighlight(reachableTiles.Keys.ToList(),unit);

    //}

    /// <summary>
    /// �ړ��\�͈͂��v�Z���A�n�C���C�g�\������
    /// </summary>
    /// <param name="unit">�ړ����郆�j�b�g</param>
    public void CalculateAndShowMovableRange(Unit unit)
    {
        //ClearMovableRangeDisplay();
        ClearAllHighlights();
        ClearPathLine();

        if (unit == null || _movableHighlightPrefab == null)
        {
            return;
        }

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableNodes =
            DijkstraPathfinder.FindReachableTiles(unit.CurrentGridPosition, unit);



        //�v�Z���ꂽ�ړ��\�͈͂̃^�C�����n�C���C�g�\��
        foreach (var entry in reachableNodes)
        {
            Vector2Int highlightPos = entry.Key;

            if (highlightPos == unit.CurrentGridPosition)
            {
                Debug.Log($"Highlighting current position: {highlightPos}");
                GameObject highlightGO = Instantiate(_movableHighlightPrefab, GetWorldPositionFromGrid(highlightPos), Quaternion.identity, transform);
                _currentHighlights.Add(highlightPos, highlightGO);
                continue;
            }

            if (!IsTileOccupiedForStooping(highlightPos, unit))
            {
                Debug.Log($"Highlighting movable (not occupied by other): {highlightPos}");
                GameObject highlightGO = Instantiate(_movableHighlightPrefab, GetWorldPositionFromGrid(highlightPos), Quaternion.identity, transform);
                _currentHighlights.Add(highlightPos, highlightGO);
            }
            else
            {
                Debug.Log($"NOT Highlighting movable (occupied by other): {highlightPos}");
                //�f�o�b�O�p
                if (_occupiedHighlightPrefab != null)
                {
                    GameObject occupiedHighlightGO = Instantiate(_occupiedHighlightPrefab, GetWorldPositionFromGrid(highlightPos), Quaternion.identity, transform);
                    _currentHighlights.Add(highlightPos, occupiedHighlightGO);
                }
            }
        }
        ShowAttackRangeHighlight(reachableNodes.Keys.ToList(), unit);
    }


    /// <summary>
    /// �U���\�͈͂��n�C���C�g�\������
    /// </summary>
    /// <param name="moveableTiles">�ړ��\�ȃ^�C�����X�g</param>
    private void ShowAttackRangeHighlight(List<Vector2Int> moveableTiles, Unit currentUnit)
    {
        //ClearAllHighlights();

        HashSet<Vector2Int> attackableTiles = new HashSet<Vector2Int>();
        Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0), };

        //�U���͈͎w��̃}���n�b�^���������ł̎���(�܂��etype�Ƃ̘A�g�͖�����)
        //�ꕔ���l�����Ƃ��Ď���2025/06
        int minAttackRange = 2;//�ŏ��˒�
        int maxAttackRange = 2;//�ő�˒�

        foreach (Vector2Int movePos in moveableTiles)
        {
            for (int x = -maxAttackRange; x <= maxAttackRange; x++)
            {
                for (int y = -maxAttackRange; y <= maxAttackRange; y++)
                {
                    //���݂̈ړ��\�^�C��(movePos)����̑��΍��W
                    Vector2Int potentialAttackPos = movePos + new Vector2Int(x, y);

                    //�}���n�b�^�������v�Z
                    int distance = Mathf.Abs(x) + Mathf.Abs(y);

                    if (distance >= minAttackRange && distance <= maxAttackRange)
                    {
                        if (IsValidGridPosition(potentialAttackPos))
                        {
                            attackableTiles.Add(potentialAttackPos);
                        }
                    }
                }
            }
        }

        //���Ƃ��ėאڂP�}�X�̂ݎ���2025 / 06
        //�e�ړ��\�^�C������1�}�X�אڂ���^�C�����U���\�͈͌��Ƃ��Ēǉ�
        //foreach (Vector2Int movePos in moveableTiles)
        //{
        //    foreach (Vector2Int dir in directions)
        //    {
        //        Vector2Int attackTargetPos = movePos + dir;
        //        //�}�b�v�͈͓̔����m�F
        //        if (IsValidGridPosition(attackTargetPos))
        //        {
        //            attackableTiles.Add(attackTargetPos);
        //        }
        //    }
        //}

        //������ύX�̂��ߍ폜
        ////�G���j�b�g�����݂���^�C���݂̂�ԐF�n�C���C�g
        //foreach (Vector2Int targetPos in attackableTiles)
        //{
        //    Tile targetTile = GetTileAt(targetPos);
        //    if (targetTile != null && targetTile.OccupyingUnit != null)
        //    {
        //        //���j�b�g���GfactionType�ł��邩�m�F
        //        if (targetTile.OccupyingUnit.Faction == FactionType.Enemy)
        //        {
        //            HighlightTile(targetPos, HighlightType.Attack);
        //        }
        //    }
        //}

        //�U���͈͂̃n�C���C�g�\��
        foreach (Vector2Int targetPos in attackableTiles)
        {
            if (!moveableTiles.Contains(targetPos))
            {
                //�G�̗L�����킸�n�C���C�g
                MyTile targetTile = GetTileAt(targetPos);
                if (targetTile != null)
                {
                    HighlightTile(targetPos, HighlightType.Attack);
                }
            }
        }
    }



    



    /// <summary>
    /// ���j�b�g���o�H�ɉ����Ĉړ�������
    /// </summary>
    /// <param name="unit">�ړ����郆�j�b�g</param>
    /// <param name="path">�ړ��o�H�̃O���b�h���W���X�g</param>
    public System.Collections.IEnumerator MoveUnitAlogPath(Unit unit,List<Vector2Int> path)
    {
        //�I�������ƃn�C���C�g�N���A�͈ړ��J�n���ɍs��2025/06
        //if(_selectedUnit != null)
        //{
        //    _selectedUnit.SetSelected(false);
        //    _selectedUnit = null;
        //}
        //ClearMovableRangeDisplay();


        float moveSpeed = 5.0f;//Unit�̌����ڂ̈ړ����x

        for(int i = 0; i < path.Count; i++)
        {
            Vector2Int targetGridPosInPath = path[i];
            Vector3 startWorldPos = unit.transform.position;
            Vector3 targetWorldPos = GetWorldPositionFromGrid(path[i]);
            Vector3 endWorldPos = GetWorldPositionFromGrid(targetGridPosInPath);
            float distance = Vector3.Distance(startWorldPos, targetWorldPos);
            float duration = distance / moveSpeed;
            float elapsed = 0f;

            //�e�^�C���֌����Ĉړ�
            while(elapsed < duration)
            {
                unit.transform.position = Vector3.Lerp(startWorldPos, targetWorldPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;//1�t���[���҂�
            }
            unit.transform.position = targetWorldPos;//�m���ɖڕW�n�_�ɓ��B������

            
            unit.UpdatePosition(path[i]);
        }

        
        ClearPathLine();//���݂͈ړ������ŃN���A
        Debug.Log("���j�b�g�̈ړ����������܂���");
    }

    public IEnumerator SmoothMoveCoroutine(Unit unit, Vector2Int startGridPos, Vector2Int endGridPos,List<Vector2Int> path)
    {
        if (path == null || path.Count <= 1) // �p�X��������Ȃ��A�܂��͓����ʒu�ɂ���ꍇ (path.Count <= 1 �͊J�n�n�_�݂̂̏ꍇ)
        {
            Debug.LogWarning("�ړ��p�X��������Ȃ����A���j�b�g�����ɖړI�n�ɂ��܂��B");
            // �v���C���[���j�b�g�̏ꍇ�̂�FinalizeMoveState���Ăяo��
            if (unit is PlayerUnit)
            {
                ConfirmMove();
            }
            yield break;
        }


        // �o�H�̊e�|�C���g��H���Ĉړ�
        // �ŏ��̗v�f�͌��݂̈ʒu�Ȃ̂ŃX�L�b�v (path.Count > 1 ���m�F)
        for (int i = 1; i < path.Count; i++) // �ŏ��̒n�_�͊��ɂ���ꏊ�Ȃ̂ŁA2�Ԗڂ̒n�_����ړ����J�n
        {
            Vector2Int targetGridPosInPath = path[i];
            Vector3 startWorldPos = unit.transform.position;
            Vector3 endWorldPos = GetWorldPositionFromGrid(targetGridPosInPath);
            float durationPerTile = 0.2f; // �^�C��1�}�X������̈ړ�����
            float elapsed = 0f;

            while (elapsed < durationPerTile)
            {
                unit.transform.position = Vector3.Lerp(startWorldPos, endWorldPos, elapsed / durationPerTile);
                elapsed += Time.deltaTime;
                yield return null;
            }
            unit.transform.position = endWorldPos; // �e�^�C���̒��S�ɐ��m�ɃX�i�b�v

        }


        // �S�Ă̈ړ�������������A�ŏI�I�ȏ�Ԃ��m��
        // ���j�b�g�̃O���b�h���W���X�V
        unit.SetGridPosition(endGridPos);

        // ���j�b�g�̐�L�^�C�����X�V (MapManager��GetTileAt��OccupyingUnit�v���p�e�B���g�p)
        MyTile oldTile = GetTileAt(startGridPos);
        if (oldTile != null) oldTile.OccupyingUnit = null; // ���̃^�C�����烆�j�b�g������

        MyTile newTile = GetTileAt(endGridPos);
        if (newTile != null) newTile.OccupyingUnit = unit; // �V�����^�C���Ƀ��j�b�g��ݒ�

        // �v���C���[���j�b�g�̏ꍇ�̂�FinalizeMoveState���Ăяo��
        if (unit is PlayerUnit)
        {
            ConfirmMove();
        }
        else // �G���j�b�g�̏ꍇ�̈ړ��������� (�K�v�ɉ����Ēǉ�)
        {
            // �G���j�b�g�ŗL�̈ړ�������̏���������΂����ɋL�q
            // ��: �^�[���I����AI�ɒʒm����Ȃ�
        }
    }


    /// <summary>
    /// ���j�b�g�̈ړ���̍s����I������
    /// </summary>
    private void ConfirmUnit()
    {
        if (_selectedUnit == null || !_isMovingOrPlanning)
        {
            return;
        }

        //�ړ���������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmMove();
        }
        //�ړ��L�����Z������
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            CancelMove();
        }
        //�U������
        else if (Input.GetKeyDown(KeyCode.D))
        {
            AttackRangeHighlight();
        }
    }


    /// <summary>
    /// �U���s������
    /// </summary>
    /// <param name="moveableTiles">�ړ��\�ȃ^�C�����X�g</param>
    private void AttackRangeHighlight()
    {
        ClearAllHighlights();
        ClearPathLine();


        HashSet<Vector2Int> attackableTiles = new HashSet<Vector2Int>();
        Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0), };

        //�U���͈͎w��̃}���n�b�^���������ł̎���(�܂��etype�Ƃ̘A�g�͖�����)
        //�ꕔ���l�����Ƃ��Ď���2025/06
        int minAttackRange = _selectedUnit._minAttackRange;//�ŏ��˒�
        int maxAttackRange = _selectedUnit._maxAttackRange;//�ő�˒�



        MyTile newTile = GetTileAt(_currentPlannedMovePositon);
        if (newTile != null)
        {
            _selectedUnit.MoveToGridPosition(_currentPlannedMovePositon, newTile);
            _selectedUnit.transform.position = GetWorldPositionFromGrid(_currentPlannedMovePositon);
        }
        Vector2Int currentPos = _selectedUnit.CurrentGridPosition;

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
                    if (IsValidGridPosition(potentialAttackPos))
                    {
                        attackableTiles.Add(potentialAttackPos);
                    }
                }
            }
        }

        //�U���͈͂̃n�C���C�g�\��
        foreach (Vector2Int targetPos in attackableTiles)
        {
            if (attackableTiles.Contains(targetPos))
            {
                //�G�̗L�����킸�n�C���C�g
                MyTile targetTile = GetTileAt(targetPos);
                if (targetTile != null)
                {
                    HighlightTile(targetPos, HighlightType.Attack);
                }
            }
        }

        //_selectedUnit.SetActedThisTrun();
        _isAttacking = true;

        //ResetMoveState();
    }




    /// <summary>
    /// �ړ����m�肷��
    /// </summary>
    private void ConfirmMove()
    {
        //if (_selectedUnit == null || _currentPlannedMovePositon == Vector2Int.zero || !_isMovingOrPlanning)
        //{
        //    return;
        //}

        //if (_selectedUnit == null)
        //{
        //    return;
        //}

        if(_selectedUnit == null || !_isMovingOrPlanning)
        {
            return;
        }


        MyTile newTile = GetTileAt(_currentPlannedMovePositon);
        if(newTile != null)
        {
            _selectedUnit.MoveToGridPosition(_currentPlannedMovePositon, newTile);
            _selectedUnit.transform.position = GetWorldPositionFromGrid(_currentPlannedMovePositon);
        }


       

        _selectedUnit.SetActedThisTrun();
        ResetMoveState();
        Debug.Log("�ړ����m�肵�܂����B");

        


        //_isMovingOrPlanning = false;
        //_selectedUnit = null;
        //ClearAllHighlights();
        //ClearMovableRangeDisplay();
        //ClearPathLine();

        TurnManager.Instance.CheckAllPlayerUnitActed();

        //������ύX���邽�߃R�����g�A�E�g2025/07
        //if(_selectedUnit != null && _currentPlannedMovePositon != Vector2Int.zero)
        //{
        //    //���j�b�g�̃O���b�h���W�ƃ��[���h���W���X�V
        //    //_selectedUnit.SetGridPosition(_currentPlannedMovePositon);
        //    //_selectedUnit.transform.position = GetWorldPositionFromGrid(_currentPlannedMovePositon);

            
        //    //���j�b�g�̍s����������Ԃɂ���
        //    _selectedUnit.SetActionTaken(true);

        //    //�n�C���C�g���N���A���A�I����Ԃ�����
        //    ClearAllHighlights();
        //    ClearMovableRangeDisplay();
        //    _selectedUnit.SetSelected(false);
        //    _selectedUnit.SetActionTaken(true);
        //    _selectedUnit = null;

        //    //��Ԃ����Z�b�g
        //    _currentPlannedMovePositon = Vector2Int.zero;
        //    _originalUnitPositon = Vector2Int.zero;
        //    _isMovingOrPlanning = false;

        //    Debug.Log("���j�b�g�̈ړ��Ə�ԃN���A���������܂���");
        //}
    }

    /// <summary>
    /// �ړ����L�����Z�����A���j�b�g�����̈ʒu�ɖ߂�
    /// </summary>
    private void CancelMove()
    {

        if (_selectedUnit != null)
        {
            //_selectedUnit.SetSelected(false);
            _selectedUnit.transform.position = MapManager.Instance.GetWorldPositionFromGrid(_originalUnitPositon);
        }


        //if (_canceled == false)
        //{
        //    if (_isMovingOrPlanning == true)
        //    {
        //        Tile newTile = GetTileAt(_originalUnitPositon);
        //        Debug.Log($"True�̂Ƃ�{_originalUnitPositon}");
        //        if (newTile != null)
        //        {
        //            _selectedUnit.MoveToGridPosition(_originalUnitPositon, newTile);
        //        }
        //    }
        //    else if (_isMovingOrPlanning == false)
        //    {
        //        //���j�b�g�����̃O���b�h���W�ƃ��[���h���W�ɖ߂�
        //        Tile newTile = GetTileAt(_currentPlannedMovePositon);
        //        Debug.Log($"False�̂Ƃ�{_currentPlannedMovePositon}");
        //        if (newTile != null)
        //        {
        //            _selectedUnit.MoveToGridPosition(_currentPlannedMovePositon, newTile);
        //        }
        //    }
        //    _canceled = true;
        //}
        //else
        //{
        //    Tile newTile = GetTileAt(_originalUnitPositon);
        //    Debug.Log($"True�̂Ƃ�{_originalUnitPositon}");
        //    if (newTile != null)
        //    {
        //        _selectedUnit.MoveToGridPosition(_originalUnitPositon, newTile);
        //    }
        //    _canceled = false;
        //}


        //_selectedUnit.SetGridPosition(_originalUnitPositon);
        //_selectedUnit.transform.position = GetWorldPositionFromGrid(_originalUnitPositon);

        //_isMovingOrPlanning = false;

        //_selectedUnit = null;
        //_originalUnitPositon= Vector2Int.zero;
        //_currentPlannedMovePositon = Vector2Int.zero;
        //ClearAllHighlights();
        //ClearMovableRangeDisplay();
        //ClearPathLine();

        ResetMoveState();
        Debug.Log("�L�����Z�����ꂽ�̂Ō��̏ꏊ�ɖ߂��܂�");


        //if (_selectedUnit != null && _currentPlannedMovePositon != Vector2Int.zero)
        //{
        //    //���j�b�g�����̃O���b�h���W�ƃ��[���h���W�ɖ߂�
        //    _selectedUnit.SetGridPosition(_originalUnitPositon);
        //    _selectedUnit.transform.position = GetWorldPositionFromGrid(_originalUnitPositon);

        //    //�n�C���C�g���N���A���A�I����Ԃ�����
        //    ClearAllHighlights();
        //    ClearMovableRangeDisplay();
        //    _selectedUnit = null;

        //    //��Ԃ����Z�b�g
        //    _currentPlannedMovePositon = Vector2Int.zero;
        //    _originalUnitPositon= Vector2Int.zero;
        //    _isMovingOrPlanning= false;

        //    Debug.Log("���j�b�g�̈ړ����L�����Z�����ꂽ�̂Ō��̏ꏊ�ɖ߂��܂�");
        //}
    }



    /// <summary>
    /// �ړ���Ԃ����Z�b�g����w���p�[���\�b�h
    /// </summary>
    private void ResetMoveState()
    {
        if(_selectedUnit != null)
        {
            _selectedUnit.SetSelected(false);
        }
        _selectedUnit = null;
        _currentPlannedMovePositon = Vector2Int.zero;
        _originalUnitPositon = Vector2Int.zero;
        _isMovingOrPlanning = false;
        _isConfirmingMove = false;
        _isAttacking = false;


        ClearAllHighlights();
        //////ClearMovableRangeDisplay();
        ClearPathLine();
    }


    //////////TIlemap���g�������\�b�h�Q

    public class Tile
    {
        public Vector2Int GridPosition { get; private set; }
        public TerrainType TerrainType { get; private set; }

        public Tile(Vector2Int gridPosition, TerrainType terrainType)
        {
            GridPosition = gridPosition;
            TerrainType = terrainType;
        }
    }


    public void GenerateMapFromTilemap()
    {
        ClearMapFromTilemap();

        if(_generateTilemap == null)
        {
            Debug.LogError("Tilemap���ݒ肳��Ă��܂���");
            return;
        }

        _tileDataFromTilemapTest.Clear();

        //Tilemap�̋��E�����擾(���̎擾�ɓ��̂��ߕύX)
        //BoundsInt bounds = _generateTilemap.cellBounds;

        // �蓮�ŕ`�悵���}�b�v�̋��E�𐳊m�Ɏ擾
        BoundsInt bounds = GetMapBoundsFromTilemap();

        //���E�̃T�C�Y����O���b�h�̃}�X�����擾
        _tilemapGridSize = new Vector2Int(bounds.size.x, bounds.size.y);

        Debug.LogWarning($"Tilemap�����ɂ��}�b�v�̃T�C�Y�F{_tilemapGridSize.x},{_tilemapGridSize.y}");

        for (int y = 0; y < _tilemapGridSize.y; y++)
        {
            for (int x = 0; x < _tilemapGridSize.x; x++)
            {
                Vector3Int Pos = new Vector3Int(x, y, 0);//���݂̃O���b�h���W
                Vector2Int gridPos = new Vector2Int(x, y);

                TileBase tileBase = _generateTilemap.GetTile(Pos);
                if (tileBase != null)
                {
                    CustomTile customTile = tileBase as CustomTile;
                    if (customTile != null)
                    {
                        TerrainType terrainType = customTile.terrainType;


                        Vector3 worldPos = GetWorldPositionFromGrid(gridPos);//�O���b�h���W�����[���h���W�ɕϊ�

                        GameObject tileGO = Instantiate(_tilePrefabTilemap, worldPos, Quaternion.identity, transform);

                        //��������GameObject����Tile�R���|�[�l���g���擾����
                        MyTile tile = tileGO.GetComponent<MyTile>();
                        if (tile == null)
                        {
                            Debug.LogError($"TilePrefab��Tile�R���|�[�l���g���A�^�b�`���ꂢ�܂���{_tilePrefabTilemap.name}");
                            Destroy(tileGO);
                            continue;
                        }

                        //�擾����Tile�R���|�[�l���g��������
                        tile.Initialize(gridPos, terrainType, false);

                        //�����E����������������Tile�I�u�W�F�N�g����Ō����ł���悤��
                        _tileDataFromTilemapTest.Add(gridPos, tile);
                        Debug.LogWarning($"GridPos{gridPos}:tileType{tile.TerrainType}");
                    }
                }
            }
        }







        //foreach (var pos in bounds.allPositionsWithin)
        //{
        //    //GetTile()�Ń^�C�����擾
        //    TileBase tileBase = _generateTilemap.GetTile(pos);


        //    //TileBase�����݂���΁A���̃^�C������_tileDataFromTilemap�Ɋi�[
        //    if (tileBase != null)
        //    {
        //        //TileBase��CustomTile�^�ɃL���X�g
        //        CustomTile customTIle = tileBase as CustomTile;

        //        //�L���X�g�����������ꍇ�iCustomTile�ł���΁j
        //        if (customTIle != null)
        //        {
        //            //CustomTile������terrainType�����擾
        //            TerrainType terrainType = customTIle.terrainType;

        //            //_tileDataFromTilemap�Ɋi�[���邽�߂�Tile�I�u�W�F�N�g���쐬
        //            Tile tile = new Tile(new Vector2Int(pos.x, pos.y), terrainType);

        //            //_tileDataFromTilemap�ɍ��W���L�[�Ƃ��ď���ǉ�
        //            _tileDataFromTilemap.Add(tile.GridPosition, tile);
        //        }
        //    }
        //}

        Debug.LogWarning($"_tileDataFromTilemap�Ɋi�[���ꂽ�^�C���̐�: {_tileDataFromTilemap.Count}");
        Debug.LogWarning($"_tileDataFromTilemap�Ɋi�[���ꂽ�^�C���̐�: {_tileDataFromTilemapTest.Count}");
    }

    private BoundsInt GetMapBoundsFromTilemap()
    {
        //�^�C�������݂���Z���̍ŏ��E�ő���W���i�[����
        Vector3Int min = new Vector3Int(int.MaxValue, int.MaxValue, 0);
        Vector3Int max = new Vector3Int(int.MinValue, int.MinValue, 0);
        bool foundTile = false;

        //Tilemap�̕`��\�ȑS�͈͂𑖍�����
        foreach (var pos in _generateTilemap.cellBounds.allPositionsWithin)
        {
            //�w�肵���Z���Ƀ^�C�������݂��邩�m�F
            if (_generateTilemap.GetTile(pos) != null)
            {
                //�^�C��������������A�ŏ��E�ő���W���X�V
                min.x = Mathf.Min(min.x, pos.x);
                min.y = Mathf.Min(min.y, pos.y);
                max.x = Mathf.Max(max.x, pos.x);
                max.y = Mathf.Max(max.y, pos.y);
                foundTile = true;
            }
        }

        if (foundTile)
        {
            //���ۂ̋��E���v�Z���ĕԂ�
            return new BoundsInt(min.x, min.y, 0, max.x - min.x + 1, max.y - min.y + 1, 1);
        }
        else
        {
            //�^�C����������Ȃ��ꍇ�͋��Bounds��Ԃ�
            return new BoundsInt();
        }
    }


    public MyTile GetTileAt (Vector2Int position)
    {
        if (_tileDataFromTilemapTest.TryGetValue(position, out MyTile tile))
        {
            return tile;
        }
        return null;
    }

    private void ClearMapFromTilemap()
    {
        foreach (var tileEntry in _tileDataFromTilemapTest)
        {
            Destroy(tileEntry.Value.gameObject);//Tile�I�u�W�F�N�g���A�^�b�`����Ă���GameObject��j��
        }
        _tileDataFromTilemapTest.Clear();         //Dictionary�̒��g���N���A
        //_currentMapData = null; //�ǂݍ��񂾃}�b�v�f�[�^���N���A
        Debug.Log("MapManager:�����̃}�b�v���N���A���܂���");
    }


    //public void GenerateMap(string mapId)
    //{
    //    ClearMap();//���Ƀ}�b�v����������Ă���\�����l�����āA��x�N���A����

    //    //MapDataLoader��CSV�t�@�C������}�b�v�f�[�^��ǂݍ���
    //    MapData mapData = MapDataLoader.LoadMapDataFromCSV(mapId);

    //    //_currentMapData = MapDataLoader.LoadMapDataFromCSV(mapId);

    //    if (mapData == null)
    //    {
    //        Debug.LogError("MapManager:�}�b�v�f�[�^�̓ǂݍ��݂Ɏ��s���܂����B�}�b�v�����ł��܂���");
    //        return;
    //    }

    //    _currentMapData = mapData;

    //    _gridSize = new Vector2Int(_currentMapData.Width, _currentMapData.Height);


    //    for (int y = 0; y < _currentMapData.Height; y++)
    //    {
    //        for (int x = 0; x < _currentMapData.Width; x++)
    //        {
    //            Vector2Int gridPos = new Vector2Int(x, y);//���݂̃O���b�h���W
    //            TerrainType terrainType = _currentMapData.GetTerrainType(gridPos);//���̍��W�̒n�`�^�C�v���擾


    //            Vector3 worldPos = GetWorldPositionFromGrid(gridPos);//�O���b�h���W�����[���h���W�ɕϊ�

    //            GameObject tileGO = Instantiate(_tilePrefab, worldPos, Quaternion.identity, transform);

    //            //��������GameObject����Tile�R���|�[�l���g���擾����
    //            MyTile tile = tileGO.GetComponent<MyTile>();
    //            if (tile == null)
    //            {
    //                Debug.LogError($"TilePrefab��Tile�R���|�[�l���g���A�^�b�`���ꂢ�܂���{_tilePrefab.name}");
    //                Destroy(tileGO);
    //                continue;
    //            }

    //            //�擾����Tile�R���|�[�l���g��������
    //            tile.Initialize(gridPos, terrainType, false);

    //            //�n�`�^�C�v�ɉ������X�v���C�g��Tile�ɐݒ�
    //            SetTileSprite(tile, terrainType);

    //            //�����E����������������Tile�I�u�W�F�N�g����Ō����ł���悤��
    //            _tileData.Add(gridPos, tile);
    //        }
    //    }
    //    Debug.Log($"MapManager:�}�b�v�𐶐����܂���({_currentMapData.Width}x{_currentMapData.Height})");

    //    //PlayerUnit�̏����z�u
    //    //PlacePlayerUnitAtInitialPostiton();
    //}













    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_mapSequence.Length > 0)
        {
            Initialize();
            InitializeCamera();
            //TurnManager.Instance.InitializeTurnManager();
            //GenerateMap(_mapSequence[_currentMapIndex]);
            //GenerateMap(_mapSequence[0]);
            //PlacePlayerUnitAtInitialPostiton();
            //PlaceEnemyUnitAtInitialPostiton();
        }
        else
        {
            Debug.LogError("MapManager:�}�b�v�V�[�P���X���ݒ肳��Ă��܂���");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleCameraInput();

        //�e�t�F�C�Y�ɉ����ē��͏�����؂�ւ���
        if (_gameManager.CurrentBattlePhase == BattlePhase.BattleDeployment)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseInputInBattlePreparation();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.LogWarning("�퓬�t�F�C�Y�ֈڍs");
                ResetMoveState();
                _gameManager.ChangePhase(BattlePhase.BattleMain);
            }

        }
        else if(_gameManager.CurrentBattlePhase == BattlePhase.BattleMain)
        {
            //�v���C���[�^�[�����̂ݓ��͂��󂯕t����
            if (TurnManager.Instance != null && TurnManager.Instance.CurrnetTurnState != TurnState.PlayerTurn)
            {
                return;
            }

            //�}�E�X��������m�i���N���b�N2025 / 06�j
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }

            if (_selectedUnit != null)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    CancelMove();
                }
            }
            if (_isConfirmingMove)
            {
                if (_selectedUnit != null)
                {
                    ConfirmUnit();
                    return;
                }
            }
        }






        //Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2Int clickedGridPos = GetGridPositionFromWorld(mouseWorldPos);

        //HandleCameraInput();
        //MoveCameraToTarget();


        //�v���C���[�^�[�����̂ݓ��͂��󂯕t����
        //if (TurnManager.Instance != null && TurnManager.Instance.CurrnetTurnState != TurnState.PlayerTurn)
        //{
        //    return;
        //}

        ////�}�E�X��������m�i���N���b�N2025 / 06�j
        //if (Input.GetMouseButtonDown(0))
        //{
        //    HandleMouseClick();
        //}

        //if(_selectedUnit != null)
        //{
        //    if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        CancelMove();
        //    }
        //}


        //if(_selectedUnit != null && _currentPlannedMovePositon != Vector2Int.zero)
        // �ړ��m��҂���Ԃ̏ꍇ�́A���͏����𐧌�����
        //if (_isConfirmingMove)
        //{
        //    if (_selectedUnit != null)
        //    {
        //        ConfirmUnit();

        //        //���F�X�y�[�X�L�[�ňړ��m��
        //        //if (Input.GetKeyDown(KeyCode.Space))
        //        //{
        //        //    //ConfirmMove();
        //        //}
        //        ////���FQ�L�[�ŃL�����Z��
        //        //else if (Input.GetKeyDown(KeyCode.Q))
        //        //{
        //        //    //CancelMove();
        //        //}
        //        //���݂͈ȏ�̓��͏����ȊO�͎󂯕t���Ȃ�2025/07
        //        //if (_isAttacking)
        //        //{
        //        //    if (Input.GetMouseButtonDown(0))
        //        //    {
        //        //        //HandleMouseClick();
        //        //    }
        //        //}
        //        return;
        //    }
            
        //}
        //Tile clickedTile = GetTileAt(clickedGridPos);
        //if(clickedTile == null)
        //{
        //    return;
        //}

        ////���j�b�g���N���b�N���ꂽ�ꍇ
        //if(clickedTile.OccupyingUnit != null)
        //{
        //    HandleUnitClick(clickedTile.OccupyingUnit);
        //}
        ////�^�C�����N���b�N���ꂽ�ꍇ
        //else
        //{
        //    HandleTileClick(clickedTile);
        //}
    }
}
