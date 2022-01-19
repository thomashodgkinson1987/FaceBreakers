using Godot;

public class MainSceneController : Node2D
{

	#region Nodes

	private GameHUDController node_gameHUDController;
	private Player node_player;

	#endregion // Nodes



	#region Fields

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
		node_player = GetNode<Player>("Player");
		node_gameHUDController = GetNode<GameHUDController>("GameHUD");
	}

	public override void _Ready()
	{
		Lives = 3;
		Score = 0;
	}

	#endregion // Godot methods

}

