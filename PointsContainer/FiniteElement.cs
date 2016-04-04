using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointsContainer
{
	public class FiniteElement
	{
		public int LevelHeightNumber { get; set; }
		public int LevelWidthNumber { get; set; }
		public int LevelDepthNumber { get; set; } 

		public FiniteElement(int levelHeightNumber, int levelWidthNumber, int levelDepthNumber)
		{
			LevelHeightNumber = levelHeightNumber;
			LevelWidthNumber = levelWidthNumber;
			LevelDepthNumber = levelDepthNumber;
			
		}
	}
}
