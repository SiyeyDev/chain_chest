using UnityEngine;
using System;

public class PrizeManager : MonoBehaviour
{
    private static PrizeManager _instance;

    public static PrizeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PrizeManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("PrizeManager");
                    _instance = go.AddComponent<PrizeManager>();
                }
            }
            return _instance;
        }
    }

    private int prize;

    public static event Action<int> OnPrizeUpdated;

    void Awake()
    {
        // Singleton pattern implementation
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

    // Add value to the prize and invoke the OnPrizeUpdated event
    public void AddPrize(int value)
    {
        prize += value;
        Debug.Log("Prize added. New prize: " + prize);
        OnPrizeUpdated?.Invoke(prize);
    }

    // Reset the prize to zero and invoke the OnPrizeUpdated event
    public void ResetPrize()
    {
        prize = 0;
        Debug.Log("Prize reset.");
        OnPrizeUpdated?.Invoke(prize);
    }

    // Get the current prize value
    public int GetPrize()
    {
        return prize;
    }
}
