using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] float range = 10f;
	[SerializeField] float attackFrequency = 3f;
	[SerializeField] float damage = 10f;
	[SerializeField] float projectileSpeed = 3f;

	[Header("Technical")]
	[SerializeField] Transform canonTransform;
	[SerializeField] GameObject bulletPrefab;

	private List<Transform> _enemiesInRange = new();
	private Transform _target;

	private float _shootDelay;
	private float _shootElapsedTime;

	private void Start()
	{
		GetComponent<SphereCollider>().radius = range;
		_shootDelay = 1f / attackFrequency;
	}

	private void Update()
	{
		_enemiesInRange.RemoveAll(item => item == null);
		_target = Utilities.GetNearest(transform.position, _enemiesInRange);

		if (_target != null)
		{
			canonTransform.rotation = Quaternion.LookRotation(_target.position - transform.position, Vector3.up);
		}
	}

	private void FixedUpdate()
	{
		if (_shootElapsedTime < _shootDelay)
		{
			_shootElapsedTime += Time.deltaTime;
		}
		else
		{
			if (_target != null)
			{
				GameObject obj = Instantiate(bulletPrefab, transform.position, canonTransform.rotation);
				obj.GetComponent<Bullet>().Setup(damage, projectileSpeed, this);
			}
			_shootElapsedTime -= _shootDelay;
		}
	}

	public void RemoveFromTargets(Enemy enemy)
	{
		if (_enemiesInRange.Contains(enemy.transform))
			_enemiesInRange.Remove(enemy.transform);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 10) // Enemy
		{
			if (!_enemiesInRange.Contains(other.transform))
				_enemiesInRange.Add(other.transform);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 10) // Enemy
		{
			if (_enemiesInRange.Contains(other.transform))
				_enemiesInRange.Remove(other.transform);
		}
	}
}
