using UnityEngine;
using UnityEngine.UI;

public class PrizeChest : ChestBase
{
    private GameController gameController;
    public int coinValue;

    public Button chestButton;

    public override void Setup(GameController controller)
    {
        gameController = controller;
        coinValue = Random.Range(1, 100); // Valor aleatorio
        chestButton = GetComponent<Button>(); // Obtén el componente Button
        chestButton.onClick.AddListener(Open); // Añade el listener al botón
    }

    public override void Open()
    {
        Debug.Log("Dio click");
        gameController.OpenChest(coinValue, false);
    }
}
