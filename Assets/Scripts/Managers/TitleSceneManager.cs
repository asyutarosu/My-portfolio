using UnityEngine;
using UnityEngine.SceneManagement;//シーン管理機能を使うために必要

public class TitleSceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //テスト用
        Debug.Log("タイトルシーンがロードされました。エンターキーを押してください");
    }

    // Update is called once per frame
    void Update()
    {
        //テスト用
        //エンターキーが押されたら
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log("エンターキーが押されました。ステージ選択シーンへ移行します。");
            SceneManager.LoadScene("StageSelect");
        }
    }
}