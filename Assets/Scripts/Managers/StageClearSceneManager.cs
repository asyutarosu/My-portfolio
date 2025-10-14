using UnityEngine;

public class StageClearSceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //テスト用
        Debug.Log("ステージクリアシーンがロードされました。エンターキーを押してください");
    }

    // Update is called once per frame
    void Update()
    {
        //テスト用
        //エンターキーが押されたら
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log("エンターキーが押されました。タイトルシーンへ戻ります。");
            //SceneManager.LoadScene("Battle1");
            GameManager.Instance.ChangeState(GameState.Title);
        }
    }
}
