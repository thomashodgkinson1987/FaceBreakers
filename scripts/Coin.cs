using Godot;

public class Coin : Node2D
{

	private Sprite node_sprite;

	public override void _Ready()
	{
		node_sprite = GetNode<Sprite>("Sprite");
	}

}

