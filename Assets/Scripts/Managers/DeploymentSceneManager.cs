using UnityEngine;
using UnityEngine.SceneManagement;//シーン管理機能を使うために必要

public class DeploymentSceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //テスト用
        Debug.Log("戦闘準備シーンがロードされました。エンターキーを押してください");
    }

    // Update is called once per frame
    void Update()
    {
        //テスト用
        //エンターキーが押されたら
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log("エンターキーが押されました。戦闘シーンへ移行します。");
            SceneManager.LoadScene("Battle");
        }
    }
}
