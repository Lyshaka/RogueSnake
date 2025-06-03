using UnityEngine;
using UnityEngine.Splines;

public class EnemySpawner : MonoBehaviour
{
	public static EnemySpawner instance;

	[Header("Properties")]
	[SerializeField] float spawnDelay = 0.2f;
	[SerializeField] int maxEnemies = 100;

	[Header("Enemies")]
	[SerializeField] GameObject enemyPrefab;


	Spline _spline;

	float _elapsedTime = 0f;
	[SerializeField] int _currentNumber = 0;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	private void Start()
	{
		_spline = GetComponent<SplineContainer>()[0];
	}

	private void Update()
	{
		HandleSpawn();
	}

	void HandleSpawn()
	{
		if (!GameManager.instance.StartedTimer)
			return;

		if (_elapsedTime < spawnDelay)
		{
			_elapsedTime += Time.deltaTime;
		}
		else
		{
			if (_currentNumber < maxEnemies)
			{
				Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity, transform);
				_currentNumber++;
			}
			_elapsedTime -= spawnDelay;
		}
	}

	public void RemoveFromCap()
	{
		_currentNumber--;
	}

	Vector3 GetRandomPosition()
	{
		return _spline.EvaluatePosition(Random.value);
	}
}
