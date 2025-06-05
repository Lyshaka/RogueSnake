using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[Header("Scene paths")]
	[SerializeField] string gameScenePath;
	[SerializeField] string upgradeScenePath;
	[SerializeField] string statisticsScenePath;
	
	[Header("Scene paths")]
	[SerializeField] GameObject deletePanel;
	[SerializeField] TextMeshProUGUI deleteNotificationTMP;

	private float _elapsedTime = 0f;
	private float _duration = 2f;
	private float _decayDuration = 2f;

	private void Update()
	{
		if (_elapsedTime > 0f)
		{
			if (_elapsedTime < _decayDuration)
			{
				Color c = deleteNotificationTMP.color;
				c.a = _elapsedTime / _decayDuration;
				deleteNotificationTMP.color = c;
			}

			_elapsedTime -= Time.unscaledDeltaTime;
		}
		else
		{
			deleteNotificationTMP.gameObject.SetActive(false);
		}
	}

	public void ButtonPlay()
	{
		//Debug.Log("Play !");
		GameManager.instance.LoadScene(gameScenePath);
	}

	public void ButtonUpgrade()
	{
		//Debug.Log("Upgrade !");
		GameManager.instance.LoadScene(upgradeScenePath);
	}

	public void ButtonStatistics()
	{
		//Debug.Log("Statistics !");
		GameManager.instance.LoadScene(statisticsScenePath);
	}

	public void ButtonQuit()
	{
		//Debug.Log("Quit !");
		Application.Quit();
	}

	public void ButtonDelete()
	{
		deletePanel.SetActive(true);
	}

	public void ButtonDeleteYes()
	{
		GameManager.instance.DeleteAllData();
		Color c = deleteNotificationTMP.color;
		c.a = 1f;
		deleteNotificationTMP.color = c;
		deleteNotificationTMP.gameObject.SetActive(true);
		_elapsedTime = _duration + _decayDuration;
		deletePanel.SetActive(false);
	}

	public void ButtonDeleteNo()
	{
		deletePanel.SetActive(false);
	}
}
