using System;

namespace FiniteElemtsMethod
{
	public class Fi
	{
		private readonly double[,,] DXYZET = new double[3, 2, 9]; //ex. x,eta,gaus
		private readonly double[] fe = new double[8];
		private readonly Func<double, double, int, double> Fii;
		public double Pn = 0.004;
		private double lyambda = 0.676;
		private double nyu = 0.48;
		private double miy = 0.014;

		public Fi(double[,,] dxyzet, Func<double, double, int, double> fi, int first, int second, bool isUnderPresure)
		{
			DXYZET = dxyzet;
			Fii = fi;
			if(!isUnderPresure)
				return;
			InitFe(first, second);
		}

		private void InitFe(int first, int second)
		{
			for (int i = 0; i < 8; i++)
			{
				int counter = 0;
				double sumGlobal = 0;
				for (int k = 0; k < 3; k++)
				{
					double sumGloba = 0;
					for (int l = 0; l < 3; l++)
					{
						double C2 = StandartSquare.C[l];
						double d = Pn * (DXYZET[first, 0, counter] * DXYZET[second, 1, counter] - DXYZET[second, 0, counter] * DXYZET[first, 1, counter]) * Fii(StandartSquare.X[k], StandartSquare.X[l], i);
						sumGloba += C2 * d;
						counter++;
					}
					sumGlobal += StandartCube.C[k] * sumGloba;
				}
				fe[i] = sumGlobal;
			}
		}

		public double[] GetFe()
		{
			return fe;
		}
	}
}