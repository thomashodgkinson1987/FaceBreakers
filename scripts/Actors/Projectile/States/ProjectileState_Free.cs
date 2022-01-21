using Godot;

public class ProjectileState_Free : ProjectileState
{

	#region Constructors

	public ProjectileState_Free(Projectile projectile) : base(projectile) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_projectile.Set_Visible(false);
		m_projectile.Set_CollisionEnabled(false);
		m_projectile.QueueFree();
	}

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta) { }

	public override void OnProcess(float delta) { }

	public override void OnAreaEnteredHitbox(Area2D area) { }

	public override void OnBodyEnteredHitbox(PhysicsBody2D body) { }

	#endregion // Public methods

}

