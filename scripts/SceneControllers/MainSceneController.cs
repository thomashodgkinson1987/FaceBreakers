using System.Collections.Generic;
using Godot;

public class MainSceneController : Node2D
{

	#region Nodes

	private GameHUDController node_gameHUDController;

	private Node2D node_actors;
	private Node2D node_enemies;

	private Position2D node_enemiesLeftBounds;
	private Position2D node_enemiesRightBounds;

	private Player node_player;

	private Position2D node_playerSpawnPosition;

	private Position2D node_alienSpawnPosition_left;
	private Position2D node_alienSpawnPosition_right;
	private Node2D node_aliens;
	private Timer node_alienSpawnTimer;

	private Node2D node_background;

	#endregion // Nodes



	#region Properties

	[Export] public float EnemyMoveSpeed { get; set; } = 1f;
	[Export] public int EnemyMoveDirection { get; set; } = 1;
	[Export] public float EnemyMoveDistance { get; set; } = 8f;

	private int _lives = 3;

	public int Lives
	{
		get => _lives;
		set
		{
			_lives = value;
			node_gameHUDController.Lives = value;
		}
	}

	private int _score = 0;

	public int Score
	{
		get => _score;
		set
		{
			_score = value;
			node_gameHUDController.Score = value;
		}
	}

	#endregion // Properties



	#region Fields

	[Export] private PackedScene m_packedScene_redAlien;
	[Export] private PackedScene m_packedScene_purpleAlien;

	private float m_enemyMoveTimer = 0f;

	private List<string> m_enemiesAlive = new List<string>();
	private List<string> m_enemiesHit = new List<string>();

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_gameHUDController = GetNode<GameHUDController>("GameHUD");

		node_actors = GetNode<Node2D>("Actors");
		node_enemies = node_actors.GetNode<Node2D>("Enemies");

		node_enemiesLeftBounds = node_enemies.GetNode<Position2D>("Left");
		node_enemiesRightBounds = node_enemies.GetNode<Position2D>("Right");

		node_player = node_actors.GetNode<Player>("Player");

		node_playerSpawnPosition = GetNode<Position2D>("PlayerSpawnPosition");

		node_alienSpawnPosition_left = GetNode<Position2D>("AlienSpawnPositionLeft");
		node_alienSpawnPosition_right = GetNode<Position2D>("AlienSpawnPositionRight");
		node_aliens = node_actors.GetNode<Node2D>("Aliens");
		node_alienSpawnTimer = GetNode<Timer>("AlienSpawnTimer");

