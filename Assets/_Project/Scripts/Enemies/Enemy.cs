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

	[Header("Technical")]
	[SerializeField] EnemyDetection detection;

	private NavMeshAgent _agent;
	private float _health;

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

	public void Damage(float value, Turret origin)
	{
		_health -= value;
		if (_health <= 0f && !_isDead)
		{
			_isDead = true;
			origin.RemoveFromTargets(this);
			EnemySpawner.instance.RemoveFromCap();
			Destroy(gameObject);
		}
	}
}
