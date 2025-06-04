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

	public const int MAX_LEVEL = 50;

	[Header("Transform parents")]
	[SerializeField] Transform bulletParent;
	[SerializeField] Transform feedbacksTextParent;

	[Header("Level loading")]
	[SerializeField] Image fadeImage;
	[SerializeField] float fadeInDuration = 0.1f;
	[SerializeField] float waitDuration = 0.1f;
	[SerializeField] float fadeOutDuration = 0.1f;

	[Header("Data management")]
	[SerializeField] string snakeDataCSVPath;
	[SerializeField] string snakeDataFileName = "snake.data";


	// Data
	[HideInInspector] public SnakeData snakeData = new();
	[HideInInspector] public SnakeProperties snakeProperties;
	private string[,] _snakeDataFromCSV;

	// Coins
	private int _coins = 0;

	// Timer
	private bool _startedTimer = false;
	private bool _isGameOver = false;
	private float _timer = 0f;
	public bool StartedTimer => _startedTimer;

	// Scene loading
	private bool _isLevelLoading = false;
	private Color _fadeImageBaseColor;

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
		_fadeImageBaseColor = fadeImage.color;

		LoadProperties();

		LoadData();
	}

	private void Update()
	{
		HandleStartGame();

		HandleTimer();
	}

	public void EnableGameOver()
	{
		_startedTimer = false;
		_isGameOver = true;

		Debug.Log("Timer : " + _timer);
		Debug.Log("Coins : " + _coins);

		// Update game over panel
		UserInterfaceManager.instance.EnableGameOver(_timer, _coins, 0);

		// Save data
		snakeData.money += _coins;
		SaveData();
		
		// Reset data
		_timer = 0f;
		_coins = 0;

		// Pause the game
		Time.timeScale = 0f;
	}

	void HandleStartGame()
	{
		if ((!_startedTimer && !_isGameOver) &&
			(Input.GetKey(KeyCode.RightArrow) ||
			Input.GetKey(KeyCode.LeftArrow) ||
			Input.GetKey(KeyCode.DownArrow) ||
			Input.GetKey(KeyCode.UpArrow)))
		{
			_startedTimer = true;
		}
	}

	#region TIMER

	void HandleTimer()
	{
		if (!_startedTimer || _isGameOver)
			return;

		_timer += Time.deltaTime;
		UserInterfaceManager.instance.SetTimer(_timer);
	}

	#endregion

	#region MONEY_&_COINS
	public void AddCoins(int value)
	{
		_coins += value;
		UserInterfaceManager.instance.SetCoin(_coins);
	}

	public void SpendMoney(int amount)
	{
		snakeData.money -= amount;
		SaveData();
	}

	public bool CanSpend(int amount)
	{
		return snakeData.money >= amount;
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
		if (fadeInDuration > 0f)
		{
			elapsedTime = 0f;
			while (elapsedTime < fadeInDuration)
			{
				_fadeImageBaseColor.a = elapsedTime / fadeInDuration;
				fadeImage.color = _fadeImageBaseColor;
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
			_fadeImageBaseColor.a = 1f;
			fadeImage.color = _fadeImageBaseColor; // Set the color to full opacity
		}

		// Skip a frame (just to be sure)
		yield return null;

		// Destroy all projectiles
		for (int i = bulletParent.childCount - 1; i >= 0; i--)
			Destroy(bulletParent.GetChild(i).gameObject);

		AsyncOperation asyncOperation =	SceneManager.LoadSceneAsync(path);

		while (!asyncOperation.isDone)
		{
			// Wait for the scene loading to finish
			yield return null;
		}

		// Wait on the black screen for a certain time
		yield return new WaitForSecondsRealtime(waitDuration);

		// Fade screen from black
		if (fadeOutDuration > 0f)
		{
			elapsedTime = 0f;
			while (elapsedTime < fadeOutDuration)
			{
				_fadeImageBaseColor.a = 1f - (elapsedTime / fadeOutDuration);
				fadeImage.color = _fadeImageBaseColor;
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
		}
		_fadeImageBaseColor.a = 0f;
		fadeImage.color = _fadeImageBaseColor; // Set the color to full transparency

		// Skip a frame (just to be sure)
		yield return null;

		// Set time scale back to normal
		Time.timeScale = 1f;

		// Reset game over flag
		_isGameOver = false;

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

		Debug.Log("Col : " + col);
		Debug.Log("Row : " + row);

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

	public string[] GetProperties(DataType type, SnakeData snakeData)
	{
		string[] str = new string[5];
		int level;

		switch (type)
		{
			case DataType.Health:
				level = snakeData.healthLevel;
				str[0] = level.ToString();
				str[1] = "Maximum Health";
				str[2] = _snakeDataFromCSV[level, 0];
				str[3] = level < MAX_LEVEL ? $"(+{float.Parse(_snakeDataFromCSV[level + 1, 0]) - float.Parse(_snakeDataFromCSV[level, 0])})" : "";
				str[4] = level < MAX_LEVEL ? _snakeDataFromCSV[level + 1, 1] : "";
				break;
			case DataType.Length:
				level = snakeData.lengthLevel;
				str[0] = level.ToString();
				str[1] = "Snake Length";
				str[2] = _snakeDataFromCSV[level, 2];
				str[3] = level < MAX_LEVEL ? $"(+{int.Parse(_snakeDataFromCSV[level + 1, 2]) - int.Parse(_snakeDataFromCSV[level, 2])})" : "";
				str[4] = level < MAX_LEVEL ? _snakeDataFromCSV[level + 1, 3] : "";
				break;
			case DataType.CoinValue:
				level = snakeData.coinValueLevel;
				str[0] = level.ToString();
				str[1] = "Coin Value";
				str[2] = _snakeDataFromCSV[level, 4];
				str[3] = level < MAX_LEVEL ? $"(+{int.Parse(_snakeDataFromCSV[level + 1, 4]) - int.Parse(_snakeDataFromCSV[level, 4])})" : "";
				str[4] = level < MAX_LEVEL ? _snakeDataFromCSV[level + 1, 5] : "";
				break;
			case DataType.FruitChance:
				level = snakeData.fruitChanceLevel;
				str[0] = level.ToString();
				str[1] = "Fruit Chance";
				str[2] = $"{(float.Parse(_snakeDataFromCSV[level, 6], CultureInfo.InvariantCulture) * 100f):0.0#}%";
				str[3] = level < MAX_LEVEL ? $"(+{((float.Parse(_snakeDataFromCSV[level + 1, 6], CultureInfo.InvariantCulture) - float.Parse(_snakeDataFromCSV[level, 6], CultureInfo.InvariantCulture)) * 100f):0.0#}%)" : "";
				str[4] = level < MAX_LEVEL ? _snakeDataFromCSV[level + 1, 7] : "";
				break;
			case DataType.FruitValue:
				level = snakeData.fruitValueLevel;
				str[0] = level.ToString();
				str[1] = "Fruit Value";
				str[2] = _snakeDataFromCSV[level, 8];
				str[3] = level < MAX_LEVEL ? $"(+{int.Parse(_snakeDataFromCSV[level + 1, 8]) - int.Parse(_snakeDataFromCSV[level, 8])})" : "";
				str[4] = level < MAX_LEVEL ? _snakeDataFromCSV[level + 1, 9] : "";
				break;
		}

		return str;
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

		public SnakeData() { }

		public SnakeData(SnakeData other)
		{
			money = other.money;
			healthLevel = other.healthLevel;
			lengthLevel = other.lengthLevel;
			coinValueLevel = other.coinValueLevel;
			fruitChanceLevel = other.fruitChanceLevel;
			fruitValueLevel = other.fruitValueLevel;
		}

		public int GetLevel(DataType type)
		{
			return type switch
			{
				DataType.Health => healthLevel,
				DataType.Length => lengthLevel,
				DataType.CoinValue => coinValueLevel,
				DataType.FruitChance => fruitChanceLevel,
				DataType.FruitValue => fruitValueLevel,
				_ => 0,
			};
		}

		public void SetLevel(DataType type, int level)
		{
			switch (type)
			{
				case DataType.Health:
					healthLevel = level;
					break;
				case DataType.Length:
					lengthLevel = level;
					break;
				case DataType.CoinValue:
					coinValueLevel = level;
					break;
				case DataType.FruitChance:
					fruitChanceLevel = level;
					break;
				case DataType.FruitValue:
					fruitValueLevel = level;
					break;
			}
		}
	}

	public enum DataType
	{
		Health,
		Length,
		CoinValue,
		FruitChance,
		FruitValue,
	}

	#endregion
}
