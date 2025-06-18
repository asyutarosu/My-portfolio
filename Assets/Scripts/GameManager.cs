using UnityEngine;
using UnityEngine.SceneManagement;//シーン管理のため導入
using System.Collections.Generic;//リストを使うため導入

/// <summary>
/// ゲームの各フェイズを定義する列挙型
/// </summary>
public enum GamePhase
{
    Title,          //タイトル画面
    StageSelect,    //ステージ選択
    Deployment,     //出撃フェイズ
    Battle,         //戦闘フェイズ
    StageClear,     //ステージクリア
    GameOver,       //ゲームオーバー
    Loading         //ロード中
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

    [SerializeField] private GamePhase _currentPhase;//現在のゲームフェイズ(Inspectorで確認用)
    public GamePhase CurrentPhase => _currentPhase;
    [SerializeField] private int _currentStageId;//現在のステージID(Inspectorで確認用)
    public int CurrentStageId => _currentStageId;

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


        ChangePhase(GamePhase.Title);//タイトル画面へ
    }

    /// <summary>
    /// フェイズを切り替える
    /// </summary>
    /// <param name="newPahse">新しいゲームフェイズ</param>
    public void ChangePhase(GamePhase newPhase)
    {
        Debug.Log($"GameManager:フェイズを{CurrentPhase}から{newPhase}へ変更します");

        //現在のフェイズの終了処理
        PhaseExit(_currentPhase);

        _currentPhase = newPhase;//フェイズを更新        

        //次のフェイズの開始処理
        PhaseEnter(_currentPhase);


        //各フェイズに応じた処理を記載する2025/06
        switch (newPhase)
        {
            case GamePhase.Title:
                break;
            case GamePhase.StageSelect:
                break;
            case GamePhase.Deployment:
                break;
            case GamePhase.Battle:
                break;
            case GamePhase.StageClear:
                break;
            case GamePhase.GameOver:
                break;
            case GamePhase.Loading:
                break;
            default:
                Debug.Log($"GameManager:未定義のゲームフェイズです{newPhase}");
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
    /// 現在のフェイズが終了する直前の処理
    /// </summary>
    private void PhaseExit(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Title:
                break;
            //他のフェイズ終了時の処理を記載2025/06
        }
    }

    /// <summary>
    /// 新しいフェイズが開始する直後の処理
    /// </summary>
    /// <param name="phase ">開始するフェイズ</param>
    private void PhaseEnter(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Title:
                break;
                //他のフェイズ終了時の処理
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
        
    }
}
