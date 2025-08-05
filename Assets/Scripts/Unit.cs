using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// ユニットの特性を定義する列挙型
/// </summary>
public enum UnitType
{
    Infantry,//歩兵
    Aquatic,//水棲
    Flying,//飛行
    Cavalry,//騎馬
    Heavy,//重兵
    Archer,//弓
    Mountain//山賊
}

public partial class Unit : MonoBehaviour
{
    [field:SerializeField]public string UnitId { get;private set; }//ユニットのユニークID
    [field:SerializeField]public string UnitName { get; private set; }//ユニット名
    [field:SerializeField]public UnitType Type { get; private set; }//ユニットタイプ
    [field: SerializeField] protected FactionType _factionType = FactionType.Player;//デフォルトはプレイヤー
    public FactionType Faction => _factionType;

    [field:SerializeField]public int CurrentHP { get; private set; }//現在のHP
    [field:SerializeField]public int MaxHP { get; private set; }//最大HP
    [field: SerializeField] public int BaseMovement { get; private set; }//基礎移動力
    [field: SerializeField] public int CurrentMovementPoints { get; private set; }//現在の移動力
    [field: SerializeField] public int AttackPower { get; private set; }//攻撃力
    [field: SerializeField] public int DefensePower { get; private set; }//防御力
    [field: SerializeField]public int Skill { get; private set; }//技

    [field: SerializeField]public int Speed { get; private set; }//速さ


    //攻撃範囲:仮実装
    [SerializeField] public int _minAttackRange = 1;//最小攻撃射程
    [SerializeField] public int _maxAttackRange = 2;//最大攻撃射程


    [field:SerializeField]protected UnitData unitData { get; private set; }//ユニットデータ取得用

    [field:SerializeField]public Weapon EquippedWeapon { get; private set; }//装備中の武器
    [field: SerializeField] public Vector2Int CurrentGridPosition { get; protected set; }//マップ上の現在のグリッド座標
    [field: SerializeField] public bool HasActedThisTurn { get; private set; }//今ターン行動済みか

    [field: SerializeField] public int CurrentExperience { get; private set; }//現在の経験値
    [field:SerializeField]public int CurrentLevel { get; private set; }//現在のレベル

    public Tile OccupyingTile { get; protected set; }

    //ユニットの選択状態を管理
    private SpriteRenderer _spriteRenderer;
    //ユニットの選択状態と非選択状態(デバッグ用)
    [SerializeField] private Color _selectedColor = Color.blue;//選択状態の色
    [SerializeField] private Color _defaultColor = Color.white;//非選択状態の色
    [SerializeField] private Color _actedColor = Color.gray;//行動済みの状態の色

    //ユニットの視覚的情報のための要素
    [SerializeField] private float _visualMoveSpeed = 5.0f;//視覚的移動速度
    


    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if( _spriteRenderer == null)
        {
            Debug.LogError($"{gameObject.name}:SpriteRendererが見つかりません");
        }

        //仮データ
        //UnitId = "NoId";

        //UnitDataからステータスを読み取る
        if(unitData != null)
        {
            UnitId = unitData.UnitId;
            UnitName  = unitData.UnitName;
            Type = unitData.Type;
            _factionType = unitData.FactionType;
            CurrentHP = unitData.MaxHP;
            MaxHP = unitData.MaxHP;
            BaseMovement = unitData.BaseMovement;
            CurrentMovementPoints = BaseMovement;
            AttackPower = unitData.BaseAttackPower;
            DefensePower = unitData.BaseDefensePower;

            Debug.Log($"{UnitName} (ID: {UnitId}) のステータスをUnitDataから設定しました。" +
                      $"HP: {CurrentHP}/{MaxHP}, 移動力: {BaseMovement}, 攻撃力: {AttackPower}, 防御力: {DefensePower}");
        }
        else
        {
            Debug.LogWarning($"{this.name} に UnitData が割り当てられていません！デフォルト値を使用します。");
            UnitId = "0";
            UnitName= "0";
            MaxHP = 1;
            BaseMovement = 1;
        }

