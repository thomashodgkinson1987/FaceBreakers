using Godot;

public class MainSceneController : Node2D
{

	#region Nodes

	private GameHUDController node_gameHUDController;

	private Node2D node_actors;
	private Node2D node_enemies;

	private Player node_player;

	private Position2D node_playerSpawnPosition;

	private Node2D node_background;

	#endregion // Nodes



	#region Fields

	private int m_enemiesLeft = 0;

	#endregion // Fields



	#region Properties

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



	#region Godot methods

	public override void _EnterTree()
	{
		node_gameHUDController = GetNode<GameHUDController>("GameHUD");

		node_actors = GetNode<Node2D>("Actors");
		node_enemies = node_actors.GetNode<Node2D>("Enemies");

		node_player = node_actors.GetNode<Player>("Player");

		node_playerSpawnPosition = GetNode<Position2D>("PlayerSpawnPosition");

		node_background = GetNode<Node2D>("Background");
	}

	public override void _Ready()
	{
		node_gameHUDController.Lives = _lives;
		node_gameHUDController.Score = _score;

		node_player.Connect(nameof(Player.OnHit), this, nameof(OnPlayerHit));

		foreach (PinkHead pinkHead in node_enemies.GetChildren())
		{
			pinkHead.Connect(nameof(PinkHead.OnHit), this, nameof(OnEnemyHit));
			m_enemiesLeft++;
		}
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

