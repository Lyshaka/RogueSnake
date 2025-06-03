using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
	[Header("Snake Segment Sprite")]
	[SerializeField] float pixelDisplacement = 600f;
	[SerializeField] float smoothTime = 0.2f;
	[SerializeField] Transform snakeSegmentsTransform;

	// Snake segment sprite displacement
	private float _targetPos = 0f;
	private float _currentVelocity = 0f;

	private void Update()
	{
		HandleSegmentSprites();
	}

	void HandleSegmentSprites()
	{
		snakeSegmentsTransform.localPosition = new(0f, Mathf.SmoothDamp(snakeSegmentsTransform.localPosition.y, _targetPos, ref _currentVelocity, smoothTime), 0f);
	}

	public void ButtonUp()
	{
		_targetPos -= pixelDisplacement;
		//snakeSegmentsTransform.position -= new Vector3(0f, pixelDisplacement, 0f);
	}

	public void ButtonDown()
	{
		_targetPos += pixelDisplacement;
		//snakeSegmentsTransform.position += new Vector3(0f, pixelDisplacement, 0f);
	}
}
