using UnityEngine;
using System.IO;//ファイル操作のため
using System.Linq;//Splitで使用
using System.Collections.Generic;//Listで使用

public class MapDataLoader
{
    /// <summary>
    /// 指定されたCSVファイルからマップデータを読み込む
    /// </summary>
    /// <param name="fliePath">Assets/Resources/以下のファイルのパス</param>
    /// <retutns>読み込まれたMapDataオブジェクト。読み込みに失敗した場合はnulll</retutns>
    public static MapData LoadMapDataFromCSV(string filePath)
    {
        //CSVファイルはプロジェクトのAssets/Resources/フォルダ以下に配置
        TextAsset csvFile = Resources.Load<TextAsset>(filePath);

        if(csvFile == null)
        {
            Debug.LogError($"MapDataLoader:CSVファイルが見つかりません:Assets/Resources/{filePath}.csv");
            return null;
        }
        
        StringReader reader = new StringReader(csvFile.text);
        List<string[]> rows = new List<string[]>();
        string line;

        //1行ずつ読み込み、カンマで分割してリストに格納
        while((line = reader.ReadLine()) != null)
        {
            //空行はスキップ
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }
            string[] values = line.Split(',');
            rows.Add(values);
        }

        if(rows.Count == 0)
        {
            Debug.LogError($"MapDataLoader:CSVファイルが空か、有効なデータがありません:{filePath}.csv");
            return null;
        }

        int height = rows.Count;    //行数がマップの高さ
        int width = rows[0].Length;  //最初の行の列数がマップの幅

        MapData mapData = new MapData(width, height);

        //CSVデータをMapDataオブジェクトに設定
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                if(x < rows[y].Length)//行の列数が異なる場合への対応
                {
                    if(int.TryParse(rows[y][x], out int terrainCode))
                    {
                        //整数値をTerrainType Enumに変換
                        mapData.SetTerrainType(x, y, (TerrainType)terrainCode);
                    }
                    else
                    {
                        Debug.LogWarning($"MapDataLoader:CSVの座標({x},{y})の値'{rows[y][x]}'が不正な地形コードです");
                        mapData.SetTerrainType(x, y, TerrainType.None);//デフォルト値
                    }
                }
                else
                {
                    Debug.LogWarning($"MapDataLoader:CSVの行{y}の列数が行{0}と異なります。({rows[y].Length}vs{width})");
                    mapData.SetTerrainType(x, y, TerrainType.None);//足りない場合はデフォルト値を設定
                }
            }
        }
        Debug.Log($"MapDataLoader:マップデータ'{filePath}.csv'({width}x{height})を読み込みました");
        return mapData;
    }
}
