using UnityEngine;
using System.Collections.Generic;
using System.Linq;//LINQ���g�p���邽��

/// <summary>
/// �퓬�V�X�e���𓝊�����V���O���g���N���X
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

    [SerializeField] private int _cuurrentTurn;//���݂̃^�[����
    public int CurrentTurn => _cuurrentTurn;
    [SerializeField] private Unit _activeUnit;//���ݍs�����̃��j�b�g
    [SerializeField] private List<Unit> _allUnits;//�}�b�v��̑S�Ẵ��j�b�g�̃��X�g

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