        //デバッグ用
        if (string.IsNullOrEmpty(UnitId))
        {
            Debug.LogWarning($"Unit:{gameObject.name}にUnitDataが初期化されていません");
        }
    }

    /// <summary>
    /// ユニットを初期化する
    /// (DataManagerからロードされたUnitDataを使用)
    /// </summary>
    /// <param name="data">ユニットのマスターデータ</param>
    public virtual void Initialize(UnitData data)
    {
        UnitId = data.UnitId;
        UnitName = data.UnitName;
        Type = data.Type;
        MaxHP = data.MaxHP;
        CurrentHP = MaxHP;
        BaseMovement = data.BaseMovement;
        CurrentMovementPoints = BaseMovement;
        AttackPower = data.BaseAttackPower;
        DefensePower = data.BaseDefensePower;
        Skill = data.BaseSkill;
        Speed = data.BaseSpeed;

        CurrentExperience = 0;
        CurrentLevel = 1;
        HasActedThisTurn = false;



        //武器の初期化処理など
        //仮データ2025/06
        EquippedWeapon = new Weapon("SWORD001","仮装備",1,1,100);
    }

    /// <summary>
    /// ユニットのグリッド座標を更新する
    /// (BatteManagerから呼ばれる)
    /// </summary>
    /// <param name="newPosition">新しいグリッド座標</param>
    public void UpdatePosition(Vector2Int newPosition){
        //MapManagerに処理の指示する予定
        CurrentGridPosition = newPosition;
        UpdateOccupyingTile();
    }

    /// <summary>
    /// 移動力を消費する
    /// </summary>
    /// <param name="cost">消費する移動力</param>
    public void ConsumeMovementPoints(int cost)
    {
        CurrentMovementPoints -= cost;
        if (CurrentMovementPoints < 0)
        {
            CurrentMovementPoints = 0;
        }
        Debug.Log($"移動力を消費しました。残り：{CurrentMovementPoints}");
    }

    /// <summary>
    /// ユニットが指定されたコストで移動可能か判定
    /// </summary>
    /// <param name="cost">移動に必要なコスト</param>
    /// <returns>移動可能ならtrue</returns>
    public bool CanMove(int cost)
    {
        return CurrentMovementPoints >= cost && !HasActedThisTurn;
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damage">受けるダメージ</param>
    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if(CurrentHP <= 0)
        {
            CurrentHP = 0;
            Die();
        }
    }


    /// <summary>
    /// ユニットの死亡処理
    /// </summary>
    public void Die()
    {
        Debug.Log($"{UnitId}{UnitName}は倒れた");

        //BattleManagerへ指示をする予定
        OnDestroy();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 経験値を獲得し、レベルアップ判定を行う
    /// </summary>
    /// <param name="exp">獲得経験値</param>
    public void GainExperience(int exp)
    {
        CurrentExperience += exp;
        Debug.Log($"{UnitId}{UnitName}が{exp}経験値を獲得。現在経験値：{CurrentExperience}");
        if(CurrentExperience >= 100)
        {
            CurrentExperience = 0;
            LevelUp();
        }

        //連続レベルアップの判定のロジック（思案中）
        /*int requiredExpForNextLevel = CalulateRequiredExp(CurrentLevel + 1);
        while(CurrentExperience >= requiredExpForNextLevel)
        {
            LevelUp();
            requiredExpForNextLevel = CalulateRequiredExp(CurrentLevel + 1);
        }
        */
    }

    /// <summary>
    /// レベルアップ処理
    /// </summary>
    private void LevelUp()
    {
        CurrentLevel++;
        //仮のステータス成長
        MaxHP += 1;
        AttackPower += 1;
        DefensePower += 1;
        Skill += 1;
        Speed += 1;

        Debug.Log($"{UnitId}{UnitName}がレベルアップ！レベル{CurrentLevel}になりました");
    }

    /// <summary>
    /// 次のレベルに必要な経験値を計算する(思案中)
    /// </summary>
    /// <param name="level">計算対象のレベル</param>
    /// <return>必要な経験値</return>
    //private int CalculateRequiredExp(int level)
    //{
    //    return 0;
    //}


    /// <summary>
    /// ターン開始時に移動力と行動済みフラグをリセットする
    /// </summary>
    public void ResetAction()
    {
        HasActedThisTurn = false;
        CurrentMovementPoints = BaseMovement;
        SetSelected(false);

    }

    /// <summary>
    /// ユニットが行動を完了したことを設定する
    /// </summary>
    public void SetActedThisTrun()
    {
        HasActedThisTurn |= true;
        CurrentMovementPoints = 0;
        SetSelected(false );
        UpdateVisualColor();
    }

    /// <summary>
    /// 行動済みフラグを設定する
    /// </summary>
    /// <param name="acted">行動済みか</param>
    public void SetActionTaken(bool acted)
    {
        HasActedThisTurn = acted;
    }

    /// <summary>
    /// 移動ポイントを初期値にリセットする
    /// </summary>
    public void ResetMovementPoints()
    {
        CurrentMovementPoints = BaseMovement;
    }

    //占有タイルを更新する
    protected virtual void UpdateOccupyingTile()
    {
        if (MapManager.Instance != null)
        {
            //古いタイルからユニットを解除
            if (OccupyingTile != null)
            {
                OccupyingTile.OccupyingUnit = null;
            }

            //新しいタイルを設定し、ユニットを占有
            OccupyingTile = MapManager.Instance.GetTileAt(CurrentGridPosition);
            if (OccupyingTile != null)
            {
                OccupyingTile.OccupyingUnit = this;//このユニットがタイルを占有
            }
        }
    }

    //データ更新ように変更2025/07
    //プレイヤーが移動を確定した時のみ呼び出される
    //ユニットが移動を完了した際に、新しいタイルを占有し、古いタイルから解放する
    public void MoveToGridPosition(Vector2Int newGridPos, Tile newTile)
    {
        //古いタイルから参照を解除
        if(OccupyingTile != null && OccupyingTile.OccupyingUnit == this)
        {
            OccupyingTile.OccupyingUnit = null;
        }

        CurrentGridPosition = newGridPos;
        //視覚的情報の更新は別のメソッドで対応
        //transform.position = MapManager.Instance.GetWorldPositionFromGrid(newGridPos);

        //新しいタイルを設定し、占有する
        OccupyingTile = newTile;
        if(OccupyingTile != null)
        {
            OccupyingTile.OccupyingUnit = this;
        }
    }

    //視覚的情報を担当するメソッド
    /// <summary>
    /// ユニットを視覚的にパスに沿って移動させる。データ上の位置は変更しない
    /// </summary>
    /// <param name="path">移動経路のグリッド座標リスト</param>
    public IEnumerator AnimateMove(List<Vector2Int> path)
    {
        if(path == null || path.Count == 0)
        {
            yield break;
        }

        // ユニットの現在の視覚的な位置を、パスの開始点に設定
        transform.position = MapManager.Instance.GetWorldPositionFromGrid(path[0]);

        for(int i = 1; i < path.Count; i++)
        {
            Vector3 startPos = MapManager.Instance.GetWorldPositionFromGrid(path[i - 1]);
            Vector3 targetPos = MapManager.Instance.GetWorldPositionFromGrid(path[i]);
            float journeyLength = Vector3.Distance(startPos, targetPos);
            float startTime = Time.time;

            while(Vector3.Distance(transform.position, targetPos)> 0.01f)
            {
                float distCovered = (Time.time - startTime) * _visualMoveSpeed;
                float fractionOfJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(startPos, targetPos, fractionOfJourney);
                yield return null;
            }
            transform.position = targetPos;
        }
        Debug.Log($"{UnitName} の視覚的な移動が完了しました。");
    }



    /// <summary>
    /// ユニットの現在のグリッド座標を更新する
    /// </summary>
    /// <param name="newGridPos">新しいグリッド座標</param>
    public void SetGridPosition(Vector2Int newGridPos)
    {
        CurrentGridPosition = newGridPos;
    }

    /// <summary>
    /// ユニットの現在のグリッド座標を取得する
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetCurrentGridPostion()
    {
        return CurrentGridPosition;
    }

    /// <summary>
    /// ユニットの移動範囲を取得する
    /// </summary>
    public int GetMoveRange()
    {
        return CurrentMovementPoints;
    }

    //ユニットの行動状態の色を設定する
    public void SetSelected(bool isSelected)
    {
        if (_spriteRenderer != null)
        {
            //_spriteRenderer.color = isSelected ? _selectedColor : _defaultColor;

            if (isSelected)
            {
                _spriteRenderer.color = _selectedColor;
            }
            else if (HasActedThisTurn)
            {
                _spriteRenderer.color = _actedColor;
            }
            else
            {
                _spriteRenderer.color = _defaultColor;
            }
        }
    }

    //ユニットの色を更新する
    private void UpdateVisualColor()
    {
        if(_spriteRenderer != null)
        {
            if(HasActedThisTurn)
                _spriteRenderer.color = _actedColor;
        }
        else
        {
            _spriteRenderer.color= _defaultColor;
        }
    }

    protected void OnDestroy()
    {
        //ユニットが破壊されるときに、占有していたタイルから参照を解除
        if (OccupyingTile != null && OccupyingTile.OccupyingUnit == this)
        {
            OccupyingTile.OccupyingUnit = null;
        }
    }

    //デバッグ用：マウスでユニットの座標確認用
    protected virtual void OnMouseEnter()
    {
        Debug.Log($"Unit:{UnitName},GridPos:{CurrentGridPosition}");
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
