using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageDataContainer", menuName = "Scriptable Objects/StageDataContainer")]
public class StageDataContainer : ScriptableObject
{
    //���X�g�̃C���f�b�N�X���X�e�[�W�ԍ��ƑΉ�������
    public List<MapUnitPlacementData> mapplacementDataList;
    public List<EnemyEncounterData> enemyEncounterDataList;
}
