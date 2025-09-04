using UnityEngine;
using UnityEngine.SceneManagement;//�V�[���Ǘ��̂��ߓ���
using System.Collections.Generic;//���X�g���g�����ߓ���
using UnityEngine.UI;

/// <summary>
/// �Q�[���̊e�V�[�����`����񋓌^
/// </summary>
public enum GameState 
{ 
    Title,          //�^�C�g�����
    StageSelect,    //�X�e�[�W�I��
    Deployment,     //�o���t�F�C�Y
    Battle,         //�퓬�t�F�C�Y
    StageClear,     //�X�e�[�W�N���A
    GameOver,       //�Q�[���I�[�o�[
    Loading         //���[�h��
}

//�퓬�V�[���̃t�F�C�Y
//�퓬�V�[�����t�F�C�Y�����āi�o���t�F�C�Y�Ɛ퓬�t�F�C�Y�j����2025/07
public enum BattlePhase
{
    BattleDeployment,     //�헪�����t�F�C�Y
    BattleMain,         //�퓬���C���t�F�C�Y
}

//���Ƃ��Ď���
public enum GameMode
{
    PlacementMode,    //�z�u���[�h
    MapMode           //�}�b�v���[�h
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
                //�V�[����ɑ��݂��Ȃ��ꍇ�͒T��
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

    [SerializeField] private GameState _currentState;//���݂̃Q�[���t�F�C�Y(Inspector�Ŋm�F�p)
    public GameState CurrentState => _currentState;
    [SerializeField] private BattlePhase _currentBattlePhase;
    public BattlePhase CurrentBattlePhase => _currentBattlePhase;
    [SerializeField] private int _currentStageId;//���݂̃X�e�[�WID(Inspector�Ŋm�F�p)
    public int CurrentStageId => _currentStageId;

    /////
    [SerializeField]private GameMode _currentgameMode;//�z�u���[�h�ƃ}�b�v���[�h�̐؂�ւ��p
    public GameMode CurrentMode => _currentgameMode;

    [SerializeField] private GameObject _placementUI; //���F�z�uUI
    [SerializeField] private GameObject _placementUI2; //���F�z�uUI
    public string ButtonText = "";
    [SerializeField] private GameObject _placeModeUI;//���F�z�u���[�hUI

    //���F����
    //�z�u�������O���b�h���W�̃��X�g
    //ScriptableObject�ɂ��Q�ƂɕύX
    //[SerializeField]private List<Vector2Int> _placementPositions;
    //���ݔz�u����ꏊ�������C���f�b�N�X
    private int _currentPlacementIndex = 0;

    [SerializeField]private MapManager _mapManager;
    [SerializeField]private MapUnitPlacementData _mapUnitPlacementData;

    [SerializeField]private TurnManager _turnManager;

