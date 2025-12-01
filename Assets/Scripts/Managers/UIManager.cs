using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject creditsPanel;
    [Header("Components References")]
    [SerializeField] private TMP_Text textComponent;
    [Header("Scripts References")]
    [SerializeField] private RippleEffectController rippleEffectController;
    // Start is called before the first frame update
    void Start()
    {
        
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

    public void LaunchScene()
    {
        SceneManager.LoadScene("ResilienceScene", LoadSceneMode.Single);
    }

    public void ChangeTextColor(Color color)
    {
        textComponent.color = color;
    }
    public void UpdateText(string text)
    {
        textComponent.text = text;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
