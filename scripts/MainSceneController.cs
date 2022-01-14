using Godot;

public class MainSceneController : Node2D
{

	private Player m_player;

	public override void _Ready()
	{
		m_player = GetNode<Player>("Player");
	}

}

