using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    private static MenuController _instance;

    public static MenuController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MenuController>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("MenuController");
                    _instance = go.AddComponent<MenuController>();
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public GameObject menuCanvas;
    public GameObject losePopup;
    public GameObject winPopup;
    public GameObject pausePopup;
    public GameObject tutorialPopup;

    public GameObject gameControllerPrefab;
    public GameObject prizeManagerPrefab;
    public GameObject gameContainerPrefab;

    public List<Button> returnToMenuButtons;
    public List<Button> repeatGameButtons;
    public Button pauseReturnToMenuButton;
    public Button pauseRepeatGameButton;

    private GameObject gameControllerInstance;
    private GameObject prizeManagerInstance;
    private GameObject gameContainerInstance;

    public int numRounds = 5;
    public int minChestsPerRound = 2;
    public int maxChestsPerRound = 5;

    private void Start()
    {
        ShowMenu(); // Show the menu at the start
        ConfigurePopupButtons(); // Configure popup buttons
        ConfigurePausePopupButtons(); // Configure pause popup buttons
    }

    // Exit the game
    public void ExitGame()
    {
        Application.Quit();
    }

    // Return to the main menu from pause
    public void ReturnToMenuFromPause()
    {
        if (gameControllerInstance != null && !losePopup.activeSelf && !winPopup.activeSelf)
        {
            // Show the win popup with the current prize
            ShowWinPopupWithCurrentPrize(false);
        }
    }

    // Restart the game from pause
    public void RestartGameFromPause()
    {
        if (gameControllerInstance != null && !losePopup.activeSelf && !winPopup.activeSelf)
        {
            // Show the win popup with the current prize
            ShowWinPopupWithCurrentPrize(false);
        }
    }

    // Return to the main menu
    public void ReturnToMenu()
    {
        DestroyInstances(); // Destroy prefab instances
        losePopup.SetActive(false);
        winPopup.SetActive(false);
        pausePopup.SetActive(false);
        ShowMenu();
    }

    // Start the game
    public void StartGame()
    {
        InstantiatePrefabs(); // Instantiate prefabs
        ConfigurePopupButtons(); // Configure popup buttons
        ShowGame();
    }

    // Restart the game
    public void RestartGame()
    {
        gameControllerInstance?.GetComponent<GameController>().ResetGame();
        ShowGame();
    }

    // Show the win popup with the current prize
    public void ShowWinPopupWithCurrentPrize(bool completedAllRounds)
    {
        winPopup.SetActive(true);
        pausePopup.SetActive(false); // Ensure pause popup is hidden

        Transform winPanelChild = winPopup.transform.GetChild(0); // Get the first child of WinPanel
        TextMeshProUGUI winPrizeText = winPanelChild.Find("WinPrizeTxt").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI winMessageText = winPanelChild.Find("WinMessageTxt").GetComponent<TextMeshProUGUI>();

        if (winPrizeText != null)
        {
            winPrizeText.text = PrizeManager.Instance.GetPrize().ToString();
        }
        else
        {
            Debug.LogError("WinPrizeTxt not found");
        }

        if (winMessageText != null)
        {
            if (completedAllRounds)
            {
                winMessageText.text = "Ganaste todas las rondas.\n\nFELICITACIONES";
            }
            else
            {
                winMessageText.text = "No terminaste todas las rondas. Puedes volver a intentarlo o volver al menú principal.";
            }        }
        else
        {
            Debug.LogError("WinMessageTxt not found");
        }
    }

    // Get number of rounds
    public int GetNumRounds()
    {
        return numRounds;
    }

    // Get minimum chests per round
    public int GetMinChestsPerRound()
    {
        return minChestsPerRound;
    }

    // Get maximum chests per round
    public int GetMaxChestsPerRound()
    {
        return maxChestsPerRound;
    }

    // Show the menu
    private void ShowMenu()
    {
        menuCanvas.SetActive(true);
        pausePopup.SetActive(false);
        if (gameControllerInstance != null)
        {
            gameControllerInstance.SetActive(false);
        }
    }

    // Show the game
    private void ShowGame()
    {
        menuCanvas.SetActive(false);
        pausePopup.SetActive(false);
        if (gameControllerInstance != null)
        {
            gameControllerInstance.SetActive(true);
        }
    }

    // Show the pause popup
    public void ShowPausePopup()
    {
        if (pausePopup != null)
        {
            pausePopup.SetActive(true);
        }
    }

    // Show the tutorial popup
    public void ShowTutorialPopup()
    {
        if (tutorialPopup != null)
        {
            tutorialPopup.SetActive(true);
        }
    }

    // Hide the pause popup
    public void HidePausePopup()
    {
        if (pausePopup != null)
        {
            pausePopup.SetActive(false);
        }
    }

    // Hide the tutorial popup
    public void HideTutorialPopup()
    {
        if (tutorialPopup != null)
        {
            tutorialPopup.SetActive(false);
        }
    }

    // Destroy instances of the prefabs
    private void DestroyInstances()
    {
        if (gameControllerInstance != null)
        {
            Destroy(gameControllerInstance);
        }
        if (prizeManagerInstance != null)
        {
            Destroy(prizeManagerInstance);
        }
        if (gameContainerInstance != null)
        {
            Destroy(gameContainerInstance);
        }
    }

    // Instantiate the prefabs
    private void InstantiatePrefabs()
    {
        prizeManagerInstance = Instantiate(prizeManagerPrefab);
        gameContainerInstance = Instantiate(gameContainerPrefab);
        gameControllerInstance = Instantiate(gameControllerPrefab);

        // Configure references in GameController
        GameController gameController = gameControllerInstance.GetComponent<GameController>();
        gameController.losePopup = losePopup;
        gameController.winPopup = winPopup;
    }

    // Configure buttons for popups
    private void ConfigurePopupButtons()
    {
        foreach (Button button in returnToMenuButtons)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ReturnToMenu());
        }

        foreach (Button button in repeatGameButtons)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => RestartGame());
        }
    }

    // Configure buttons for pause popup
    private void ConfigurePausePopupButtons()
    {
        if (pauseReturnToMenuButton != null)
        {
            pauseReturnToMenuButton.onClick.RemoveAllListeners();
            pauseReturnToMenuButton.onClick.AddListener(() => ReturnToMenuFromPause());
        }

        if (pauseRepeatGameButton != null)
        {
            pauseRepeatGameButton.onClick.RemoveAllListeners();
            pauseRepeatGameButton.onClick.AddListener(() => RestartGameFromPause());
        }
    }
}
