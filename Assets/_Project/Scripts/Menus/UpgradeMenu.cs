using TMPro;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
	public static UpgradeMenu instance;

	[Header("References and properties")]
	[SerializeField] string mainMenuPath;
	[SerializeField] TextMeshProUGUI moneyTMP;
	[SerializeField] TextMeshProUGUI validateButtonTMP;
	[SerializeField] PropertyUpgradePanel[] pups; // mdr

	[Header("Snake Segment Sprite")]
	[SerializeField] float pixelDisplacement = 600f;
	[SerializeField] float smoothTime = 0.2f;
	[SerializeField] Transform snakeSegmentsTransform;
	[SerializeField] GameObject snakeSegmentPrefab;

	[Header("Upgrade Snake")]
	[SerializeField] TextMeshProUGUI titleTMP;

	public GameManager.SnakeData newSnakeData;

	private int _currentMoneySpent;
	public int CurrentMoneySpent => _currentMoneySpent;

	private int _currentIndex;

	// Snake segment sprite displacement
	private int _length;
	private float _targetPos = 0f;
	private float _currentVelocity = 0f;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	private void Start()
	{
		// Load data
		GameManager.instance.LoadData();

		// Create a new snake data for the new values from the existing data
		newSnakeData = new(GameManager.instance.snakeData);

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

		// Update interface with all the info
		UpdateInterface();
	}

	private void Update()
	{
		HandleSegmentSprites();
	}

	public void AddMoneyExpense(int amount)
	{
		_currentMoneySpent += amount;
		UpdateInterface();
	}

	public void ButtonBack()
	{
		GameManager.instance.LoadScene(mainMenuPath);
	}

	public void ButtonValidate()
	{
		GameManager.instance.snakeData = newSnakeData;
		GameManager.instance.SpendMoney(_currentMoneySpent);
		_currentMoneySpent = 0;
		GameManager.instance.SaveData();
		UpdateInterface();
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

	void UpdateInterface()
	{
		moneyTMP.text = GameManager.instance.snakeData.money.ToString();

		if (_currentMoneySpent > 0)
			validateButtonTMP.text = $"VALIDATE <color=#FF0000>(-{_currentMoneySpent})";
		else
			validateButtonTMP.text = "VALIDATE";

		foreach (var pup in pups)
			pup.UpdateInterface();
	}
}
