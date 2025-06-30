using UnityEngine;

public partial class Tile : MonoBehaviour
{


    [field: SerializeField] public Vector2Int GridPosition { get; private set; }//グリッドの座標
    [field: SerializeField] public TerrainType Type { get; private set; }//地形の種類
    [field: SerializeField] public Unit OccupyingUnit { get; private set; }//そのグリッドに存在するユニット

    private SpriteRenderer _spriteRenderer;

    //コンストラクタ
    public Tile(Vector2Int position, TerrainType type)
    {
        GridPosition = position;
        Type = type;
        OccupyingUnit = null;//初期状態ではユニットはいない
    }

    /// <summary>
    /// タイルの地形タイプを設定する
    /// (Typeプロパティがprivate　setのため、外部から変更するためのメソッド)
    /// </summary>
    /// <param name="newType"></param>
    public void SetType(TerrainType newType)
    {
        Type = newType;
    }

    /// <summary>
    /// このタイルにユニットを設定する
    /// </summary>
    /// <param name="unit">設定するユニット</param>
    public void SetOccupyingUnit(Unit unit)
    {
        OccupyingUnit = unit;
    }

    /// <summary>
    /// このタイルの防御ボーナスを取得
    /// </summary>
    /// c<return>防御ボーナス</return>
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
    /// このタイルの回避ボーナスを取得
    /// </summary>
    /// c<return>回避ボーナス</return>
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
            _spriteRenderer = GetComponent<SpriteRenderer>();//まだ取得していなければ取得
        }
        if(_spriteRenderer != null)
        {
            _spriteRenderer.sprite = sprite;//スプライトを設定
        }
        else
        {
            Debug.LogWarning($"Tile({GridPosition.x},{GridPosition.y}):SpriteRendererが見つかりません");
        }
    }

    public void Initialize(Vector2Int gridPos, TerrainType type)
    {
        GridPosition = gridPos;
        Type = type;
        OccupyingUnit=null;
        //GameObjectの名前をデバッグ用に設定
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
