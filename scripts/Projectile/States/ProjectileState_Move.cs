using Godot;

public class ProjectileState_Move : ProjectileState
{

	#region Constructors

	public ProjectileState_Move(Projectile projectile) : base(projectile) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter() { }

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta)
	{
		if (m_projectile.LifetimeTimer < m_projectile.Lifetime)
		{
			m_projectile.LifetimeTimer += delta;
			if (m_projectile.LifetimeTimer < m_projectile.Lifetime)
			{
				Vector2 translation = (Vector2.Up).Rotated(m_projectile.Rotation);
				translation *= m_projectile.Speed * delta;
				m_projectile.Translate(translation);
			}
			else
			{
				m_projectile.Set_State(Projectile.EState.Destroy);
			}
		}
	}

	public override void OnProcess(float delta) { }

	public override void OnAreaEntered(Area2D area)
	{
		m_projectile.Set_State(Projectile.EState.Destroy);
	}

	public override void OnBodyEntered(PhysicsBody2D body)
	{
		m_projectile.Set_State(Projectile.EState.Destroy);
	}

	#endregion // Public methods

}

