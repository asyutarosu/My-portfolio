using UnityEngine;

[System.Serializable]
public partial class Weapon
{
    [field: SerializeField] public string WeaponId { get; private set; }//武器のユニットID
    [field: SerializeField] public string WeaponName { get; private set; }//武器名
    [field:SerializeField]public int AttackPower { get; private set; }//武器威力
    [field:SerializeField]public int Range { get; private set; }//攻撃射程
    [field:SerializeField]public int HitRate { get; private set; }//命中率

    [field:SerializeField]public int CurrentWeaponExperience { get; private set; }//武器経験値
    [field:SerializeField]public int CurrentWeaponLevel { get; private set; }//武器レベル



    //コンストラクタ
    public Weapon(string id,string name, int attack, int range,int hitRate)
    {
        WeaponId = id;
        WeaponName = name;
        AttackPower = attack;
        Range = range;
        HitRate = hitRate;
        CurrentWeaponExperience = 0;
        CurrentWeaponLevel = 1;
    }

    ///<summary>
    /// 武器経験値を獲得し、レベルアップ判定を行う
    ///</summary>
    ///<param name="exp">獲得経験値</param>
    public void GainWeaponExperience(int exp)
    {
        CurrentWeaponExperience += exp;
        Debug.Log($"{WeaponId}{WeaponName}が{exp}武器経験値を獲得。現在経験値：{CurrentWeaponExperience}");

        if(CurrentWeaponExperience >= 100)
        {
            CurrentWeaponExperience = 0;
            WeaponLevelUp();
        }
    }

    ///<summary>
    ///武器レベルアップ処理
    ///</summary>
    private void WeaponLevelUp()
    {
        CurrentWeaponLevel++;
        //仮のステータス成長
        AttackPower += 1;
        HitRate += 1;
        Debug.Log($"{WeaponId}{WeaponName}がレベルアップ！レベル{CurrentWeaponLevel}になりました");
    }
}
