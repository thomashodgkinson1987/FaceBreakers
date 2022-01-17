using Godot;

public class ProjectileState_Destroy : ProjectileState
{
    public ProjectileState_Destroy() { }

    public override void OnEnter(Projectile projectile)
    {
        projectile.PlaySound_Free();
        projectile.HideSprite();
        projectile.CallDeferred(nameof(projectile.DisableCollision));
    }
    public override void OnExit(Projectile projectile) { }

    public override void OnPhysicsProcess(Projectile projectile, float delta) { }
    public override void OnProcess(Projectile projectile, float delta)
    {
        if (!projectile.IsPlaying_Instantiate() && !projectile.IsPlaying_Free())
        {
            projectile.QueueFree();
        }
    }

    public override void OnAreaEntered(Projectile projectile, Area2D area) { }
    public override void OnBodyEntered(Projectile projectile, PhysicsBody2D body) { }
}

