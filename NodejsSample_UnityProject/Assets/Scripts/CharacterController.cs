using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    public CharacterInfo Info;

    [SerializeField]
    bool m_isDefaultCharacter = false;

    TMP_Text m_nameText;
    RawImage m_characterImage;

    void Awake()
    {
        m_nameText = GetComponentInChildren<TMP_Text>();
        m_characterImage = GetComponentInChildren<RawImage>();
    }

    void Start()
    {
        // Setup Button UI
        if (m_nameText != null)
        {
            m_nameText.text = Info.characterName;
        }
            
        if (m_characterImage != null)
        {
            m_characterImage.texture = Info.characterImage;
        }

        if (m_isDefaultCharacter)
        {
            if (AppManager.Instance != null)
            {
                AppManager.Instance.SetCharacter(Info);
            }
                
            if (AppManager_WS.Instance != null)
            {
                AppManager_WS.Instance.SetCharacter(Info);
            }
        }
    }

    public void OnButtonClick()
    {
        if(AppManager.Instance != null)
        {
            AppManager.Instance.SetCharacter(Info);
        }   

        if(AppManager_WS.Instance != null)
        {
            AppManager_WS.Instance.SetCharacter(Info);
        }
    }
}
