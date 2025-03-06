using Godot;

public partial class Player : CharacterBody2D
{
    [Export]
    public float Speed = 300.0f;
    [Export]
    public float JumpVelocity = -400.0f;
    
    private AnimatedSprite2D sprite;
    private Vector2 viewport;
    private bool attacking;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        viewport = GetViewportRect().Size;
        sprite.AnimationFinished += OnAnimationFinished;
        GD.Print(viewport.Y);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Apply gravity
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        // Handle Jump
        if (Input.IsActionJustPressed("Jump") && IsOnFloor() && !attacking)
        {
            velocity.Y = JumpVelocity;
        }

        // Attack logic
        if (Input.IsActionJustPressed("Attack") && IsOnFloor())
        {
            attacking = true;
            GetNode<Area2D>("AttackArea").Monitoring = true;
            
            // Only change attack animation, let movement animations continue
            if (velocity.X != 0) {
                sprite.Animation = "run_attack";
            } else {
                sprite.Animation = "idle_attack";
			}
            sprite.Play();
        }

        // Get movement input
        Vector2 direction = Input.GetVector("Left", "Right", "Jump", "Down");
        if (direction != Vector2.Zero)
        {
            sprite.FlipH = direction.X < 0;
            velocity.X = direction.X * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        // Movement animations (ONLY IF NOT ATTACKING)
        if (!attacking)
        {
            if (!IsOnFloor())
            {
                sprite.Animation = "jump";
            }
            else if (direction != Vector2.Zero)
            {
                sprite.Animation = "run";
            }
            else
            {
                sprite.Animation = "idle";
            }

            sprite.Play();
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    // Handle animation finishing
    private void OnAnimationFinished()
    {
        if (sprite.Animation == "run_attack" || sprite.Animation == "idle_attack") 
        {
            attacking = false;
            GetNode<Area2D>("AttackArea").Monitoring = false;
        }
    }



	public void OnBodyEntered(Node2D body) 
	{
		if (body.IsInGroup("enemy")) 
		{
			// damage logic to enemy
		}
	}
}
