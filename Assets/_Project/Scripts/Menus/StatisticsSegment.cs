using TMPro;
using UnityEngine;

public class StatisticsSegment : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] Utilities.ParseType parseType;
	[SerializeField, Tooltip("Spacing for time parsing")] int spacing = -1;
	
	[Header("TMP Reference")]
	[SerializeField] TextMeshProUGUI statTMP;

	public void SetValue(object value)
	{
		statTMP.text = Utilities.Parse(value, parseType, spacing);
	}
}
