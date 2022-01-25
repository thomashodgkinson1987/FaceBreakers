using System.Collections.Generic;
using Godot;

public class MainSceneController : Node2D
{

	#region Enums

	public enum EDirection { Left, Right }

	#endregion // Enums



	#region Nodes

	private GameHUDController node_gameHUDController;

	private Node2D node_actors;
	private Node2D node_enemies;
	private Node2D node_aliens;
	private Player node_player;

	private Position2D node_playerSpawnPosition;

	private Position2D node_alienSpawnPosition_left;
	private Position2D node_alienSpawnPosition_right;

	private Node2D node_background;

	#endregion // Nodes



	#region Properties

	[Export] public float EnemyMoveTime { get; set; } = 1f;
	[Export] public EDirection EnemyMoveDirection { get; set; } = EDirection.Right;
	[Export] public float EnemyMoveDistance { get; set; } = 8f;

	public int Lives
	{
		get => _lives;
		set
		{
			_lives = value;
			node_gameHUDController.Lives = value;
		}
	}

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

	private int _lives = 3;
	private int _score = 0;

	[Export] private PackedScene m_packedScene_redAlien;
	[Export] private PackedScene m_packedScene_purpleAlien;

	private float m_enemyMoveTimeTimer = 0f;

	private List<string> m_enemiesAlive = new List<string>();
	private List<string> m_enemiesHit = new List<string>();

	private float m_enemyLeftPosition = 256;
	private float m_enemyRightPosition = 0;
	private float m_enemyBottomPosition = 0;

	private int m_hitCountToSpawnAlien = 6;
	private int m_hitCountToSpawnAlienCounter = 0;

	private RandomNumberGenerator m_rng = new RandomNumberGenerator();

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_gameHUDController = GetNode<GameHUDController>("GameHUD");

		node_actors = GetNode<Node2D>("Actors");
		node_enemies = node_actors.GetNode<Node2D>("Enemies");
		node_aliens = node_actors.GetNode<Node2D>("Aliens");
		node_player = node_actors.GetNode<Player>("Player");

		node_playerSpawnPosition = GetNode<Position2D>("PlayerSpawnPosition");

		node_alienSpawnPosition_left = GetNode<Position2D>("AlienSpawnPositionLeft");
		node_alienSpawnPosition_right = GetNode<Position2D>("AlienSpawnPositionRight");

