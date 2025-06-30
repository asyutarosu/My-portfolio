using UnityEngine;

public partial class Tile : MonoBehaviour
{


    [field: SerializeField] public Vector2Int GridPosition { get; private set; }//�O���b�h�̍��W
    [field: SerializeField] public TerrainType Type { get; private set; }//�n�`�̎��
    [field: SerializeField] public Unit OccupyingUnit { get; private set; }//���̃O���b�h�ɑ��݂��郆�j�b�g

    private SpriteRenderer _spriteRenderer;

    //�R���X�g���N�^
    public Tile(Vector2Int position, TerrainType type)
    {
        GridPosition = position;
        Type = type;
        OccupyingUnit = null;//������Ԃł̓��j�b�g�͂��Ȃ�
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

    public void SetSprite(Sprite sprite)
    {
        if(_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();//�܂��擾���Ă��Ȃ���Ύ擾
        }
        if(_spriteRenderer != null)
        {
            _spriteRenderer.sprite = sprite;//�X�v���C�g��ݒ�
        }
        else
        {
            Debug.LogWarning($"Tile({GridPosition.x},{GridPosition.y}):SpriteRenderer��������܂���");
        }
    }

    public void Initialize(Vector2Int gridPos, TerrainType type)
    {
        GridPosition = gridPos;
        Type = type;
        OccupyingUnit=null;
        //GameObject�̖��O���f�o�b�O�p�ɐݒ�
        gameObject.name = $"({gridPos.x},{gridPos.y})";
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
