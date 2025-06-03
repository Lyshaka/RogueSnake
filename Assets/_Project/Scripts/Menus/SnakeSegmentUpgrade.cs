using System;
using UnityEngine;
using UnityEngine.UI;

public class SnakeSegmentUpgrade : MonoBehaviour
{
	[Header("Snake sprites")]
	[SerializeField] Sprite headSprite;
	[SerializeField] Sprite segmentSprite;
	[SerializeField] Sprite tailSprite;

	[Header("Image Renderers")]
	[SerializeField] Image segmentImage;
	[SerializeField] Image turretBaseImage;
	[SerializeField] Image turretCanonImage;

	public void SetSprites(SegmentType type)
	{
		switch (type)
		{
			case SegmentType.Head:
				segmentImage.sprite = headSprite;
				turretBaseImage.gameObject.SetActive(false);
				turretCanonImage.gameObject.SetActive(false);
				break;
			case SegmentType.Segment:
				segmentImage.sprite = segmentSprite;
				break;
			case SegmentType.Tail:
				segmentImage.sprite = tailSprite;
				turretBaseImage.gameObject.SetActive(false);
				turretCanonImage.gameObject.SetActive(false);
				break;
		}
	}

	[Serializable]
	public enum SegmentType
	{
		Head,
		Segment,
		Tail,
	}
}
