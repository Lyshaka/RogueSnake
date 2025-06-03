using TMPro;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
	[Header("Buttons references and properties")]
	[SerializeField] string mainMenuPath;

	[Header("Snake Segment Sprite")]
	[SerializeField] float pixelDisplacement = 600f;
	[SerializeField] float smoothTime = 0.2f;
	[SerializeField] Transform snakeSegmentsTransform;
	[SerializeField] GameObject snakeSegmentPrefab;

	[Header("Upgrade Snake")]
	[SerializeField] TextMeshProUGUI titleTMP;

	private int _currentIndex;

	// Snake segment sprite displacement
	private int _length;
	private float _targetPos = 0f;
	private float _currentVelocity = 0f;

	private void Start()
	{
		// Spawn snake segments sprites
		_length = GameManager.instance.snakeProperties.snakeLength + 2; // Accounting for head and tail
		for (int i = 0; i < _length; i++)
		{
			GameObject obj = Instantiate(snakeSegmentPrefab, snakeSegmentsTransform.position - new Vector3(0f, pixelDisplacement * i, 0f), Quaternion.identity, snakeSegmentsTransform);
			SnakeSegmentUpgrade ssu = obj.GetComponent<SnakeSegmentUpgrade>();
			if (i == 0)
				ssu.SetSprites(SnakeSegmentUpgrade.SegmentType.Head);
			else if (i == _length - 1)
				ssu.SetSprites(SnakeSegmentUpgrade.SegmentType.Tail);
			else
				ssu.SetSprites(SnakeSegmentUpgrade.SegmentType.Segment);
		}
		_currentIndex = 0;
	}

	private void Update()
	{
		HandleSegmentSprites();
	}

	public void ButtonBack()
	{
		GameManager.instance.LoadScene(mainMenuPath);
	}

	public void ButtonValidate()
	{

	}

	public void ButtonUp()
	{
		if (_currentIndex > 0)
			_currentIndex--;

		UpdateSpriteInterface();
	}

	public void ButtonDown()
	{
		if (_currentIndex < _length - 1)
			_currentIndex++;

		UpdateSpriteInterface();
	}

	void HandleSegmentSprites()
	{
		snakeSegmentsTransform.localPosition = new(0f, Mathf.SmoothDamp(snakeSegmentsTransform.localPosition.y, _targetPos, ref _currentVelocity, smoothTime), 0f);
	}

	void UpdateSpriteInterface()
	{
		if (_currentIndex == 0)
			titleTMP.text = "Snake";
		else if (_currentIndex == _length - 1)
			titleTMP.text = "Tail";
		else
			titleTMP.text = "Segment #" + _currentIndex;

		_targetPos = pixelDisplacement * _currentIndex;
	}
}
