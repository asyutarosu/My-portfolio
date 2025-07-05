using UnityEngine;


//ユニットのタイプを識別するEnum


public class PlayerUnit : Unit
{

    //[SerializeField] private int _moveRange = 3;//移動範囲
    //[SerializeField] private int _currentMovementPoints = 3;

    //private Vector2Int _currentPosition;//現在のグリッド座標

    //プロトタイプ用で簡易的な情報のみを記載2025/06
    //private string _unitName;//ユニットの名前
    //[SerializeField]private UnitType _unitType = UnitType.Infantry;
    //[SerializeField] private FactionType _factionUnitType = FactionType.Player;

    //プロトタイプ用で簡易的な情報のみを記載2025/06
    //public int CurretHP { get; private set; } = 10;//仮のHP


    //ユニットの選択状態を管理
    //private SpriteRenderer _spriteRenderer;
    //ユニットの選択状態と非選択状態
    //[SerializeField] private Color _selectedColor = Color.blue;//選択状態の色
    //[SerializeField] private Color _defaultColor = Color.white;//非選択状態の色

    //外部から移動力とタイプの参照用
    //public int MoveRange => _moveRange;
    //public int CurrentMovementPoints => _currentMovementPoints;
    //public UnitType Type => _unitType;

    //ユニットが占有しているタイルへの参照
    //public Tile OccupyingTile { get; private set; }

    /// <summary>
    /// プレイヤーユニットを初期化
    /// </summary>
    /// <param name="initialGridPos">初期配置されるグリッド座標</param>
    /// <param name="name">ユニットの名前</param>
    public override void Initialize(UnitData data)
    {
        //_currentPosition = initialGridPos;
        //_unitName = name;
        //Debug.Log($"PlayerUnit'{_unitName}'initialized at grid:{_currentPosition}");

        base.Initialize(data);

        _factionType = FactionType.Player;
        Debug.Log($"Player Unit '{UnitName}' (Type: {Type}, Faction: {Faction}) initialized at grid: {CurrentGridPosition}");
    }

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// ユニットの現在のグリッド座標を更新する
    /// </summary>
    /// <param name="newGridPos">新しいグリッド座標</param>
    //public void SetGridPosition(Vector2Int newGridPos)
    //{
    //    _currentPosition = newGridPos;
    //}

    /// <summary>
    /// ユニットの現在のグリッド座標を取得する
    /// </summary>
    /// <returns></returns>
    //public Vector2Int GetCurrentGridPostion()
    //{
    //    return _currentPosition;
    //}

    /// <summary>
    /// ユニットの移動範囲を取得する
    /// </summary>
    //public int GetMoveRange()
    //{
    //    return _moveRange;
    //}



    //プロトタイプ用で現時点では移動のみを記載2025/06
    //public void Attack(PlayerUnit target)
    //{
    //    //攻撃ロジック
    //}

    //ユニットの選択状態を設定する
    //public void SetSelected(bool isSelected)
    //{
    //    if(_spriteRenderer != null)
    //    {
    //        _spriteRenderer.color = isSelected ? _selectedColor : _defaultColor;
    //    }
    //}

    //private void UpdateOccupyingTile()
    //{
    //    if(MapManager.Instance != null)
    //    {
    //        //古いタイルからユニットを解除
    //        if(OccupyingTile != null)
    //        {
    //            OccupyingTile.OccupyingUnit = null;
    //        }
    //        //新しいタイルを設定し、ユニットを占有
    //        OccupyingTile = MapManager.Instance.GetTileAt(_currentPosition);
    //        if(OccupyingTile != null)
    //        {
    //            //OccupyingTile.OccupyingUnit = this;//このユニットがタイルを占有
    //        }
    //    }
    //}

    //private void OnDestroy()
    //{
    //    //ユニットが破壊されるときに、占有していたタイルから参照を解除
    //    if(OccupyingTile != null && OccupyingTile.OccupyingUnit == this)
    //    {
    //        OccupyingTile.OccupyingUnit = null;
    //    }
    //}


    //デバッグ用：マウスでユニットの座標確認用
    //private void OnMouseEnter()
    //{
    //    Debug.Log($"Unit:{_unitName},GridPos:{_currentPosition}");
    //}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
