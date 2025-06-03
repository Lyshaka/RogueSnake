using System;
using System.Collections;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[Header("Transform parents")]
	[SerializeField] Transform bulletParent;
	[SerializeField] Transform feedbacksTextParent;

	[Header("Level loading")]
	[SerializeField] Image fadeImage;
	[SerializeField] float fadeInDuration = 0.2f;
	[SerializeField] float fadeOutDuration = 0.2f;

	[Header("Data management")]
	[SerializeField] string snakeDataCSVPath;
	[SerializeField] string snakeDataFileName = "snake.data";


	// Data
	[HideInInspector] public SnakeData snakeData = new();
	[HideInInspector] public SnakeProperties snakeProperties;
	private string[,] _snakeDataFromCSV;

	// Coins
	[HideInInspector] public int coins = 0;

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

	private void Start()
	{
		LoadProperties();

		LoadData();
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

	#region COINS
	public void AddCoins(int value)
	{
		coins += value;
		UserInterfaceManager.instance.SetCoin(coins);
	}
	#endregion

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

	#region DATA

	public void LoadProperties()
	{
		TextAsset temp = Resources.Load<TextAsset>(snakeDataCSVPath);
		string[] tempArr = temp.text.Split("\r\n");

		int col = tempArr[0].Split(",").Length - 1;
		int row = tempArr.Length - 1;

		//Debug.Log("Col : " + col);
		//Debug.Log("Row : " + row);

		_snakeDataFromCSV = new string[row, col];

		for (int i = 0; i < row; i++)
		{
			for (int j = 0; j < col; j++)
			{
				_snakeDataFromCSV[i,j] = tempArr[i + 1].Split(",")[j + 1];
				//Debug.Log($"Data {i},{j} : {_snakeDataFromCSV[i, j]}");
			}
		}
	}

	public void LoadData()
	{
		string savePath = Path.Combine(Application.persistentDataPath, snakeDataFileName);

		if (!File.Exists(savePath))
		{
			snakeData = new();
			snakeProperties = new(snakeData, _snakeDataFromCSV);
			SaveData();
			return;
		}

		string json = File.ReadAllText(savePath);
		snakeData = JsonUtility.FromJson<SnakeData>(json);
		snakeProperties = new(snakeData, _snakeDataFromCSV);
		Debug.Log("Game Loaded from : " + savePath + " !\n" + json);
	}

	public void SaveData()
	{
		string savePath = Path.Combine(Application.persistentDataPath, snakeDataFileName);

		string json = JsonUtility.ToJson(snakeData, true);
		File.WriteAllText(savePath, json);
		Debug.Log("Game Saved at : " + savePath + " !\n" + json);
	}

	public class SnakeProperties
	{
		// Properties
		public float maxHealth = 1000f;
		public int snakeLength = 1; // Doesn't include head and tail
		public int coinValue = 100; // Amount of money a coin gets
		public float spawnFruitChance = 0.1f; // Value between 0 and 1 (percentage)
		public float fruitValue = 100f; // Amount of health a fruit gets

		// Costs
		public int maxHealthCost;
		public int snakeLengthCost;
		public int coinValueCost;
		public int spawnFruitChanceCost;
		public int fruitValueCost;

		public SnakeProperties(SnakeData data, string[,] csvData)
		{
			maxHealth = float.Parse(csvData[data.healthLevel, 0], CultureInfo.InvariantCulture);
			maxHealthCost = int.Parse(csvData[data.healthLevel, 1], CultureInfo.InvariantCulture);

			snakeLength = int.Parse(csvData[data.lengthLevel, 2], CultureInfo.InvariantCulture);
			snakeLengthCost = int.Parse(csvData[data.lengthLevel, 3], CultureInfo.InvariantCulture);
			
			coinValue = int.Parse(csvData[data.coinValueLevel, 4], CultureInfo.InvariantCulture);
			coinValueCost = int.Parse(csvData[data.coinValueLevel, 5], CultureInfo.InvariantCulture);

			spawnFruitChance = float.Parse(csvData[data.fruitChanceLevel, 6], CultureInfo.InvariantCulture);
			spawnFruitChanceCost = int.Parse(csvData[data.fruitChanceLevel, 7], CultureInfo.InvariantCulture);

			fruitValue = float.Parse(csvData[data.fruitValueLevel, 8], CultureInfo.InvariantCulture);
			fruitValueCost = int.Parse(csvData[data.fruitValueLevel, 9], CultureInfo.InvariantCulture);
		}
	}

	public class SnakeData
	{
		public int money = 0;

		public int healthLevel = 0;
		public int lengthLevel = 0;
		public int coinValueLevel = 0;
		public int fruitChanceLevel = 0;
		public int fruitValueLevel = 0;
	}

	#endregion
}
