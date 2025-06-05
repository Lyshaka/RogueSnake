using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
	[Header("Turret")]
	[SerializeField] GameObject turretPrefab;

	[Header("Sprites")]
	[SerializeField] Sprite[] headSprites;
	[SerializeField] Sprite[] tailSprites;
	[SerializeField] Sprite[] straightSprites;
	[SerializeField] Sprite[] cornerSprites;

	[Header("VFX")]
	[SerializeField] ParticleSystem damageParticleSystem;

	private SpriteRenderer _spriteRenderer;
	private Snake.Segment _segment;

	private Turret _turret;

	Dictionary<(Vector2Int, Vector2Int), Sprite> _segmentSprites;

	private void Start()
	{
		_spriteRenderer = GetComponentInChildren<SpriteRenderer>();

		_segmentSprites = new()
		{
			{ (Vector2Int.up, Vector2Int.down), straightSprites[0] },
			{ (Vector2Int.down, Vector2Int.up), straightSprites[2] },
			{ (Vector2Int.left, Vector2Int.right), straightSprites[1] },
			{ (Vector2Int.right, Vector2Int.left), straightSprites[3] },

			{ (Vector2Int.up, Vector2Int.right), cornerSprites[3] },
			{ (Vector2Int.right, Vector2Int.up), cornerSprites[3] },

			{ (Vector2Int.up, Vector2Int.left), cornerSprites[2] },
			{ (Vector2Int.left, Vector2Int.up), cornerSprites[2] },

			{ (Vector2Int.down, Vector2Int.right), cornerSprites[0] },
			{ (Vector2Int.right, Vector2Int.down), cornerSprites[0] },

			{ (Vector2Int.down, Vector2Int.left), cornerSprites[1] },
			{ (Vector2Int.left, Vector2Int.down), cornerSprites[1] },
		};
	}

	public void Damage(float value)
	{
		damageParticleSystem.Play();
		Snake.instance.Damage(value);
	}

	public void UpdateSegment()
	{
		//Debug.Log($"{_segment.index}: prev={_segment.prev?.dir}, cur={_segment.dir}, next={_segment.next?.dir}");

		_spriteRenderer.sortingOrder = 500 - _segment.index;

		if (_segment.prev == null) // Head
		{
			if (_segment.dir == Vector2Int.up)
				_spriteRenderer.sprite = headSprites[0];
			else if (_segment.dir == Vector2Int.right)
				_spriteRenderer.sprite = headSprites[1];
			else if (_segment.dir == Vector2Int.down)
				_spriteRenderer.sprite = headSprites[2];
			else if (_segment.dir == Vector2Int.left)
				_spriteRenderer.sprite = headSprites[3];
		}
		else if (_segment.next == null) // Tail
		{
			if (_segment.prev.dir == Vector2Int.up)
				_spriteRenderer.sprite = tailSprites[0];
			else if (_segment.prev.dir == Vector2Int.right)
				_spriteRenderer.sprite = tailSprites[1];
			else if (_segment.prev.dir == Vector2Int.down)
				_spriteRenderer.sprite = tailSprites[2];
			else if (_segment.prev.dir == Vector2Int.left)
				_spriteRenderer.sprite = tailSprites[3];
		}
		else // All the others
		{
			Vector2Int dirToPrev = GetDirection(_segment.pos, _segment.prev.pos);
			Vector2Int dirToNext = GetDirection(_segment.pos, _segment.next.pos);

			if (_segmentSprites.TryGetValue((dirToPrev, dirToNext), out Sprite sprite))
				_spriteRenderer.sprite = sprite;
		}
	}

	public void SpawnTurret()
	{
		if (Snake.instance.IsMiddle(_segment))
		{
			GameObject obj = Instantiate(turretPrefab, _segment.obj.transform.position, Quaternion.identity, _segment.obj.transform);
			_turret = obj.GetComponentInChildren<Turret>();
			_turret.Setup(GameManager.instance.GetTurretProperties(_segment.index - 1));
		}
	}

	public void SetSegment(Snake.Segment segment)
	{
		_segment = segment;
	}

	private Vector2Int GetDirection(Vector2Int from, Vector2Int to)
	{
		Vector2Int delta = to - from;

		// Handle horizontal wrapping
		if (Mathf.Abs(delta.x) > LevelManager.instance.GridSize.x / 2)
			delta.x = (int)-Mathf.Sign(delta.x);

		// Handle vertical wrapping
		if (Mathf.Abs(delta.y) > LevelManager.instance.GridSize.y / 2)
			delta.y = (int)-Mathf.Sign(delta.y);

		// Clamp to valid cardinal directions
		if (delta.x != 0) delta.x = (int)Mathf.Sign(delta.x);
		if (delta.y != 0) delta.y = (int)Mathf.Sign(delta.y);

		return delta;
	}


}
