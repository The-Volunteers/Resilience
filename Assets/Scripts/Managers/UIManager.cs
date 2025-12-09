using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject endPanel;
    [Header("Components References")]
    [SerializeField] private TMP_Text textComponent;

    [Header("Dialogue Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float dialogueDuration = 4f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private Ease fadeEase = Ease.InOutQuad;

    private Sequence dialogueFadeSequence;
    private CanvasGroup dialogueCanvasGroup;
    //[Header("Scripts References")]
    //[SerializeField] private RippleEffectController rippleEffectController;
    // Start is called before the first frame update
    void Start()
    {
        dialogueCanvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
        if(dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.alpha = 0f;
            dialoguePanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivatePanel(GameObject panel)
    {
        panel.SetActive(true);
    }
    public void DeActivatePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void LaunchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void ChangeTextColor(Color color)
    {
        textComponent.color = color;
    }
    public void UpdateText(string text)
    {
        textComponent.text = text;
    }

    public void ShowMessage(string message)
    {
        if (textComponent != null)
        {
            textComponent.text = message;
        }

        PlayFadeSequence();
    }

    private void PlayFadeSequence()
    {
        dialogueFadeSequence?.Kill();

        dialoguePanel.SetActive(true);
        dialogueCanvasGroup.alpha = 0f;

        dialogueFadeSequence = DOTween.Sequence();

        dialogueFadeSequence.Append(dialogueCanvasGroup.DOFade(1f, fadeInDuration).SetEase(fadeEase))
            .AppendInterval(dialogueDuration)
            .Append(dialogueCanvasGroup.DOFade(0f, fadeOutDuration).SetEase(fadeEase))
            .OnComplete(() => dialoguePanel.SetActive(false))
            .SetUpdate(true); // ignore timeScale !
    }

    public void CancelFade()
    {
        dialogueFadeSequence?.Kill() ;
        dialogueCanvasGroup.alpha = 0f;
        dialoguePanel.SetActive(false);
    }

    public void ActivateEndPanel()
    {
        endPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        dialogueFadeSequence?.Kill();
    }



    public void QuitGame()
    {
        Application.Quit();
    }
}
