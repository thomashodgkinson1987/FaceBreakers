using Godot;

public class MainSceneController : Node2D
{

    #region Nodes

    private GameHUDController node_gameHUDController;

    private Node2D node_actors;
    private Node2D node_projectiles;

    private Player node_player;

    #endregion // Nodes



    #region Godot methods

    public override void _Ready()
    {
        node_gameHUDController = GetNode<GameHUDController>("GameHUD");

        node_actors = GetNode<Node2D>("Actors");
        node_projectiles = GetNode<Node2D>("Projectiles");

        node_player = node_actors.GetNode<Player>("Player");

        node_player.Connect(nameof(Player.OnJustPressed_Shoot), this, nameof(OnShoot_Player));
    }

    #endregion // Godot methods



    #region Private methods

    private void OnShoot_Player(Player player, Projectile projectile)
    {
        projectile.GetParent().RemoveChild(projectile);
        node_projectiles.AddChild(projectile);
    }

    #endregion // Private methods

}

