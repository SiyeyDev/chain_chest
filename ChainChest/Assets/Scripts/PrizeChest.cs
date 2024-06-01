using UnityEngine;
using UnityEngine.UI;

public class PrizeChest : ChestBase
{
    private GameController gameController;
    public int coinValue;
    public Button chestButton;
    public Sprite openWithGoldSprite; 
    private Image image; 
    public AudioSource winAudioSource; 

    void Awake()
    {
        image = GetComponent<Image>();
        winAudioSource = GetComponent<AudioSource>();
    }

    // Setup the chest with the GameController and assign a random coin value
    public override void Setup(GameController controller)
    {
        gameController = controller;
        coinValue = Random.Range(1, 100); 
        chestButton = GetComponent<Button>();
        chestButton.onClick.AddListener(Open); 
    }

    // Handle the chest opening logic
    public override void Open()
    {
        if (!gameController.CanOpenChest) return;

        Debug.Log("PrizeChest opened with value: " + coinValue);
        image.sprite = openWithGoldSprite; 
        chestButton.onClick.RemoveAllListeners(); 
        winAudioSource.Play(); 
        gameController.OpenChest(coinValue, false);
    }
}
