using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField, Tooltip("Time before it shrinks")] float duration = 0.3f;
	[SerializeField, Tooltip("Time it takes to shrink, and after what it disappears")] float shrinkDuration = 0.2f;
	[SerializeField, Tooltip("Displacement in the direction")] float displacement = 1f;

	[Header("Technical")]
	[SerializeField] TextMeshPro tmp;

	float _elapsedTime;
	Vector3 _direction;

	private void Update()
	{
		if (_elapsedTime < (duration + shrinkDuration))
		{
			if (_elapsedTime > duration)
				tmp.transform.localScale = (1f - ((_elapsedTime - duration) / (shrinkDuration + duration))) * Vector3.one;
			tmp.transform.localPosition = (_elapsedTime / (duration + shrinkDuration)) * displacement * _direction;
			_elapsedTime += Time.deltaTime;
		}
		else
			Destroy(gameObject);
	}

	public void Setup(float value, Vector3 direction)
	{
		tmp.text = $"{value:0.}";
		_direction = direction;
	}
}
