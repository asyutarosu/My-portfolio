using UnityEngine;

public class PlayerUnit : MonoBehaviour
{

    [SerializeField] private int _moveRange = 3;//移動範囲

    private Vector2Int _currentPosition;//現在のグリッド座標

    //プロトタイプ用で簡易的な情報のみを記載2025/06
    private string _unitName;//ユニットの名前

    //プロトタイプ用で簡易的な情報のみを記載2025/06
    public int CurretHP { get; private set; } = 10;//仮のHP


    /// <summary>
    /// プレイヤーユニットを初期化
    /// </summary>
    /// <param name="initialGridPos">初期配置されるグリッド座標</param>
    /// <param name="name">ユニットの名前</param>
    public void Initialize(Vector2Int initialGridPos, string name)
    {
        _currentPosition = initialGridPos;
        _unitName = name;
        Debug.Log($"PlayerUnit'{_unitName}'initialized at grid:{_currentPosition}");
    }

    /// <summary>
    /// ユニットの現在のグリッド座標を更新する
    /// </summary>
    /// <param name="newGridPos">新しいグリッド座標</param>
    public void SetGridPosition(Vector2Int newGridPos)
    {
        _currentPosition = newGridPos;
    }

    /// <summary>
    /// ユニットの現在のグリッド座標を取得する
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetCurrentGridPostion()
    {
        return _currentPosition;
    }

    /// <summary>
    /// ユニットの移動範囲を取得する
    /// </summary>
    public int GetMoveRange()
    {
        return _moveRange;
    }

    //プロトタイプ用で現時点では移動のみを記載2025/06
    public void Attack(PlayerUnit target)
    {
        //攻撃ロジック
    }

    //デバッグ用：マウスでユニットの座標確認用
    private void OnMouseEnter()
    {
        Debug.Log($"Unit:{_unitName},GridPos:{_currentPosition}");
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
