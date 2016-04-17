using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Accord.Math;
using FiniteElemtsMethod;
using PointsContainer;
using SharpGL;

namespace FEView
{
	/// <summary>
	///     The main form class.
	/// </summary>
	public partial class SharpGLForm : Form
	{
		private readonly List<int> _localPointNumberingSquare = new List<int>() {0, 8, 1, 9, 2, 10, 3, 11, 4, 16, 5, 17, 6, 18, 7, 19};
		private readonly List<int> _localPointNumberingVertical = new List<int>() {0, 12, 4, 1, 13, 5, 2, 14, 6, 3, 15, 7};
		private PContainer _container;
		private double _coef;
		private double[] _result;
		/// <summary>
		private float _rotation = 0;

		private bool _lock = false;

		/// Initializes a new instance of the
		/// <see cref="SharpGLForm" />
		/// class.
		/// </summary>
		public SharpGLForm()
		{
			InitializeComponent();
		}

		/// <summary>
		///     Handles the OpenGLDraw event of the openGLControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RenderEventArgs" /> instance containing the event data.</param>
		private void openGLControl_OpenGLDraw(object sender, RenderEventArgs e)
		{
			if (_lock)
				return;
			//  Get the OpenGL object.
			OpenGL gl = openGLControl.OpenGL;

			//  Clear the color and depth buffer.
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			//  Load the identity matrix.
			gl.LoadIdentity();

			openGLControl.OpenGL.Rotate(_rotation, 0.0f, 1.0f, 0.0f);
			for (int i = 0; i < _container.FiniteElements.Count; i++)
			{
				DrawFe(i);
			}


			//  Rotate around the Y axis.
			//gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);

			////  Draw a coloured pyramid.
			//gl.Begin(OpenGL.GL_TRIANGLES);
			//gl.Color(1.0f, 0.0f, 0.0f);
			//gl.Vertex(0.0f, 1.0f, 0.0f);
			//gl.Color(0.0f, 1.0f, 0.0f);
			//gl.Vertex(-1.0f, -1.0f, 1.0f);
			//gl.Color(0.0f, 0.0f, 1.0f);
			//gl.Vertex(1.0f, -1.0f, 1.0f);
			//gl.Color(1.0f, 0.0f, 0.0f);
			//gl.Vertex(0.0f, 1.0f, 0.0f);
			//gl.Color(0.0f, 0.0f, 1.0f);
			//gl.Vertex(1.0f, -1.0f, 1.0f);
			//gl.Color(0.0f, 1.0f, 0.0f);
			//gl.Vertex(1.0f, -1.0f, -1.0f);
			//gl.Color(1.0f, 0.0f, 0.0f);
			//gl.Vertex(0.0f, 1.0f, 0.0f);
			//gl.Color(0.0f, 1.0f, 0.0f);
			//gl.Vertex(1.0f, -1.0f, -1.0f);
			//gl.Color(0.0f, 0.0f, 1.0f);
			//gl.Vertex(-1.0f, -1.0f, -1.0f);
			//gl.Color(1.0f, 0.0f, 0.0f);
			//gl.Vertex(0.0f, 1.0f, 0.0f);
			//gl.Color(0.0f, 0.0f, 1.0f);
			//gl.Vertex(-1.0f, -1.0f, -1.0f);
			//gl.Color(0.0f, 1.0f, 0.0f);
			//gl.Vertex(-1.0f, -1.0f, 1.0f);
			//gl.End();

			//  Nudge the rotation.
		}


		/// <summary>
		///     Handles the OpenGLInitialized event of the openGLControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
		private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
		{
			//  TODO: Initialise OpenGL here.

			//  Get the OpenGL object.
			OpenGL gl = openGLControl.OpenGL;

			//  Set the clear color.
			gl.ClearColor(0, 0, 0, 0);
		}

