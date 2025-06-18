using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// �n�`�̎�ނ��`����񋓌^
/// </summary>
public enum TerrainType
{
    Plain,//����
    Forest,//�X
    Mountain,//�R
    Desert,//����
    Water,//����
    River,//��
    Snow,//�ϐ�
    Flooded,//���Q
    Landslide,//�y������
    Paved//�ܑ�
}


/// <summary>
/// �}�b�v��̊e�O���b�h�̏���ێ�����N���X
/// </summary>
public partial class Tile
{
    [field:SerializeField]public Vector2Int GridPosition { get; private set; }//�O���b�h�̍��W
    [field:SerializeField]public TerrainType Type { get; private set; }//�n�`�̎��
    [field:SerializeField]public Unit OccupyingUnit { get; private set; }//���̃O���b�h�ɑ��݂��郆�j�b�g

    //�R���X�g���N�^
    public Tile(Vector2Int position, TerrainType type)
    {
        GridPosition = position;
        Type = type;
        OccupyingUnit = null;//������Ԃł̓��j�b�g�͂��Ȃ�
    }

    /// <summary>
    /// ���̃^�C���Ƀ��j�b�g��ݒ肷��
    /// </summary>
    /// <param name="unit">�ݒ肷�郆�j�b�g</param>
    public void SetOccupyingUnit(Unit unit)
    {
        OccupyingUnit = unit;
    }

    /// <summary>
    /// ���̃^�C���̖h��{�[�i�X���擾
    /// </summary>
    /// c<return>�h��{�[�i�X</return>
    public int GetDefenseBonus()
    {
        switch (Type)
        {
            case TerrainType.Forest: return 1;
            case TerrainType.Mountain: return 1;
            case TerrainType.Desert: return 1;
            case TerrainType.Snow: return 1;
            //case TerrainType.Flooded: return -1;
            //case TerrainType.Landslide: return -1;
            default: return 0;
        }
    }

    /// <summary>
    /// ���̃^�C���̉���{�[�i�X���擾
    /// </summary>
    /// c<return>����{�[�i�X</return>
    public int GetEvadeBonus()
    {
        switch (Type)
        {
            case TerrainType.Forest: return 10;
            case TerrainType.Mountain: return 10;
            //case TerrainType.Desert: return -10;
            //case TerrainType.Snow: return -10;
            //case TerrainType.Flooded: return -10;
            //case TerrainType.Landslide: return -10;
            default: return 0;
        }
    }

    /// <summary>
    /// �^�C���̒n�`�^�C�v��ݒ肷��
    /// (Type�v���p�e�B��private�@set�̂��߁A�O������ύX���邽�߂̃��\�b�h)
    /// </summary>
    /// <param name="newType"></param>
    public void SetType(TerrainType newType)
    {
        Type = newType;
    }
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

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
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
        //DataManager����̃}�b�v�f�[�^�̎擾


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
        if(tile == null) return int.MinValue; //�͈͊O�͈ړ��s��

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



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
