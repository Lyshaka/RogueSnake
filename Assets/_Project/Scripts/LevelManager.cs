using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;

	[Header("Properties")]
	[SerializeField] Vector2Int gridSize = new(60, 30);

	[Header("Coins")]
	[SerializeField] int coinValue = 100;
	[SerializeField] GameObject coinPrefab;
	[SerializeField] GameObject coinFeedbackTextPrefab;

	[Header("Fruit")]
	[SerializeField] GameObject fruitPrefab;
	[SerializeField] GameObject fruitFeedbackTextPrefab;

	[Header("Technical")]
	[SerializeField] TileBase groundTile;
	[SerializeField] Tilemap tilemapGrid;

	// Coin
	private GameObject _coinObj;
	private CoinObject _coin;
	private Vector2Int _coinPosition;

	// Fruit
	private GameObject _fruitObj;
	private FruitObject _fruit;
	private Vector2Int _fruitPosition;
	private bool _fruitIsActive = false;

	public Vector2Int GridCenter { get => new(gridSize.x / 2, gridSize.y / 2); }
	public Vector2Int GridSize => gridSize;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	private void Start()
	{
		// Create Grid
		for (int i = 0; i < gridSize.x; i++)
		{
			for(int j = 0; j < gridSize.y; j++)
			{
				tilemapGrid.SetTile(new Vector3Int(i, j, 0), groundTile);
			}
		}

		// Spawn Coin
		_coinPosition = new(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
		_coinObj = Instantiate(coinPrefab, new(_coinPosition.x, 0f, _coinPosition.y), Quaternion.identity, transform);
		_coin = _coinObj.GetComponent<CoinObject>();

		// Spawn Fruit
		_fruitPosition = new(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
		_fruitObj = Instantiate(fruitPrefab, new(_fruitPosition.x, 0f, _fruitPosition.y), Quaternion.identity, transform);
		_fruit = _fruitObj.GetComponent<FruitObject>();
		_fruitIsActive = true;
		_fruitObj.SetActive(_fruitIsActive);
	}

	public void TryEatCoin()
	{
		if (Snake.instance.HeadGridPosition == _coinPosition)
		{
			GameManager.instance.AddCoins(coinValue);
			SpawnCoinText(coinValue, Utilities.GridToWorld(_coinPosition));
			_coinPosition = new(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
			_coinObj.transform.position = Utilities.GridToWorld(_coinPosition);
			_coin.PlayAnim();

			// Spawn a fruit (maybe ?)
			if (!_fruitIsActive)
			{
				_fruitIsActive = true;
				_fruitObj.SetActive(_fruitIsActive);
				_fruitPosition = new(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
				_fruitObj.transform.position = Utilities.GridToWorld(_fruitPosition);
				_fruit.PlayAnim();
			}
		}

		if (_fruitIsActive && Snake.instance.HeadGridPosition == _fruitPosition)
		{
			Snake.instance.Heal(100f);
			SpawnHealText(100f, Utilities.GridToWorld(_fruitPosition));
			_fruit.StopAnim();
			_fruitIsActive = false;
			_fruitObj.SetActive(_fruitIsActive);
		}
	}

	public void SpawnCoinText(int value, Vector3 position)
	{
		GameObject obj = Instantiate(coinFeedbackTextPrefab, position, Quaternion.identity, GameManager.instance.FeedbackTextParent);
		obj.GetComponent<CoinText>().Setup(value);
	}

	public void SpawnHealText(float value, Vector3 position)
	{
		GameObject obj = Instantiate(fruitFeedbackTextPrefab, position, Quaternion.identity, GameManager.instance.FeedbackTextParent);
		obj.GetComponent<HealText>().Setup(value);
	}
}
