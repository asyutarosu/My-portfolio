using UnityEngine;
using UnityEngine.SceneManagement;//シーン管理のため導入
using System.Collections.Generic;//リストを使うため導入
using UnityEngine.UI;

/// <summary>
/// ゲームの各シーンを定義する列挙型
/// </summary>
public enum GameState 
{ 
    Title,          //タイトル画面
    StageSelect,    //ステージ選択
    Deployment,     //出撃フェイズ
    Battle,         //戦闘フェイズ
    StageClear,     //ステージクリア
    GameOver,       //ゲームオーバー
    Loading         //ロード中
}

//戦闘シーンのフェイズ
//戦闘シーンをフェイズ化して（出撃フェイズと戦闘フェイズ）統合2025/07
public enum BattlePhase
{
    BattleDeployment,     //戦略準備フェイズ
    BattleMain,         //戦闘メインフェイズ
}

//仮として実装
public enum GameMode
{
    PlacementMode,    //配置モード
    MapMode           //マップモード
}

public partial class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                //シーン上に存在しない場合は探す
                _instance = FindObjectOfType<GameManager>();
                if(_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager");
                    _instance = singletonObject.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private GameState _currentState;//現在のゲームフェイズ(Inspectorで確認用)
    public GameState CurrentState => _currentState;
    [SerializeField] private BattlePhase _currentBattlePhase;
    public BattlePhase CurrentBattlePhase => _currentBattlePhase;
    [SerializeField] private int _currentStageId;//現在のステージID(Inspectorで確認用)
    public int CurrentStageId => _currentStageId;

    /////
    [SerializeField]private GameMode _currentgameMode;//配置モードとマップモードの切り替え用
    public GameMode CurrentMode => _currentgameMode;

    [SerializeField] private GameObject _placementUI; //仮：配置UI
    public string ButtonText = "";
    [SerializeField] private GameObject _placeModeUI;//仮：配置モードUI

    //仮：実装
    //配置したいグリッド座標のリスト
    //ScriptableObjectによる参照に変更
    //[SerializeField]private List<Vector2Int> _placementPositions;
    //現在配置する場所を示すインデックス
    private int _currentPlacementIndex = 0;

    [SerializeField]private MapManager _mapManager;
    [SerializeField]private MapUnitPlacementData _mapUnitPlacementData;

    [SerializeField]private TurnManager _turnManager;

    //スクリプトインスタンスがロードされたときに呼び出される
    void Awake()
    {
        if( _instance != null && _instance != this)
        {
            Destroy(this.gameObject);//既にインスタンスがあれば破棄
        }
        else
        {
            _instance = this;
            //シーン遷移しても破棄されないようにする
            //ゲーム全体を管理するMangaerオブジェクトに設定する
            DontDestroyOnLoad(this.gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();//ゲーム全体の初期化を開始する
    }

    /// <summary>
    /// ゲーム全体の初期化処理
    /// </summary>
    public void Initialize()
    {
        Debug.Log("GameManager:ゲームの初期化を開始します");

        //各Managerクラスの初期化を指示
        _currentBattlePhase = BattlePhase.BattleDeployment;
        _currentgameMode = GameMode.MapMode;

        _placementUI.SetActive(false);
        _placeModeUI.SetActive(false);


        //ChangeState(GameState.Title);//タイトル画面へ
    }


    /// <summary>
    /// フェイズを切り替える
    /// </summary>
    /// <param name="battlePhase">新しいゲームフェイズ</param>
    public void ChangePhase(BattlePhase battlePhase)
    {
        PhaseExit(_currentBattlePhase);

        _currentBattlePhase = battlePhase;

        PhaseEnter(_currentBattlePhase);

        //各フェイズに応じた処理を記載する2025/07
        switch (battlePhase)
        {
            case BattlePhase.BattleDeployment:
                break;
            case BattlePhase.BattleMain:
                break;
            default:
                Debug.Log($"GameManager:未定義のゲームフェイズです{battlePhase}");
                break;
        }
    }

    /// <summary>
    /// 現在のフェイズが終了する直前の処理
    /// </summary>
    private void PhaseExit(BattlePhase phase)
    {
        switch (phase)
        {
            case BattlePhase.BattleMain:
                break;
                //他のフェイズ終了時の処理を記載2025/06
        }
    }

    /// <summary>
    /// 新しいフェイズが開始する直後の処理
    /// </summary>
    /// <param name="phase ">開始するフェイズ</param>
    private void PhaseEnter(BattlePhase phase)
    {
        switch (phase)
        {
            case BattlePhase.BattleMain:
                break;
                //他のフェイズ終了時の処理
        }
    }

    /// <summary>
    /// シーンを切り替える
    /// </summary>
    /// <param name="newPahse">新しいゲームシーン</param>
    public void ChangeState(GameState newPhase)
    {
        Debug.Log($"GameManager:シーンを{CurrentState}から{newPhase}へ変更します");

        //現在のシーンの終了処理
        StateExit(_currentState);

        _currentState = newPhase;//シーンを更新        

        //次のシーンの開始処理
        SceneEnter(_currentState);


        //各シーンに応じた処理を記載する2025/06
        switch (newPhase)
        {
            case GameState.Title:
                LoadScene("Title");
                break;
            case GameState.StageSelect:
                LoadScene("StageSelect");
                break;
            case GameState.Deployment:
                LoadScene("Deployment");
                break;
            case GameState.Battle:
                LoadScene("Battle");
                break;
            case GameState.StageClear:
                LoadScene("StageClear");
                break;
            case GameState.GameOver:
                LoadScene("GameOver");
                break;
            case GameState.Loading:
                LoadScene("Loading");
                break;
            default:
                Debug.Log($"GameManager:未定義のゲームシーンです{newPhase}");
                break;
        }
    }

    

    /// <summary>
    /// シーンをロードする
    /// </summary>
    /// <param name="sceneName">ロードするシーン名</param>
    public void LoadScene(string sceneName)
    {
        Debug.Log($"GameManager:シーン{sceneName}をロードします");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// ゲームをセーブする
    ///DataManagerへの呼び出し
    /// </summary>
    public void SaveGame()
    {
        Debug.Log("GameManager:ゲームをセーブします");
        //未実装2025/06
        Debug.Log("現在セーブ機能は未実装です");
    }

    /// <summary>
    /// ゲームをロードする
    /// DataManagerへの呼び出し
    /// </summary>
    public void LoadGame()
    {
        Debug.Log("GameManager:ゲームをロードします");
        //未実装2025/06
        Debug.Log("現在ロード機能は未実装です");
    }

   

    /// <summary>
    /// 現在のシーンが終了する直前の処理
    /// </summary>
    private void StateExit(GameState phase)
    {
        switch (phase)
        {
            case GameState.Title:
                break;
                //他のシーン終了時の処理を記載2025/06
        }
    }

    /// <summary>
    /// 新しいシーンが開始する直後の処理
    /// </summary>
    /// <param name="phase ">開始するシーン</param>
    private void SceneEnter(GameState phase)
    {
        switch (phase)
        {
            case GameState.Title:
                break;
                //他のシーン終了時の処理
        }
    }


    //モードの変更をする
    private void ToggleMode()
    {
        if(CurrentMode == GameMode.PlacementMode)
        {
            _currentgameMode = GameMode.MapMode;
            Debug.LogWarning("モードをマップモードに切り替えました");
            _placementUI.SetActive(false);
            _placeModeUI.SetActive(false);
        }
        else
        {
            _currentgameMode = GameMode.PlacementMode;
            _placementUI.SetActive(true);
            _placeModeUI.SetActive(true);
            Debug.LogWarning("モードを配置モードに切り替えました");
        }
    }

    //////仮：実装メソッド群

    //Buttonからテキストを取得する
    public string GetButtonText(Button targetButton)
    {
        // ボタンの子オブジェクトからTextMeshProUGUIコンポーネントを取得
        Text tmpText = targetButton.GetComponentInChildren<Text>();
        ButtonText = tmpText.text;
        if (tmpText != null)
        {
            return tmpText.text;
        }

        Debug.LogWarning("指定されたボタンにTextMeshProUGUIコンポーネントが見つかりません。");
        return string.Empty;
    }

    //Button押下処理
    public void OnEnterPlacementMode(Button targetButton)
    {
        Text tmpText = targetButton.GetComponentInChildren<Text>();
        ButtonText = tmpText.text;
    }


    //配置するユニットの座標を確認しユニットの配置をMapManagerに通達
    public void PlaceNextUnit()
    {
        //配置する座標が残っているか確認
        if (_currentPlacementIndex < _mapUnitPlacementData.placementPositions.Count)
        {
            //リストから現在のグリッド座標を取得
            Vector2Int targetPos = _mapUnitPlacementData.placementPositions[_currentPlacementIndex];

            //MapManager経由でユニットを配置
            _mapManager.PlaceOrMoveUnit(targetPos);

            //次の配置場所へインデックスを更新
            _currentPlacementIndex++;
        }
        else
        {
            Debug.LogWarning("すべてのプレイヤーユニットを配置しました");
        }
    }




    /// <summary>
    /// ステージIDを設定する(ステージ選択などで呼ばれる想定)
    /// </summary>
    /// <param name="stageId">設定するステージID</param>
    public void SetCurrentStage(int stageId)
    {
        _currentStageId = stageId;
        Debug.Log($"GameManager:現在のステージIDを{stageId}に設定しました");

    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentBattlePhase == BattlePhase.BattleDeployment)
        {
            // 「C」キーが押されたらモードを切り替える
            if (Input.GetKeyDown(KeyCode.C))
            {
                ToggleMode();
            }
        }
        else if (CurrentBattlePhase == BattlePhase.BattleMain)
        {
            _placementUI.SetActive(false);
        }
    }
}
