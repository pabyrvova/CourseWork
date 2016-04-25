using System;
using System.Collections.Generic;
using System.Linq;

namespace PointsContainer
{
	public class PContainer
	{
		public readonly List<int> LocalGlobalOrderMapping = new List<int> {0, 8, 1, 11, 9, 3, 10, 2, 12, 13, 15, 14, 4, 16, 5, 19, 17, 7, 18, 6};
		//Size
		private double _totalHeight = 10;
		private double _totalWidth = 10;
		private double _totalDepth = 30;

		//Number of elements per side
		private int _feNumberPerH = 3;
		private int _feNumberPerW = 2;
		private int _feNumberPerD = 3;

		//Number of levels per side
		private int _levelNumberPerH;
		private int _levelNumberPerW;
		private int _levelNumberPerD;

		//Total number of points
		private int _totalPointsNumber;
		private readonly List<GlobalPoint> _globalPoints = new List<GlobalPoint>();
		private readonly List<FiniteElement> _finiteElements = new List<FiniteElement>();
		private readonly Dictionary<int, List<int>> _globalPointsFe = new Dictionary<int, List<int>>();
		private int[,] _localGlobalMapping;

		public void Init()
		{
			InitDimensionLevelNumber(out _levelNumberPerH, _feNumberPerH);
			InitDimensionLevelNumber(out _levelNumberPerW, _feNumberPerW);
			InitDimensionLevelNumber(out _levelNumberPerD, _feNumberPerD);
			InitPointsTotalNumber();
			InitFiniteElements();
			InitGlobalPointsContainer();
			InitLocalGlobalMapping();
		}

		public double TotalHeight
		{
			get { return _totalHeight; }
			set { _totalHeight = value; }
		}

		public double TotalWidth
		{
			get { return _totalWidth; }
			set { _totalWidth = value; }
		}

		public double TotalDepth
		{
			get { return _totalDepth; }
			set { _totalDepth = value; }
		}

		public int NumberOfLastFEUnderPresure
		{
			get { return _feNumberPerH * _feNumberPerW; }
		}

		public int FeNumberPerH
		{
			get { return _feNumberPerH; }
			set { _feNumberPerH = value; }
		}

		public int FeNumberPerW
		{
			get { return _feNumberPerW; }
			set { _feNumberPerW = value; }
		}

		public int FeNumberPerD
		{
			get { return _feNumberPerD; }
			set { _feNumberPerD = value; }
		}

		public int[,] LocalGlobalMapping
		{
			get { return _localGlobalMapping; }
			set { _localGlobalMapping = value; }
		}

		public List<FiniteElement> FiniteElements
		{
			get { return _finiteElements; }
		}

		public List<GlobalPoint> GlobalPoints
		{
			get { return _globalPoints; }
		}

		private void InitLocalGlobalMapping()
		{
			_localGlobalMapping = new int[_finiteElements.Count, 20];
			for (int lF = 0; lF < _finiteElements.Count; lF++)
			{
				List<int> feAllGlobalPoints = GetFeAllGlobalPoints(lF);
				for (int l = 0; l < feAllGlobalPoints.Count; l++)
				{
					int indexOf = LocalGlobalOrderMapping.IndexOf(l);
					if (indexOf == -1)
					{
						throw new Exception();
					}
					_localGlobalMapping[lF, l] = feAllGlobalPoints[indexOf];
					//Trace.WriteLine(string.Format("FEN:{0} LocalPointN:{1} GlobalPointN:{2}", lF, l, _localGlobalMapping[lF, l]));
				}
			}
		}

		private List<int> GetFeAllGlobalPoints(int feNumber)
		{
			List<int> fePonits = new List<int>();
			foreach (KeyValuePair<int, List<int>> pair in _globalPointsFe)
			{
				if (pair.Value.Contains(feNumber))
				{
					fePonits.Add(pair.Key);
				}
			}
			if (fePonits.Count != 20)
			{
				throw new Exception();
			}
			return fePonits;
		}

		private void InitFiniteElements()
		{
			for (int lD = 0; lD < _feNumberPerD; lD++)
			{
				for (int lW = 0; lW < _feNumberPerW; lW++)
				{
					for (int lH = 0; lH < _feNumberPerH; lH++)
					{
						_finiteElements.Add(new FiniteElement(lH, lW, lD));
					}
				}
			}
		}

		private void InitGlobalPointsContainer()
		{
			for (int lD = 0; lD < _levelNumberPerD; lD++)
			{
				bool skipD = lD % 2 != 0;
				for (int lW = 0; lW < _levelNumberPerW; lW++)
				{
					bool skipW = lW % 2 != 0;
					if (skipD && skipW)
					{
						continue;
					}
					for (int lH = 0; lH < _levelNumberPerH; lH++)
					{
						bool skipH = lH % 2 != 0;
						if ((skipW && skipH) || (skipD && skipH))
						{
							continue;
						}
						_globalPoints.Add(new GlobalPoint(GetPointByLevel(lH, lW, lD), lH, lW, lD));
						_globalPointsFe.Add(_globalPoints.Count - 1, GetFeByPoint(lH, lW, lD));
					}
				}
			}
		}

