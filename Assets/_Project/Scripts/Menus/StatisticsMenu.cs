using UnityEngine;

public class StatisticsMenu : MonoBehaviour
{
	[Header("Totals references")]
	[SerializeField] StatisticsSegment totalPlaytime;
	[SerializeField] StatisticsSegment totalMoneyEarned;
	[SerializeField] StatisticsSegment totalMoneySpent;
	[SerializeField] StatisticsSegment totalEnemiesKilled;
	[SerializeField] StatisticsSegment totalTurretDamage;

	[Header("High scores references")]
	[SerializeField] StatisticsSegment longestPlaytime;
	[SerializeField] StatisticsSegment mostMoneyEarned;
	[SerializeField] StatisticsSegment mostEnemiesKilled;
	[SerializeField] StatisticsSegment mostTurretDamage;

	[Header("Main Menu path")]
	[SerializeField] string mainMenuPath;

	private void Start()
	{
		totalPlaytime.SetValue(GameManager.instance.snakeStats.totalPlaytime);
		totalMoneyEarned.SetValue(GameManager.instance.snakeStats.totalMoneyEarned);
		totalMoneySpent.SetValue(GameManager.instance.snakeStats.totalMoneySpent);
		totalEnemiesKilled.SetValue(GameManager.instance.snakeStats.totalEnemiesKilled);
		totalTurretDamage.SetValue(GameManager.instance.snakeStats.totalTurretDamage);
		longestPlaytime.SetValue(GameManager.instance.snakeStats.longestPlaytime);
		mostMoneyEarned.SetValue(GameManager.instance.snakeStats.mostMoneyEarned);
		mostEnemiesKilled.SetValue(GameManager.instance.snakeStats.mostEnemiesKilled);
		mostTurretDamage.SetValue(GameManager.instance.snakeStats.mostTurretDamage);
	}

	public void BackToMenu()
	{
		GameManager.instance.LoadScene(mainMenuPath);
	}
}
