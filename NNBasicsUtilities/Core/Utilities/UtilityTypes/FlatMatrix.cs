﻿using System;
using System.Text;

namespace NNBasicsUtilities.Core.Utilities.UtilityTypes
{
	public struct FlatMatrix
	{
		private double[] _data;
		public int Rows;
		public int Cols;

		public override string ToString()
		{
			var builder = new StringBuilder();
			for (var i = 0; i < _data.Length; ++i)
			{
				if (i % (Cols) == 0)
				{
					builder.Append("| ");
				}
				builder.Append(_data[i]).Append(' ');
				if ((i + 1) % (Cols) == 0)
				{
					builder.Append("|\n");
				}
			}

			return builder.ToString();
		}

		private FlatMatrix(in int rows, in int cols)
		{
			if (rows < 0 || cols < 0) throw new ArgumentException("Negative size is not supported");
			Rows = rows;
			Cols = cols;
			_data = new double[rows * cols];
		}

		private FlatMatrix(FlatMatrix toCopy)
		{
			Rows = toCopy.Rows;
			Cols = toCopy.Cols;
			_data = new double[Rows * Cols];
			Buffer.BlockCopy(toCopy._data, 0, _data, 0, toCopy._data.Length);
		}

		private FlatMatrix(double[] data, int rows, int cols)
		{
			Rows = rows;
			Cols = cols;
			_data = new double[Rows * Cols];
			Buffer.BlockCopy(data, 0, _data, 0, data.Length);
		}

		public double this[int x, int y]
		{
			get => _data[x * Cols + y];
			set => _data[x * Cols + y] = value;
		}

		public FlatMatrix this[Range rows, Range cols]
		{
			get
			{
				var newCols = cols.End.Value - cols.Start.Value;
				var newRows = rows.End.Value - rows.Start.Value;
				var startCol = cols.Start.Value;
				var startRow = rows.Start.Value;
				var data = new double[newRows * newCols];
				for (var i = 0; i < newRows; ++i)
				{
					var startInd = (startRow + i) * Cols + startCol;
					var endInd = startInd + newCols;
					var range = _data[startInd..endInd];
					Console.WriteLine(range);
					Buffer.BlockCopy(range, 0 * sizeof(double), data, i * newCols * sizeof(double), newCols* sizeof(double));
				}

				var dst = new FlatMatrix {_data = data, Cols = newCols, Rows = newRows};
				return dst;
			}
		}

		public void ApplyFunction(Func<double, double> foo)
		{
			for (var i = 0; i < _data.Length; _data[i] = foo(_data[i]), ++i) { }
		}

		public static FlatMatrix Of(int rows, int cols)
		{
			return new FlatMatrix(rows, cols);
		}

		public static FlatMatrix Of(FlatMatrix toCopy)
		{
			return new FlatMatrix(toCopy);
		}

		public FlatMatrix T()
		{
			var dst = new FlatMatrix(Cols, Rows);
			for (var n = 0; n < _data.Length; ++n)
			{
				var i = n / Rows;
				var j = n % Rows;
				dst._data[n] = _data[Cols * j + i];
			}

			return dst;
		}
		public void SubtractMatrix(ref FlatMatrix other)
		{
			if (Cols != other.Cols || Rows != other.Rows)
			{
				throw new ArgumentException("Subtraction cannot be performed, provided matrices don't match the rule of size matching");
			}
			for (var i = 0; i < _data.Length; _data[i] -= (other._data[i]), ++i) { }
		}

		public void AddMatrix(ref FlatMatrix other)
		{
			if (Cols != other.Cols || Rows != other.Rows)
			{
				throw new ArgumentException("Addition cannot be performed, provided matrices don't match the rule of size matching");
			}
			for (var i = 0; i < _data.Length; _data[i] += (other._data[i]), ++i) { }
		}

		public static FlatMatrix operator +(FlatMatrix first, FlatMatrix other)
		{
			if (first.Cols != other.Cols || first.Rows != other.Rows)
			{
				throw new ArgumentException("Addition cannot be performed, provided matrices don't match the rule of size matching");
			}

			var dst = new FlatMatrix(first);
			for (var i = 0; i < first._data.Length; dst._data[i] += other._data[i], ++i) { }
			return dst;
		}

		public static FlatMatrix operator -(FlatMatrix first, FlatMatrix other)
		{
			if (first.Cols != other.Cols || first.Rows != other.Rows)
			{
				throw new ArgumentException("Subtraction cannot be performed, provided matrices don't match the rule of size matching");
			}

			var dst = new FlatMatrix(first);
			for (var i = 0; i < first._data.Length; dst._data[i] -= other._data[i], ++i) { }
			return dst;
		}

		public static FlatMatrix operator *(FlatMatrix first, FlatMatrix other)
		{
			if (first.Cols != other.Rows)
			{
				throw new ArgumentException($"Multiplication cannot be performed, provided matrices don't match the rule of size left.Cols = {first.Cols} != right.Rows = {other.Rows} ");
			}

			var dst = new FlatMatrix(first.Rows, other.Cols);
			for (var i = 0; i < dst.Rows; ++i)
			{
				for (var j = 0; j < dst.Cols; ++j)
				{
					var res = 0.0;
					for (var k = 0; k < first.Cols; ++k)
					{
						res += first._data[i * first.Cols + k] * other._data[k * other.Cols + j];
					}

					dst._data[i * dst.Cols + j] = res;
				}
			}
			return dst;
		}

		public static FlatMatrix TwoLoopMultiply(ref FlatMatrix first, ref FlatMatrix other)
		{
			if (first.Cols != other.Rows)
			{
				throw new ArgumentException($"Multiplication cannot be performed, provided matrices don't match the rule of size left.Cols = {first.Cols} != right.Rows = {other.Rows} ");
			}

			var dst = new FlatMatrix(first.Rows, other.Cols);
			for (var n = 0; n < dst._data.Length; ++n)
			{
				var i = n / other.Cols;
				var j = n % other.Cols;
				var res = 0.0;
				for (var k = 0; k < first.Cols; ++k)
				{
					res += first._data[i * first.Cols + k] * other._data[k * other.Cols + j];
				}

				dst._data[i * other.Cols + j] = res;

			}
			return dst;
		}

		public static FlatMatrix operator *(FlatMatrix matrix, double alpha)
		{
			var mat = new FlatMatrix(matrix.Rows, matrix.Cols);
			for (var i = 0; i < mat._data.Length; ++i)
			{
				mat._data[i] = alpha * matrix._data[i];
			}

			return mat;
		}

		public FlatMatrix HadamardProduct(ref FlatMatrix other)
		{
			if (Cols != other.Cols || Rows != other.Rows)
			{
				throw new ArgumentException($"Hadamard product cannot be computed: ({Rows}, {Cols}) != ({other.Rows}, {other.Cols})");
			}

			var mat = new FlatMatrix(Rows, Cols);

			for (var i = 0; i < mat._data.Length; ++i)
			{
				mat._data[i] = _data[i] * other._data[i];
			}

			return mat;
		}
	}
}
