using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance { get; private set; }


    /// <summary>
    /// 仕様変更のため将棋ベースの戦闘処理を実装2025/07
    /// </summary>
    /// <param name="attacker">攻撃側のユニット</param>
    /// <param name="target">防衛側のユニット</param>
    public void ResolveBattle_ShogiBasetest(Unit attacker, Unit target)
    {
        Debug.LogError($"{attacker.gameObject.name}が{target.gameObject.name}に攻撃！");

        // 攻撃側が先に攻撃したため、相手を倒す
        target.Die();
    }

    /// <summary>
    /// ユニット間の戦闘処理（ステータスベース：仕様変更に伴い実装の見送り2025/07）
    /// </summary>
    /// <param name="attacker">攻撃側のユニット</param>
    /// <param name="target">防衛側のユニット</param>
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
