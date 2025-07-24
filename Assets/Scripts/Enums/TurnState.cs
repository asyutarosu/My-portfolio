using UnityEngine;

/// <summary>
/// 現在のゲームのターン状態を定義する列挙型
/// </summary>
public enum TurnState
{
    PreGame,    //ゲーム開始前（初期化）
    PlayerTurn, //プレイヤーの行動フェイズ
    EnemyTurn,  //敵の行動フェイズ
    NeutralTurn,//中立ユニットの行動フェイズ
    PostTurn,   //ターン終了後のクリーンアップ
    Cutscene,   //イベントシーン用
    GameOver    //ゲーム終了
}
