using UnityEngine;

[System.Serializable]

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Scriptable Objects/Unit Data")]
public partial class UnitData : ScriptableObject
{
    [field:SerializeField]public string UnitId { get; private set; }
    [field: SerializeField] public string UnitName { get; private set; }
    [field: SerializeField] public UnitType Type { get; private set; }
    [field: SerializeField] public FactionType FactionType { get; private set; }
    [field: SerializeField]public EnemyAIType EnemyAIType { get; private set; } 
    [field: SerializeField] public int MaxHP { get; private set; }
    [field: SerializeField] public int BaseMovement { get; private set; }
    [field: SerializeField] public int BaseAttackPower { get; private set; }
    [field: SerializeField] public int BaseDefensePower { get; private set; }
    [field: SerializeField] public int BaseSkill { get; private set; }
    [field: SerializeField] public int BaseSpeed { get; private set; }
    [field: SerializeField] public int MinAttackRange { get; private set; }
    [field: SerializeField] public int MaxAttackRange { get; private set; }
}
