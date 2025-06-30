using UnityEngine;
using UnityEngine.SceneManagement;//�V�[���Ǘ��̂��ߓ���
using System.Collections.Generic;//���X�g���g�����ߓ���

/// <summary>
/// �Q�[���̊e�t�F�C�Y���`����񋓌^
/// </summary>
public enum GamePhase
{
    Title,          //�^�C�g�����
    StageSelect,    //�X�e�[�W�I��
    Deployment,     //�o���t�F�C�Y
    Battle,         //�퓬�t�F�C�Y
    StageClear,     //�X�e�[�W�N���A
    GameOver,       //�Q�[���I�[�o�[
    Loading         //���[�h��
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

    [SerializeField] private GamePhase _currentPhase;//���݂̃Q�[���t�F�C�Y(Inspector�Ŋm�F�p)
    public GamePhase CurrentPhase => _currentPhase;
    [SerializeField] private int _currentStageId;//���݂̃X�e�[�WID(Inspector�Ŋm�F�p)
    public int CurrentStageId => _currentStageId;

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


        ChangePhase(GamePhase.Title);//�^�C�g����ʂ�
    }

    /// <summary>
    /// �t�F�C�Y��؂�ւ���
    /// </summary>
    /// <param name="newPahse">�V�����Q�[���t�F�C�Y</param>
    public void ChangePhase(GamePhase newPhase)
    {
        Debug.Log($"GameManager:�t�F�C�Y��{CurrentPhase}����{newPhase}�֕ύX���܂�");

        //���݂̃t�F�C�Y�̏I������
        PhaseExit(_currentPhase);

        _currentPhase = newPhase;//�t�F�C�Y���X�V        

        //���̃t�F�C�Y�̊J�n����
        PhaseEnter(_currentPhase);


        //�e�t�F�C�Y�ɉ������������L�ڂ���2025/06
        switch (newPhase)
        {
            case GamePhase.Title:
                LoadScene("Title");
                break;
            case GamePhase.StageSelect:
                LoadScene("StageSelect");
                break;
            case GamePhase.Deployment:
                LoadScene("Deployment");
                break;
            case GamePhase.Battle:
                LoadScene("Battle");
                break;
            case GamePhase.StageClear:
                LoadScene("StageClear");
                break;
            case GamePhase.GameOver:
                LoadScene("GameOver");
                break;
            case GamePhase.Loading:
                LoadScene("Loading");
                break;
            default:
                Debug.Log($"GameManager:����`�̃Q�[���t�F�C�Y�ł�{newPhase}");
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
    /// ���݂̃t�F�C�Y���I�����钼�O�̏���
    /// </summary>
    private void PhaseExit(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Title:
                break;
            //���̃t�F�C�Y�I�����̏������L��2025/06
        }
    }

    /// <summary>
    /// �V�����t�F�C�Y���J�n���钼��̏���
    /// </summary>
    /// <param name="phase ">�J�n����t�F�C�Y</param>
    private void PhaseEnter(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Title:
                break;
                //���̃t�F�C�Y�I�����̏���
        }
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
