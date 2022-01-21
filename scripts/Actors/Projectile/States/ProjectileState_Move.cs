using Godot;

public class ProjectileState_Move : ProjectileState
{

	#region Fields

	private bool m_wasDieConditionMet = false;
	private bool m_isDieConditionMet = false;

	#endregion // Fields



	#region Constructors

	public ProjectileState_Move(Projectile projectile) : base(projectile) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter() { }

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta)
	{
		if (!m_isDieConditionMet)
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
					m_projectile.Set_State(Projectile.EState.Free);
				}
			}
		}
		
		if (m_isDieConditionMet && !m_wasDieConditionMet)
		{
			m_wasDieConditionMet = true;

			m_projectile.Set_State(Projectile.EState.Die);
		}
	}

	public override void OnProcess(float delta) { }

	public override void OnAreaEntered(Area2D area)
	{
		m_isDieConditionMet = true;
	}

	public override void OnBodyEntered(PhysicsBody2D body)
	{
		m_isDieConditionMet = true;
	}

	#endregion // Public methods

}

