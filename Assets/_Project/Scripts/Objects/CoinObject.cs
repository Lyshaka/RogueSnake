using UnityEngine;

public class CoinObject : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] float animDuration = 0.2f;
	[SerializeField] AnimationCurve animCurve;
	
	[Header("Technical")]
	[SerializeField] Transform coinTransform;
	[SerializeField] ParticleSystem coinParticle;

	float _elapsedTime = 0f;

	private void Update()
	{
		if (_elapsedTime > 0f)
		{
			coinTransform.localScale =  animCurve.Evaluate(1f - (_elapsedTime / animDuration)) * Vector3.one;
			_elapsedTime -= Time.deltaTime;
		}
	}

	public void PlayAnim()
	{
		_elapsedTime = animDuration;
		coinParticle.Play();
	}
}