    //�X�N���v�g�C���X�^���X�����[�h���ꂽ�Ƃ��ɌĂяo�����
    void Awake()
    {
        if( _instance != null && _instance != this)
        {
            Destroy(this.gameObject);//���ɃC���X�^���X������Δj��
        }
        else
        {
            _instance = this;
            //�V�[���J�ڂ��Ă��j������Ȃ��悤�ɂ���
            //�Q�[���S�̂��Ǘ�����Mangaer�I�u�W�F�N�g�ɐݒ肷��
            DontDestroyOnLoad(this.gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();//�Q�[���S�̂̏��������J�n����
    }

    /// <summary>
    /// �Q�[���S�̂̏���������
    /// </summary>
    public void Initialize()
    {
        Debug.Log("GameManager:�Q�[���̏��������J�n���܂�");

        //�eManager�N���X�̏��������w��
        _currentBattlePhase = BattlePhase.BattleDeployment;
        _currentgameMode = GameMode.MapMode;

        _placementUI.SetActive(false);
        _placementUI2.SetActive(false);
        _placeModeUI.SetActive(false);


        //ChangeState(GameState.Title);//�^�C�g����ʂ�
    }


    /// <summary>
    /// �t�F�C�Y��؂�ւ���
    /// </summary>
    /// <param name="battlePhase">�V�����Q�[���t�F�C�Y</param>
    public void ChangePhase(BattlePhase battlePhase)
    {
        PhaseExit(_currentBattlePhase);

        _currentBattlePhase = battlePhase;

        PhaseEnter(_currentBattlePhase);

        //�e�t�F�C�Y�ɉ������������L�ڂ���2025/07
        switch (battlePhase)
        {
            case BattlePhase.BattleDeployment:
                break;
            case BattlePhase.BattleMain:
                break;
            default:
                Debug.Log($"GameManager:����`�̃Q�[���t�F�C�Y�ł�{battlePhase}");
                break;
        }
    }

    /// <summary>
    /// ���݂̃t�F�C�Y���I�����钼�O�̏���
    /// </summary>
    private void PhaseExit(BattlePhase phase)
    {
        switch (phase)
        {
            case BattlePhase.BattleMain:
                break;
                //���̃t�F�C�Y�I�����̏������L��2025/06
        }
    }

    /// <summary>
    /// �V�����t�F�C�Y���J�n���钼��̏���
    /// </summary>
    /// <param name="phase ">�J�n����t�F�C�Y</param>
    private void PhaseEnter(BattlePhase phase)
    {
        switch (phase)
        {
            case BattlePhase.BattleMain:
                break;
                //���̃t�F�C�Y�I�����̏���
        }
    }

    /// <summary>
    /// �V�[����؂�ւ���
    /// </summary>
    /// <param name="newPahse">�V�����Q�[���V�[��</param>
    public void ChangeState(GameState newPhase)
    {
        Debug.Log($"GameManager:�V�[����{CurrentState}����{newPhase}�֕ύX���܂�");

        //���݂̃V�[���̏I������
        StateExit(_currentState);

        _currentState = newPhase;//�V�[�����X�V        

        //���̃V�[���̊J�n����
        SceneEnter(_currentState);


        //�e�V�[���ɉ������������L�ڂ���2025/06
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
                Debug.Log($"GameManager:����`�̃Q�[���V�[���ł�{newPhase}");
                break;
        }
    }

    

    /// <summary>
    /// �V�[�������[�h����
    /// </summary>
    /// <param name="sceneName">���[�h����V�[����</param>
    public void LoadScene(string sceneName)
    {
        Debug.Log($"GameManager:�V�[��{sceneName}�����[�h���܂�");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// �Q�[�����Z�[�u����
    ///DataManager�ւ̌Ăяo��
    /// </summary>
    public void SaveGame()
    {
        Debug.Log("GameManager:�Q�[�����Z�[�u���܂�");
        //������2025/06
        Debug.Log("���݃Z�[�u�@�\�͖������ł�");
    }

    /// <summary>
    /// �Q�[�������[�h����
    /// DataManager�ւ̌Ăяo��
    /// </summary>
    public void LoadGame()
    {
        Debug.Log("GameManager:�Q�[�������[�h���܂�");
        //������2025/06
        Debug.Log("���݃��[�h�@�\�͖������ł�");
    }

   

    /// <summary>
    /// ���݂̃V�[�����I�����钼�O�̏���
    /// </summary>
    private void StateExit(GameState phase)
    {
        switch (phase)
        {
            case GameState.Title:
                break;
                //���̃V�[���I�����̏������L��2025/06
        }
    }

    /// <summary>
    /// �V�����V�[�����J�n���钼��̏���
    /// </summary>
    /// <param name="phase ">�J�n����V�[��</param>
    private void SceneEnter(GameState phase)
    {
        switch (phase)
        {
            case GameState.Title:
                break;
                //���̃V�[���I�����̏���
        }
    }


    //���[�h�̕ύX������
    private void ToggleMode()
    {
        if(CurrentMode == GameMode.PlacementMode)
        {
            _currentgameMode = GameMode.MapMode;
            Debug.LogWarning("���[�h���}�b�v���[�h�ɐ؂�ւ��܂���");
            _placementUI.SetActive(false);
            _placementUI2.SetActive(false);
            _placeModeUI.SetActive(false);
        }
        else
        {
            _currentgameMode = GameMode.PlacementMode;
            _placementUI.SetActive(true);
            _placementUI2.SetActive(true);
            _placeModeUI.SetActive(true);
            Debug.LogWarning("���[�h��z�u���[�h�ɐ؂�ւ��܂���");
        }
    }

    //////���F�������\�b�h�Q

    //Button����e�L�X�g���擾����
    public string GetButtonText(Button targetButton)
    {
        // �{�^���̎q�I�u�W�F�N�g����TextMeshProUGUI�R���|�[�l���g���擾
        Text tmpText = targetButton.GetComponentInChildren<Text>();
        ButtonText = tmpText.text;
        if (tmpText != null)
        {
            return tmpText.text;
        }

        Debug.LogWarning("�w�肳�ꂽ�{�^����TextMeshProUGUI�R���|�[�l���g��������܂���B");
        return string.Empty;
    }

    //Button��������
    public void OnEnterPlacementMode(Button targetButton)
    {
        Text tmpText = targetButton.GetComponentInChildren<Text>();
        ButtonText = tmpText.text;
    }


    //�z�u���郆�j�b�g�̍��W���m�F�����j�b�g�̔z�u��MapManager�ɒʒB
    public void PlaceNextUnit()
    {

        //�����ɕs����������̂ŃR�����g�A�E�g��
        //�z�u������W���c���Ă��邩�m�F
        //if (_currentPlacementIndex < _mapUnitPlacementData.placementPositions.Count)
        //{
        //    //���X�g���猻�݂̃O���b�h���W���擾
        //    Vector2Int targetPos = _mapUnitPlacementData.placementPositions[_currentPlacementIndex];

        //    //MapManager�o�R�Ń��j�b�g��z�u
        //    _mapManager.PlaceOrMoveUnit(targetPos);

        //    //���̔z�u�ꏊ�փC���f�b�N�X���X�V
        //    _currentPlacementIndex++;
        //}
        //else
        //{
        //    Debug.LogWarning("���ׂẴv���C���[���j�b�g��z�u���܂���");
        //}

        for(int i = 0;i < _mapUnitPlacementData.placementPositions.Count; i++)
        {
            Vector2Int targetPos = _mapUnitPlacementData.placementPositions[i];
            MyTile targetTile = _mapManager.GetTileAt(targetPos);

            //�}�X���󂢂Ă��邩�m�F
            if (targetTile != null && targetTile.OccupyingUnit == null)
            {
                //�󂢂Ă���}�X������������A�����Ƀ��j�b�g��z�u
                _mapManager.PlaceOrMoveUnit(targetPos);
                Debug.Log($"���j�b�g���󂫃}�X ({targetPos}) �ɔz�u���܂���");
                return;
            }

        }
        Debug.LogWarning("���ׂẴv���C���[���j�b�g��z�u���܂���");
    }

    


    /// <summary>
    /// �X�e�[�WID��ݒ肷��(�X�e�[�W�I���ȂǂŌĂ΂��z��)
    /// </summary>
    /// <param name="stageId">�ݒ肷��X�e�[�WID</param>
    public void SetCurrentStage(int stageId)
    {
        _currentStageId = stageId;
        Debug.Log($"GameManager:���݂̃X�e�[�WID��{stageId}�ɐݒ肵�܂���");

    }

    //�V�[��������Q�Ɛ�̃f�[�^�ԍ����擾
    public int GetCurrentStageNumber()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        //���݂̓V�[�������uBattle1�v�A�uBattle2�v�Ƃ������`���ō쐬
        string numberString = sceneName.Replace("Battle", "");

        //������𐮐��ɕϊ�
        if(int.TryParse(numberString, out int stagenumber))
        {
            return stagenumber;
        }

        //�t�H�[�}�b�g���������Ȃ��ꍇ�̓G���[��Ԃ����A�f�t�H���g�l��Ԃ�
        Debug.LogError("�V�[�������z�肳�ꂽ�`���ł͂���܂���i��: Battle1�j");
        return 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (CurrentBattlePhase == BattlePhase.BattleDeployment)
        {
            // �uC�v�L�[�������ꂽ�烂�[�h��؂�ւ���
            if (Input.GetKeyDown(KeyCode.C))
            {
                ToggleMode();
            }
        }
        else if (CurrentBattlePhase == BattlePhase.BattleMain)
        {
            _placementUI.SetActive(false);
            _placementUI2.SetActive(false);
        }
    }
}
