using System.Collections.Generic;
using Godot;

public class MainSceneController : Node2D
{

	#region Nodes

	private GameHUDController node_gameHUDController;

	private Node2D node_actors;
	private Node2D node_enemies;

	private Player node_player;

	private Position2D node_playerSpawnPosition;

	private Node2D node_tileMapLayer;

	#endregion // Nodes



	#region Fields

	private int m_enemiesLeft = 0;

	private List<Node2D> m_groups;

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

		node_tileMapLayer = GetNode<Node2D>("TileMapLayer");
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

		m_groups = new List<Node2D>();
		foreach(Node2D node in node_tileMapLayer.GetChildren())
		{
			m_groups.Add(node);
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		Node2D group1 = m_groups[0];
		Node2D group2 = m_groups[1];
		Node2D group3 = m_groups[2];
		Node2D group4 = m_groups[3];

		float posX = node_player.Position.x;
		posX -= 128;

		float xMod = -posX / 128;
		float xSpeed = 64 * xMod;

		float ySpeed = 64f;

		for(int i = 0; i < m_groups.Count; i++)
		{
			Node2D node = m_groups[i];

			float x = node.Position.x + (xSpeed * delta);
			float y = node.Position.y + (ySpeed * delta);
			node.Position = new Vector2(x, y);
		}

		if (group1.Position.x < -256)
		{
			float x1 = group2.Position.x;
			float y1 = group1.Position.y;
			group1.Position = new Vector2(x1, y1);
			float x2 = group2.Position.x + 256;
			float y2 = group2.Position.y;
			group2.Position = new Vector2(x2, y2);
			float x3 = group4.Position.x;
			float y3 = group3.Position.y;
			group3.Position = new Vector2(x3, y3);
			float x4 = group4.Position.x + 256;
			float y4 = group4.Position.y;
			group4.Position = new Vector2(x4, y4);
		}
		else if (group1.Position.x > 0)
		{
			float x1 = group1.Position.x - 256;
			float y1 = group1.Position.y;
			group1.Position = new Vector2(x1, y1);
			float x2 = group1.Position.x + 256;
			float y2 = group2.Position.y;
			group2.Position = new Vector2(x2, y2);
			float x3 = group1.Position.x;
			float y3 = group3.Position.y;
			group3.Position = new Vector2(x3, y3);
			float x4 = group2.Position.x;
			float y4 = group4.Position.y;
			group4.Position = new Vector2(x4, y4);
		}

		if (group1.Position.y < -384)
		{
			float x1 = group1.Position.x;
			float y1 = group3.Position.y;
			group1.Position = new Vector2(x1, y1);
			float x2 = group2.Position.x;
			float y2 = group1.Position.y;
			group2.Position = new Vector2(x2,y2);
			float x3 = group3.Position.x;
			float y3 = group1.Position.y + 384;
			group3.Position = new Vector2(x3, y3);
			float x4 = group4.Position.x;
			float y4 = group3.Position.y;
			group4.Position = new Vector2(x4, y4);
		}
		else if (group1.Position.y > 0)
		{
			float x1 = group1.Position.x;
			float y1 = group1.Position.y - 384;
			group1.Position = new Vector2(x1, y1);
			float x2 = group2.Position.x;
			float y2 = group1.Position.y;
			group2.Position = new Vector2(x2, y2);
			float x3 = group3.Position.x;
			float y3 = group1.Position.y + 384;
			group3.Position = new Vector2(x3, y3);
			float x4 = group4.Position.x;
			float y4 = group3.Position.y;
			group4.Position = new Vector2(x4, y4);
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

