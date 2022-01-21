using Godot;

public class MainSceneController : Node2D
{

	#region Nodes

	private GameHUDController node_gameHUDController;
	private Player node_player;

	private Node2D node_enemyProjectiles;
	private Position2D node_enemyProjectileSpawnPosition;
	private Timer node_enemyProjectileSpawnTimer;

	#endregion // Nodes



	#region Fields

	[Export] private PackedScene m_enemyProjectilePackedScene;

	private int _lives = 3;
	private int _score = 0;

	#endregion // Fields



	#region Properties

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



	#region Godot methods

	public override void _EnterTree()
	{
		node_gameHUDController = GetNode<GameHUDController>("GameHUD");
		node_player = GetNode<Player>("Player");
		node_enemyProjectiles = GetNode<Node2D>("EnemyProjectiles");
		node_enemyProjectileSpawnPosition = GetNode<Position2D>("EnemyProjectileSpawnPosition");
		node_enemyProjectileSpawnTimer = GetNode<Timer>("EnemyProjectileSpawnTimer");
	}

	public override void _Ready()
	{
		Lives = 3;
		Score = 0;
	}

	#endregion // Godot methods



	#region Public methods

	public void FireEnemyProjectile()
	{
		Projectile projectile = m_enemyProjectilePackedScene.Instance<Projectile>();
		node_enemyProjectiles.AddChild(projectile);
		projectile.GlobalPosition = node_enemyProjectileSpawnPosition.GlobalPosition;
		projectile.Rotation = node_enemyProjectileSpawnPosition.Rotation;
	}

	#endregion // Public methods



	#region Private methods

	private void OnEnemyProjectileSpawnTimerTimeout()
	{
		FireEnemyProjectile();
	}

	#endregion // Private methods

}

