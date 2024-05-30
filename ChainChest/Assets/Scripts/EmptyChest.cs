using UnityEngine;
using UnityEngine.UI;

public class EmptyChest : ChestBase
{
    private GameController gameController;
    public Button chestButton;

    public override void Setup(GameController controller)
    {
        gameController = controller;
        chestButton = GetComponent<Button>(); // Obtén el componente Button
        chestButton.onClick.AddListener(Open); // Añade el listener al botón
    }

    public override void Open()
    {
        gameController.OpenChest(0, true);
    }
}
