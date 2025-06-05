using UnityEngine;

public class Snake : MonoBehaviour
{
	public static Snake instance;

	[Header("Properties")]
	[SerializeField] float speed = 3f;

	[Header("Technical")]
	[SerializeField] GameObject snakeSpritePrefab;

	private bool _isKilled = false;

	private float _health;

	private Segment _head;
	private Segment _tail;

	private Vector2Int _moveInput;
	private Vector2Int _currentDir;

	private float _loopDuration;

	private float _loopElapsedTime = 0f;

	public Vector3 HeadPosition => _head.obj.transform.position;
	public Vector2Int HeadGridPosition => _head.pos;

	public bool IsHead(Segment s) => s.prev == null;
	public bool IsTail(Segment s) => s.next == null;
	public bool IsMiddle(Segment s) => s.next != null && s.prev != null;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	private void Start()
	{
		// Init
		GameManager.instance.LoadData();
		_currentDir = Vector2Int.zero;
		_loopDuration = 1f / speed;
		_health = GameManager.instance.snakeProperties.maxHealth;
		UserInterfaceManager.instance.SetHealth(_health, GameManager.instance.snakeProperties.maxHealth);

		// Spawn snake in the center of the grid
		_head = new(LevelManager.instance.GridCenter, snakeSpritePrefab, transform);
		_head.obj.name = "Segment_" + _head.index;
		_tail = _head;

		for (int i = 0; i < GameManager.instance.snakeProperties.snakeLength + 1; i++)
		{
			AddSegment();
		}

		// Spawn Turrets
		Segment current = _head;
		while (current != _tail)
		{
			current.SpawnTurret();
			current = current.next;
		}
	}

	private void Update()
	{
		// Inputs
		if (Input.GetKey(KeyCode.RightArrow) && _head.dir != Vector2Int.left)
			_moveInput = Vector2Int.right;
		else if (Input.GetKey(KeyCode.LeftArrow) && _head.dir != Vector2Int.right)
			_moveInput = Vector2Int.left;
		else if (Input.GetKey(KeyCode.UpArrow) && _head.dir != Vector2Int.down)
			_moveInput = Vector2Int.up;
		else if (Input.GetKey(KeyCode.DownArrow) && _head.dir != Vector2Int.up)
			_moveInput = Vector2Int.down;
		else
			_moveInput = Vector2Int.zero;

		// Debug, not meant to stay
		if (Input.GetKeyDown(KeyCode.KeypadPlus))
		{
			AddSegment();
		}

		//Debug.Log("Input : " + _moveInput);

		if (_moveInput != Vector2Int.zero)
			_currentDir = _moveInput;

		//if (!GameManager.instance.StartedTimer && _currentDir != Vector2Int.zero)
		//{
		//	GameManager.instance.StartGame();
		//}

		if (_loopElapsedTime < _loopDuration)
		{
			_loopElapsedTime += Time.deltaTime;
		}
		else
		{
			// Logic here
			// Move snake step by step
			MoveSnake();

			_loopElapsedTime -= _loopDuration;
		}
	}

	public void Damage(float value)
	{
		_health -= value;
		if (_health <= 0f)
		{
			_health = 0f;
			Kill();
		}
		UserInterfaceManager.instance.SetHealth(_health, GameManager.instance.snakeProperties.maxHealth);
	}

	public void Heal(float value)
	{
		_health += value;
		if (_health > GameManager.instance.snakeProperties.maxHealth)
			_health = GameManager.instance.snakeProperties.maxHealth;
		UserInterfaceManager.instance.SetHealth(_health, GameManager.instance.snakeProperties.maxHealth);
	}

	void Kill()
	{
		if (!_isKilled)
		{
			_isKilled = true;
			GameManager.instance.EnableGameOver();
		}
	}

	void AddSegment()
	{
		Segment newSegment = new(_tail.pos, snakeSpritePrefab, transform)
		{
			pos = _tail.pos,
			dir = _tail.dir,
			prev = _tail,
			next = null,
			index = _tail.index + 1
		};
		newSegment.obj.name = "Segment_" + newSegment.index;
		_tail.next = newSegment;
		_tail = newSegment;
	}

	void MoveSnake()
	{
		//Debug.Log("Move !");

		// Move each segment
		Segment current = _tail;
		while (current != _head)
		{
			current.pos = current.prev.pos;
			current.dir = current.prev.dir;
			current = current.prev;
		}

		// Move the head to the new position
		_head.dir = _currentDir;
		_head.pos += _head.dir;
		_head.obj.transform.position += new Vector3(_head.dir.x, 0f, _head.dir.y);

		// Warp the head around the circuit
		if (_head.pos.x < 0) _head.pos.x = LevelManager.instance.GridSize.x - 1;
		if (_head.pos.x >= LevelManager.instance.GridSize.x) _head.pos.x = 0;
		if (_head.pos.y < 0) _head.pos.y = LevelManager.instance.GridSize.y - 1;
		if (_head.pos.y >= LevelManager.instance.GridSize.y) _head.pos.y = 0;

		// Update each segment sprite and object position
		current = _head;
		while (current != null)
		{
			current.obj.transform.position = new(current.pos.x, 0f, current.pos.y);
			current.UpdateSegment();
			current = current.next;
		}

		// Check if the snake hit a wall
		//if (_head.pos.x < 0 || _head.pos.x >= LevelManager.instance.GridSize.x ||
		//	_head.pos.y < 0 || _head.pos.y >= LevelManager.instance.GridSize.y)
		//{
		//	Kill();
		//}

		// Check if the snake hit itself
		current = _head.next;

		while (current != null)
		{
			if (_head.pos == current.pos && _currentDir != Vector2Int.zero)
			{
				Kill();
			}
			current = current.next;
		}

		// Check if the snake is on the coin
		LevelManager.instance.TryEatCoin();
	}


	public class Segment
	{
		public Vector2Int pos;
		public Vector2Int dir = Vector2Int.zero;
		public GameObject obj;
		public Segment prev = null;
		public Segment next = null;
		public int index = 0;

		private readonly SnakeSegment _snakeSegment;

		public Segment(Vector2Int pos, GameObject prefab, Transform parent)
		{
			this.pos = pos;
			obj = Instantiate(prefab, parent);
			obj.transform.position = new(pos.x, 0f, pos.y);
			_snakeSegment = obj.GetComponent<SnakeSegment>();
			_snakeSegment.SetSegment(this);
		}

		public void SpawnTurret()
		{
			_snakeSegment.SpawnTurret();
		}

		public void UpdateSegment()
		{
			_snakeSegment.UpdateSegment();
		}
	}
}
