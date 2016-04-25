using System;
using System.Collections.Generic;
using PointsContainer;

namespace FiniteElemtsMethod
{
	public class StandartSquare
	{
		private readonly List<GlobalPoint> _globalPoints = new List<GlobalPoint>();
		private readonly List<Point> _localPoints = new List<Point>();
		private FiniteElement _element;
		private readonly double[,,] DXYZET = new double[3, 2, 9]; //ex. x,eta,Gaus
		public static double[] X = {-Math.Sqrt(0.6), 0, Math.Sqrt(0.6)};
		public static double[] C = {5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0};
		private readonly double[] FE = new double[60];
		public readonly Dictionary<int, List<int>> Dictionary = new Dictionary<int, List<int>>();
		public int presureSurfaceNumber { get; set; }
		public bool isUnderPresure;
		public double Pn { get; set; }

		public bool IsUnderPresure
		{
			get { return isUnderPresure; }
			set { isUnderPresure = value; }
		}

		public double[] Fe
		{
			get { return FE; }
		}

		public void Init()
		{
			InitDictionary();
			InitLocalPoints();
			InitDXYZET();
			InitFe();
		}

		private void InitDictionary()
		{
			Dictionary.Add(0, new List<int> {0, 1, 5, 4, 8, 13, 16, 12});
			Dictionary.Add(1, new List<int> {1, 2, 6, 5, 9, 14, 17, 13});
			Dictionary.Add(2, new List<int> {2, 3, 7, 6, 10, 15, 18, 14});
			Dictionary.Add(3, new List<int> {3, 0, 4, 7, 11, 12, 19, 15});
			Dictionary.Add(4, new List<int> {0, 1, 2, 3, 8, 9, 10, 11});
			Dictionary.Add(5, new List<int> {4, 5, 6, 7, 16, 17, 18, 19});
		}

		private void InitFe()
		{
			List<Fi> list = new List<Fi> { new Fi(DXYZET, Fi, 1, 2, isUnderPresure, Pn), new Fi(DXYZET, Fi, 2, 0, isUnderPresure, Pn), new Fi(DXYZET, Fi, 0, 1, isUnderPresure, Pn) };
			List<int> ints = Dictionary[presureSurfaceNumber];
			int levelCounter = 20;
			for (int i = 0; i < 3; i++)
			{
				int coefI = i * levelCounter;
				double[] doubles = list[i].GetFe();
				for (int j = 0; j < doubles.GetLength(0); j++)
				{
					FE[coefI + ints[j]] = doubles[j];
				}
			}
		}

