using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] float detectionRange = 20f;
	[SerializeField] float attackRange = 3f;
	[SerializeField] float maxHealth = 100f;
	[SerializeField] float damage = 10f;
	[SerializeField] float moveSpeed = 8f;
	[SerializeField] int coins = 10;

	[Header("Technical")]
	[SerializeField] EnemyDetection detection;
	[SerializeField] SpriteRenderer spriteRenderer;
	[SerializeField] GameObject damageNumberPrefab;

	private NavMeshAgent _agent;
	private float _health;

	// Damage Blink
	private float _blinkElapsedTime = 0f;
	private float _blinkDuration = 0.1f;

	private bool _isDead = false;

	private void Start()
	{
		_agent = GetComponentInChildren<NavMeshAgent>();
		_agent.speed = moveSpeed;
		_health = maxHealth;
		detection.SetRange(detectionRange);
	}

	private void Update()
	{
		GetTargetAndMove();

		HandleDamageBlink();
	}

	void GetTargetAndMove()
	{
		if (detection.target != null)
		{
			_agent.SetDestination(detection.target.position);
			if (Utilities.IsInRange(transform.position, detection.target.position, attackRange))
			{
				detection.target.GetComponentInChildren<SnakeSegment>().Damage(damage * Time.deltaTime);
			}
		}
		else
		{
			_agent.SetDestination(Snake.instance.HeadPosition);
		}
	}

	void HandleDamageBlink()
	{
		if (_blinkElapsedTime < _blinkDuration)
		{
			spriteRenderer.color = Color.Lerp(new(0.8f, 0.4f, 0.4f, 0.95f), Color.white, _blinkElapsedTime / _blinkDuration);
			_blinkElapsedTime += Time.deltaTime;
		}
	}

	public void Damage(float value, Turret origin, Vector3 projectileDirection)
	{
		_health -= value;
		_blinkElapsedTime = 0f;
		GameObject obj = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity);
		obj.GetComponent<DamageText>().Setup(value, projectileDirection);
		if (_health <= 0f && !_isDead)
		{
			_isDead = true;
			origin.RemoveFromTargets(this);
			EnemySpawner.instance.RemoveFromCap();
			GameManager.instance.AddCoins(coins);
			LevelManager.instance.SpawnCoinText(coins, transform.position);
			Destroy(gameObject);
		}
	}
}
