using Godot;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

public partial class FloorGenerator : Node
{
	[Export]
	private Texture2D testFloor = null;

	[Export(PropertyHint.FilePath)]
	private string floorSquareObject;

	[Export]
	private Vector2 floorScale = new Vector2(10, 10);
	[Export]
	private float floorHeight = 0.0f;

#region Singleton
	private static FloorGenerator INSTANCE = null;

	public FloorGenerator()
	{
		if(INSTANCE == null)
		{
			INSTANCE = this;
		}
	}
#endregion

	public static void GenerateNextFloor()
	{
		if(INSTANCE.testFloor != null)
		{
			GenerateFloor(INSTANCE.testFloor.GetImage());
		}
	}

	public static void GenerateFloor(Image floorImage)
	{
		if(floorImage.IsCompressed())
		{
			Error e = floorImage.Decompress();
			if(e != Error.Ok)
			{
				GD.PrintErr("Image decompression error: " + e.ToString());
				return;
			}
		}

		PackedScene floorSquare = GD.Load<PackedScene>(INSTANCE.floorSquareObject);

		int width = floorImage.GetWidth(), height = floorImage.GetHeight();

		for(int x = 0; x < width; x++)
		{
			for(int y = 0; y < height; y++)
			{
				Color pixel = floorImage.GetPixel(x, y);
				generateFloorSquare(new Vector2(x, y) * INSTANCE.floorScale * 2, pixel, floorSquare);
			}
		} 
	}

	private static void generateFloorSquare(Vector2 floorPosition, Color color, PackedScene floorSquare)
	{
		Node floorSquareInstance = floorSquare.Instantiate();

		if(floorSquareInstance is FloorSquareTile parent)
		{
			parent.Position = new Vector3(floorPosition.X, INSTANCE.floorHeight, floorPosition.Y);
			parent.Scale *= new Vector3(INSTANCE.floorScale.X, 1.0f, INSTANCE.floorScale.Y);
			parent.SetColor(color);
		}

		INSTANCE.AddChild(floorSquareInstance);
	}

	/// <summary>
	/// Removes the floor square tiles that match the given color.
	/// </summary>
	/// <param name="color"></param>
	public static void DropFloor(Color color)
	{
		foreach(Node child in INSTANCE.GetChildren())
		{
			if(child is FloorSquareTile floorSquare)
			{
				if(floorSquare.IsColor(color))
				{
					floorSquare.Drop();
				}
			}
		}
	}
}
