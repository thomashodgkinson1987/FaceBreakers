using Godot;

public class MainSceneController : Node2D
{

	private float m_speed = 10f;
	private Sprite m_sprite;

	public override void _Ready()
	{
		m_sprite = GetNode<Sprite>("Sprite");
	}

	public override void _Process(float delta)
	{
		m_sprite.Position += Vector2.Right * m_speed * delta;
	}

}

