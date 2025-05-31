using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] float range = 8f;
	[SerializeField] float maxHealth;
	[SerializeField] float moveSpeed = 8f;

	[Header("Technical")]


	private NavMeshAgent _agent;
	private float _health;

	private void Start()
	{
		_agent = GetComponentInChildren<NavMeshAgent>();
		_health = maxHealth;
	}
}
