using System.Collections.Generic;
using Godot;

public class PlayerInputController
{

	#region Enums

	public enum EButton { Left, Right, Up, Down, Shoot }

	#endregion // Enums



	#region Fields

	private Dictionary<EButton, string> m_buttons;

	#endregion // Fields



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

	public PlayerInputController()
	{
		m_buttons = new Dictionary<EButton, string>();
		m_buttons.Add(EButton.Left, "player_move_left");
		m_buttons.Add(EButton.Right, "player_move_right");
		m_buttons.Add(EButton.Up, "player_move_up");
		m_buttons.Add(EButton.Down, "player_move_down");
		m_buttons.Add(EButton.Shoot, "player_shoot");
	}

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

		if (Input.IsActionPressed(m_buttons[EButton.Left])) _directionX -= 1;
		if (Input.IsActionPressed(m_buttons[EButton.Right])) _directionX += 1;
		if (Input.IsActionPressed(m_buttons[EButton.Up])) _directionY -= 1;
		if (Input.IsActionPressed(m_buttons[EButton.Down])) _directionY += 1;

		IsJustPressed_Shoot = Input.IsActionJustPressed(m_buttons[EButton.Shoot]);
	}

	#endregion // Public methods

}

