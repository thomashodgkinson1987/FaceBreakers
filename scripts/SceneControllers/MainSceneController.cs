using Godot;

public class MainSceneController : Node2D
{

	#region Nodes

	private GameHUDController node_gameHUDController;

	private Node2D node_actors;
	private Node2D node_enemies;

	private Player node_player;

	private Position2D node_playerSpawnPosition;

	#endregion // Nodes



	#region Fields

	[Export] private PackedScene m_enemyProjectilePackedScene;

	private int _lives = 3;
	private int _score = 0;

	private int m_enemiesLeft = 0;

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

		node_actors = GetNode<Node2D>("Actors");
		node_enemies = node_actors.GetNode<Node2D>("Enemies");

		node_player = node_actors.GetNode<Player>("Player");

		node_playerSpawnPosition = GetNode<Position2D>("PlayerSpawnPosition");
	}

	public override void _Ready()
	{
		node_gameHUDController.Lives = _lives;
		node_gameHUDController.Score = _score;

		node_player.Lives = _lives;
		node_player.Connect(nameof(Player.OnHit), this, nameof(OnPlayerHit));

		foreach (PinkHead pinkHead in node_enemies.GetChildren())
		{
			pinkHead.Connect(nameof(PinkHead.OnHit), this, nameof(OnEnemyHit));
			m_enemiesLeft++;
		}
	}

	#endregion // Godot methods



	#region Private methods

	private void OnPlayerHit()
	{
		if (Lives > 0)
		{
			Lives -= 1;
			node_player.Position = node_playerSpawnPosition.Position;
		}
		else
		{
			GetTree().ReloadCurrentScene();
		}
	}

	private void OnEnemyHit()
	{
		Score += 100;
		m_enemiesLeft--;
		if (m_enemiesLeft <= 0)
		{
			GetTree().ReloadCurrentScene();
		}
	}

	#endregion // Private methods

}

