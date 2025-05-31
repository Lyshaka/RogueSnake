using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;

	[Header("Properties")]
	[SerializeField] Vector2Int gridSize = new(60, 30);

	[Header("Enemies")]
	[SerializeField] SplineContainer enemySpline;

	[Header("Technical")]
	[SerializeField] TileBase groundTile;
	[SerializeField] Tilemap tilemapGrid;

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
		// Place Camera in the middle
		//Camera.main.transform.position = new(GridCenter.x, 50f, GridCenter.y);

		// Create Grid
		for (int i = 0; i < gridSize.x; i++)
		{
			for(int j = 0; j < gridSize.y; j++)
			{
				tilemapGrid.SetTile(new Vector3Int(i, j, 0), groundTile);
			}
		}
	}
}
