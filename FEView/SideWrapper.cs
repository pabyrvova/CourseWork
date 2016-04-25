using System.Collections.Generic;
using PointsContainer;

namespace FEView
{
	public class SideWrapper
	{
		private bool First { get; set; }
		public int FeNumberPerH { get; set; }
		public int FeNumberPerW { get; set; }
		public int FeNumberPerD { get; set; }
		public ESide SideUnderPresure { get; set; }
		public int Number { get; set; }

		public SideWrapper(int feNumberPerH, int feNumberPerW, int feNumberPerD, ESide sideUnderPresure, bool first, int number)
		{
			First = first;
			FeNumberPerH = feNumberPerH;
			FeNumberPerW = feNumberPerW;
			FeNumberPerD = feNumberPerD;
			SideUnderPresure = sideUnderPresure;
			Number = number;
		}

		public bool IsFeUnderPresure(FiniteElement element)
		{
			LevelsLinePresure levelsLinePresure = GetLevelsLine();
			return levelsLinePresure.LevelsH.Contains(element.LevelHeightNumber) && levelsLinePresure.LevelsW.Contains(element.LevelWidthNumber) && levelsLinePresure.LevelsD.Contains(element.LevelDepthNumber);
		}

		private LevelsLinePresure GetLevelsLine()
		{
			List<int> levelsH = new List<int>();
			List<int> levelsD = new List<int>();
			List<int> levelsW = new List<int>();
			int leadingLevel;
			switch (SideUnderPresure)
			{
				case ESide.SideDh:
					leadingLevel = First ? 0 : FeNumberPerW - 1;
					levelsW.Add(leadingLevel);
					for (int i = 0; i < FeNumberPerH; i++)
					{
						levelsH.Add(i);
					}
					for (int j = 0; j < FeNumberPerD; j++)
					{
						levelsD.Add(j);
					}
					break;
				case ESide.SideDw:
					leadingLevel = First ? 0 : FeNumberPerH - 1;
					levelsH.Add(leadingLevel);
					for (int i = 0; i < FeNumberPerW; i++)
					{
						levelsW.Add(i);
					}
					for (int j = 0; j < FeNumberPerD; j++)
					{
						levelsD.Add(j);
					}
					break;
				case ESide.SideHw:
					leadingLevel = First ? 0 : FeNumberPerD - 1;
					levelsD.Add(leadingLevel);
					for (int i = 0; i < FeNumberPerW; i++)
					{
						levelsW.Add(i);
					}
					for (int j = 0; j < FeNumberPerH; j++)
					{
						levelsH.Add(j);
					}
					break;
			}
			return new LevelsLinePresure(levelsH, levelsW, levelsD);
		}

		public override string ToString()
		{
			return Number.ToString();
		}
	}

	public class LevelsLinePresure
	{
		private readonly List<int> _levelsH = new List<int>();
		private readonly List<int> _levelsW = new List<int>();
		private readonly List<int> _levelsD = new List<int>();

		public LevelsLinePresure(List<int> levelsH, List<int> levelsW, List<int> levelsD)
		{
			LevelsH = levelsH;
			LevelsW = levelsW;
			LevelsD = levelsD;
		}

		public LevelsLinePresure()
		{
		}

		public List<int> LevelsH
		{
			get { return _levelsH; }
			set
			{
				_levelsH.Clear();
				if (value == null)
				{
					return;
				}
				_levelsH.AddRange(value);
			}
		}

		public List<int> LevelsW
		{
			get { return _levelsW; }
			set
			{
				_levelsW.Clear();
				if (value == null)
				{
					return;
				}
				_levelsW.AddRange(value);
			}
		}

		public List<int> LevelsD
		{
			get { return _levelsD; }
			set
			{
				_levelsD.Clear();
				if (value == null)
				{
					return;
				}
				_levelsD.AddRange(value);
			}
		}
	}

	public enum ESide
	{
		SideDh,
		SideDw,
		SideHw
	}
}