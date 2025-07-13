using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//ユニットのタイプを識別するEnum


public class PlayerUnit : Unit
{
    [SerializeField] private PlayerUnit _playerUnit;//対象のプレイヤーユニット
    private MapManager _tileHighlighter;//MapManagerのハイライト関連への参照

    public static PlayerUnit Instance;

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


    //ユニットが選択された場合に呼び出す
    public void OnUnitSelected()
    {
        _tileHighlighter.ClearAllHighlights();//既存のハイライトをクリア

        Dictionary<Vector2Int, DijkstraPathfinder.PathNode> reachableTiles = DijkstraPathfinder.FindReachableTiles(
            _playerUnit.GetCurrentGridPostion(),_playerUnit);

        foreach (Vector2Int pos in reachableTiles.Keys)
        {
            _tileHighlighter.HighlightTile(pos, HighlightType.Move);
        }

        //攻撃可能範囲を計算し、赤色でハイライト
        ShowAttackRangeHighlight(reachableTiles.Keys.ToList());

        //確認
        Debug.LogError("呼ばれたよ！");
    }

    public void OnUnitActionCompleted()
    {
        _tileHighlighter.ClearAllHighlights();//全てのハイライトをクリア
    }

    /// <summary>
    /// 攻撃可能範囲をハイライト表示する
    /// </summary>
    /// <param name="moveableTiles">移動可能なタイルリスト</param>
    private void ShowAttackRangeHighlight(List<Vector2Int> moveableTiles)
    {
        HashSet<Vector2Int> attackableTiles = new HashSet<Vector2Int>();
        Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0), };

        //各移動可能タイルから1マス隣接するタイルを攻撃可能範囲候補として追加
        foreach (Vector2Int movePos in moveableTiles)
        {
            foreach(Vector2Int dir in directions)
            {
                Vector2Int attackTargetPos = movePos + dir;
                //マップの範囲内か確認
                if (MapManager.Instance.IsValidGridPosition(attackTargetPos))
                {
                    attackableTiles.Add(attackTargetPos);
                }
            }
        }

        //敵ユニットが存在するタイルのみを赤色ハイライト
        foreach(Vector2Int targetPos in attackableTiles)
        {
            Tile targetTile = MapManager.Instance.GetTileAt(targetPos);
            if(targetTile != null && targetTile.OccupyingUnit != null)
            {
                //ユニットが敵factionTypeであるか確認
                if(targetTile.OccupyingUnit.Faction == FactionType.Enemy)
                {
                    _tileHighlighter.HighlightTile(targetPos,HighlightType.Attack);
                }
            }
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tileHighlighter = FindObjectOfType<MapManager>();//シーン内のMapManagerを取得
        if( _tileHighlighter == null)
        {
            Debug.LogError("MapManagerが見つかりません");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
