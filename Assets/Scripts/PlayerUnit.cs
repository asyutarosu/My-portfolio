using UnityEngine;

public class PlayerUnit : MonoBehaviour
{

    [SerializeField] private int _moveRange = 3;//�ړ��͈�

    private Vector2Int _currentPosition;//���݂̃O���b�h���W

    //�v���g�^�C�v�p�ŊȈՓI�ȏ��݂̂��L��2025/06
    private string _unitName;//���j�b�g�̖��O

    //�v���g�^�C�v�p�ŊȈՓI�ȏ��݂̂��L��2025/06
    public int CurretHP { get; private set; } = 10;//����HP


    /// <summary>
    /// �v���C���[���j�b�g��������
    /// </summary>
    /// <param name="initialGridPos">�����z�u�����O���b�h���W</param>
    /// <param name="name">���j�b�g�̖��O</param>
    public void Initialize(Vector2Int initialGridPos, string name)
    {
        _currentPosition = initialGridPos;
        _unitName = name;
        Debug.Log($"PlayerUnit'{_unitName}'initialized at grid:{_currentPosition}");
    }

    /// <summary>
    /// ���j�b�g�̌��݂̃O���b�h���W���X�V����
    /// </summary>
    /// <param name="newGridPos">�V�����O���b�h���W</param>
    public void SetGridPosition(Vector2Int newGridPos)
    {
        _currentPosition = newGridPos;
    }

    /// <summary>
    /// ���j�b�g�̌��݂̃O���b�h���W���擾����
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetCurrentGridPostion()
    {
        return _currentPosition;
    }

    /// <summary>
    /// ���j�b�g�̈ړ��͈͂��擾����
    /// </summary>
    public int GetMoveRange()
    {
        return _moveRange;
    }

    //�v���g�^�C�v�p�Ō����_�ł͈ړ��݂̂��L��2025/06
    public void Attack(PlayerUnit target)
    {
        //�U�����W�b�N
    }

    //�f�o�b�O�p�F�}�E�X�Ń��j�b�g�̍��W�m�F�p
    private void OnMouseEnter()
    {
        Debug.Log($"Unit:{_unitName},GridPos:{_currentPosition}");
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
