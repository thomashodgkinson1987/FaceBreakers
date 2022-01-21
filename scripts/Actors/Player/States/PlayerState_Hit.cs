using Godot;

public class PlayerState_Hit : PlayerState
{

	#region Constructors

	public PlayerState_Hit(Player player) : base(player) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_player.SetProjectileStates(Projectile.EState.Die);

		if (m_player.Lives > 0)
		{
			m_player.Lives -= 1;
			m_player.EmitSignal(nameof(Player.OnHit));
			m_player.Set_State(Player.EState.Move);
		}
		else
		{
			m_player.EmitSignal(nameof(Player.OnHit));
			m_player.Set_State(Player.EState.Die);
		}
	}

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta) { }

	public override void OnProcess(float delta) { }

	public override void OnAreaEnteredHitbox(Area2D area) { }

	public override void OnBodyEnteredHitbox(PhysicsBody2D body) { }

	#endregion // Public methods

}

