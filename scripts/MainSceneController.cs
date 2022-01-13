using Godot;

public class MainSceneController : Node2D
{

	private Sprite node_sprite;

	public override void _Ready()
	{
		node_sprite = GetNode<Sprite>("Sprite");
	}

	public override void _Process(float delta)
	{
		node_sprite.Position += Vector2.Right * delta;
	}

}

