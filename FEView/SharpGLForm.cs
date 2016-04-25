using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Accord.Math;
using FiniteElemtsMethod;
using PointsContainer;
using SharpGL;

namespace FEView
{
	public partial class SharpGLForm : Form
	{
		private readonly List<int> _localPointNumberingSquare = new List<int>() {0, 8, 1, 9, 2, 10, 3, 11, 4, 16, 5, 17, 6, 18, 7, 19};
		private readonly List<int> _localPointNumberingVertical = new List<int>() {0, 12, 4, 1, 13, 5, 2, 14, 6, 3, 15, 7};
		private PContainer _container;
		private double _coef;
		private double[] _result;
		private double _angleX;
		private double _angleY;
		private int _scale;
		private readonly List<GlobalPoint> _pointsToDraw = new List<GlobalPoint>();

		public SharpGLForm()
		{
			InitializeComponent();
		}

		private void OpenGlControlOpenGlDraw(object sender, RenderEventArgs e)
		{
			OpenGL gl = openGLControl.OpenGL;
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
			gl.LoadIdentity();
			//openGLControl.OpenGL.Rotate(_rotation, 0.0f, 1.0f, 0.0f);
			gl.Translate(-_scale, -_scale, -_scale);
			if (_angleX == 360.0 || _angleX == -360.0)
			{
				_angleX = 0;
			}
			gl.Rotate(_angleY, 1.0, 0.0, 0.0);
			gl.Rotate(_angleX, 0.0, 1.0, 0.0);
			gl.PointSize(8);
			gl.Begin(OpenGL.GL_POINTS);
			gl.Color(0.5, 0.5, 0.5);
			gl.Vertex(0, 0, 0);
			gl.End();
			for (int i = 0; i < _container.FiniteElements.Count; i++)
			{
				if (i < _container.NumberOfLastFEUnderPresure)
				{
					DrawFe(i, true);
				}
				else
				{
					DrawFe(i, false);
				}
			}
		}

