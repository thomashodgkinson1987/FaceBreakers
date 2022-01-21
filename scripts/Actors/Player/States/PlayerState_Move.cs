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

	public override void OnEnter() { }

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
			Vector2 direction = m_player.InputController.Direction;
			float speed = m_player.Speed;
			Vector2 velocity = direction * speed;
			m_player.Velocity = velocity;
			m_player.Move(delta);

			if (!m_isHit)
			{
				if (m_isShootQueued)
				{
					m_isShootQueued = false;
					m_player.ShootProjectile();
				}
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

			m_isShootQueued = m_player.InputController.IsJustPressed_Shoot;
			int directionX = m_player.InputController.DirectionX;

			if (directionX == 0)
			{
				m_player.Set_Animation(Player.EAnimation.Straight);
			}
			else if (directionX == -1)
			{
				m_player.Set_Animation(Player.EAnimation.Left);
			}
			else if (directionX == 1)
			{
				m_player.Set_Animation(Player.EAnimation.Right);
			}
		}
	}

	public override void OnAreaEnteredHitbox(Area2D area)
	{
		m_isHit = true;
	}

	public override void OnBodyEnteredHitbox(PhysicsBody2D body)
	{
		m_isHit = true;
	}

	#endregion // PlayerState methods

}

