using UnityEngine;
using UnityEngine.UI;

public class EmptyChest : ChestBase
{
    private GameController gameController;
    public Button chestButton;

    public override void Setup(GameController controller)
    {
        gameController = controller;
        chestButton = GetComponent<Button>(); // Obt�n el componente Button
        chestButton.onClick.AddListener(Open); // A�ade el listener al bot�n
    }

    public override void Open()
    {
        gameController.OpenChest(0, true);
    }
}
