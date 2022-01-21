using Godot;

public class ProjectileState_Move : ProjectileState
{

	#region Fields

	private bool m_wasHit = false;
	private bool m_isHit = false;

	private bool m_wasTimeout = false;
	private bool m_isTimeout = false;

	#endregion // Fields



	#region Constructors

	public ProjectileState_Move(Projectile projectile) : base(projectile) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		Vector2 direction = (Vector2.Up).Rotated(m_projectile.Rotation);
		m_projectile.Velocity = direction * m_projectile.Speed;
	}

	public override void OnExit()
	{
		m_wasHit = false;
		m_isHit = false;

		m_wasTimeout = false;
		m_isTimeout = false;

		m_projectile.LifetimeTimer = 0f;
	}

	public override void OnPhysicsProcess(float delta)
	{
		if (!m_isTimeout)
		{
			if (m_projectile.LifetimeTimer < m_projectile.Lifetime)
			{
				m_projectile.LifetimeTimer += delta;
				if (m_projectile.LifetimeTimer < m_projectile.Lifetime)
				{
					m_projectile.Move(delta);
				}
				else
				{
					m_isTimeout = true;
				}
			}
		}

		if (m_isTimeout && !m_wasTimeout)
		{
			m_wasTimeout = true;
			m_projectile.Set_State(Projectile.EState.Free);
		}
		else if (m_isHit && !m_wasHit)
		{
			m_wasHit = true;
			m_projectile.Set_State(Projectile.EState.Die);
		}
	}

	public override void OnProcess(float delta) { }

	public override void OnAreaEnteredHitbox(Area2D area)
	{
		m_isHit = true;
	}

	public override void OnBodyEnteredHitbox(PhysicsBody2D body)
	{
		m_isHit = true;
	}

	#endregion // Public methods

}

