using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] float range = 8f;
	[SerializeField] float maxHealth = 100f;
	[SerializeField] float moveSpeed = 8f;

	[Header("Technical")]
	[SerializeField] EnemyDetection detection;

	private NavMeshAgent _agent;
	private float _health;

	private void Start()
	{
		_agent = GetComponentInChildren<NavMeshAgent>();
		_agent.speed = moveSpeed;
		_health = maxHealth;
		detection.SetRange(range);
	}

	private void Update()
	{
		if (detection.target != null)
		{
			_agent.SetDestination(detection.target.position);
		}
		else
		{
			_agent.SetDestination(Snake.instance.HeadPosition);
		}

	}

	public void Damage(float value, Turret origin)
	{
		_health -= value;
		if (_health <= 0f)
		{
			origin.RemoveFromTargets(this);
			Destroy(gameObject);
		}
	}
}
