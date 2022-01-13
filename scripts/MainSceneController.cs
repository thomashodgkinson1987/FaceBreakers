using Godot;

public class MainSceneController : Node2D
{

	private float m_speed = 10f;
	private Sprite m_sprite;

	private string m_name = "Name";

	public override void _Ready()
	{

	}

	public override void _Process(float delta)
	{
		m_sprite.Position += Vector2.Right * delta;
	}

}

