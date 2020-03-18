﻿using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace NNBasics.Lab1
{
   public class OutputNeuron : NeuronBase
   {
      private List<double> _weights;
      public List<double> Weights
      {
         get => _weights ?? new List<double>();
         set =>_weights = value ?? throw new NullReferenceException("This param cannot be null");
      }

      public static implicit operator OutputNeuron(List<double> values)
      {
         return new OutputNeuron() { Weights = values };
      }
   }

}
