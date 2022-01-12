using Godot;

public class MainSceneController : Node2D
{

	public override void _Ready()
	{

		Sprite sprite = GetNode<Sprite>("");
		AnimatedSprite animatedSprite = GetNode<AnimatedSprite>("");

		animatedSprite.Play("");

	}

}

