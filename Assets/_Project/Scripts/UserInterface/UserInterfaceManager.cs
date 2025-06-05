using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
	public static UserInterfaceManager instance;

	[Header("Health Bar")]
	[SerializeField] float healthMaskPixelsAmount = 1024f;
	[SerializeField] RectMask2D healthBarMask;
	[SerializeField] TextMeshProUGUI healthBarText;
	
	[Header("Coins")]
	[SerializeField] TextMeshProUGUI coinText;
	[SerializeField] Transform coinTransform;
	[SerializeField] float coinGrowDuration = 0.2f;

	[Header("Timer")]
	[SerializeField] TextMeshProUGUI timerText;

	[Header("Game Over panel")]
	[SerializeField] GameObject gameOverPanel;
	[SerializeField] TextMeshProUGUI gameOverTimeTMP;
	[SerializeField] TextMeshProUGUI gameOverCoinTMP;
	[SerializeField] TextMeshProUGUI gameOverKillTMP;
	[SerializeField] TextMeshProUGUI gameOverDamagesTMP;

	[Header("Other")]
	[SerializeField] string mainMenuPath;

	float _coinElapsedTime = 0f;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	private void Start()
	{
		gameOverPanel.SetActive(false);
	}

	private void Update()
	{
		if (_coinElapsedTime > 0f)
		{
			coinTransform.localScale = Mathf.Lerp(1f, 1.2f, _coinElapsedTime / coinGrowDuration) * Vector3.one;
			_coinElapsedTime -= Time.deltaTime;
		}
	}

	public void EnableGameOver(float timerSeconds, int coins, int kills, float damages)
	{
		gameOverTimeTMP.text = Utilities.TimeToString(timerSeconds, 23);
		gameOverCoinTMP.text = coins.ToString();
		gameOverKillTMP.text = kills.ToString();
		gameOverDamagesTMP.text = damages.ToString();

		gameOverPanel.SetActive(true);
	}

	public void BackToMenu()
	{
		GameManager.instance.LoadScene(mainMenuPath);
	}

	public void SetHealth(float health, float healthMax)
	{
		// Update health bar
		Vector4 padding = healthBarMask.padding;
		padding.z = healthMaskPixelsAmount * (1f - (health / healthMax));
		healthBarMask.padding = padding;

		// Update health bar text
		healthBarText.text = $"{health:0.0} / {healthMax:0.0}";
	}

	public void SetCoin(int value)
	{
		_coinElapsedTime = coinGrowDuration;
		coinText.text = value.ToString();
	}

	public void SetTimer(float totalSeconds)
	{
		timerText.text = Utilities.TimeToString(totalSeconds, 36);
	}
}
