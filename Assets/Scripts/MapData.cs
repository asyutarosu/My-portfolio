using UnityEngine;

public class MapData
{
    public int Width { get; private set; }//�}�b�v�̕��iX������)
    public int Height { get; private set; }//�}�b�v�̍���(Y������)
    private TerrainType[,] _terrainTypes;//�e���W�̒n�`�^�C�v��ێ�����2�����z��

    //�R���X�g���N�^
    public MapData(int width, int height)
    {
        Width = width;
        Height = height;
        _terrainTypes = new TerrainType[Width, Height];
    }

    //�w�肳�ꂽ���W�̒n�`�^�C�v��ݒ�
    public void SetTerrainType(int x, int y, TerrainType type)
    {
        if(x >= 0 && x < Width && y >= 0 && y < Height)
        {
            _terrainTypes[x, y] = type;
        }
        else
        {
            Debug.LogWarning($"MapData.SetTerrainType:�͈͊O�̍��W�i{x},{y}�j�ɒn�`�^�C�v��ݒ肵�悤�Ƃ��܂��܂���");
        }
    }

    //�w�肳�ꂽ���W�̒n�`�^�C�v���擾
    public TerrainType GetTerrainType(Vector2Int position)
    {
        if(position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height)
        {
            return _terrainTypes[position.x, position.y];
        }
        //�͈͊O�̏ꍇ�̓G���[���O��ʒm
        Debug.LogWarning($"MapData.GetTerrainType:({position.x},{position.y})�̒n�`�^�C�v���擾���悤�Ƃ��܂��܂���");
        return TerrainType.None;
    }
}
