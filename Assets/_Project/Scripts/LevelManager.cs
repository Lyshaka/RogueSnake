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

	[Header("Technical")]
	[SerializeField] TileBase groundTile;
	[SerializeField] Tilemap tilemapGrid;

	private GameObject _coinObj;
	private Vector2Int _coinPosition;

	public Vector2Int GridCenter { get => new(gridSize.x / 2, gridSize.y / 2); }

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
	}

	public void TryEatCoin()
	{
		if (Snake.instance.HeadGridPosition == _coinPosition)
		{
			Debug.Log("+100 !");
			_coinPosition = new(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
			_coinObj.transform.position = new(_coinPosition.x, 0f, _coinPosition.y);
		}
	}
}
