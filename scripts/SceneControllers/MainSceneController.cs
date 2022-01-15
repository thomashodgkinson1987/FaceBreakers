using Godot;

public class MainSceneController : Node2D
{

	#region Exports

	#endregion // Exports



	#region Nodes

	private Player node_player;
	private Player1 node_player1;

	private Node2D node_actors;
	private Node2D node_projectiles;

	#endregion // Nodes



	#region Godot methods

	public override void _EnterTree()
	{
		node_actors = GetNode<Node2D>("Actors");
		node_projectiles = GetNode<Node2D>("Projectiles");

		node_player = node_actors.GetNode<Player>("Player");
		node_player1 = node_actors.GetNode<Player1>("Player1");
	}

	public override void _Ready()
	{
		node_player.Connect(nameof(Player.OnShootJustPressed), this, nameof(OnPlayerShoot));
		node_player1.Connect(nameof(Player1.OnShootJustPressed), this, nameof(OnPlayer1Shoot));
	}

	#endregion // Godot methods



	#region Public methods

	#endregion // Public methods



	#region Private methods

	private void OnPlayerShoot(Player player, Projectile projectile)
	{
		node_projectiles.AddChild(projectile);
	}

	private void OnPlayer1Shoot(Player1 player1, Projectile projectile)
	{
		projectile.GetParent().RemoveChild(projectile);
		node_projectiles.AddChild(projectile);
	}

	#endregion // Private methods

}

