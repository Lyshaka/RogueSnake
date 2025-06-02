using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	// Coins
	public int coins = 0;

	private void Awake()
	{
		if (instance == null)
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
		}
		else
			Destroy(gameObject);
	}

	public void AddCoins(int value)
	{
		coins += value;
		UserInterfaceManager.instance.SetCoin(coins);
	}
}
