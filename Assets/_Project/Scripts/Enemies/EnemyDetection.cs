using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
	private List<Transform> _segmentInRange = new();
	private Transform _target;

	public Transform target => _target;

	private void Update()
	{
		_segmentInRange.RemoveAll(item => item == null);
		_target = Utilities.GetNearest(transform.position, _segmentInRange);
	}

	public void SetRange(float range)
	{
		GetComponent<SphereCollider>().radius = range;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 3) // Snake Segment
		{
			if (!_segmentInRange.Contains(other.transform))
				_segmentInRange.Add(other.transform);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 3) // Snake Segment
		{
			if (_segmentInRange.Contains(other.transform))
				_segmentInRange.Remove(other.transform);
		}
	}
}
