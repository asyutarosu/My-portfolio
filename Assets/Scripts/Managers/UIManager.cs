using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]public TMP_Text movementPointsUI;

    /// <summary>
    /// UIÇÃï\é¶ÇÃêÿÇËë÷Ç¶
    /// </summary>
    /// <param name="targetText"></param>
    /// <param name="isVisible"></param>
    public void SetTextVisibility(TMP_Text targetText, bool isVisible)
    {
        if(targetText != null)
        {
            targetText.enabled = isVisible;
        }
    }

    public void UpdaateMovePointUI(int current)
    {
        movementPointsUI.text = $"{current}";
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
