using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[Header("Scene paths")]
	[SerializeField] string gameScenePath;
	[SerializeField] string upgradeScenePath;
	[SerializeField] string statisticsScenePath;

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
}
