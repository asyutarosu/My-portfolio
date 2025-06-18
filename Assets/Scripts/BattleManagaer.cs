using UnityEngine;
using System.Collections.Generic;
using System.Linq;//LINQを使用するため

/// <summary>
/// 戦闘システムを統括するシングルトンクラス
/// </summary>
public partial class BattleManager : MonoBehaviour
{
    private static BattleManager _instanse;
    public static BattleManager Instance
    { 
        get 
        { 
            if(_instanse == null)
            {
                _instanse = FindAnyObjectByType<BattleManager>();
                if(_instanse == null)
                {
                    GameObject singletonObject = new GameObject("BatteleManager");
                    _instanse = singletonObject.AddComponent<BattleManager>();
                }
            }
            return _instanse; 
        } 
    }

    [SerializeField] private int _cuurrentTurn;//現在のターン数
    public int CurrentTurn => _cuurrentTurn;
    [SerializeField] private Unit _activeUnit;//現在行動中のユニット
    [SerializeField] private List<Unit> _allUnits;//マップ上の全てのユニットのリスト

    private void Awake()
    {
        if(_instanse != null && _instanse != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instanse = this;
        }
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
