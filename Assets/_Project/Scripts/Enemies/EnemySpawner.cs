using UnityEngine;
using UnityEngine.Splines;

public class EnemySpawner : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] float spawnDelay = 0.2f;

	[Header("Enemies")]
	[SerializeField] GameObject enemyPrefab;


	Spline _spline;

	float _elapsedTime = 0f;

	private void Start()
	{
		_spline = GetComponent<SplineContainer>()[0];
	}

	private void Update()
	{
		if (_elapsedTime < spawnDelay)
		{
			_elapsedTime += Time.deltaTime;
		}
		else
		{
			Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity, transform);
			_elapsedTime -= spawnDelay;
		}
	}

	Vector3 GetRandomPosition()
	{
		return _spline.EvaluatePosition(Random.value);
	}
}
