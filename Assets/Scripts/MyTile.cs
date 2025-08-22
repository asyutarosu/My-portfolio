using UnityEngine;
using UnityEngine.Tilemaps;

public partial class MyTile : MonoBehaviour
{


    [field: SerializeField] public Vector2Int GridPosition { get; private set; }//グリッドの座標
    [field: SerializeField] public TerrainType TerrainType { get; private set; }//地形の種類
    [field: SerializeField] public int MovementCost { get; private set; }//地形の移動コスト
    [field: SerializeField] public TileBase tileBase { get; private set; }

    //[field: SerializeField] public Unit OccupyingUnit { get; set; }//そのグリッドに存在するユニット種族
    //確認用として一部宣言の追加及びログの追加
    private Unit _occupyingUnit;
    public Unit OccupyingUnit//プロパティにログを追加
    {
        get { return _occupyingUnit; }
        set
        {
            //値が実際に変更される場合のみログを出力
            if(_occupyingUnit != value)
            {
                Debug.Log($"Tile {GridPosition}: OccupyingUnit changed from " +
                          $"{(_occupyingUnit != null ? _occupyingUnit.UnitName : "None")} to " +
                          $"{(value != null ? value.UnitName : "None")}");
                _occupyingUnit = value;
            }
        }
    }


    [field:SerializeField] public bool IsGool { get; private set; }

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    //コンストラクタ
    public MyTile(Vector2Int position, TerrainType terrainType)
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


    public void SetTerrainTypeAndCost(TerrainType newType, int newCost)
    {
        TerrainType = newType;
        MovementCost = newCost;
        Debug.Log($"Tile {GridPosition}: TerrainTypeを{TerrainType}に変更し、移動コストを{MovementCost}に設定");
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

    public void Initialize(Vector2Int gridPos, TerrainType terraintype,bool isGool, int movementCost)
    {
        GridPosition = gridPos;
        TerrainType = terraintype;
        IsGool = isGool;
        OccupyingUnit=null;
        MovementCost = movementCost;
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
