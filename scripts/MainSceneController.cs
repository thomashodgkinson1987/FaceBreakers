using Godot;

public class MainSceneController : Node2D
{

	private Sprite sprite;

	public override void _Ready()
	{
		sprite = GetNode<Sprite>("Sprite");
	}

	public override void _Process(float delta)
	{
		sprite.Position += Vector2.Right * delta;

		int bob = 1;

		f;
	}

}

