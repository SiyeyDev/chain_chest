using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

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

    public int numRounds = 5;
    public int numChestsPerRound = 6;
    public Transform chestContainer;
    public TextMeshProUGUI prizeText;
    public TextMeshProUGUI messageText;
    public ChestFactory chestFactory;

    private int currentRound = 0;

    void Start()
    {
        PrizeManager.OnPrizeUpdated += UpdatePrizeText;
        StartRound();
    }

    void SetupGrid()
    {
        GridLayoutGroup gridLayout = chestContainer.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            gridLayout = chestContainer.gameObject.AddComponent<GridLayoutGroup>();
        }

        // Calcula el número óptimo de columnas y filas para que la grilla se vea bien
        int columns = Mathf.CeilToInt(Mathf.Sqrt(numChestsPerRound));
        int rows = Mathf.CeilToInt((float)numChestsPerRound / columns);

        // Ajusta las dimensiones del GridLayoutGroup
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        // Ajusta el tamaño de la celda y el espaciado según sea necesario
        float cellSize = 100f; // Ajusta según el tamaño de tus cofres
        gridLayout.cellSize = new Vector2(cellSize, cellSize);
        gridLayout.spacing = new Vector2(10f, 10f); // Ajusta según el espaciado que desees
    }

    void StartRound()
    {
        ClearChests();
        SetupGrid();
        List<bool> chestTypes = GenerateChestTypes();
        foreach (var isEmpty in chestTypes)
        {
            ChestBase chest = chestFactory.CreateChest(isEmpty, chestContainer);
            chest.Setup(this);
        }
        currentRound++;
    }

    List<bool> GenerateChestTypes()
    {
        List<bool> types = new List<bool>();
        if (currentRound == 0) // Primera ronda
        {
            for (int i = 0; i < numChestsPerRound; i++)
            {
                types.Add(false); // Sólo cofres con premio
            }
        }
        else // Rondas subsiguientes
        {
            for (int i = 0; i < numChestsPerRound - 1; i++)
            {
                types.Add(false); // Cofres con premio
            }
            types.Add(true); // Cofre vacío
        }
        return ShuffleList(types);
    }

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

    public void OpenChest(int value, bool isEmpty)
    {
        if (isEmpty)
        {
            messageText.text = "¡Has perdido! El premio acumulado es 0.";
            PrizeManager.Instance.ResetPrize();
            // Mostrar menú de reinicio
        }
        else
        {
            PrizeManager.Instance.AddPrize(value);
            if (currentRound >= numRounds)
            {
                messageText.text = "¡Felicidades! Has ganado " + PrizeManager.Instance.GetPrize() + " monedas.";
                // Mostrar menú de reinicio
            }
            else
            {
                StartRound();
            }
        }
    }

    void ClearChests()
    {
        foreach (Transform child in chestContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdatePrizeText(int newPrize)
    {
        prizeText.text = "Premio acumulado: " + newPrize;
    }

    void OnDestroy()
    {
        PrizeManager.OnPrizeUpdated -= UpdatePrizeText;
    }
}
