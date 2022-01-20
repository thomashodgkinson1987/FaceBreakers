using Godot;

public class PlayerState_Move : PlayerState
{

	#region Fields

	private bool m_isHit = false;
	private bool m_wasHit = false;

	private bool m_isShootQueued = false;

	#endregion // Fields



	#region Constructors

	public PlayerState_Move(Player player) : base(player) { }

	#endregion // Constructors



	#region PlayerState methods

	public override void OnEnter()
	{
		m_player.InputController.Reset();
		m_player.Set_Animation("straight");
	}

	public override void OnExit()
	{
		m_wasHit = false;
		m_isHit = false;

		m_isShootQueued = false;
	}

	public override void OnPhysicsProcess(float delta)
	{
		if (!m_isHit)
		{
			m_player.Velocity = m_player.InputController.Direction * m_player.Speed;
			m_player.Move();
		}

		if (!m_isHit)
		{
			if (m_isShootQueued)
			{
				m_isShootQueued = false;
				m_player.ShootProjectile();
			}
		}

		if (m_isHit && !m_wasHit)
		{
			m_wasHit = true;

			m_player.Set_State(Player.EState.Hit);
		}
	}

	public override void OnProcess(float delta)
	{
		if (!m_isHit)
		{
			m_player.InputController.Poll();

			if (m_player.InputController.IsJustPressed_Shoot)
			{
				m_isShootQueued = true;
			}

			if (m_player.InputController.DirectionX == 0)
			{
				m_player.Set_Animation("straight");
			}
			else if (m_player.InputController.DirectionX == -1)
			{
				m_player.Set_Animation("left");
			}
			else if (m_player.InputController.DirectionX == 1)
			{
				m_player.Set_Animation("right");
			}
			else
			{
				m_player.Set_Animation("default");
			}
		}
	}

	public override void OnAreaEntered(Area2D area)
	{
		m_isHit = true;
	}

	public override void OnBodyEntered(PhysicsBody2D body)
	{
		m_isHit = true;
	}

	#endregion // PlayerState methods

}

