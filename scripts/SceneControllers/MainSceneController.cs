using System.Collections.Generic;
using Godot;

public class MainSceneController : Node2D
{

	#region Enums

	public enum EDirection { Left, Right }

	#endregion // Enums



	#region Nodes

	private GameHUDController node_gameHUDController;

	private AudioStreamPlayer node_music;
	private AudioStreamPlayer node_bossMusic;

	private Node2D node_actors;
	private Node2D node_enemies;
	private Node2D node_aliens;
	private Node2D node_bosses;
	private Player node_player;

	private Position2D node_playerSpawnPosition;

	private Position2D node_alienSpawnPosition_left;
	private Position2D node_alienSpawnPosition_right;

	private Node2D node_background;

	private Boss node_boss;

	private AnimationPlayer node_animationPlayer;

	#endregion // Nodes



	#region Properties

	[Export] public float EnemyMoveTime { get; set; } = 1f;
	[Export] public EDirection EnemyMoveDirection { get; set; } = EDirection.Right;
	[Export] public float EnemyMoveDistance { get; set; } = 8f;

	[Export] public Vector2 BackgroundScrollSpeed { get; set; } = new Vector2(16, 32);
	[Export] public float BackgroundScollSpeedModX { get; set; } = 16;

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
	[Export] private PackedScene m_packedScene_boss;

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

		node_music = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Music");
		node_bossMusic = GetNode<AudioStreamPlayer>("AudioStreamPlayer_BossMusic");

		node_actors = GetNode<Node2D>("Actors");
		node_enemies = node_actors.GetNode<Node2D>("Enemies");
		node_aliens = node_actors.GetNode<Node2D>("Aliens");
		node_bosses = node_actors.GetNode<Node2D>("Bosses");
		node_player = node_actors.GetNode<Player>("Player");

		node_playerSpawnPosition = GetNode<Position2D>("PlayerSpawnPosition");

		node_alienSpawnPosition_left = GetNode<Position2D>("AlienSpawnPositionLeft");
		node_alienSpawnPosition_right = GetNode<Position2D>("AlienSpawnPositionRight");

		node_background = GetNode<Node2D>("Background");

		node_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Ready()
	{
		m_rng.Randomize();

		Lives = (int)ProjectSettings.GetSetting("global/starting_lives");
		Score = _score;

		node_player.Connect(nameof(Player.OnHit), this, nameof(OnPlayerHit));
		node_player.Connect(nameof(Player.OnDie), this, nameof(OnPlayerDie));

		if ((bool)ProjectSettings.GetSetting("global/invincible"))
		{
			node_player.Set_HitboxEnabled(false);
		}

		if ((bool)ProjectSettings.GetSetting("global/no_clip"))
		{
			node_player.Set_CollisionEnabled(false);
		}

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
		speed.x = BackgroundScrollSpeed.x * distancePercentageX;
		speed.y = BackgroundScrollSpeed.y;

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

		m_enemyLeftPosition += translation.x;
		m_enemyRightPosition += translation.x;
		m_enemyBottomPosition += translation.y;
	}

	private void HandleEnemyMoving(float delta)
	{
		if (m_enemiesAlive.Count > 0)
		{
			m_enemyMoveTimeTimer += delta;

			if (m_enemyMoveTimeTimer >= EnemyMoveTime)
			{
				m_enemyMoveTimeTimer = 0;

				float margin = 16;

				bool isLeft = EnemyMoveDirection == EDirection.Left && m_enemyLeftPosition - margin <= 0;
				bool isRight = EnemyMoveDirection == EDirection.Right && m_enemyRightPosition + margin >= 256;

				if (isLeft || isRight)
				{
					if (m_enemyBottomPosition + margin + EnemyMoveDistance > 256)
					{
						GetTree().ReloadCurrentScene();
						return;
					}

					MoveAllEnemies(new Vector2(0, EnemyMoveDistance));
					EnemyMoveTime -= 0.1f;
					EnemyMoveTime = Mathf.Clamp(EnemyMoveTime, 0.1f, Mathf.Inf);
					EnemyMoveDirection = EnemyMoveDirection == EDirection.Left ? EDirection.Right : EDirection.Left;
				}
				else
				{
					int direction = EnemyMoveDirection == EDirection.Left ? -1 : 1;
					Vector2 translation = new Vector2(EnemyMoveDistance * direction, 0);
					MoveAllEnemies(translation);
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
		RecalculateEnemyPositions();
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
			node_animationPlayer.Play("cross_fade_music_to_boss");
		}
	}

	private void OnAlienHit(Alien alien)
	{
		Score += alien.ScoreValue;
	}
	private void OnAlienDie(Alien alien) { }

	private void OnBossHit(Boss boss)
	{
		Score += 100;
		node_gameHUDController.BossHealth = boss.Health;
	}
	private void OnBossDie(Boss boss)
	{
		node_animationPlayer.Play("boss_die_0001");
		node_gameHUDController.RunCredits();
		node_gameHUDController.HideBossHealthBar();
	}

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

	private void SpawnBoss()
	{
		node_boss = m_packedScene_boss.Instance<Boss>();
		node_bosses.AddChild(node_boss);
		node_boss.Position = new Vector2(128, -128);
		node_boss.Connect(nameof(Boss.OnHit), this, nameof(OnBossHit));
		node_boss.Connect(nameof(Boss.OnDie), this, nameof(OnBossDie));
		node_boss.Play_Intro1();
		node_gameHUDController.ShowBossHealthBar();
	}

	#endregion // Private methods

}

