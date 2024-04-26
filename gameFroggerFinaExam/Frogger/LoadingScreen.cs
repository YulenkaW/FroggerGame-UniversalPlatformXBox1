using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.System;

public class StartScreenManager : MonoBehaviour
{
    public Button startButton;
    public Button optionsButton;
    public Button quitButton;

    async void Start()
    {
        // Assigning  UI buttons
        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        optionsButton = GameObject.Find("OptionsButton").GetComponent<Button>();
        quitButton = GameObject.Find("QuitButton").GetComponent<Button>();

        // Add listeners 
        startButton.onClick.AddListener(StartGame);
        optionsButton.onClick.AddListener(ShowOptions);
        quitButton.onClick.AddListener(QuitGame);

        // Launch the UWP app
        await LaunchUwpApplicationAsync("YourUwpAppName");
    }

    async Task LaunchUwpApplicationAsync(string appName)
    {
        // Check if the UWP app is installed
        if (PackageCatalog.FindPackagesForUser("") != null)
        {
            // Launch app
            await Launcher.LaunchUriAsync(new Uri(appName + ":"));
        }
        else
        {
            Debug.Log("UWP app not installed.");
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Xbox_AButton"))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    void ShowOptions()
    {
        
       GameObject optionsPanel = GameObject.Find("OptionsPanel");
          if (optionsPanel != null)
          {
              optionsPanel.SetActive(true);

              Slider volumeSlider = optionsPanel.transform.Find("VolumeSlider").GetComponent<Slider>();
              volumeSlider.value = GetVolume(); 
              volumeSlider.onValueChanged.AddListener(delegate { SetVolume(volumeSlider.value); });
          }
      }

      float GetVolume()
      {
          // we need to add volume code here
          return PlayerPrefs.GetFloat("Volume", 1.0f);
      }

      void SetVolume(float volume)
      {
  
          PlayerPrefs.SetFloat("Volume", volume);
          PlayerPrefs.Save(); // Save volume setting
          Debug.Log("Volume set to: " + volume);
      }

    void QuitGame()
    {
        Application.Quit();
    }
}
