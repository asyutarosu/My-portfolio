using UnityEngine;

/// <summary>
/// ���݂̃Q�[���̃^�[����Ԃ��`����񋓌^
/// </summary>
public enum TurnState
{
    PreGame,    //�Q�[���J�n�O�i�������j
    PlayerTurn, //�v���C���[�̍s���t�F�C�Y
    EnemyTurn,  //�G�̍s���t�F�C�Y
    NeutralTurn,//�������j�b�g�̍s���t�F�C�Y
    PostTurn,   //�^�[���I����̃N���[���A�b�v
    Cutscene,   //�C�x���g�V�[���p
    StageClear, //�X�e�[�W�N���A
    GameOver    //�Q�[���I��
}
