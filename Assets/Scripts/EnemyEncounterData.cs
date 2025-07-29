using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class EnemyPlacement
{
    public EnemyUnit enemyPrefab;//�z�u����G���j�b�g��Prefab�ւ̎Q��
    public Vector2Int gridPosition;//�����z�u�O���b�h���W

    //���݂�Prefab�̃X�e�[�^�X���Q�Ƃ���2025/07
    public int overrideHP = -1;//Prefab��HP���g�p
    public int overrideMovement = -1;//Prefab�̈ړ��͂��g�p
}



[CreateAssetMenu(fileName = "NewEnemyEncounterData", menuName = "Scriptable Objects/Enemy Encounter Data")]
public class EnemyEncounterData : ScriptableObject
{
    public string mapId;// // ���̓G�O���[�v���R�Â��}�b�v��ID
    public List<EnemyPlacement> enemyPlacements = new List<EnemyPlacement>();
}
