using Godot;

public class PlayerState_Hit : PlayerState
{

	#region Fields

	private bool m_wasFinishedEmitting = false;
	private bool m_isFinishedEmitting = false;

	#endregion // Fields



	#region Constructors

	public PlayerState_Hit(Player player) : base(player) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_player.Set_Visible(false);
		m_player.Set_CollisionEnabled(false);
		m_player.Set_Emitting(true);
	}

	public override void OnExit()
	{
		m_wasFinishedEmitting = false;
		m_isFinishedEmitting = false;
	}

	public override void OnPhysicsProcess(float delta)
	{
		if (!m_isFinishedEmitting)
		{
			if (!m_player.Get_Emitting())
			{
				m_isFinishedEmitting = true;
			}
		}

		if (m_isFinishedEmitting && !m_wasFinishedEmitting)
		{
			m_wasFinishedEmitting = true;
			m_player.EmitSignal(nameof(Player.OnHit));
			m_player.Set_State(Player.EState.Init);
		}
	}

	public override void OnProcess(float delta) { }

	public override void OnAreaEnteredHitbox(Area2D area) { }

	public override void OnBodyEnteredHitbox(PhysicsBody2D body) { }

	#endregion // Public methods

}

