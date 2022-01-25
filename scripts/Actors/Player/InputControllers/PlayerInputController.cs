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

	private int _directionX = 0;
	private int _directionY = 0;

	public int DirectionX
	{
		get => _directionX;
		set
		{
			if (value < 0) _directionX = -1;
			else if (value > 0) _directionX = 1;
			else _directionX = 0;
		}
	}
	public int DirectionY
	{
		get => _directionY;
		set
		{
			if (value < 0) _directionY = -1;
			else if (value > 0) _directionY = 1;
			else _directionY = 0;
		}
	}
	public Vector2 Direction
	{
		get => new Vector2(DirectionX, DirectionY);
		set
		{
			DirectionX = (int)value.x;
			DirectionY = (int)value.y;
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

		_directionY = 0;

		IsJustPressed_Shoot = Input.IsActionJustPressed(m_buttons[EButton.Shoot]);
	}

	#endregion // Public methods

}

