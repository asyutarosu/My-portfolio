using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class EnemyEncounterManager : MonoBehaviour
{
    public static EnemyEncounterManager Instance { get; private set; }

    [SerializeField] private List<EnemyEncounterData> _allEnemyEncounters;// 全ての敵遭遇データをインスペクターから登録

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
            DontDestroyOnLoad(gameObject);//シーンを遷移でも破棄されないように
            InitializeEncounterData();
        }
    }

    private void InitializeEncounterData()
    {
        _encounterDataMap = _allEnemyEncounters.ToDictionary(data => data.mapId, data => data);
        Debug.Log($"EnemyEncounterManager: {_encounterDataMap.Count} 件の敵遭遇データをロードしました。");
    }

    /// <summary>
    /// 指定されたマップIDに対応する敵遭遇データを取得する
    /// </summary>
    /// <param name="mapId"></param>
    public EnemyEncounterData GetEnemyEncounterData(string mapId)
    {
        if(_encounterDataMap.TryGetValue(mapId, out EnemyEncounterData data))
        {
            return data;
        }
        Debug.LogWarning($"EnemyEncounterManager: マップID '{mapId}' に対応する敵遭遇データが見つかりません");
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
