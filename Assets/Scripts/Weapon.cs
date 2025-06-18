using UnityEngine;

[System.Serializable]
public partial class Weapon
{
    [field: SerializeField] public string WeaponId { get; private set; }//����̃��j�b�gID
    [field: SerializeField] public string WeaponName { get; private set; }//���햼
    [field:SerializeField]public int AttackPower { get; private set; }//����З�
    [field:SerializeField]public int Range { get; private set; }//�U���˒�
    [field:SerializeField]public int HitRate { get; private set; }//������

    [field:SerializeField]public int CurrentWeaponExperience { get; private set; }//����o���l
    [field:SerializeField]public int CurrentWeaponLevel { get; private set; }//���탌�x��



    //�R���X�g���N�^
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
    /// ����o���l���l�����A���x���A�b�v������s��
    ///</summary>
    ///<param name="exp">�l���o���l</param>
    public void GainWeaponExperience(int exp)
    {
        CurrentWeaponExperience += exp;
        Debug.Log($"{WeaponId}{WeaponName}��{exp}����o���l���l���B���݌o���l�F{CurrentWeaponExperience}");

        if(CurrentWeaponExperience >= 100)
        {
            CurrentWeaponExperience = 0;
            WeaponLevelUp();
        }
    }

    ///<summary>
    ///���탌�x���A�b�v����
    ///</summary>
    private void WeaponLevelUp()
    {
        CurrentWeaponLevel++;
        //���̃X�e�[�^�X����
        AttackPower += 1;
        HitRate += 1;
        Debug.Log($"{WeaponId}{WeaponName}�����x���A�b�v�I���x��{CurrentWeaponLevel}�ɂȂ�܂���");
    }
}
