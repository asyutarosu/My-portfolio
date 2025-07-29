using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class EnemyPlacement
{
    public EnemyUnit enemyPrefab;//配置する敵ユニットのPrefabへの参照
    public Vector2Int gridPosition;//初期配置グリッド座標

    //現在はPrefabのステータスを参照する2025/07
    public int overrideHP = -1;//PrefabのHPを使用
    public int overrideMovement = -1;//Prefabの移動力を使用
}



[CreateAssetMenu(fileName = "NewEnemyEncounterData", menuName = "Scriptable Objects/Enemy Encounter Data")]
public class EnemyEncounterData : ScriptableObject
{
    public string mapId;// // この敵グループが紐づくマップのID
    public List<EnemyPlacement> enemyPlacements = new List<EnemyPlacement>();
}