		private List<int> GetFeByPoint(int levelH, int levelW, int levelD)
		{
			List<int> feByHightLevel = GetFeByHeightLevel(levelH);
			List<int> feByWidthLevel = GetFeByWidthLevel(levelW);
			List<int> feByDepthLevel = GetFeByDepthLevel(levelD);
			return feByHightLevel.Intersect(feByWidthLevel).Intersect(feByDepthLevel).ToList();
		}

		private List<int> GetFeByHeightLevel(int levelH)
		{
			int i = levelH + 1;
			double ceiling = Math.Ceiling((double) (i * _feNumberPerH) / _levelNumberPerH + 0.00000000000001 - 1);
			List<int> finiteElemsnts = new List<int>();
			if (i % 2 == 0)
			{
				for (int l = 0; l < _finiteElements.Count; l++)
				{
					if (_finiteElements[l].LevelHeightNumber == ceiling)
					{
						finiteElemsnts.Add(l);
					}
				}
			}
			else
			{
				for (int l = 0; l < _finiteElements.Count; l++)
				{
					if (_finiteElements[l].LevelHeightNumber == ceiling || _finiteElements[l].LevelHeightNumber == (ceiling - 1))
					{
						finiteElemsnts.Add(l);
					}
				}
			}
			return finiteElemsnts;
		}

		private List<int> GetFeByWidthLevel(int levelW)
		{
			int i = levelW + 1;
			double ceiling = Math.Ceiling((double) (i * _feNumberPerW) / _levelNumberPerW + 0.00000000000001 - 1);
			List<int> finiteElemsnts = new List<int>();
			if (i % 2 == 0)
			{
				for (int l = 0; l < _finiteElements.Count; l++)
				{
					if (_finiteElements[l].LevelWidthNumber == ceiling)
					{
						finiteElemsnts.Add(l);
					}
				}
			}
			else
			{
				for (int l = 0; l < _finiteElements.Count; l++)
				{
					if (_finiteElements[l].LevelWidthNumber == ceiling || _finiteElements[l].LevelWidthNumber == (ceiling - 1))
					{
						finiteElemsnts.Add(l);
					}
				}
			}
			return finiteElemsnts;
		}

		private List<int> GetFeByDepthLevel(int levelD)
		{
			int i = levelD + 1;
			double ceiling = Math.Ceiling((double) (i * _feNumberPerD) / _levelNumberPerD + 0.00000000000001 - 1);
			List<int> finiteElemsnts = new List<int>();
			if (i % 2 == 0)
			{
				for (int l = 0; l < _finiteElements.Count; l++)
				{
					if (_finiteElements[l].LevelDepthNumber == ceiling)
					{
						finiteElemsnts.Add(l);
					}
				}
			}
			else
			{
				for (int l = 0; l < _finiteElements.Count; l++)
				{
					if (_finiteElements[l].LevelDepthNumber == ceiling || _finiteElements[l].LevelDepthNumber == (ceiling - 1))
					{
						finiteElemsnts.Add(l);
					}
				}
			}
			return finiteElemsnts;
		}

		private Point GetPointByLevel(int levelH, int levelW, int levelD)
		{
			double halfHeight = _totalHeight / 2.0;
			double halfWidth = _totalWidth / 2.0;
			double halfDepth = _totalDepth / 2.0;

			double heightStep = _totalHeight / (_levelNumberPerH - 1);
			double widthStep = _totalWidth / (_levelNumberPerW - 1);
			double depthtStep = _totalDepth / (_levelNumberPerD - 1);

			double x = halfHeight - (_levelNumberPerH - 1 - levelH) * heightStep;
			double y = halfWidth - (_levelNumberPerW - 1 - levelW) * widthStep;
			double z = halfDepth - (_levelNumberPerD - 1 - levelD) * depthtStep;

			return new Point(x, y, z);
		}

		private void InitPointsTotalNumber()
		{
			for (int l = 0; l < _levelNumberPerD; l++)
			{
				_totalPointsNumber += GetPointsNumberPerLevel(l % 2 == 0);
			}
		}

		public int TotalPointsNumber
		{
			get { return _totalPointsNumber; }
			set { _totalPointsNumber = value; }
		}

		private int GetPointsNumberPerLevel(bool isEven)
		{
			if (isEven)
			{
				return _levelNumberPerH * _levelNumberPerW - _feNumberPerH * _feNumberPerW;
			}
			return (_feNumberPerH + 1) * (_feNumberPerW + 1);
		}

		private void InitDimensionLevelNumber(out int levelNumberPer, int feNumberPer)
		{
			levelNumberPer = feNumberPer * 2 + 1;
		}
	}
}