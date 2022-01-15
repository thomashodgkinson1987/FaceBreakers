using Godot;

public class MainSceneController : Node2D
{

	#region Exports

	#endregion // Exports



	#region Nodes

	private Node2D node_actors;
	private Node2D node_projectiles;

	private Player node_player;

	#endregion // Nodes



	#region Godot methods

	public override void _Ready()
	{
		node_actors = GetNode<Node2D>("Actors");
		node_projectiles = GetNode<Node2D>("Projectiles");

		node_player = node_actors.GetNode<Player>("Player");

		node_player.Connect(nameof(Player.OnShootJustPressed), this, nameof(OnPlayerShoot));
	}

	#endregion // Godot methods



	#region Public methods

	#endregion // Public methods



	#region Private methods

	private void OnPlayerShoot(Player player, Projectile projectile)
	{
		projectile.GetParent().RemoveChild(projectile);
		node_projectiles.AddChild(projectile);
	}

	#endregion // Private methods

}

