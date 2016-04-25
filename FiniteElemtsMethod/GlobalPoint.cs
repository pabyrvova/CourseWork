namespace PointsContainer
{
	public class GlobalPoint
	{
		public Point Point { get; set; }
		public int LevelHeightNumber { get; set; }
		public int LevelWidthNumber { get; set; }
		public int LevelDepthNumber { get; set; }

		public GlobalPoint(Point point, int levelHeightNumber, int levelWidthNumber, int levelDepthNumber)
		{
			Point = point;
			LevelHeightNumber = levelHeightNumber;
			LevelWidthNumber = levelWidthNumber;
			LevelDepthNumber = levelDepthNumber;
		}

		public GlobalPoint Clone()
		{
			return new GlobalPoint(new Point(Point.X, Point.Y, Point.Z), LevelHeightNumber, LevelWidthNumber, LevelDepthNumber);
		}
	}

	public class Point
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		public Point(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}
}