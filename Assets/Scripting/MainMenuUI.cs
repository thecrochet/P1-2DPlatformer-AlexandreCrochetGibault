using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    void Awake()
    {
        if(settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Settings panel reference is not set in the inspector.");
        }
    }
    

    public void OnStartClicked()
    {         
        // Load the main game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("UISetup");
    }

    public void OnSettingsClicked()
    {
        bool isActive = settingsPanel.activeSelf;
        settingsPanel.SetActive(!isActive);
    }

    public void OnBackClicked()
    {
        // Hide the settings panel
        settingsPanel.SetActive(false);
    }

    public void OnExitClicked()
    {
        // Exit the application
        Application.Quit();
    }
}
