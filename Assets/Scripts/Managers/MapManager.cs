using UnityEngine;
using System.Collections.Generic;


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
                if(_instance == null)
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
    [SerializeField]private Tile[,] _tileGrid;//�e�O���b�h�̏����i�[����2�����z��(Inspector�\���s�̂���[SerializeField]�͖���)

    //�}�b�v�쐬�p
    [SerializeField] private string[] _mapSequence;//Resouces�t�H���_�ȉ���CSV�t�@�C���̃p�X
    [SerializeField] private GameObject _tilePrefab;//�e�^�C�������Ɏg�p����Tile�R���|�[�l���g���t����Prefab
    [SerializeField] private float _tileSize = 1.0f;//�O���b�h��1�}�X������̃��[���h���W�ł̃T�C�Y
    [SerializeField] private Sprite[] _terrainSprite;//�n�`�^�C�v�ɑΉ�����X�v���C�g��ݒ肷�邽�߂̔z��

    [SerializeField] private TerrainCost[] _terrainCosts;

    //�ړ��֘A
    [SerializeField] private GameObject _movableHighlightPrefab;
    private Dictionary<Vector2Int,GameObject> _currentHighlights = new Dictionary<Vector2Int, GameObject>();
    [SerializeField] private LineRenderer _pathLinePrefab;//�o�H�\���p�̃v���n�u
    private LineRenderer _currentPathLine;//���ݕ\������Ă���o�H���C��
    


    private int _currentMapIndex = 0;//���݃��[�h���Ă���}�b�v�̃C���f�b�N�X
    private MapData _currentMapData;//MapDtaLoader�ɂ���ēǂݍ��܂��}�b�v�f�[�^

    private Dictionary<Vector2Int ,Tile> _tiles = new Dictionary<Vector2Int, Tile>();//�������ꂽ�S�Ă�Tile�I�u�W�F�N�g�ƃO���b�h���W���Ǘ�����

    //PlayerUnit�֘A
    [SerializeField] private GameObject _playerUnitPrefab;
    private Unit _currentPlayerUnit;//���݂̃v���C���[���j�b�g�̎Q��
    private Unit _selectedUnit;//�I�𒆂̃v���C���[���j�b�g

    [System.Serializable]public class TerrainCost
    {
        public TerrainType terrainType;
        public int cost;
    }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
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
    public void GenerateMap(string mapPath)
    {
        ClearMap();//���Ƀ}�b�v����������Ă���\�����l�����āA��x�N���A����

        //MapDataLoader��CSV�t�@�C������}�b�v�f�[�^��ǂݍ���
        _currentMapData = MapDataLoader.LoadMapDataFromCSV(mapPath);

        if(_currentMapData == null)
        {
            Debug.LogError("MapManager:�}�b�v�f�[�^�̓ǂݍ��݂Ɏ��s���܂����B�}�b�v�����ł��܂���");
            return;
        }

        for(int y = 0; y < _currentMapData.Height; y++)
        {
            for(int x = 0; x < _currentMapData.Width; x++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);//���݂̃O���b�h���W
                TerrainType terrainType = _currentMapData.GetTerrainType(gridPos);//���̍��W�̒n�`�^�C�v���擾
                

                Vector3 worldPos = GetWorldPosition(gridPos);//�O���b�h���W�����[���h���W�ɕϊ�

                GameObject tileGO = Instantiate(_tilePrefab,worldPos,Quaternion.identity, transform);

                //��������GameObject����Tile�R���|�[�l���g���擾����
                Tile tile = tileGO.GetComponent<Tile>();
                if(tile == null)
                {
                    Debug.LogError($"TilePrefab��Tile�R���|�[�l���g���A�^�b�`���ꂢ�܂���{_tilePrefab.name}");
                    Destroy(tileGO);
                    continue;
                }

                //�擾����Tile�R���|�[�l���g��������
                tile.Initialize(gridPos, terrainType,false);

                //�n�`�^�C�v�ɉ������X�v���C�g��Tile�ɐݒ�
                SetTileSprite(tile,terrainType);

                //�����E����������������Tile�I�u�W�F�N�g����Ō����ł���悤��
                _tiles.Add(gridPos,tile);
            }
        }
        Debug.Log($"MapManager:�}�b�v�𐶐����܂���({_currentMapData.Width}x{_currentMapData.Height})");

        //PlayerUnit�̏����z�u
        //PlacePlayerUnitAtInitialPostiton();
    }


    /// <summary>
    /// �w�肳�ꂽ�O���b�h���W�̃^�C�������擾����
    /// </summary>
    /// <param name="position">�O���b�h���W</param>
    /// <return>Tile�I�u�W�F�N�g�A�͈͊O�Ȃ�null</return>
    public Tile GetTileAt(Vector2Int position)
    {
        if(_tiles.TryGetValue(position, out Tile tile))
        {
            return tile;
        }
        return null;
    }

    /// <summary>
    /// �w��O���b�h�ƃ��j�b�g�^�C�v�ɉ������ړ��R�X�g��Ԃ�
    /// </summary>
    /// <param name="positon">�O���b�h���W</param>
    /// <param name="unitType">���j�b�g�^�C�v</param>
    /// <return>�ړ��R�X�g</return>
    public int GetMovementCost(Vector2Int position, UnitType unitType)
    {
        Tile tile = GetTileAt(position);
        if (tile == null)
        {
            return int.MinValue; //�͈͊O�͈ړ��s��
        }

        //�n�`�ƃ��j�b�g�^�C�v�ɉ������ړ��R�X�g�̃��W�b�N
        switch (tile.TerrainType)
        {
            case TerrainType.Plain://���n
                return 1;
            case TerrainType.Forest://�X
                    return 2;
            case TerrainType.Mountain://�R
                    if(unitType == UnitType.Flying) { return 1; }//��s���j�b�g�̓R�X�g��
                    if(unitType == UnitType.Mountain) { return 1; }//�R�����j�b�g�̓R�X�g��
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
    public void ChangeTerrain(Vector2Int position, TerrainType newType)
    {
        Tile tile = GetTileAt(position);
        if(tile == null)
        {
            Debug.Log($"MapManager:{position}�̒n�`��{tile.TerrainType}����{newType}�ω����܂���");
            //�����ڂ̕ω��̂��ߎ��o�I�������w��
        }
    }

    /// <summary>
    /// Tile�N���X��Type���Z�b�g����
    /// </summary>
    /// <param name="positon"></param>
    /// <param name="newType"></param>
    public void SetTileType(Vector2Int positon, TerrainType newType)
    {
        Tile tile = GetTileAt(positon);
        if(tile != null)
        {
            tile.SetType(newType);
        }
    }

    /// <summary>
    /// �w�肳�ꂽ�^�C���ɁA���̒n�`�^�C�v�ɉ������X�v���C�g��ݒ肷��
    /// </summary>
    /// <param name="tile">�X�v���C�g��ݒ肵����Tile�I�u�W�F�N�g</param>
    /// <param name="type">���̃^�C���̒n�`�^�C�v</param>
    private void SetTileSprite(Tile tile, TerrainType type)
    {
        //�n�`��Enum�̒l���L���X�g���āA_terrainSprites�z��̃C���f�b�N�X�Ƃ��Ďg�p
        int typeIndex = (int)type;

        //�z��͈̔̓`�F�b�N�ƃX�v���C�g��Ins@ector�Őݒ肳��Ă��邩�̊m�F
        if(typeIndex >= 0 && typeIndex < _terrainSprite.Length && _terrainSprite[typeIndex] != null)
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
    public Vector3 GetWorldPosition(Vector2Int gridPos)
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
        foreach(var tileEntry in _tiles)
        {
            Destroy(tileEntry.Value.gameObject);//Tile�I�u�W�F�N�g���A�^�b�`����Ă���GameObject��j��
        }
        _tiles.Clear();         //Dictionary�̒��g���N���A
        _currentMapData = null; //�ǂݍ��񂾃}�b�v�f�[�^���N���A
        Debug.Log("MapManager:�����̃}�b�v���N���A���܂���");
    }

    /// <summary>
    /// �v���C���[���j�b�g�������ʒu�ɔz�u����
    /// </summary>
    private void PlacePlayerUnitAtInitialPostiton()
    {
        //�v���g�^�C�v�p�̉��������W
        Vector2Int initialPlayerGridPos = new Vector2Int(0, 0);


        //�w�肳�ꂽ���W���}�b�v�͈͓����m�F
        if(initialPlayerGridPos.x < 0 || initialPlayerGridPos.x >= _currentMapData.Width ||
            initialPlayerGridPos.y < 0 || initialPlayerGridPos.y >= _currentMapData.Height)
        {
            Debug.LogError($"MapManager:�v���C���[���j�b�g�̏����z�u���W({initialPlayerGridPos})���}�b�v�͈͊O�ł�");
        }

        //�v���C���[���j�b�g�����ɑ��݂���ꍇ�͔j��(�V�[���J�ڂȂǂōĐ�������ꍇ)
        if(_currentPlayerUnit != null)
        {
            Destroy(_currentPlayerUnit.gameObject);
        }

        //�v���C���[���j�b�g�v���n�u�̐���
        GameObject unitGO = Instantiate(_playerUnitPrefab, GetWorldPosition(initialPlayerGridPos), Quaternion.identity, transform);
        Unit playerUnit = unitGO.GetComponent<Unit>();

        if(playerUnit == null)
        {
            Debug.LogError($"MapManager:PlayerUnitPrefub��PlayerUnit�R���|�[�l���g���A�^�b�`����Ă��܂���:{_playerUnitPrefab.name}");
            Destroy(unitGO);
            return;
        }
        _currentPlayerUnit = playerUnit;

        UnitData dummyData = new UnitData();
        dummyData.UnitId = "PLAYER001";
        dummyData.UnitName = "none";
        dummyData.Type = UnitType.Infantry;
        dummyData.BaseMovement = 5;
        dummyData.BaseAttackPower = 5;
        dummyData.BaseDefensePower = 5;
        dummyData.BaseHP = 5;
        dummyData.BaseSkill = 5;
        dummyData.BaseSpeed = 5;

        //�v���C���[���j�b�g�̏������ƃ��[���h���W�ւ̔z�u
        _currentPlayerUnit.Initialize(dummyData);//���j�b�g�����n��
        _currentPlayerUnit.UpdatePosition(initialPlayerGridPos);//���[���h���W��ݒ�
        
        

        Debug.Log($"PlayerUnit'{_currentPlayerUnit.name}'placed at grid:{initialPlayerGridPos}");
    }

    private void HandleMouseClick()
    {
        //�}�E�X�̃X�N���[�����W�����[���h���W�ɕϊ�
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;//2D�Q�[���Ȃ̂�Z��0�ɌŒ�

        Debug.Log($"�N���b�N���ꂽ���[���h���W�F{mouseWorldPos}");

        //���[���h���W���O���b�h���W�ɕϊ�
        Vector2Int clickedGridPos = GetGridPositionFromWorld(mouseWorldPos);

        Debug.Log($"�N���b�N���ꂽ�O���b�h���W�F{clickedGridPos}");

        //Ryacast���g���ăN���b�N���ꂽ�I�u�W�F�N�g�����o
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if(hit.collider != null)
        {
            //�N���b�N���ꂽ�̂��v���C���[���j�b�g���m�F
            PlayerUnit clickedUnit = hit.collider.GetComponent<PlayerUnit>();
            if(clickedUnit != null)
            {
                //���j�b�g���N���b�N���ꂽ�ꍇ�̓^�C���N���b�N�������s��Ȃ�
                HandleUnitClick(clickedUnit);
                return;
            }

            //�N���b�N���ꂽ�̂��^�C�����m�F
            Tile clickedTile = hit.collider.GetComponent<Tile>();
            if(clickedTile != null)
            {
                HandleTileClick(clickedTile);
                return;
            }
        }
        else
        {
            //�����N���b�N����Ȃ������ꍇ�i�}�b�v�O�Ȃǁj
            Debug.Log("�����N���b�N����܂���ł���");
            //�I�𒆂̃��j�b�g������Δ�I����Ԃɂ���
            if(_selectedUnit != null)
            {
                _selectedUnit.SetSelected(false);
                _selectedUnit = null;
                Debug.Log("���j�b�g�̑I�����������܂����B");
            }
        }

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

    //���j�b�g���N���b�N���ꂽ���̏���
    private void HandleUnitClick(Unit clickedUnit)
    {
        if(_selectedUnit == clickedUnit)
        {
            _selectedUnit.SetSelected(false);
            _selectedUnit = null;
            ClearMovableRangeDisplay();
            Debug.Log("���j�b�g�̑I�����������܂���");
            return;
        }

        if(_selectedUnit != null)
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
    private void HandleTileClick(Tile clickedTile)
    {
        Debug.Log($"�N���b�N���ꂽ�^�C���F{clickedTile.GridPosition},�n�`�F{clickedTile.TerrainType}");
        
        if(_selectedUnit != null)
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

                if(path != null && path.Count > 0)
                {
                    ShowPathLine(path);
                    StartCoroutine(MoveUnitAlogPath(_selectedUnit, path));
                }
                else
                {
                    Debug.LogWarning("�o�H��������Ȃ����A�v�Z���ꂽ�o�H����ł��B");
                }
            }
            Debug.Log("�ړ��s�\�ȃ^�C�����N���b�N����܂����B");

            //���i�K�ł͈ړ���I����Ԃ���������悤�ɂ���
            //_selectdUnit.SetSelected(false);
            //_selectdUnit = null;

            //�ړ��͈͂̕\���̃N���A
            //ClearMovableRangeDisplay();
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

    //�ړ��͈͂̕\�����N���A����
    private void ClearMovableRangeDisplay()
    {
        //�S�Ẵ^�C�����f�t�H���g�ɖ߂�
        foreach(var highlight in _currentHighlights.Values)
        {
            Destroy(highlight);
        }
        _currentHighlights.Clear();
    }

    /// <summary>
    /// �o�H���C����\������
    /// </summary>
    /// <param name="path">�o�H�̃O���b�h���W���X�g</param>
    private void ShowPathLine(List<Vector2Int> path)
    {
        ClearPathLine();//�����̃��C�����N���A

        if(path == null || path.Count < 2)
        {
            return;
        }

        if(_pathLinePrefab == null)
        {
            Debug.LogWarning("Path Line Prefub���ݒ肳��Ă��܂���");
            return;
        }

        _currentPathLine = Instantiate(_pathLinePrefab, Vector3.zero, Quaternion.identity);
        _currentPathLine.transform.SetParent(transform);//MapManager�̎q�ɂ���

        //�o�H�̃O���b�h���W�����[���h���W�̃��X�g�ɕϊ�
        Vector3[] worldPoints = new Vector3[path.Count];
        for(int i = 0; i < path.Count; i++)
        {
            worldPoints[i] = GetWorldPosition(path[i]);
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
        if(_currentPathLine != null)
        {
            Destroy(_currentPathLine.gameObject);
            _currentPathLine = null;
        }
    }

    /// <summary>
    /// �ړ��\�͈͂��v�Z���A�n�C���C�g�\������
    /// </summary>
    /// <param name="unit">�ړ����郆�j�b�g</param>
    private void CalculateAndShowMovableRange(Unit unit)
    {
        ClearMovableRangeDisplay();
        ClearPathLine();

        if(unit == null || _movableHighlightPrefab == null)
        {
            return;
        }

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableNodes =
            DijkstraPathfinder.FindReachableTiles(unit.CurrentGridPosition, unit);

        //�v�Z���ꂽ�ړ��\�͈͂̃^�C�����n�C���C�g�\��
        foreach(var entry in reachableNodes)
        {
            Vector2Int highlightPos = entry.Key;

            GameObject highlightGO = Instantiate(_movableHighlightPrefab, GetWorldPosition(highlightPos), Quaternion.identity, transform);
            _currentHighlights.Add(highlightPos,highlightGO);
        }
    } 

    /// <summary>
    /// ���j�b�g���o�H�ɉ����Ĉړ�������
    /// </summary>
    /// <param name="unit">�ړ����郆�j�b�g</param>
    /// <param name="path">�ړ��o�H�̃O���b�h���W���X�g</param>
    private System.Collections.IEnumerator MoveUnitAlogPath(Unit unit,List<Vector2Int> path)
    {
        //�I�������ƃn�C���C�g�N���A�͈ړ��J�n���ɍs��2025/06
        if(_selectedUnit != null)
        {
            _selectedUnit.SetSelected(false);
            _selectedUnit = null;
        }
        ClearMovableRangeDisplay();


        float moveSpeed = 5.0f;//Unit�̌����ڂ̈ړ����x

        for(int i = 0; i < path.Count; i++)
        {
            Vector3 startWorldPos = unit.transform.position;
            Vector3 targetWorldPos = GetWorldPosition(path[i]);
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_mapSequence.Length > 0)
        {
            //GenerateMap(_mapSequence[_currentMapIndex]);
            GenerateMap(_mapSequence[0]);
            PlacePlayerUnitAtInitialPostiton();
        }
        else
        {
            Debug.LogError("MapManager:�}�b�v�V�[�P���X���ݒ肳��Ă��܂���");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2Int clickedGridPos = GetGridPositionFromWorld(mouseWorldPos);
        
        
        //�}�E�X��������m�i���N���b�N2025 / 06�j
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

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
