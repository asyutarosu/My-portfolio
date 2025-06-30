using UnityEngine;
using System.IO;//�t�@�C������̂���
using System.Linq;//Split�Ŏg�p
using System.Collections.Generic;//List�Ŏg�p

public class MapDataLoader
{
    /// <summary>
    /// �w�肳�ꂽCSV�t�@�C������}�b�v�f�[�^��ǂݍ���
    /// </summary>
    /// <param name="fliePath">Assets/Resources/�ȉ��̃t�@�C���̃p�X</param>
    /// <retutns>�ǂݍ��܂ꂽMapData�I�u�W�F�N�g�B�ǂݍ��݂Ɏ��s�����ꍇ��nulll</retutns>
    public static MapData LoadMapDataFromCSV(string filePath)
    {
        //CSV�t�@�C���̓v���W�F�N�g��Assets/Resources/�t�H���_�ȉ��ɔz�u
        TextAsset csvFile = Resources.Load<TextAsset>(filePath);

        if(csvFile == null)
        {
            Debug.LogError($"MapDataLoader:CSV�t�@�C����������܂���:Assets/Resources/{filePath}.csv");
            return null;
        }
        
        StringReader reader = new StringReader(csvFile.text);
        List<string[]> rows = new List<string[]>();
        string line;

        //1�s���ǂݍ��݁A�J���}�ŕ������ă��X�g�Ɋi�[
        while((line = reader.ReadLine()) != null)
        {
            //��s�̓X�L�b�v
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }
            string[] values = line.Split(',');
            rows.Add(values);
        }

        if(rows.Count == 0)
        {
            Debug.LogError($"MapDataLoader:CSV�t�@�C�����󂩁A�L���ȃf�[�^������܂���:{filePath}.csv");
            return null;
        }

        int height = rows.Count;    //�s�����}�b�v�̍���
        int width = rows[0].Length;  //�ŏ��̍s�̗񐔂��}�b�v�̕�

        MapData mapData = new MapData(width, height);

        //CSV�f�[�^��MapData�I�u�W�F�N�g�ɐݒ�
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                if(x < rows[y].Length)//�s�̗񐔂��قȂ�ꍇ�ւ̑Ή�
                {
                    if(int.TryParse(rows[y][x], out int terrainCode))
                    {
                        //�����l��TerrainType Enum�ɕϊ�
                        mapData.SetTerrainType(x, y, (TerrainType)terrainCode);
                    }
                    else
                    {
                        Debug.LogWarning($"MapDataLoader:CSV�̍��W({x},{y})�̒l'{rows[y][x]}'���s���Ȓn�`�R�[�h�ł�");
                        mapData.SetTerrainType(x, y, TerrainType.None);//�f�t�H���g�l
                    }
                }
                else
                {
                    Debug.LogWarning($"MapDataLoader:CSV�̍s{y}�̗񐔂��s{0}�ƈقȂ�܂��B({rows[y].Length}vs{width})");
                    mapData.SetTerrainType(x, y, TerrainType.None);//����Ȃ��ꍇ�̓f�t�H���g�l��ݒ�
                }
            }
        }
        Debug.Log($"MapDataLoader:�}�b�v�f�[�^'{filePath}.csv'({width}x{height})��ǂݍ��݂܂���");
        return mapData;
    }
}
