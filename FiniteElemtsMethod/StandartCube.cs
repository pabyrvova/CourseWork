using System;
using System.Collections.Generic;
using Accord.Math;
using PointsContainer;

namespace FiniteElemtsMethod
{
	public class StandartCube
	{
		public static double[] C = {5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0};
		public static double[] X = {-Math.Sqrt(0.6), 0, Math.Sqrt(0.6)};
		private readonly List<GlobalPoint> _globalPoints = new List<GlobalPoint>();
		private readonly List<Point> _localPoints = new List<Point>();
		private FiniteElement _element;
		private readonly double[,,] DXYZABG = new double[3, 3, 27]; //ex. x,alpha,Gaus
		private readonly double[,,] DFIXYZ = new double[27, 20, 3]; //ex. Gaus,Fi,X
		private readonly double[] DJ = new double[27];
		private readonly double[,] MGE = new double[60,60];

		public void Init()
		{
			InitLocalPoints();
			InitDXYZABG();
			InitJacobi();
			InitDFIXYZ();
			InitMGE();
		}

		public void InitDXYZABG()
		{
			int counter = 0;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 3; k++)
					{
						DXYZABG[i, j, counter] = Sum(index =>
						{
							if (i == 0)
							{
								return _globalPoints[index].Point.X * FiDerivate(X[i], X[j], X[k], index, j);
							}
							if (i == 1)
							{
								return _globalPoints[index].Point.Y * FiDerivate(X[i], X[j], X[k], index, j);
							}
							return _globalPoints[index].Point.Z * FiDerivate(X[i], X[j], X[k], index, j);
						},
							20);
						counter++;
					}
				}
			}
		}

		public void InitJacobi()
		{
			for (int l = 0; l < 27; l++)
			{
				DJ[l] = DXYZABG[0, 0, l] * DXYZABG[1, 1, l] * DXYZABG[2, 2, l] + DXYZABG[0, 1, l] * DXYZABG[1, 2, l] * DXYZABG[2, 0, l] + DXYZABG[0, 2, l] * DXYZABG[1, 0, l] * DXYZABG[2, 1, l] -
						DXYZABG[0, 2, l] * DXYZABG[1, 1, l] * DXYZABG[2, 0, l] - DXYZABG[0, 0, l] * DXYZABG[1, 2, l] * DXYZABG[2, 1, l] - DXYZABG[0, 1, l] * DXYZABG[1, 0, l] * DXYZABG[2, 2, l];
			}
		}

		public void InitDFIXYZ()
		{
			int counter = 0;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 3; k++)
					{
						double[,] matrix = new double[3, 3];
						for (int l = 0; l < 3; l++)
						{
							for (int m = 0; m < 3; m++)
							{
								matrix[l, m] = DXYZABG[l, m, counter];
							}
						}
						for (int i1 = 0; i1 < 20; i1++)
						{
							double[] rightSide = new double[3];
							for (int abg = 0; abg < rightSide.Length; abg++)
							{
								rightSide[abg] = FiDerivate(X[i], X[j], X[k], i1, abg);
							}
							double[] solve = matrix.Solve(rightSide,true);
							for (int l = 0; l < solve.Length; l++)
							{
								DFIXYZ[counter, i1, l] = solve[l];
							}
						}
						counter++;
					}
				}
			}
		}

		private void InitMGE()
		{
			List<IAMatrix> list = new List<IAMatrix>
			{new Aii(DFIXYZ, DJ, 0, 1, 2), new Aij(DFIXYZ, DJ, 0, 1), new Aij(DFIXYZ, DJ, 0, 2), new Aii(DFIXYZ, DJ, 1, 0, 2), new Aij(DFIXYZ, DJ, 1, 2), new Aii(DFIXYZ, DJ, 2, 0, 1)};
			IAMatrix[,] matrices = new IAMatrix[3,3];
			int counter = 0;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (j < i)
						matrices[i, j] = matrices[j, i];
					else
					{
						matrices[i, j] = list[counter];
						counter++;
					}
				}
			}
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					int coefI = i+1;
					int coefJ = j+1;
					double[,] doubles = matrices[i,j].GetMge();
					for (int k = 0; k < 20; k++)
					{
						for (int l = 0; l < 20; l++)
						{
							MGE[k * coefI, l * coefJ] = doubles[k, l];
						}
					}
				}
			}
		}

		private void InitLocalPoints()
		{
			_localPoints.Add(new Point(-1, 1, -1));
			_localPoints.Add(new Point(1, 1, -1));
			_localPoints.Add(new Point(1, -1, -1));
			_localPoints.Add(new Point(-1, -1, -1));
			_localPoints.Add(new Point(-1, 1, 1));
			_localPoints.Add(new Point(1, 1, 1));
			_localPoints.Add(new Point(1, -1, 1));
			_localPoints.Add(new Point(-1, -1, 1));
			_localPoints.Add(new Point(0, 1, -1));
			_localPoints.Add(new Point(1, 0, -1));
			_localPoints.Add(new Point(0, -1, -1));
			_localPoints.Add(new Point(-1, 0, -1));
			_localPoints.Add(new Point(-1, 1, 0));
			_localPoints.Add(new Point(1, 1, 0));
			_localPoints.Add(new Point(1, -1, 0));
			_localPoints.Add(new Point(-1, -1, 0));
			_localPoints.Add(new Point(0, 1, 1));
			_localPoints.Add(new Point(1, 0, 1));
			_localPoints.Add(new Point(0, -1, 1));
			_localPoints.Add(new Point(-1, 0, 1));
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

		private double Fi(double alpha, double beta, double gama, int i)
		{
			if (i <= 7)
			{
				return Fi18(alpha, beta, gama, i);
			}
			return Fi920(alpha, beta, gama, i);
		}

		private double FiDerivate(double alpha, double beta, double gama, int i, int abg)
		{
			if (i <= 7)
			{
				if (abg == 0)
				{
					return Fi18DerivateAlpha(alpha, beta, gama, i);
				}
				if (abg == 1)
				{
					return Fi18DerivateBeta(alpha, beta, gama, i);
				}
				return Fi18DerivateGama(alpha, beta, gama, i);
			}
			if (abg == 0)
			{
				return Fi920DerivateAlpha(alpha, beta, gama, i);
			}
			if (abg == 1)
			{
				return Fi920DerivateBeta(alpha, beta, gama, i);
			}
			return Fi920DerivateGama(alpha, beta, gama, i);
		}

		private double Fi18(double alpha, double beta, double gama, int i)
		{
			if (i > 7)
			{
				throw new Exception();
			}
			double alphaAlphaI = alpha * _localPoints[i].X;
			double betaBetaI = beta * _localPoints[i].Y;
			double gamaGamaI = gama * _localPoints[i].Z;
			return 1.0 / 8.0 * (1 + alphaAlphaI) * (1 + betaBetaI) * (1 + gamaGamaI) * (alphaAlphaI + betaBetaI + gamaGamaI - 2);
		}

		private double Fi18DerivateAlpha(double alpha, double beta, double gama, int i)
		{
			if (i > 7)
			{
				throw new Exception();
			}
			double alphaAlphaI = alpha * _localPoints[i].X;
			double betaBetaI = beta * _localPoints[i].Y;
			double gamaGamaI = gama * _localPoints[i].Z;
			return 1.0 / 8.0 * _localPoints[i].X * (1 + betaBetaI) * (1 + gamaGamaI) * (2 * alphaAlphaI + betaBetaI + gamaGamaI - 1);
		}

		private double Fi18DerivateBeta(double alpha, double beta, double gama, int i)
		{
			if (i > 7)
			{
				throw new Exception();
			}
			double alphaAlphaI = alpha * _localPoints[i].X;
			double betaBetaI = beta * _localPoints[i].Y;
			double gamaGamaI = gama * _localPoints[i].Z;
			return 1.0 / 8.0 * _localPoints[i].Y * (1 + alphaAlphaI) * (1 + gamaGamaI) * (alphaAlphaI + 2 * betaBetaI + gamaGamaI - 1);
		}

		private double Fi18DerivateGama(double alpha, double beta, double gama, int i)
		{
			if (i > 7)
			{
				throw new Exception();
			}
			double alphaAlphaI = alpha * _localPoints[i].X;
			double betaBetaI = beta * _localPoints[i].Y;
			double gamaGamaI = gama * _localPoints[i].Z;
			return 1.0 / 8.0 * _localPoints[i].Z * (1 + alphaAlphaI) * (1 + betaBetaI) * (alphaAlphaI + betaBetaI + 2 * gamaGamaI - 1);
		}

		private double Fi920(double alpha, double beta, double gama, int i)
		{
			if (i < 8)
			{
				throw new Exception();
			}
			double alphaAlphaI = alpha * _localPoints[i].X;
			double betaBetaI = beta * _localPoints[i].Y;
			double gamaGamaI = gama * _localPoints[i].Z;
			return 1.0 / 8.0 * (1 + alphaAlphaI) * (1 + betaBetaI) * (1 + gamaGamaI) *
					(1 - Math.Pow(alpha * _localPoints[i].Y * _localPoints[i].Z, 2) - Math.Pow(beta * _localPoints[i].X * _localPoints[i].Z, 2) - Math.Pow(gama * _localPoints[i].X * _localPoints[i].Y, 2));
		}

		private double Fi920DerivateAlpha(double alpha, double beta, double gama, int i)
		{
			if (i < 8)
			{
				throw new Exception();
			}
			double alphaAlphaI = alpha * _localPoints[i].X;
			double betaBetaI = beta * _localPoints[i].Y;
			double gamaGamaI = gama * _localPoints[i].Z;
			return 1.0 / 4.0 * _localPoints[i].X * (1 + betaBetaI) * (1 + gamaGamaI) *
					(1 - Math.Pow(alpha * _localPoints[i].Y * _localPoints[i].Z, 2) - Math.Pow(beta * _localPoints[i].X * _localPoints[i].Z, 2) - Math.Pow(gama * _localPoints[i].X * _localPoints[i].Y, 2)) -
					1.0 / 2.0 * Math.Pow(_localPoints[i].Y * _localPoints[i].Z, 2) * alpha * (1 + alphaAlphaI) * (1 + betaBetaI) * (1 + gamaGamaI);
		}

		private double Fi920DerivateBeta(double alpha, double beta, double gama, int i)
		{
			if (i < 8)
			{
				throw new Exception();
			}
			double alphaAlphaI = alpha * _localPoints[i].X;
			double betaBetaI = beta * _localPoints[i].Y;
			double gamaGamaI = gama * _localPoints[i].Z;
			return 1.0 / 4.0 * _localPoints[i].Y * (1 + alphaAlphaI) * (1 + gamaGamaI) *
					(1 - Math.Pow(alpha * _localPoints[i].Y * _localPoints[i].Z, 2) - Math.Pow(beta * _localPoints[i].X * _localPoints[i].Z, 2) - Math.Pow(gama * _localPoints[i].X * _localPoints[i].Y, 2)) -
					1.0 / 2.0 * Math.Pow(_localPoints[i].X * _localPoints[i].Z, 2) * beta * (1 + alphaAlphaI) * (1 + betaBetaI) * (1 + gamaGamaI);
		}

		private double Fi920DerivateGama(double alpha, double beta, double gama, int i)
		{
			if (i < 8)
			{
				throw new Exception();
			}
			double alphaAlphaI = alpha * _localPoints[i].X;
			double betaBetaI = beta * _localPoints[i].Y;
			double gamaGamaI = gama * _localPoints[i].Z;
			return 1.0 / 4.0 * _localPoints[i].Z * (1 + alphaAlphaI) * (1 + betaBetaI) *
					(1 - Math.Pow(alpha * _localPoints[i].Y * _localPoints[i].Z, 2) - Math.Pow(beta * _localPoints[i].X * _localPoints[i].Z, 2) - Math.Pow(gama * _localPoints[i].X * _localPoints[i].Y, 2)) -
					1.0 / 2.0 * Math.Pow(_localPoints[i].X * _localPoints[i].Y, 2) * gama * (1 + alphaAlphaI) * (1 + betaBetaI) * (1 + gamaGamaI);
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
	}
}