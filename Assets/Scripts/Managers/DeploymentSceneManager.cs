using UnityEngine;
using UnityEngine.SceneManagement;//�V�[���Ǘ��@�\���g�����߂ɕK�v

public class DeploymentSceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //�e�X�g�p
        Debug.Log("�퓬�����V�[�������[�h����܂����B�G���^�[�L�[�������Ă�������");
    }

    // Update is called once per frame
    void Update()
    {
        //�e�X�g�p
        //�G���^�[�L�[�������ꂽ��
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log("�G���^�[�L�[��������܂����B�퓬�V�[���ֈڍs���܂��B");
            SceneManager.LoadScene("Battle");
        }
    }
}
