using Godot;

public class ProjectileState_Die : ProjectileState
{

	#region Fields

	private bool m_wasFree = false;
	private bool m_isFree = false;

	private CPUParticles2D m_particles;

	#endregion // Fields



	#region Constructors

	public ProjectileState_Die(Projectile projectile) : base(projectile) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_projectile.Velocity = Vector2.Zero;
		m_projectile.Stop_Sound_All();
		m_projectile.Play_Sound(Projectile.ESound.Die);
		m_projectile.Set_Visible(false);
		m_projectile.Set_CollisionEnabled(false);

		m_particles = m_projectile.PackedScene_DieParticles.Instance<CPUParticles2D>();
		m_projectile.AddChild(m_particles);
		m_particles.Emitting = true;
	}

	public override void OnExit()
	{
		m_wasFree = false;
		m_isFree = false;

		m_projectile.RemoveChild(m_particles);
		m_particles.QueueFree();
	}

	public override void OnPhysicsProcess(float delta)
	{
		if (!m_isFree)
		{
			if (!m_projectile.Get_Sound_Playing(Projectile.ESound.Die)
					&& !m_particles.Emitting)
			{
				m_isFree = true;
			}
		}

		if (m_isFree && !m_wasFree)
		{
			m_wasFree = true;
			m_projectile.Set_State(Projectile.EState.Free);
		}
	}

	public override void OnProcess(float delta) { }

	public override void OnAreaEnteredHitbox(Area2D area) { }

	public override void OnBodyEnteredHitbox(PhysicsBody2D body) { }

	#endregion // Public methods

}

