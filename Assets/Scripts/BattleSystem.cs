using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance { get; private set; }


    /// <summary>
    /// �d�l�ύX�̂��ߏ����x�[�X�̐퓬����������2025/07
    /// </summary>
    /// <param name="attacker">�U�����̃��j�b�g</param>
    /// <param name="target">�h�q���̃��j�b�g</param>
    public void ResolveBattle_ShogiBase(Unit attacker, Unit target)
    {
        Debug.LogError($"{attacker.gameObject.name}��{target.gameObject.name}�ɍU���I");

        // �U��������ɍU���������߁A�����|��
        target.Die();
    }

    /// <summary>
    /// ���j�b�g�Ԃ̐퓬�����i�X�e�[�^�X�x�[�X�F�d�l�ύX�ɔ��������̌�����2025/07�j
    /// </summary>
    /// <param name="attacker">�U�����̃��j�b�g</param>
    /// <param name="target">�h�q���̃��j�b�g</param>
    public void ResolveBattleSystem(Unit attacker,Unit target)
    {

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
