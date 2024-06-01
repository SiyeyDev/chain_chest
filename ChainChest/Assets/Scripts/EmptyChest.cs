using UnityEngine;
using UnityEngine.UI;

public class EmptyChest : ChestBase
{
    private GameController gameController;
    public Button chestButton;
    public Sprite openEmptySprite;
    private Image image; 
    public AudioSource loseAudioSource; 

    void Awake()
    {
        image = GetComponent<Image>();
        loseAudioSource = GetComponent<AudioSource>();
    }

    // Setup the chest with the GameController
    public override void Setup(GameController controller)
    {
        gameController = controller;
        chestButton = GetComponent<Button>(); 
        chestButton.onClick.AddListener(Open); 
    }

    // Handle the chest opening logic
    public override void Open()
    {
        if (!gameController.CanOpenChest) return;

        Debug.Log("EmptyChest opened");
        image.sprite = openEmptySprite; 
        chestButton.onClick.RemoveAllListeners(); 
        loseAudioSource.Play(); 
        gameController.OpenChest(0, true);
    }
}
