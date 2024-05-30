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
        chestButton = GetComponent<Button>(); // Obt�n el componente Button
        chestButton.onClick.AddListener(Open); // A�ade el listener al bot�n
    }

    public override void Open()
    {
        Debug.Log("Dio click");
        gameController.OpenChest(coinValue, false);
    }
}
