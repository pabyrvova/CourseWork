using System.Collections.Generic;
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
				lCubes.Add(standartCube);
			}
		}
	}
}
