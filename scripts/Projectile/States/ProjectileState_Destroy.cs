using Godot;

public class ProjectileState_Destroy : ProjectileState
{

	#region Constructors

	public ProjectileState_Destroy(Projectile projectile) : base(projectile) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_projectile.PlaySound(Projectile.ESound.Free);
		m_projectile.Set_SpriteVisible(false);
		m_projectile.Set_CollisionEnabled(false);
	}

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta) { }

	public override void OnProcess(float delta)
	{
		if (!m_projectile.IsPlaying(Projectile.ESound.Init) && !m_projectile.IsPlaying(Projectile.ESound.Free))
		{
			m_projectile.QueueFree();
		}
	}

	public override void OnAreaEntered(Area2D area) { }

	public override void OnBodyEntered(PhysicsBody2D body) { }

	#endregion // Public methods

}