		node_background = GetNode<Node2D>("Background");
	}

	public override void _Ready()
	{
		node_gameHUDController.Lives = _lives;
		node_gameHUDController.Score = _score;

		node_player.Connect(nameof(Player.OnHit), this, nameof(OnPlayerHit));
		node_player.Connect(nameof(Player.OnDie), this, nameof(OnPlayerDie));

		for(int i = 0; i < node_enemies.GetChildCount(); i++)
		{
			PinkHead pinkHead = node_enemies.GetChildOrNull<PinkHead>(i);
			GreyHead greyHead = node_enemies.GetChildOrNull<GreyHead>(i);

			if (pinkHead != null)
			{
				pinkHead.Connect(nameof(PinkHead.OnHit), this, nameof(OnPinkHeadHit));
				pinkHead.Connect(nameof(PinkHead.OnDie), this, nameof(OnPinkHeadDie));
				m_enemiesAlive.Add(pinkHead.Name);
			}
			else if (greyHead != null)
			{
				greyHead.Connect(nameof(GreyHead.OnHit), this, nameof(OnGreyHeadHit));
				greyHead.Connect(nameof(GreyHead.OnDie), this, nameof(OnGreyHeadDie));
				m_enemiesAlive.Add(greyHead.Name);
			}
		}

		RecalculateEnemyBounds();
	}

	public override void _PhysicsProcess(float delta)
	{
		float relativePositionX = node_player.Position.x - 128;
		float distancePercentageX = -relativePositionX / 128;
		float speedModifierX = 16f;

		Vector2 speed = new Vector2();
		speed.x = speedModifierX * distancePercentageX;
		speed.y = 32f;

		Vector2 position = node_background.Position;
		position += speed * delta;

		position.x = Mathf.Wrap(position.x, -256, 0);
		position.y = Mathf.Wrap(position.y, -384, 0);

		node_background.Position = position;

		if (m_enemiesAlive.Count > 0)
		{
			Vector2 left = node_enemiesLeftBounds.GlobalPosition;
			Vector2 right = node_enemiesRightBounds.GlobalPosition;

			m_enemyMoveTimer += delta;
			if (m_enemyMoveTimer >= EnemyMoveSpeed)
			{
				m_enemyMoveTimer = 0;
				if (EnemyMoveDirection == -1 && left.x <= 0)
				{
					node_enemies.Translate(new Vector2(0, 16));
					EnemyMoveSpeed -= 0.1f;
					EnemyMoveSpeed = Mathf.Clamp(EnemyMoveSpeed, 0.1f, Mathf.Inf);
					EnemyMoveDirection = 1;
				}
				else if (EnemyMoveDirection == 1 && right.x >= 256)
				{
					node_enemies.Translate(new Vector2(0, 16));
					EnemyMoveSpeed -= 0.1f;
					EnemyMoveSpeed = Mathf.Clamp(EnemyMoveSpeed, 0.1f, Mathf.Inf);
					EnemyMoveDirection = -1;
				}
				else
				{
					Vector2 translation = new Vector2(EnemyMoveDistance * EnemyMoveDirection, 0);
					node_enemies.Position += translation;
				}
			}
			if (node_enemiesLeftBounds.GlobalPosition.y >= 360)
			{
				GetTree().ReloadCurrentScene();
			}
		}
	}

	#endregion // Godot methods



	#region Private methods

	private void RecalculateEnemyBounds()
	{
		float left = 256;
		float right = 0;
		float bottom = 0;

		for(int i = 0; i < node_enemies.GetChildCount(); i++)
		{
			PinkHead pinkHead = node_enemies.GetChildOrNull<PinkHead>(i);
			GreyHead greyHead = node_enemies.GetChildOrNull<GreyHead>(i);

			if (pinkHead == null && greyHead == null) continue;

			Node2D node = node_enemies.GetChild<Node2D>(i);

			if (!m_enemiesAlive.Contains(node.Name)) continue;

			if (i == 0)
			{
				left = node.GlobalPosition.x;
				right = node.GlobalPosition.x;
				bottom = node.GlobalPosition.y;
			}
			else
			{
				if (node.GlobalPosition.x < left)
				{
					left = node.GlobalPosition.x;
				}
				if (node.GlobalPosition.x > right)
				{
					right = node.GlobalPosition.x;
				}
				if (node.GlobalPosition.y > bottom)
				{
					bottom = node.GlobalPosition.y;
				}
			}
		}

		node_enemiesLeftBounds.GlobalPosition = new Vector2(left - 16, bottom + 16);
		node_enemiesRightBounds.GlobalPosition = new Vector2(right + 16, bottom + 16);
	}

	private void OnPlayerHit(Player player) { }
	private void OnPlayerDie(Player player)
	{
		if (Lives > 0)
		{
			Lives -= 1;
			player.Position = node_playerSpawnPosition.Position;
		}
		else
		{
			GetTree().ReloadCurrentScene();
		}
	}

	private void OnPinkHeadHit(PinkHead pinkHead)
	{
		Score += 100;
		m_enemiesAlive.Remove(pinkHead.Name);
		m_enemiesHit.Add(pinkHead.Name);
		RecalculateEnemyBounds();
	}
	private void OnPinkHeadDie(PinkHead pinkHead)
	{
		m_enemiesHit.Remove(pinkHead.Name);
		if (m_enemiesAlive.Count == 0 && m_enemiesHit.Count == 0)
		{
			GetTree().ReloadCurrentScene();
		}
	}

	private void OnGreyHeadHit(GreyHead greyHead)
	{
		Score += 250;
		m_enemiesAlive.Remove(greyHead.Name);
		m_enemiesHit.Add(greyHead.Name);
		RecalculateEnemyBounds();
	}
	private void OnGreyHeadDie(GreyHead greyHead)
	{
		m_enemiesHit.Remove(greyHead.Name);
		if (m_enemiesAlive.Count == 0 && m_enemiesHit.Count == 0)
		{
			GetTree().ReloadCurrentScene();
		}
	}

	private void OnRedAlienHit(RedAlien redAlien)
	{
		Score += 500;
	}
	private void OnRedAlienDie(RedAlien redAlien) { }

	private void OnPurpleAlienHit(PurpleAlien purpleAlien)
	{
		Score += 500;
	}
	private void OnPurpleAlienDie(PurpleAlien purpleAlien) { }

	private void OnAlienSpawnTimerTimeout()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.Randomize();
		bool left = rng.RandiRange(0, 1) == 0 ? false : true;
		rng.Randomize();
		bool red = rng.RandiRange(0, 1) == 0 ? false : true;

		if (red)
		{
			RedAlien alien = m_packedScene_redAlien.Instance<RedAlien>();
			node_aliens.AddChild(alien);
			alien.Position = left ? node_alienSpawnPosition_left.Position : node_alienSpawnPosition_right.Position;
			float speed = 64;
			alien.Velocity = left ? Vector2.Right * speed : Vector2.Left * speed;

			alien.Connect(nameof(RedAlien.OnHit), this, nameof(OnRedAlienHit));
			alien.Connect(nameof(RedAlien.OnDie), this, nameof(OnRedAlienDie));
		}
		else
		{
			PurpleAlien alien = m_packedScene_purpleAlien.Instance<PurpleAlien>();
			node_aliens.AddChild(alien);
			alien.Position = left ? node_alienSpawnPosition_left.Position : node_alienSpawnPosition_right.Position;
			float speed = 64;
			alien.Velocity = left ? Vector2.Right * speed : Vector2.Left * speed;

			alien.Connect(nameof(PurpleAlien.OnHit), this, nameof(OnPurpleAlienHit));
			alien.Connect(nameof(PurpleAlien.OnDie), this, nameof(OnPurpleAlienDie));
		}
	}

	#endregion // Private methods

}

