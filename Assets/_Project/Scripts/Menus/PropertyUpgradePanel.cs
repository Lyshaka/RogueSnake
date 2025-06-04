using TMPro;
using UnityEngine;

public class PropertyUpgradePanel : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] GameManager.DataType type;

	[Header("Technical")]
	[SerializeField] TextMeshProUGUI levelTMP;
	[SerializeField] TextMeshProUGUI contentTMP;
	[SerializeField] TextMeshProUGUI valueTMP;
	[SerializeField] TextMeshProUGUI upgradedValueTMP;
	[SerializeField] TextMeshProUGUI costTMP;
	[SerializeField] GameObject buttonObject;

	private int _level;
	private int _currentCost;
	private GameManager.SnakeData _snakeData;

	private void Start()
	{
		_snakeData = UpgradeMenu.instance.newSnakeData;
		_level = _snakeData.GetLevel(type);
		string[] properties = GameManager.instance.GetProperties(type, _snakeData);
		SetValues(properties);
	}

	public void AddLevel()
	{
		if (_currentCost + UpgradeMenu.instance.CurrentMoneySpent > GameManager.instance.snakeData.money)
			return;

		_level++;
		_snakeData.SetLevel(type, _level);
		UpgradeMenu.instance.AddMoneyExpense(_currentCost);
		string[] properties = GameManager.instance.GetProperties(type, _snakeData);
		SetValues(properties);
	}

	public void UpdateInterface()
	{
		if (_currentCost + UpgradeMenu.instance.CurrentMoneySpent > GameManager.instance.snakeData.money)
			costTMP.text = "<color=#FF0000>" + _currentCost;
		else
			costTMP.text = "" + _currentCost;
	}

	void SetValues(string[] values)
	{
		levelTMP.text = values[0];
		contentTMP.text = values[1];
		valueTMP.text = values[2];
		upgradedValueTMP.text = values[3];
		if (values[4] != "")
		{
			costTMP.text = values[4];
			_currentCost = int.Parse(values[4]);
		}
		else
		{
			buttonObject.SetActive(false);
		}

			UpdateInterface();
	}
}
