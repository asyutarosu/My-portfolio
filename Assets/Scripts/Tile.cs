using UnityEngine;

public partial class Tile : MonoBehaviour
{


    [field: SerializeField] public Vector2Int GridPosition { get; private set; }//グリッドの座標
    [field: SerializeField] public TerrainType TerrainType { get; private set; }//地形の種類
    [field: SerializeField] public Unit OccupyingUnit { get; set; }//そのグリッドに存在するユニット種族
    [field:SerializeField] public bool IsGool { get; private set; }

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    //コンストラクタ
    public Tile(Vector2Int position, TerrainType terrainType)
    {
        GridPosition = position;
        TerrainType = terrainType;
        OccupyingUnit = null;//初期状態ではユニットはいない
    }

    /// <summary>
    /// タイルの地形タイプを設定する
    /// (Typeプロパティがprivate　setのため、外部から変更するためのメソッド)
    /// </summary>
    /// <param name="TerrainType"></param>
    public void SetType(TerrainType newTerrainType)
    {
        TerrainType = newTerrainType;
    }

    /// <summary>
    /// このタイルにユニットを設定する
    /// </summary>
    /// <param name="unit">設定するユニット</param>
    //public void SetOccupyingUnit(Unit unit)
    //{
    //    OccupyingUnit = unit;
    //}

    /// <summary>
    /// このタイルの防御ボーナスを取得
    /// </summary>
    /// c<return>防御ボーナス</return>
    public int GetDefenseBonus()
    {
        switch (TerrainType)
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
        switch (TerrainType)
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

    public void Initialize(Vector2Int gridPos, TerrainType terraintype,bool isGool)
    {
        GridPosition = gridPos;
        TerrainType = terraintype;
        IsGool = isGool;
        OccupyingUnit=null;
        //GameObjectの名前をデバッグ用に設定
        gameObject.name = $"({gridPos.x},{gridPos.y})";

        //SpriteRendererが見つからない場合は追加
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError($"Tile at {gridPos}:SpriteRendererが見つかりません");
            _spriteRenderer = GetComponent<SpriteRenderer>();

        }

        //BoxCollider2Dがなければ追加し、サイズを設定
        _boxCollider = gameObject.AddComponent<BoxCollider2D>();
        if (_boxCollider == null)
        {
            Debug.LogError($"Tile at {gridPos}:BoxCollider2Dが見つかりません");
            _boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        _boxCollider.size = new Vector2(1.0f,1.0f);
        _boxCollider.isTrigger = true;
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