		public void InitDXYZET()
		{
			List<int> ints = Dictionary[presureSurfaceNumber];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					int counter = 0;
					for (int k = 0; k < 3; k++)
					{
						for (int l = 0; l < 3; l++)
						{
							DXYZET[i, j, counter] = Sum(index =>
							{
								if (i == 0)
								{
									return _globalPoints[ints[index]].Point.X * FiDerivate(X[k], X[l], index, j);
								}
								if (i == 1)
								{
									return _globalPoints[ints[index]].Point.Y * FiDerivate(X[k], X[l], index, j);
								}
								return _globalPoints[ints[index]].Point.Z * FiDerivate(X[k], X[l], index, j);
							},
								8);
							counter++;
						}
					}
				}
			}
		}

		private double Fi(double eta, double tau, int i)
		{
			if (i <= 3)
			{
				return Fi14(eta, tau, i);
			}
			if (i == 4 || i == 6)
			{
				return Fi57(eta, tau, i);
			}
			return Fi68(eta, tau, i);
		}

		private double FiDerivate(double eta, double tau, int i, int et)
		{
			if (i <= 3)
			{
				if (et == 0)
				{
					return Fi14DerivateEta(eta, tau, i);
				}
				return Fi14DerivateTau(eta, tau, i);
			}
			if (i == 4 || i == 6)
			{
				if (et == 0)
				{
					return Fi57DerivateEta(eta, tau, i);
				}
				return Fi57DerivateTau(eta, i);
			}
			if (et == 0)
			{
				return Fi68DerivateEta(tau, i);
			}
			return Fi68DerivateTau(eta, tau, i);
		}

		public static double Sum(Func<int, double> func, int n)
		{
			double sum = 0;
			for (int l = 0; l < n; l++)
			{
				sum += func(l);
			}
			return sum;
		}

		private void InitLocalPoints()
		{
			_localPoints.Add(new Point(-1, -1, 0));
			_localPoints.Add(new Point(1, -1, 0));
			_localPoints.Add(new Point(1, 1, 0));
			_localPoints.Add(new Point(-1, 1, 0));
			_localPoints.Add(new Point(0, -1, 0));
			_localPoints.Add(new Point(1, 0, 0));
			_localPoints.Add(new Point(0, 1, 0));
			_localPoints.Add(new Point(-1, 0, 0));
		}

		public List<GlobalPoint> GlobalPoints
		{
			get { return _globalPoints; }
			set
			{
				_globalPoints.Clear();
				if (value == null)
				{
					return;
				}
				_globalPoints.AddRange(value);
			}
		}

		public FiniteElement Element
		{
			get { return _element; }
			set { _element = value; }
		}

		private double Fi14(double eta, double tau, int i)
		{
			if (i > 3)
			{
				throw new Exception();
			}
			double etaEtaI = eta * _localPoints[i].X;
			double tauTauI = tau * _localPoints[i].Y;
			return 1.0 / 4.0 * (1 + etaEtaI) * (1 + tauTauI) * (etaEtaI + tauTauI - 1);
		}

		private double Fi14DerivateEta(double eta, double tau, int i)
		{
			if (i > 3)
			{
				throw new Exception();
			}
			double etaEtaI = eta * _localPoints[i].X;
			double tauTauI = tau * _localPoints[i].Y;
			return 1.0 / 4.0 * _localPoints[i].X * (1 + tauTauI) * (2 * etaEtaI + tauTauI);
		}

		private double Fi14DerivateTau(double eta, double tau, int i)
		{
			if (i > 3)
			{
				throw new Exception();
			}
			double etaEtaI = eta * _localPoints[i].X;
			double tauTauI = tau * _localPoints[i].Y;
			return 1.0 / 4.0 * _localPoints[i].Y * (1 + etaEtaI) * (etaEtaI + 2 * tauTauI);
		}

		private double Fi57(double eta, double tau, int i)
		{
			if (i != 4 && i != 6)
			{
				throw new Exception();
			}
			double tauTauI = tau * _localPoints[i].Y;
			return 1.0 / 2.0 * (1 - Math.Pow(eta, 2)) * (1 + tauTauI);
		}

		private double Fi57DerivateEta(double eta, double tau, int i)
		{
			if (i != 4 && i != 6)
			{
				throw new Exception();
			}
			double tauTauI = tau * _localPoints[i].Y;
			return (-eta) * (1 + tauTauI);
		}

		private double Fi57DerivateTau(double eta, int i)
		{
			if (i != 4 && i != 6)
			{
				throw new Exception();
			}
			return -1.0 / 2.0 * _localPoints[i].Y * (Math.Pow(eta, 2) - 1);
		}

		private double Fi68(double eta, double tau, int i)
		{
			if (i != 5 && i != 7)
			{
				throw new Exception();
			}
			double etaEtaI = eta * _localPoints[i].X;
			return 1.0 / 2.0 * (1 - Math.Pow(tau, 2)) * (1 + etaEtaI);
		}

		private double Fi68DerivateEta(double tau, int i)
		{
			if (i != 5 && i != 7)
			{
				throw new Exception();
			}
			return -1.0 / 2.0 * _localPoints[i].X * (Math.Pow(tau, 2) - 1);
		}

		private double Fi68DerivateTau(double eta, double tau, int i)
		{
			if (i != 5 && i != 7)
			{
				throw new Exception();
			}
			double etaEtaI = eta * _localPoints[i].X;
			return -(tau) * (1 + etaEtaI);
		}
	}
}