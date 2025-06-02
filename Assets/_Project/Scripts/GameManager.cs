using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[SerializeField] Transform bulletParent;
	[SerializeField] Transform feedbacksTextParent;

	// Coins
	public int coins = 0;

	// Bullets
	public Transform BulletParent => bulletParent;
	public Transform FeedbackTextParent => feedbacksTextParent;


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
