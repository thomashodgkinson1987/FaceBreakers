using Godot;

public class MainSceneController : Node2D
{

	#region Exports

	[Export] public PackedScene PackedScene_PlayerProjectile { get; set; }

	#endregion // Exports



	#region Nodes

	private Player node_player;

	private Node2D node_projectiles;

	#endregion // Nodes



	#region Godot methods

	public override void _EnterTree()
	{
		node_player = GetNode<Player>("Player");
		node_projectiles = GetNode<Node2D>("Projectiles");
	}

	public override void _Ready()
	{
		node_player.Connect(nameof(Player.OnShootJustPressed), this, nameof(OnPlayerShoot));
	}

	#endregion // Godot methods



	#region Private methods

	private void OnPlayerShoot(Player player)
	{
		Projectile projectile = PackedScene_PlayerProjectile.Instance() as Projectile;
		node_projectiles.AddChild(projectile);
		projectile.GlobalRotation = player.GlobalRotation;
		projectile.GlobalPosition = player.GlobalProjectileInstantiationPosition;
	}

	#endregion // Private methods

}

