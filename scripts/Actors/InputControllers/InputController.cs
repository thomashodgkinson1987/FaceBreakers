using Godot;

public class PlayerInputController
{
	
	#region Properties

	private float _directionX = 0;
	private float _directionY = 0;

	public float DirectionX
	{
		get => _directionX;
		set
		{
			if (value < -0.5f) _directionX = -1f;
			else if (value > 0.5f) _directionX = 1f;
			else _directionX = 0;
		}
	}
	public float DirectionY
	{
		get => _directionY;
		set
		{
			if (value < -0.5f) _directionY = -1f;
			else if (value > 0.5f) _directionY = 1f;
			else _directionY = 0;
		}
	}
	public Vector2 Direction
	{
		get => new Vector2(DirectionX, DirectionY);
		set
		{
			DirectionX = value.x;
			DirectionY = value.y;
		}
	}

	public bool IsJustPressed_Shoot { get; set; } = false;

	#endregion // Properties



	#region Constructors

	public PlayerInputController() { }

	#endregion // Constructors



	#region Public methods

	public void Reset()
	{
		_directionX = 0;
		_directionY = 0;

		IsJustPressed_Shoot = false;
	}

	public void Poll()
	{
		_directionX = 0;
		_directionY = 0;

		if (Input.IsActionPressed("player_move_left")) _directionX -= 1;
		if (Input.IsActionPressed("player_move_right")) _directionX += 1;
		if (Input.IsActionPressed("player_move_up")) _directionY -= 1;
		if (Input.IsActionPressed("player_move_down")) _directionY += 1;

		IsJustPressed_Shoot = Input.IsActionJustPressed("player_shoot");
	}

	#endregion // Public methods

}

