using Godot;

public class ProjectileState_Destroying : ProjectileState
{

	#region Constructors

	public ProjectileState_Destroying(Projectile projectile) : base(projectile) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_projectile.PlaySound_Free();
		m_projectile.HideSprite();
		m_projectile.DisableCollision();
	}

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta) { }

	public override void OnProcess(float delta)
	{
		if (!m_projectile.IsPlaying_Instantiate() && !m_projectile.IsPlaying_Free())
		{
			m_projectile.QueueFree();
		}
	}

	public override void OnAreaEntered(Area2D area) { }

	public override void OnBodyEntered(PhysicsBody2D body) { }

	#endregion // Public methods

}

