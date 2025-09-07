using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageDataContainer", menuName = "Scriptable Objects/StageDataContainer")]
public class StageDataContainer : ScriptableObject
{
    //リストのインデックスをステージ番号と対応させる
    public List<MapUnitPlacementData> mapplacementDataList;
    public List<EnemyEncounterData> enemyEncounterDataList;
}
