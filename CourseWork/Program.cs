using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml.Schema;
using Accord.Math;
using FiniteElemtsMethod;
using PointsContainer;
namespace CourseWork
{
	class Program
	{
		static void Main(string[] args)
		{
			PContainer pContainer = new PContainer();
			int[,] localGlobalMapping = pContainer.LocalGlobalMapping;
			List<StandartCube> lCubes = new List<StandartCube>();
			List<StandartSquare> lSquares = new List<StandartSquare>();
			for (int i = 0; i < localGlobalMapping.GetLength(0); i++)
			{
				List<GlobalPoint> list = new List<GlobalPoint>();
				for (int j = 0; j < localGlobalMapping.GetLength(1); j++)
				{
					list.Add(pContainer.GlobalPoints[localGlobalMapping[i, j]]);
				}
				StandartCube standartCube = new StandartCube();
				standartCube.GlobalPoints = list;
				standartCube.Element = pContainer.FiniteElements[i];
				standartCube.Init();
				StandartSquare standartSquare = new StandartSquare();
				standartSquare.GlobalPoints = list;
				standartSquare.Element = pContainer.FiniteElements[i];
				standartSquare.IsUnderPresure = (i >= (localGlobalMapping.GetLength(0) - 1 - pContainer.NumberOfLastFEUnderPresure) && i <= localGlobalMapping.GetLength(0) - 1);
				standartSquare.Init();
				lCubes.Add(standartCube);
				lSquares.Add(standartSquare);
			}
			//
			int npq = pContainer.TotalPointsNumber;
			List<int> fixedLocalPointNumber = new List<int> {0,8,1,9,2,10,3,11};
			double[,] mg = new double[3 * npq, 3 * npq];
			for (int i = 0; i < mg.GetLength(0); i++)
			{
				for (int j = 0; j < mg.GetLength(1); j++)
				{
					mg[i, j] = -100000;
				}
			}
			double[] f = new double[3 * npq];
			for (int i = 0; i < localGlobalMapping.GetLength(0); i++)
			{
				double[,] mge = lCubes[i].Mge;
				double[] fe = lSquares[i].Fe;
				for (int j = 0; j < mge.GetLength(0); j++)
				{
					int derivateByRow = j / 20;
					int localPointNumberRow = j % 20;
					int globalNumberRow = 3 * (localGlobalMapping[i, localPointNumberRow]) + derivateByRow;
					for (int k = 0; k < mge.GetLength(1); k++)
					{
						if (j == k && fixedLocalPointNumber.Contains(localPointNumberRow) && i < 4)
							mge[j, k] = Math.Pow(10, 50);
						int derivateByColumn = k / 20 ;
						int localPointNumberColumn = k % 20;
						int globalNumberColumn = 3 * (localGlobalMapping[i, localPointNumberColumn]) + derivateByColumn;
						mg[globalNumberRow, globalNumberColumn] += mge[j, k];
					}
					f[globalNumberRow] += fe[j];
				}
			}
			StreamWriter lStreamWriter = new StreamWriter("E:\\Programming\\CourseWork\\trunk\\CourseWork\\bin\\Debug\\mg.txt");
			for (int i = 0; i < mg.GetLength(0); i++)
			{
				StringBuilder builder = new StringBuilder();
				for (int j = 0; j < mg.GetLength(1); j++)
				{
					builder.Append("	" + String.Format("{0:0.00}", mg[i,j]));
				}
				lStreamWriter.WriteLine(builder.ToString());
			}
			double determinant = mg.Determinant();
			double[] result = mg.Solve(f,false);
		}
	}
}
