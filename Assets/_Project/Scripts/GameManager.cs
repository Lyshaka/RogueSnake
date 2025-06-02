using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[SerializeField] Transform bulletParent;
	[SerializeField] Transform feedbacksTextParent;

	// Coins
	public int coins = 0;

	private float _timer = 0f;

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

	private void Update()
	{
		HandleTimer();
	}

	void HandleTimer()
	{
		_timer += Time.deltaTime;
		UserInterfaceManager.instance.SetTimer(_timer);
	}

	public void AddCoins(int value)
	{
		coins += value;
		UserInterfaceManager.instance.SetCoin(coins);
	}
}
