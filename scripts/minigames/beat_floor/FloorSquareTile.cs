using Godot;
using System;

public partial class FloorSquareTile : Node3D
{
	[Export]
	private MeshInstance3D mesh;

	private const float COLOR_DIFFERENCE_CHECK = 0.1f;
	private const double DROP_ANIMATION_LENGTH = 1.33;
	
	public Color Color {get; private set;}
	
	private double dropTimer = 0.0;

	public void SetColor(Color color)
	{
		StandardMaterial3D newMaterial = new StandardMaterial3D();
		newMaterial.AlbedoColor = color;
		newMaterial.MetallicSpecular = 0;
		newMaterial.Metallic = 0;
		newMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha; //To allow fade out animation.

		mesh.MaterialOverride = newMaterial;

		this.Color = color;
	}

	/// <summary>
	/// Returns true if the given color is within a threshold of similarity to the color of this floor tile.
	/// </summary>
	/// <param name="color"></param>
	/// <returns></returns>
	public bool IsColor(Color color)
	{
		float rDiff = Mathf.Abs(this.Color.R - color.R);
		float gDiff = Mathf.Abs(this.Color.G - color.G);
		float bDiff = Mathf.Abs(this.Color.B - color.B);

		float totalDiff = rDiff + gDiff + bDiff;

		return totalDiff < COLOR_DIFFERENCE_CHECK;
	}

	/// <summary>
	/// Causes the floor to begin disappearing.
	/// </summary>
	public void Drop()
	{
		if(dropTimer > 0.0){ return; }
		dropTimer += 0.01;
	}

    public override void _Process(double delta)
    {
		//If the tile is supposed to be fading out.
        if(dropTimer > 0.0)
		{
			//Animation done, remove object.
			if(dropTimer > DROP_ANIMATION_LENGTH)
			{
				this.QueueFree();
				return;
			}

			//Slowly fade out the square if it is dropping.
			if(mesh.MaterialOverride is StandardMaterial3D material)
			{
				Color col = material.AlbedoColor;
				material.AlbedoColor = new Color(col.R, col.G, col.B, (float)(1.0 - dropTimer / DROP_ANIMATION_LENGTH));
			}

			//Slowly drop the square down.
			this.Translate(new Vector3(0.0f, -(float)delta, 0.0f));

			dropTimer += delta;
		}
    }

}
