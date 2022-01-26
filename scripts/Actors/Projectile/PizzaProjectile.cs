using Godot;

public class PizzaProjectile : Projectile
{

	#region Godot methods

	public override void _PhysicsProcess(float delta)
	{
		node_sprite.Rotate(Mathf.Deg2Rad(5.625f));
		base._PhysicsProcess(delta);
	}

	#endregion // Godot methods

}

