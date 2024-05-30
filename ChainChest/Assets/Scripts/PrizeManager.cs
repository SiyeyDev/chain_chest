using UnityEngine;

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

    public delegate void PrizeUpdated(int newPrize);
    public static event PrizeUpdated OnPrizeUpdated;

    private int prize;

    public void AddPrize(int amount)
    {
        prize += amount;
        OnPrizeUpdated?.Invoke(prize);
    }

    public void ResetPrize()
    {
        prize = 0;
        OnPrizeUpdated?.Invoke(prize);
    }

    public int GetPrize()
    {
        return prize;
    }
}