		/// <summary>
		///     Handles the Resized event of the openGLControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
		private void openGLControl_Resized(object sender, EventArgs e)
		{
			//  TODO: Set the projection matrix here.

			//  Get the OpenGL object.
			OpenGL gl = openGLControl.OpenGL;

			////	//  Set the projection matrix.
			gl.MatrixMode(OpenGL.GL_PROJECTION);

			//	//  Load the identity.
			gl.LoadIdentity();

			//	//  Create a perspective transformation.
			gl.Perspective(20.0f, (double) Width / (double) Height, 0.01, 100.0);

			//	//  Use the 'look at' helper function to position and aim the camera.
			gl.LookAt(5, 3, 5, 0, 0, 0, 0, 1, 0);

			//	//  Set the modelview matrix.
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
		}

		private void OpenGlControlKeyPress(object sender, KeyPressEventArgs e)
		{
			EMove moveMode = (EMove) e.KeyChar;
			switch (moveMode)
			{
				case EMove.Up:

				case EMove.Right:
				case EMove.Left:
				case EMove.Down:
				case EMove.Plus:
					_rotation += 3.0f;
					break;
				case EMove.Minus:
					_rotation -= 3.0f;
					break;
			}
		}

		private void Calculate()
		{
			int[,] localGlobalMapping = _container.LocalGlobalMapping;
			List<StandartCube> lCubes = new List<StandartCube>();
			List<StandartSquare> lSquares = new List<StandartSquare>();
			for (int i = 0; i < localGlobalMapping.GetLength(0); i++)
			{
				List<GlobalPoint> list = new List<GlobalPoint>();
				for (int j = 0; j < localGlobalMapping.GetLength(1); j++)
				{
					list.Add(_container.GlobalPoints[localGlobalMapping[i, j]]);
				}
				StandartCube standartCube = new StandartCube();
				standartCube.GlobalPoints = list;
				standartCube.Element = _container.FiniteElements[i];
				standartCube.Init();
				StandartSquare standartSquare = new StandartSquare();
				standartSquare.GlobalPoints = list;
				standartSquare.Element = _container.FiniteElements[i];
				standartSquare.IsUnderPresure = (i >= (localGlobalMapping.GetLength(0) - 1 - _container.NumberOfLastFEUnderPresure) && i <= localGlobalMapping.GetLength(0) - 1);
				standartSquare.Init();
				lCubes.Add(standartCube);
				lSquares.Add(standartSquare);
			}
			//
			int npq = _container.TotalPointsNumber;
			List<int> fixedLocalPointNumber = new List<int> {0, 8, 1, 9, 2, 10, 3, 11};
			double[,] mg = new double[3 * npq, 3 * npq];
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
						{
							mge[j, k] = Math.Pow(10, 50);
						}
						int derivateByColumn = k / 20;
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
					builder.Append("	" + String.Format("{0:0.00}", mg[i, j]));
				}
				lStreamWriter.WriteLine(builder.ToString());
			}
			double determinant = mg.Determinant();
			_result = mg.Solve(f);
		}

		private enum EMove
		{
			Up = 56,
			Right = 54,
			Left = 52,
			Down = 50,
			Plus = 43,
			Minus = 45
		}

		private void SharpGLForm_Load(object sender, EventArgs e)
		{
			_container = new PContainer();
			double max = Math.Max(_container.TotalDepth, _container.TotalHeight);
			max = Math.Max(max, _container.TotalWidth);
			_coef = 1 / max;
		}

		private void DrawFe(int feNumber)
		{
			List<GlobalPoint> pointsToDraw = new List<GlobalPoint>();
			for (int i = 0; i < 8; i++)
			{
				int globalPointNumber = _container.LocalGlobalMapping[feNumber, _localPointNumberingSquare[i]];
				pointsToDraw.Add(_container.GlobalPoints[globalPointNumber]);
			}
			DrawSquare(pointsToDraw);
			pointsToDraw.Clear();
			for (int i = 8; i < 16; i++)
			{
				int globalPointNumber = _container.LocalGlobalMapping[feNumber, _localPointNumberingSquare[i]];
				pointsToDraw.Add(_container.GlobalPoints[globalPointNumber]);
			}
			DrawSquare(pointsToDraw);
			pointsToDraw.Clear();
			int counter = 0;
			for (int i = 0; i < _localPointNumberingVertical.Count; i++)
			{
				int globalPointNumber = _container.LocalGlobalMapping[feNumber, _localPointNumberingVertical[i]];
				pointsToDraw.Add(_container.GlobalPoints[globalPointNumber]);
				if (counter == 2)
				{
					DrawVerticalLines(pointsToDraw);
					pointsToDraw.Clear();
					counter = 0;
				}
				else
				{
					counter++;
				}
			}
		}

		private void DrawVerticalLines(List<GlobalPoint> points)
		{
			OpenGL gl = openGLControl.OpenGL;
			for (int i = 0; i < points.Count-1; i++)
			{
				GlobalPoint curPoint = points[i];
				GlobalPoint nextPoint = points[i+1];
				gl.PointSize(5);
				gl.Begin(OpenGL.GL_POINTS);
				gl.Color(0.0f, 1.0f, 0.0f);
				gl.Vertex(curPoint.Point.X * _coef, curPoint.Point.Y * _coef, curPoint.Point.Z * _coef);
				gl.End();

				gl.Begin(OpenGL.GL_LINES);
				gl.Color(1.0f, 0.0f, 0.0f);
				gl.Vertex(curPoint.Point.X * _coef, curPoint.Point.Y * _coef, curPoint.Point.Z * _coef);
				gl.Color(1.0f, 0.0f, 0.0f);
				gl.Vertex(nextPoint.Point.X * _coef, nextPoint.Point.Y * _coef, nextPoint.Point.Z * _coef);
				gl.End();
			}
			gl.PointSize(5);
			gl.Begin(OpenGL.GL_POINTS);
			gl.Color(0.0f, 1.0f, 0.0f);
			gl.Vertex(points[points.Count - 1].Point.X * _coef, points[points.Count - 1].Point.Y * _coef, points[points.Count - 1].Point.Z * _coef);
			gl.End();
		}

		private void DrawSquare(List<GlobalPoint> points)
		{
			OpenGL gl = openGLControl.OpenGL;
			for (int i = 0; i < points.Count; i++)
			{
				GlobalPoint curPoint = points[i];
				GlobalPoint nextPoint = points[0];
				if (i + 1 != points.Count)
				{
					nextPoint = points[i + 1];
				}
				gl.PointSize(5);
				gl.Begin(OpenGL.GL_POINTS);
				gl.Color(0.0f, 1.0f, 0.0f);
				gl.Vertex(curPoint.Point.X * _coef, curPoint.Point.Y * _coef, curPoint.Point.Z * _coef);
				gl.End();

				gl.Begin(OpenGL.GL_LINES);
				gl.Color(1.0f, 0.0f, 0.0f);
				gl.Vertex(curPoint.Point.X * _coef, curPoint.Point.Y * _coef, curPoint.Point.Z * _coef);
				gl.Color(1.0f, 0.0f, 0.0f);
				gl.Vertex(nextPoint.Point.X * _coef, nextPoint.Point.Y * _coef, nextPoint.Point.Z * _coef);
				gl.End();
			}
		}

		private void BtnCalculateClick(object sender, EventArgs e)
		{
			Calculate();
			_lock = true;
			for (int l = 0; l < _container.GlobalPoints.Count; l++)
			{
				int startnumber = l * 3;
				_container.GlobalPoints[l].Point.X += _result[startnumber];
				_container.GlobalPoints[l].Point.Y += _result[startnumber+1];
				_container.GlobalPoints[l].Point.Z += _result[startnumber+2];
			}
			_lock = false;
		}
	}
}