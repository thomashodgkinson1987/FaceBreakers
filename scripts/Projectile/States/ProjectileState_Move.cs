using Godot;

public class ProjectileState_Move : ProjectileState
{

	#region Constructors

	public ProjectileState_Move(Projectile projectile) : base(projectile) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_projectile.PlaySound_Instantiate();
	}

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta)
	{
		if (m_projectile.LifetimeTimer < m_projectile.Lifetime)
		{
			m_projectile.LifetimeTimer += delta;
			if (m_projectile.LifetimeTimer < m_projectile.Lifetime)
			{
				Vector2 translation = (Vector2.Up).Rotated(m_projectile.Rotation);
				translation *= m_projectile.MoveSpeed * delta;
				m_projectile.Translate(translation);
			}
			else
			{
				//TODO: Find better way of managing states (FSM)
				m_projectile.SetState(new ProjectileState_Destroy(m_projectile));
			}
		}
	}

	public override void OnProcess(float delta) { }

	public override void OnAreaEntered(Area2D area)
	{
		//TODO Find better way of managing states (FSM)
		m_projectile.SetState(new ProjectileState_Destroy(m_projectile));
	}

	public override void OnBodyEntered(PhysicsBody2D body)
	{
		//TODO Find better way of managing states (FSM)
		m_projectile.SetState(new ProjectileState_Destroy(m_projectile));
	}

	#endregion // Public methods

}

