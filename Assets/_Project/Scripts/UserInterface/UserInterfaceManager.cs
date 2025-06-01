using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
	public static UserInterfaceManager instance;

	[Header("Health Bar")]
	[SerializeField] float healthMaskPixelsAmount = 1024f;
	[SerializeField] RectMask2D healthBarMask;
	[SerializeField] TextMeshProUGUI healthBarText;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	public void SetHealth(float health, float healthMax)
	{
		// Update health bar
		Vector4 padding = healthBarMask.padding;
		padding.z = healthMaskPixelsAmount * (1f - (health / healthMax));
		healthBarMask.padding = padding;

		// Update health bar text
		healthBarText.text = $"{health:0.0} / {healthMax:0.0}";
	}
}
