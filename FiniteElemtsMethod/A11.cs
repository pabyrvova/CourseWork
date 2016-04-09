using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiniteElemtsMethod
{
	public class A11
	{
		private readonly double[, ,] DFIXYZ = new double[27, 20, 3]; //ex. Gaus,Fi,X
		private readonly double[,] feMge = new double[20,20];
		private readonly double[] DJ = new double[27];
		private double lyambda = 12;
		private double nyu = 100;
		private double miy = 100;

		public A11(double[, ,] dfixyz, double[] dj)
		{
			DFIXYZ = dfixyz;
			DJ = dj;
			InitMge();
		}

		private void InitMge()
		{
			for (int i = 0; i < 20; i++)
			{
				for (int j = 0; j < 20; j++)
				{
					int counter = 0;
					double sumGlobal = 0;
					for (int k = 0; k < 3; k++)
					{
						double sumGloba = 0;
						for (int l = 0; l < 3; l++)
						{
							double sumGlob = 0;
							for (int m = 0; m < 3; m++)
							{
								double C3 = StandartCube.C[m];
								double d = lyambda * (1 - nyu) * DFIXYZ[counter, i, 0] * DFIXYZ[counter, j, 0] + miy * DFIXYZ[counter, i, 1] * DFIXYZ[counter, j, 1] + miy * DFIXYZ[counter, i, 2] * DFIXYZ[counter, j, 2];
								sumGlob += C3 * d * DJ[counter];
								counter++;
							}
							sumGloba += StandartCube.C[l] * sumGlob;
						}
						sumGlobal += StandartCube.C[k] * sumGloba;
					}
					feMge[i, j] = sumGlobal;
				}
			}
		}
	}
}
