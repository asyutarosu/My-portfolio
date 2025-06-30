using UnityEngine;
using System.Collections.Generic;


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

    [field: SerializeField] private Vector2Int _gridSize;//�}�b�v�̃O���b�h�T�C�Y
    public Vector2Int GridSize => _gridSize;
    [SerializeField]private Tile[,] _tileGrid;//�e�O���b�h�̏����i�[����2�����z��(Inspector�\���s�̂���[SerializeField]�͖���)

    //�}�b�v�쐬�p
    [SerializeField] private string[] _mapSequence;//Resouces�t�H���_�ȉ���CSV�t�@�C���̃p�X
    [SerializeField] private GameObject _tilePrefab;//�e�^�C�������Ɏg�p����Tile�R���|�[�l���g���t����Prefab
    [SerializeField] private float _tileSize = 1.0f;//�O���b�h��1�}�X������̃��[���h���W�ł̃T�C�Y
    [SerializeField] private Sprite[] _terrainSprite;//�n�`�^�C�v�ɑΉ�����X�v���C�g��ݒ肷�邽�߂̔z��

    private int _currentMapIndex = 0;//���݃��[�h���Ă���}�b�v�̃C���f�b�N�X
    private MapData _currentmapData;//MapDtaLoader�ɂ���ēǂݍ��܂��}�b�v�f�[�^

    private Dictionary<Vector2Int ,Tile> _tiles = new Dictionary<Vector2Int, Tile>();//�������ꂽ�S�Ă�Tile�I�u�W�F�N�g�ƃO���b�h���W���Ǘ�����

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

    /// <summary>
    /// �X�e�[�WID�Ɋ�Â��ă}�b�v�f�[�^�����[�h���ATileGrid�𐶐�����
    /// </summary>
    /// <param name="stageId">���[�h����X�e�[�WID</param>
    public void LoadMap(int stageId)
    {

        Debug.Log($"MapManager:�X�e�[�W{stageId}�̃}�b�v�����[�h���܂�");

        //_mapCsvFilesPath = new string "";

        //���}�b�v�𐶐�
        _gridSize = new Vector2Int(10, 10);
        _tileGrid = new Tile[_gridSize.x, _gridSize.y];

        for (int y = 0; y < _gridSize.y; y++)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                //�n�`�^�C�v��ݒ肷�郍�W�b�N�i�}�b�v�f�[�^����̓ǂݍ��݁j
                //���݂͉��̃}�b�v�𐶐�����@2025/06
                TerrainType type = TerrainType.Plain;
                if(x == 5 && y == 5) { type = TerrainType.Forest; }//���̐X
                if(x== 0 || y == 0 || x == _gridSize.x -1 || y == _gridSize.y -1) { type = TerrainType.Water; }//���̐���@�O��
                if(x % 2 == 0 && y % 2 == 0) { type = TerrainType.Desert; }//���̍���
                _tileGrid[y, x] = new Tile(new Vector2Int(x, y), type);
            }
        }
        Debug.Log($"MapManager:�}�b�v�T�C�Y{_gridSize.x}�~{_gridSize.y}�Ő���");

        //���F�^�C���̎��o�I�ɕ\�����鏈���֌W
    }

    /// <summary>
    /// �}�b�v�f�[�^�Ɋ�Â��ă}�b�v�𐶐�����
    /// </summary>
    public void GenerateMap(string mapPath)
    {
        ClearMap();//���Ƀ}�b�v����������Ă���\�����l�����āA��x�N���A����

        //MapDataLoader��CSV�t�@�C������}�b�v�f�[�^��ǂݍ���
        _currentmapData = MapDataLoader.LoadMapDataFromCSV(mapPath);

        if(_currentmapData == null)
        {
            Debug.LogError("MapManager:�}�b�v�f�[�^�̓ǂݍ��݂Ɏ��s���܂����B�}�b�v�����ł��܂���");
            return;
        }

        for(int y = 0; y < _currentmapData.Height; y++)
        {
            for(int x = 0; x < _currentmapData.Width; x++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);//���݂̃O���b�h���W
                TerrainType terrainType = _currentmapData.GetTerrainType(gridPos);//���̍��W�̒n�`�^�C�v���擾

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
                tile.Initialize(gridPos, terrainType);

                //�n�`�^�C�v�ɉ������X�v���C�g��Tile�ɐݒ�
                SetTileSprite(tile,terrainType);

                //�����E����������������Tile�I�u�W�F�N�g����Ō����ł���悤��
                _tiles.Add(gridPos,tile);
            }
        }
        Debug.Log($"MapManager:�}�b�v�𐶐����܂���({_currentmapData.Width}x{_currentmapData.Height})");
    }


    /// <summary>
    /// �w�肳�ꂽ�O���b�h���W�̃^�C�������擾����
    /// </summary>
    /// <param name="position">�O���b�h���W</param>
    /// <return>Tile�I�u�W�F�N�g�A�͈͊O�Ȃ�null</return>
    public Tile GetTileAt(Vector2Int position)
    {
        if(position.x >= 0 && position.x <= _gridSize.x && position.y >= 0 && position.y < _gridSize.y)
        {
            return _tileGrid[position.x, position.y];
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
        switch (tile.Type)
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
            Debug.Log($"MapManager:{position}�̒n�`��{tile.Type}����{newType}�ω����܂���");
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
        return new Vector3(gridPos.x * _tileSize, gridPos.y * _tileSize, 0);
    }

    ///<summary>
    /// ���[���h���W���O���b�h���W�ɕϊ�����
    /// </summary>
    /// <param name="worldPos">�ϊ����������[���h���W</param>
    /// <return>�Ή�����O���b�h���W</return>
    public Vector2Int GetGridPositio(Vector3 worldPos)
    {
        //���[���h���W���^�C���T�C�Y�Ŋ���A�l�̌ܓ����Đ����O���b�h���W�ɂ���
        int x = Mathf.RoundToInt(worldPos.x / _tileSize);
        int y = Mathf.RoundToInt(worldPos.y / _tileSize);
        return new Vector2Int(x, y);
    }

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
        _currentmapData = null; //�ǂݍ��񂾃}�b�v�f�[�^���N���A
        Debug.Log("MapManager:�����̃}�b�v���N���A���܂���");
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_mapSequence.Length > 0)
        {
            //GenerateMap(_mapSequence[_currentMapIndex]);
            GenerateMap(_mapSequence[0]);
        }
        else
        {
            Debug.LogError("MapManager:�}�b�v�V�[�P���X���ݒ肳��Ă��܂���");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
