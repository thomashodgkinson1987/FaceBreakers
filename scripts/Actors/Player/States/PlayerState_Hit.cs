using Godot;

public class PlayerState_Hit : PlayerState
{

	#region Fields

	private bool m_wasDie = false;
	private bool m_isDie = false;

	private CPUParticles2D m_particles;

	#endregion // Fields



	#region Constructors

	public PlayerState_Hit(Player player) : base(player) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_player.Set_Visible(false);
		m_player.Set_CollisionEnabled(false);

		m_particles = m_player.PackedScene_DieParticles.Instance<CPUParticles2D>();
		m_player.AddChild(m_particles);
		m_particles.Emitting = true;
	}

	public override void OnExit()
	{
		m_wasDie = false;
		m_isDie = false;

		m_player.RemoveChild(m_particles);
		m_particles.QueueFree();
	}

	public override void OnPhysicsProcess(float delta)
	{
		if (!m_isDie)
		{
			if (!m_particles.Emitting)
			{
				m_isDie = true;
			}
		}

		if (m_isDie && !m_wasDie)
		{
			m_wasDie = true;
			m_player.EmitSignal(nameof(Player.OnHit));
			m_player.Set_State(Player.EState.Init);
		}
	}

	public override void OnProcess(float delta) { }

	public override void OnAreaEnteredHitbox(Area2D area) { }

	public override void OnBodyEnteredHitbox(PhysicsBody2D body) { }

	#endregion // Public methods

}

