using System;
using System.Collections.Generic;
using PointsContainer;

namespace FiniteElemtsMethod
{
	internal class StandartSquare
	{
		private readonly List<GlobalPoint> _globalPoints = new List<GlobalPoint>();
		private readonly List<Point> _localPoints = new List<Point>();
		private FiniteElement _element;

		public StandartSquare()
		{
			InitLocalPoints();
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
			if (i != 5 || i != 7)
			{
				throw new Exception();
			}
			double tauTauI = tau * _localPoints[i].Y;
			return 1.0 / 2.0 * (1 - Math.Pow(eta, 2)) * (1 + tauTauI);
		}

		private double Fi57DerivateEta(double eta, double tau, int i)
		{
			if (i != 5 || i != 7)
			{
				throw new Exception();
			}
			double tauTauI = tau * _localPoints[i].Y;
			return (-eta) * (1 + tauTauI);
		}

		private double Fi57DerivateTau(double eta, int i)
		{
			if (i != 5 || i != 7)
			{
				throw new Exception();
			}
			return -1.0 / 2.0 * _localPoints[i].Y * (Math.Pow(eta, 2) - 1);
		}

		private double Fi68(double eta, double tau, int i)
		{
			if (i != 6 && i != 8)
			{
				throw new Exception();
			}
			double etaEtaI = eta * _localPoints[i].X;
			return 1.0 / 2.0 * (1 - Math.Pow(tau, 2)) * (1 + etaEtaI);
		}

		private double Fi68DerivateEta(double tau, int i)
		{
			if (i != 6 && i != 8)
			{
				throw new Exception();
			}
			return -1.0 / 2.0 * _localPoints[i].X * (Math.Pow(tau, 2) - 1);
		}

		private double Fi68DerivateTau(double eta,double tau, int i)
		{
			if (i != 6 && i != 8)
			{
				throw new Exception();
			}
			double etaEtaI = eta * _localPoints[i].X;
			return -(tau) * (1 + etaEtaI);
		}
	}
}