		private void OpenGlControlMouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta.Equals(120))
			{
				_scale -= 1;
			}
			else
			{
				_scale += 1;
			}
			openGLControl.Invalidate();
		}

		private void OpenGlControlOpenGlInitialized(object sender, EventArgs e)
		{
			OpenGL gl = openGLControl.OpenGL;
			gl.ClearColor(0, 0, 0, 0);
		}

		private void OpenGlControlResized(object sender, EventArgs e)
		{
			OpenGL gl = openGLControl.OpenGL;
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.LoadIdentity();
			gl.Perspective(20.0f, Width / (double) Height, 0.01, 500.0);
			gl.LookAt(3, 3, 3, 0, 0, 0, 0, 1, 0);
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
		}

		private void Calculate()
		{
			int[,] localGlobalMapping = _container.LocalGlobalMapping;
			List<StandartCube> lCubes = new List<StandartCube>();
			List<StandartSquare> lSquares = new List<StandartSquare>();
			SideWrapper sideWrapper = _comboObjectSide.SelectedItem as SideWrapper;
			int feSide = int.Parse((string) _comboFESide.SelectedItem) - 1;
			if (sideWrapper == null)
			{
				throw new Exception();
			}
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
				standartSquare.IsUnderPresure = sideWrapper.IsFeUnderPresure(_container.FiniteElements[i]);
				standartSquare.presureSurfaceNumber = feSide;
				standartSquare.Pn = GetPresure();
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
						if (j == k && fixedLocalPointNumber.Contains(localPointNumberRow) && i < _container.NumberOfLastFEUnderPresure)
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
			//StreamWriter lStreamWriter = new StreamWriter("E:\\Programming\\CourseWork\\trunk\\CourseWork\\bin\\Debug\\mg.txt");
			//for (int i = 0; i < mg.GetLength(0); i++)
			//{
			//	StringBuilder builder = new StringBuilder();
			//	for (int j = 0; j < mg.GetLength(1); j++)
			//	{
			//		builder.Append("	" + String.Format("{0:0.00}", mg[i, j]));
			//	}
			//	lStreamWriter.WriteLine(builder.ToString());
			//}
			//double determinant = mg.Determinant();
			_result = mg.Solve(f);
		}

		private double GetPresure()
		{
			SideWrapper sideWrapper = _comboObjectSide.SelectedItem as SideWrapper;
			int feSide = int.Parse((string)_comboFESide.SelectedItem);
			return (sideWrapper.Number == 5 && (feSide == 6 || feSide == 6)) ? -6e+9 : -3e+9;
		}

		private void SharpGlFormLoad(object sender, EventArgs e)
		{
			_container = new PContainer();
			_container.Init();
			double max = Math.Max(_container.TotalDepth, _container.TotalHeight);
			max = Math.Max(max, _container.TotalWidth);
			_coef = 1 / max;
			ReInitComboSide();
			InitValues();
			ReInitPointsToDraw();
			_comboObjectSide.SelectedIndex = 0;
			_comboFESide.SelectedIndex = 0;
			openGLControl.MouseWheel += OpenGlControlMouseWheel;
		}

		private void SubscribeEvents()
		{
			_comboFESide.SelectedValueChanged += NumerHightValueChanged;
			_comboObjectSide.SelectedValueChanged += NumerHightValueChanged;
			_numerHight.ValueChanged += NumerHightValueChanged;
			_numericWidth.ValueChanged += NumerHightValueChanged;
			_numericDepth.ValueChanged += NumerHightValueChanged;
			_numericFENumberPerH.ValueChanged += NumerHightValueChanged;
			_numericFENumberPerW.ValueChanged += NumerHightValueChanged;
			_numericFENumberPerD.ValueChanged += NumerHightValueChanged;
		}

		private void DesubscribeEvents()
		{
			_comboFESide.SelectedValueChanged -= NumerHightValueChanged;
			_comboObjectSide.SelectedValueChanged -= NumerHightValueChanged;
			_numerHight.ValueChanged -= NumerHightValueChanged;
			_numericWidth.ValueChanged -= NumerHightValueChanged;
			_numericDepth.ValueChanged -= NumerHightValueChanged;
			_numericFENumberPerH.ValueChanged -= NumerHightValueChanged;
			_numericFENumberPerW.ValueChanged -= NumerHightValueChanged;
			_numericFENumberPerD.ValueChanged -= NumerHightValueChanged;
		}

		private void InitValues()
		{
			DesubscribeEvents();
			_numerHight.Value = (int) _container.TotalHeight;
			_numericWidth.Value = (int) _container.TotalWidth;
			_numericDepth.Value = (int) _container.TotalDepth;
			_numericFENumberPerH.Value = _container.FeNumberPerH;
			_numericFENumberPerW.Value = _container.FeNumberPerW;
			_numericFENumberPerD.Value = _container.FeNumberPerD;
			SubscribeEvents();
		}

		private void ReInitContainer()
		{
			DesubscribeEvents();
			_container = new PContainer();
			_container.TotalHeight = (double) _numerHight.Value;
			_container.TotalWidth = (double) _numericWidth.Value;
			_container.TotalDepth = (double) _numericDepth.Value;
			_container.FeNumberPerH = (int) _numericFENumberPerH.Value;
			_container.FeNumberPerW = (int) _numericFENumberPerW.Value;
			_container.FeNumberPerD = (int) _numericFENumberPerD.Value;
			_container.Init();
			double max = Math.Max(_container.TotalDepth, _container.TotalHeight);
			max = Math.Max(max, _container.TotalWidth);
			_coef = 1 / max;
			ReInitPointsToDraw();
			ReInitComboSide();
			SubscribeEvents();
		}

		private void ReInitPointsToDraw()
		{
			_pointsToDraw.Clear();
			for (int l = 0; l < _container.GlobalPoints.Count; l++)
			{
				GlobalPoint globalPoint = _container.GlobalPoints[l].Clone();
				_pointsToDraw.Add(globalPoint);
			}
		}

		private void ReInitComboSide()
		{
			int selectedIndex = _comboObjectSide.SelectedIndex;
			_comboObjectSide.Items.Clear();
			_comboObjectSide.Items.Add(new SideWrapper(_container.FeNumberPerH, _container.FeNumberPerW, _container.FeNumberPerD, ESide.SideDh, true, 1));
			_comboObjectSide.Items.Add(new SideWrapper(_container.FeNumberPerH, _container.FeNumberPerW, _container.FeNumberPerD, ESide.SideDw, false, 2));
			_comboObjectSide.Items.Add(new SideWrapper(_container.FeNumberPerH, _container.FeNumberPerW, _container.FeNumberPerD, ESide.SideDh, false, 3));
			_comboObjectSide.Items.Add(new SideWrapper(_container.FeNumberPerH, _container.FeNumberPerW, _container.FeNumberPerD, ESide.SideDw, true, 4));
			_comboObjectSide.Items.Add(new SideWrapper(_container.FeNumberPerH, _container.FeNumberPerW, _container.FeNumberPerD, ESide.SideHw, false, 5));
			_comboObjectSide.SelectedIndex = selectedIndex;
		}

		private void DrawFe(int feNumber, bool isUnderPresure)
		{
			List<GlobalPoint> pointsToDraw = new List<GlobalPoint>();
			int counter = 0;
			for (int i = 0; i < _localPointNumberingVertical.Count; i++)
			{
				int globalPointNumber = _container.LocalGlobalMapping[feNumber, _localPointNumberingVertical[i]];
				pointsToDraw.Add(_pointsToDraw[globalPointNumber]);
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
			pointsToDraw.Clear();
			for (int i = 0; i < 8; i++)
			{
				int globalPointNumber = _container.LocalGlobalMapping[feNumber, _localPointNumberingSquare[i]];
				pointsToDraw.Add(_pointsToDraw[globalPointNumber]);
			}
			DrawSquare(pointsToDraw, isUnderPresure);
			pointsToDraw.Clear();
			for (int i = 8; i < 16; i++)
			{
				int globalPointNumber = _container.LocalGlobalMapping[feNumber, _localPointNumberingSquare[i]];
				pointsToDraw.Add(_pointsToDraw[globalPointNumber]);
			}
			DrawSquare(pointsToDraw, false);
		}

		private void DrawVerticalLines(List<GlobalPoint> points)
		{
			OpenGL gl = openGLControl.OpenGL;
			for (int i = 0; i < points.Count - 1; i++)
			{
				GlobalPoint curPoint = points[i];
				GlobalPoint nextPoint = points[i + 1];
				gl.PointSize(3);
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
			gl.PointSize(3);
			gl.Begin(OpenGL.GL_POINTS);
			gl.Color(0.0f, 1.0f, 0.0f);
			gl.Vertex(points[points.Count - 1].Point.X * _coef, points[points.Count - 1].Point.Y * _coef, points[points.Count - 1].Point.Z * _coef);
			gl.End();
		}

		private void DrawSquare(List<GlobalPoint> points, bool isUnderPresure)
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
				if (isUnderPresure)
				{
					gl.PointSize(5);
				}
				else
				{
					gl.PointSize(3);
				}
				gl.Begin(OpenGL.GL_POINTS);
				if (isUnderPresure)
				{
					gl.Color(0.0f, 0.0f, 1.0f);
				}
				else
				{
					gl.Color(0.0f, 1.0f, 0.0f);
				}
				gl.Vertex(curPoint.Point.X * _coef, curPoint.Point.Y * _coef, curPoint.Point.Z * _coef);
				gl.End();

				gl.Begin(OpenGL.GL_LINES);
				if (isUnderPresure)
				{
					gl.Color(0.0f, 0.0f, 1.0f);
				}
				else
				{
					gl.Color(1.0f, 0.0f, 0.0f);
				}
				gl.Vertex(curPoint.Point.X * _coef, curPoint.Point.Y * _coef, curPoint.Point.Z * _coef);
				if (isUnderPresure)
				{
					gl.Color(0.0f, 0.0f, 1.0f);
				}
				else
				{
					gl.Color(1.0f, 0.0f, 0.0f);
				}
				gl.Vertex(nextPoint.Point.X * _coef, nextPoint.Point.Y * _coef, nextPoint.Point.Z * _coef);
				gl.End();
			}
		}

		private void BtnCalculateClick(object sender, EventArgs e)
		{
			Calculate();
			for (int l = 0; l < _pointsToDraw.Count; l++)
			{
				int startnumber = l * 3;
				_pointsToDraw[l].Point.X += _result[startnumber];
				_pointsToDraw[l].Point.Y += _result[startnumber + 1];
				_pointsToDraw[l].Point.Z += _result[startnumber + 2];
			}
		}

		private void OpenGlControlMouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				_angleX = e.X;
				_angleY = e.Y;
			}
		}

		private void NumerHightValueChanged(object sender, EventArgs e)
		{
			ReInitContainer();
		}
	}
}