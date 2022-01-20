using Godot;

public class PlayerState_Init : PlayerState
{

	#region Constructors

	public PlayerState_Init(Player player) : base(player) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_player.Velocity = Vector2.Zero;
		m_player.InputController.Reset();
		m_player.FreeAllProjectiles();
		m_player.Set_Visible(true);
		m_player.Set_CollisionEnabled(true);
		m_player.Set_Animation("default");
		m_player.Set_State(Player.EState.Move);
	}

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta) { }

	public override void OnProcess(float delta) { }

	public override void OnAreaEntered(Area2D area) { }

	public override void OnBodyEntered(PhysicsBody2D body) { }

	#endregion // Public methods

}

