﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using NNBasicsUtilities.Core.Utilities.UtilityTypes;
using NNBasicsUtilities.Extensions;

namespace NNBasicsUtilities.Core.Abstracts
{
	public abstract class Layer
	{
		protected Matrix Ins;
		protected Matrix Ons;
		protected Matrix LatestAnswer;
		protected Matrix LatestDeltas;

		private double _alpha;

		public List<ImmutableList<double>> Weights => Ons.Select(neuron => neuron.ToImmutableList()).ToList();

		public double Alpha
		{
			get => _alpha;
			set
			{
				const double min = 0.0;
				const double max = 1;
				if (!value.Between(min, max))
				{
					throw new ArgumentException($"Provided value: {value} is out of range of <{min}, {max}>");
				}

				_alpha = value;
			}
		}

		public Matrix Proceed(Matrix input)
		{
			//var time = Stopwatch.GetTimestamp();
			Ins = input;
			//time = Stopwatch.GetTimestamp() - time;
			//Console.WriteLine($"Layer Ins assignment time: {time}");
			//time = Stopwatch.GetTimestamp();
			var ans = NeuralEngine.Proceed(input, Ons);
			//time = Stopwatch.GetTimestamp() - time;
			//Console.WriteLine($"Layer proceed time: {time}");
			//time = Stopwatch.GetTimestamp();
			LatestAnswer = ans;
			//time = Stopwatch.GetTimestamp() - time;
			//Console.WriteLine($"Layer LatestAnswer assignment time: {time}");
			return Matrix.Copy(ans);
		}

		protected Layer(Matrix ons)
		{
			Ons = ons;
		}

		protected void UpdateWeights(Matrix deltas)
		{
			var mat = deltas.Transpose() * Ins;
			Ons.SubtractMatrix(mat * Alpha);
		}
	}
}