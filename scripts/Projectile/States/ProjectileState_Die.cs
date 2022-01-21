using Godot;

public class ProjectileState_Die : ProjectileState
{

	#region Fields

	private bool m_wasFreeConditionMet = false;
	private bool m_isFreeConditionMet = false;

	#endregion // Fields



	#region Constructors

	public ProjectileState_Die(Projectile projectile) : base(projectile) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_projectile.Play_Sound(Projectile.ESound.Die);
		m_projectile.Set_Visible(false);
		m_projectile.Set_CollisionEnabled(false);
	}

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta) { }

	public override void OnProcess(float delta)
	{
		if (!m_isFreeConditionMet)
		{
			if (!m_projectile.Get_SoundPlaying(Projectile.ESound.Init) && !m_projectile.Get_SoundPlaying(Projectile.ESound.Die))
			{
				m_isFreeConditionMet = true;
			}
		}

		if (m_isFreeConditionMet && !m_wasFreeConditionMet)
		{
			m_wasFreeConditionMet = true;

			m_projectile.Set_State(Projectile.EState.Free);
		}
	}

	public override void OnAreaEntered(Area2D area) { }

	public override void OnBodyEntered(PhysicsBody2D body) { }

	#endregion // Public methods

}

