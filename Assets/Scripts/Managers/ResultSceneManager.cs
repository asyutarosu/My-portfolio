using UnityEngine;
using UnityEngine.SceneManagement;//�V�[���Ǘ��@�\���g�����߂ɕK�v

public class ResultSceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //�e�X�g�p
        Debug.Log("���U���g�V�[�������[�h����܂����B�G���^�[�L�[�������Ă�������");
    }

    // Update is called once per frame
    void Update()
    {
        //�e�X�g�p
        //�G���^�[�L�[�������ꂽ��
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log("�G���^�[�L�[��������܂����B�^�C�g���V�[���ֈڍs���܂��B");
            Debug.LogWarning("�V�[�P���X��������܂����I�I");
            //SceneManager.LoadScene("Title");
            GameManager.Instance.ChangeState(GameState.Title);
        }
    }
}
