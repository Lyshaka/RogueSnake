using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[Header("Transform Parents")]
	[SerializeField] Transform bulletParent;
	[SerializeField] Transform feedbacksTextParent;

	[Header("Level Loading")]
	[SerializeField] Image fadeImage;
	[SerializeField] float fadeInDuration = 0.2f;
	[SerializeField] float fadeOutDuration = 0.2f;

	// Coins
	public int coins = 0;

	// Timer
	private bool _startedTimer = false;
	private float _timer = 0f;
	public bool StartedTimer => _startedTimer;

	// Scene loading
	private bool _isLevelLoading = false;

	// Bullets
	public Transform BulletParent => bulletParent;

	// Feedback texts
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

	#region TIMER

	void HandleTimer()
	{
		if (!_startedTimer)
			return;

		_timer += Time.deltaTime;
		UserInterfaceManager.instance.SetTimer(_timer);
	}

	public void StartTimer()
	{
		_startedTimer = true;
	}

	#endregion

	public void AddCoins(int value)
	{
		coins += value;
		UserInterfaceManager.instance.SetCoin(coins);
	}

	#region LEVEL_LOADER

	public void LoadScene(string path)
	{
		if (!_isLevelLoading)
		{
			StartCoroutine(LoadSceneCoroutine(path));
		}
	}

	IEnumerator LoadSceneCoroutine(string path)
	{
		_isLevelLoading = true;

		float elapsedTime;

		// Fade screen to black
		elapsedTime = 0f;
		while (elapsedTime < fadeInDuration)
		{
			fadeImage.color = new(0f, 0f, 0f, elapsedTime / fadeInDuration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		fadeImage.color = new(0f, 0f, 0f, 1f); // Full black

		// Skip a frame (just to be sure)
		yield return null;

		AsyncOperation asyncOperation =	SceneManager.LoadSceneAsync(path);

		while (!asyncOperation.isDone)
		{
			// Wait for the scene loading to finish
			yield return null;
		}

		// Skip a frame (just to be sure)
		yield return null;

		// Fade screen from black
		elapsedTime = 0f;
		while (elapsedTime < fadeOutDuration)
		{
			fadeImage.color = new(0f, 0f, 0f, 1f - (elapsedTime / fadeOutDuration));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		fadeImage.color = new(0f, 0f, 0f, 0f); // Full transparent

		// Skip a frame (just to be sure)
		yield return null;

		_isLevelLoading = false;
	}

	#endregion
}
