﻿using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using NNBasicsUtilities.Core.Utilities.UtilityTypes;
using BenchmarkAttribute = BenchmarkDotNet.Attributes.BenchmarkAttribute;

namespace MatricesBenchmark
{
	[RPlotExporter]
	
	[JsonExporter("haha.json", true, false)]
	[LegacyJitX64Job, LegacyJitX86Job, RyuJitX64Job]
	public class Benchmarking
	{
		private FlatMatrix array;
		private Matrix matrix;

		[GlobalSetup]
		public void Setup()
		{
			array = FlatMatrix.Of(500, 500);
			array.ApplyFunction(d => 1);
			matrix = new Matrix((new Tuple<int, int>(500, 500)));
			matrix.ApplyFunction(d => 1);
		}

		[Benchmark(OperationsPerInvoke = 16, Baseline = true)]
		public void StaticMultiply()
		{
			ref var mat1 = ref array;
			var mat = FlatMatrix.Of(mat1.Rows, mat1.Cols);
			for (var i = 0; i < 16; ++i)
			{
				FlatMatrix.Multiply(mat1, mat1, mat);
			}
		}


		[Benchmark(OperationsPerInvoke = 16)]
		public void MatrixOpMul()
		{
			for (var i = 0; i < 16; ++i)
			{
				var mat = matrix * matrix;
			}
		}
	}
}