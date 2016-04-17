namespace FiniteElemtsMethod
{
	public interface IAMatrix
	{
		double[,] GetMge();
	}

	public class Aii:IAMatrix
	{
		private readonly double[, ,] DFIXYZ = new double[27, 20, 3]; //ex. Gaus,Fi,X
		private readonly double[,] feMge = new double[20,20];
		private readonly double[] DJ = new double[27];
		private double lyambda = 5.55037e+10;
		private double nyu = 0.34;
		private double miy = 2.61194e+10;

		public Aii(double[, ,] dfixyz, double[] dj, int i, int j, int k)
		{
			DFIXYZ = dfixyz;
			DJ = dj;
			InitMge(i,j,k);
		}

		private void InitMge(int first, int second, int third)
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
								double d = lyambda * (1 - nyu) * DFIXYZ[counter, i, first] * DFIXYZ[counter, j, first] + miy * DFIXYZ[counter, i, second] * DFIXYZ[counter, j, second] + miy * DFIXYZ[counter, i, third] * DFIXYZ[counter, j, third];
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

		public double[,] GetMge()
		{
			return feMge;
		}
	}
}