		node_background = GetNode<Node2D>("Background");
	}

	public override void _Ready()
	{
		m_rng.Randomize();

		node_gameHUDController.Lives = _lives;
		node_gameHUDController.Score = _score;

		node_player.Connect(nameof(Player.OnHit), this, nameof(OnPlayerHit));
		node_player.Connect(nameof(Player.OnDie), this, nameof(OnPlayerDie));

		for(int i = 0; i < node_enemies.GetChildCount(); i++)
		{
			Enemy enemy = node_enemies.GetChild<Enemy>(i);

			enemy.Connect(nameof(Enemy.OnHit), this, nameof(OnEnemyHit));
			enemy.Connect(nameof(Enemy.OnDie), this, nameof(OnEnemyDie));
			m_enemiesAlive.Add(enemy.Name);
		}

		RecalculateEnemyPositions();
	}

	public override void _PhysicsProcess(float delta)
	{
		HandleBackgroundScrolling(delta);
		HandleEnemyMoving(delta);
	}

	#endregion // Godot methods



	#region Private methods

	private void HandleBackgroundScrolling(float delta)
	{
		float relativePositionX = node_player.Position.x - 128;
		float distancePercentageX = -relativePositionX / 128;

		Vector2 speed = new Vector2();
		speed.x = 16 * distancePercentageX;
		speed.y = 32;

		Vector2 position = node_background.Position;
		position += speed * delta;

		position.x = Mathf.Wrap(position.x, -256, 0);
		position.y = Mathf.Wrap(position.y, -384, 0);

		node_background.Position = position;
	}

	private void MoveAllEnemies(Vector2 translation)
	{
		foreach(Enemy enemy in node_enemies.GetChildren())
		{
			if (!m_enemiesAlive.Contains(enemy.Name)) continue;

			enemy.Translate(translation);
		}
	}

	private void HandleEnemyMoving(float delta)
	{
		if (m_enemiesAlive.Count > 0)
		{
			m_enemyMoveTimeTimer += delta;

			while (m_enemyMoveTimeTimer >= EnemyMoveTime)
			{
				m_enemyMoveTimeTimer -= EnemyMoveTime;

				float margin = 16;

				bool isLeft = EnemyMoveDirection == EDirection.Left && m_enemyLeftPosition - margin <= 0;
				bool isRight = EnemyMoveDirection == EDirection.Right && m_enemyRightPosition + margin >= 256;

				if (isLeft || isRight)
				{
					MoveAllEnemies(new Vector2(0, EnemyMoveDistance));
					m_enemyBottomPosition += EnemyMoveDistance;
					EnemyMoveTime -= 0.1f;
					EnemyMoveTime = Mathf.Clamp(EnemyMoveTime, 0.1f, Mathf.Inf);
					EnemyMoveDirection = EnemyMoveDirection == EDirection.Left ? EDirection.Right : EDirection.Left;

					if (m_enemyBottomPosition + margin >= 360)
					{
						GetTree().ReloadCurrentScene();
					}
				}
				else
				{
					int direction = EnemyMoveDirection == EDirection.Left ? -1 : 1;
					Vector2 translation = new Vector2(EnemyMoveDistance * direction, 0);
					node_enemies.Position += translation;
					m_enemyLeftPosition += translation.x;
					m_enemyRightPosition += translation.x;
				}
			}
		}
	}

	private void RecalculateEnemyPositions()
	{
		m_enemyLeftPosition = 256;
		m_enemyRightPosition = 0;
		m_enemyBottomPosition = 0;

		foreach(Enemy enemy in node_enemies.GetChildren())
		{
			if (!m_enemiesAlive.Contains(enemy.Name)) continue;

			m_enemyLeftPosition = enemy.GlobalPosition.x < m_enemyLeftPosition ? enemy.GlobalPosition.x : m_enemyLeftPosition;
			m_enemyRightPosition = enemy.GlobalPosition.x > m_enemyRightPosition ? enemy.GlobalPosition.x : m_enemyRightPosition;
			m_enemyBottomPosition = enemy.GlobalPosition.y > m_enemyBottomPosition ? enemy.GlobalPosition.y : m_enemyBottomPosition;
		}
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

	private void OnEnemyHit(Enemy enemy)
	{
		Score += enemy.ScoreValue;
		m_enemiesAlive.Remove(enemy.Name);
		m_enemiesHit.Add(enemy.Name);
		//RecalculateEnemyPositions();
		m_hitCountToSpawnAlienCounter++;
		if (m_hitCountToSpawnAlienCounter == m_hitCountToSpawnAlien)
		{
			m_hitCountToSpawnAlienCounter = 0;
			SpawnAlien();
		}
	}
	private void OnEnemyDie(Enemy enemy)
	{
		m_enemiesHit.Remove(enemy.Name);
		if (m_enemiesAlive.Count == 0 && m_enemiesHit.Count == 0)
		{
			GetTree().ReloadCurrentScene();
		}
	}

	private void OnAlienHit(Alien alien)
	{
		Score += alien.ScoreValue;
	}
	private void OnAlienDie(Alien alien) { }

	private void SpawnAlien()
	{
		m_rng.Randomize();

		int direction = m_rng.RandiRange(0, 1) == 0 ? -1 : 1;
		PackedScene packedScene = m_rng.RandiRange(0, 1) == 0 ? m_packedScene_redAlien : m_packedScene_purpleAlien;

		Alien alien = packedScene.Instance<Alien>();
		node_aliens.AddChild(alien);

		Vector2 leftSpawnPosition = node_alienSpawnPosition_left.Position;
		Vector2 rightSpawnPosition = node_alienSpawnPosition_right.Position;

		alien.Position = direction == -1 ? rightSpawnPosition : leftSpawnPosition;
		alien.Velocity = new Vector2(alien.Speed * direction, 0);

		alien.Connect(nameof(Alien.OnHit), this, nameof(OnAlienHit));
		alien.Connect(nameof(Alien.OnDie), this, nameof(OnAlienDie));
	}

	#endregion // Private methods

}

