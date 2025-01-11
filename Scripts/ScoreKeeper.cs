using System;
using System.Security.Cryptography;
using Godot;

public partial class ScoreKeeper : CharacterBody3D //attach to hat/cart ideally
{
	RichTextLabel Score;
	double timeLeftInLevel;
	double timeElapsedInLevel;
	bool counting = false;
	public override void _Ready()
	{
		
		base._Ready();
		
		Score = GetNode<RichTextLabel>("Score");
		counting = true;
		ResetScore();
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
		HandleCollisions();
		timeLeftInLevel -= delta;
		timeElapsedInLevel += delta;
	}

	private void HandleCollisions()
	{
		// Check for collisions after movement
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            // if (collision.GetCollider().GetMeta("Owner").ToString() != "")
			CollisionShape3D _collider = (CollisionShape3D)collision.GetCollider();
            string colliderMetaName = collision.GetCollider().GetMeta("Name").AsString();

            //handle Ball collision
            if (colliderMetaName == "ScoreableItem")
            {
				
                int score = Score.Text.ToInt();
				ScoreableShelfContent ssc = (ScoreableShelfContent)_collider.GetParent();
				score+=ssc.ScoreValue; //ScorableShelfContent.Score;
				Score.Text = score.ToString();
				ssc.DeleteSelf();
            }
            // GD.Print($"Collided with: {collision.GetCollider().GetMeta("Name").AsString()}");
        }
	}

	private void ResetScore()
	{
		Score.Text = "0";
	}

}
