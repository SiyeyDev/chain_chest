using UnityEngine;

public class ChestFactory : MonoBehaviour
{
    public GameObject prizeChestPrefab;
    public GameObject emptyChestPrefab;

    public ChestBase CreateChest(bool isEmpty, Transform parent)
    {
        GameObject chestObj = isEmpty ? Instantiate(emptyChestPrefab, parent) : Instantiate(prizeChestPrefab, parent);
        ChestBase chest = chestObj.GetComponent<ChestBase>();
        return chest;
    }
}
