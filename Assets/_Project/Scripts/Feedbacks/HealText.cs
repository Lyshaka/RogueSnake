using TMPro;
using UnityEngine;

public class HealText : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField, Tooltip("Time before it shrinks")] float duration = 0.5f;
	[SerializeField, Tooltip("Displacement in the direction")] float displacement = 1f;

	[Header("Technical")]
	[SerializeField] TextMeshPro tmp;

	float _elapsedTime;

	private void Update()
	{
		if (_elapsedTime < duration)
		{
			tmp.transform.localPosition = (_elapsedTime / duration) * displacement * Vector3.forward;
			Color c = tmp.color;
			c.a = 1f - (_elapsedTime / duration);
			tmp.color = c;
			_elapsedTime += Time.deltaTime;
		}
		else
			Destroy(gameObject);
	}

	public void Setup(float value)
	{
		tmp.text = $"+{value:0.}<space=5.5><voffset=4><sprite=0>";
	}
}
