using UnityEngine;

public class FruitObject : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] float animDuration = 0.2f;
	[SerializeField] AnimationCurve animCurve;

	[Header("Technical")]
	[SerializeField] Transform fruitTransform;
	[SerializeField] ParticleSystem fruitParticle;

	float _elapsedTime = 0f;

	private void Update()
	{
		if (_elapsedTime > 0f)
		{
			fruitTransform.localScale = animCurve.Evaluate(1f - (_elapsedTime / animDuration)) * Vector3.one;
			_elapsedTime -= Time.deltaTime;
		}
	}

	public void PlayAnim()
	{
		_elapsedTime = animDuration;
		fruitParticle.Play();
	}

	public void StopAnim()
	{
		fruitParticle.Stop();
	}
}
