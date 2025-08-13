using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelController : MonoBehaviour
{
    [SerializeField]
    GameObject m_tutorialMessage;

    [SerializeField]
    GameObject m_charactersPanel;

    [SerializeField]
    RawImage m_characterDisplay;

    [SerializeField]
    TMP_Text m_conversation;

    void Awake()
    {
        // Show Tutorial Message for First Time users
        int tutorialMessageDisplayed = PlayerPrefs.GetInt("TutorialMessageDisplayed");
        m_tutorialMessage.SetActive(tutorialMessageDisplayed == 0);
    }

    public void OnBackButtonPressed()
    {
        m_charactersPanel.SetActive(false);
    }

    public void OnCharacterButtonPressed()
    {
        m_charactersPanel.SetActive(true);
    }

    public void OnTutorialMessageButtonPressed()
    {
        m_tutorialMessage.SetActive(false);
        PlayerPrefs.SetInt("TutorialMessageDisplayed", 1);
        PlayerPrefs.Save();
    }

    public void UpdateDisplay(CharacterInfo newCharacter)
    {
        m_characterDisplay.texture = newCharacter.characterImage;
        m_conversation.text = "";
    }

}
