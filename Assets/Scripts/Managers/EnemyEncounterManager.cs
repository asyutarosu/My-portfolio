using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class EnemyEncounterManager : MonoBehaviour
{
    public static EnemyEncounterManager Instance { get; private set; }

    [SerializeField] private List<EnemyEncounterData> _allEnemyEncounters;// �S�Ă̓G�����f�[�^���C���X�y�N�^�[����o�^

    private Dictionary<string, EnemyEncounterData> _encounterDataMap;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);//�V�[����J�ڂł��j������Ȃ��悤��
            InitializeEncounterData();
        }
    }

    private void InitializeEncounterData()
    {
        _encounterDataMap = _allEnemyEncounters.ToDictionary(data => data.mapId, data => data);
        Debug.Log($"EnemyEncounterManager: {_encounterDataMap.Count} ���̓G�����f�[�^�����[�h���܂����B");
    }

    /// <summary>
    /// �w�肳�ꂽ�}�b�vID�ɑΉ�����G�����f�[�^���擾����
    /// </summary>
    /// <param name="mapId"></param>
    public EnemyEncounterData GetEnemyEncounterData(string mapId)
    {
        if(_encounterDataMap.TryGetValue(mapId, out EnemyEncounterData data))
        {
            return data;
        }
        Debug.LogWarning($"EnemyEncounterManager: �}�b�vID '{mapId}' �ɑΉ�����G�����f�[�^��������܂���");
        return null;
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
