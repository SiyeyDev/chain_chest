using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    private static GameController _instance;

    public static GameController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameController");
                    _instance = go.AddComponent<GameController>();
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

    public string chestContainerName = "ChestContainer"; // Nombre del ChestContainer en la escena
    public GameObject losePopup; // Asignado por MenuController
    public GameObject winPopup; // Asignado por MenuController
    public ChestFactory chestFactory;
    public string pauseButtonName = "PauseGameBtn"; // Nombre del botón de pausa en el prefab
    public string tutorialButtonName = "TutorialGameBtn"; // Nombre del botón de tutorial en el prefab

    private int currentRound = 0;
    private int numRounds;
    private int minChestsPerRound;
    private int maxChestsPerRound;
    private Transform chestContainer;
    private Button pauseButton;
    private Button tutorialButton;
    private TextMeshProUGUI prizeText;
    private TextMeshProUGUI roundText;
    private TextMeshProUGUI winPrizeText;
    public bool CanOpenChest { get; set; } = true;

    void Start()
    {
        // Obtener configuraciones del MenuController
        numRounds = MenuController.Instance.GetNumRounds();
        minChestsPerRound = MenuController.Instance.GetMinChestsPerRound();
        maxChestsPerRound = MenuController.Instance.GetMaxChestsPerRound();

        // Buscar y asignar referencias
        if (chestFactory == null)
        {
            chestFactory = FindObjectOfType<ChestFactory>();
        }

        prizeText = GameObject.Find("PrizeTxt").GetComponent<TextMeshProUGUI>();
        roundText = GameObject.Find("RoundTxt").GetComponent<TextMeshProUGUI>();

        chestContainer = GameObject.Find(chestContainerName).transform;

        pauseButton = GameObject.Find(pauseButtonName).GetComponent<Button>();
        tutorialButton = GameObject.Find(tutorialButtonName).GetComponent<Button>();

        pauseButton.onClick.AddListener(MenuController.Instance.ShowPausePopup);
        tutorialButton.onClick.AddListener(MenuController.Instance.ShowTutorialPopup);

        PrizeManager.OnPrizeUpdated += UpdatePrizeText;
        losePopup.SetActive(false);
        winPopup.SetActive(false);
        StartRound();
    }

    // Setup the grid layout for chests
    void SetupGrid()
    {
        GridLayoutGroup gridLayout = chestContainer.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            gridLayout = chestContainer.gameObject.AddComponent<GridLayoutGroup>();
        }

        int numChestsPerRound = Random.Range(minChestsPerRound, maxChestsPerRound + 1);
        int columns = Mathf.CeilToInt(Mathf.Sqrt(numChestsPerRound));
        int rows = Mathf.CeilToInt((float)numChestsPerRound / columns);

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        float cellSize = 125f;
        gridLayout.cellSize = new Vector2(cellSize, cellSize);
        gridLayout.spacing = new Vector2(175f, 10f);
    }

    // Start a new round
    void StartRound()
    {
        Debug.Log("Starting Round " + currentRound);
        ClearChests();
        SetupGrid();
        List<bool> chestTypes = GenerateChestTypes();
        foreach (var isEmpty in chestTypes)
        {
            ChestBase chest = chestFactory.CreateChest(isEmpty, chestContainer);
            chest.Setup(this);
        }
        CanOpenChest = true;
        currentRound++;
        UpdateRoundText();
    }

    // Generate chest types (prize or empty)
    List<bool> GenerateChestTypes()
    {
        int numChestsPerRound = Random.Range(minChestsPerRound, maxChestsPerRound + 1);
        List<bool> types = new List<bool>();

        if (currentRound == 0)
        {
            // All chests are prize chests in the first round
            for (int i = 0; i < numChestsPerRound; i++)
            {
                types.Add(false);
            }
        }
        else
        {
            // 1/3 of the chests are empty from the second round onwards
            int numEmptyChests = Mathf.CeilToInt(numChestsPerRound / 3.0f);
            int numPrizeChests = numChestsPerRound - numEmptyChests;

            for (int i = 0; i < numPrizeChests; i++)
            {
                types.Add(false);
            }

            for (int i = 0; i < numEmptyChests; i++)
            {
                types.Add(true);
            }

            types = ShuffleList(types);
        }

        return types;
    }

    // Shuffle the list of chest types
    List<bool> ShuffleList(List<bool> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            bool temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    // Handle chest opening logic
    public void OpenChest(int value, bool isEmpty)
    {
        if (!CanOpenChest)
        {
            Debug.Log("CanOpenChest is false, exiting OpenChest");
            return;
        }

        Debug.Log("OpenChest called with value: " + value + " and isEmpty: " + isEmpty);

        CanOpenChest = false;
        if (isEmpty)
        {
            Debug.Log("Chest is empty. Showing losePopup.");
            losePopup.SetActive(true);
            PrizeManager.Instance.ResetPrize();
        }
        else
        {
            Debug.Log("Chest has prize. Adding prize value.");
            PrizeManager.Instance.AddPrize(value);
            UpdatePrizeText(PrizeManager.Instance.GetPrize());
            if (currentRound >= numRounds)
            {
                Debug.Log("All rounds completed. Showing winPopup.");
                MenuController.Instance.ShowWinPopupWithCurrentPrize(true);
            }
            else
            {
                Debug.Log("Starting next round delay.");
                StartCoroutine(NextRoundDelay(1f));
            }
        }
    }

    // Delay before starting the next round
    private IEnumerator NextRoundDelay(float delay)
    {
        Debug.Log("NextRoundDelay started with delay: " + delay);
        yield return new WaitForSeconds(delay);
        Debug.Log("NextRoundDelay completed. Starting new round.");
        StartRound();
    }

    // Reset the game
    public void ResetGame()
    {
        currentRound = 0;
        PrizeManager.Instance.ResetPrize();
        UpdatePrizeText(0);
        losePopup.SetActive(false);
        winPopup.SetActive(false);
        StartRound();
    }

    // Clear all chests
    void ClearChests()
    {
        foreach (Transform child in chestContainer)
        {
            Destroy(child.gameObject);
        }
    }

    // Update the prize text
    void UpdatePrizeText(int newPrize)
    {
        Debug.Log("UpdatePrizeText called with newPrize: " + newPrize);
        prizeText.text = newPrize.ToString();
    }

    // Update the round text
    void UpdateRoundText()
    {
        roundText.text = currentRound.ToString();
    }

    void OnDestroy()
    {
        PrizeManager.OnPrizeUpdated -= UpdatePrizeText;
    }
}
