using Godot;

public class TitleScreenController : Control
{

	#region Nodes

	private AnimationPlayer node_animationPlayer;

	#endregion // Nodes



	#region Fields

	private bool m_isFadingIn = false;
	private bool m_isFadingOut = false;
	private bool m_hasInputBeenPressed = false;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Ready()
	{
		node_animationPlayer.Play("fade_in");
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event is InputEventKey || @event is InputEventJoypadButton)
		{
			if (!m_hasInputBeenPressed)
			{
				m_hasInputBeenPressed = true;
				node_animationPlayer.Play("fade_out");
			}
		}
	}

	#endregion // Godot methods



	#region Private methods

	private void OnFadeInStart()
	{
		m_isFadingIn = true;
	}

	private void OnFadeInEnd()
	{
		m_isFadingIn = false;
	}

	private void OnFadeOutStart()
	{
		m_isFadingOut = true;
	}

	private void OnFadeOutEnd()
	{
		m_isFadingOut = false;
		GetTree().ChangeScene("res://scenes/MainScene.tscn");
	}

	#endregion // Private methods

}

