using Godot;

public class MainSceneController : Node2D
{

	public override void _Ready()
	{

		Sprite sprite = GetNode<Sprite>("Sprite");

		sprite.FlipH = true;

	}

}

