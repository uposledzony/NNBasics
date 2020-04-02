﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NNBasics.NNBasicsLimak.Extensions;

namespace NNBasics.NNBasicsLimak.Core.UtilityTypes
{
   public class Matrix : List<List<double>>
   {
      private int _rows;
      private readonly int _cols;

      public override string ToString()
      {
         var builder = new StringBuilder();
         foreach (var row in this)
         {
            builder.Append("| ");
            foreach (var d in row)
            {
               builder.Append(d).Append(" ");
            }

            builder.Append("|\n");
         }

         return builder.ToString();
      }

      public Matrix(List<List<double>> values)
      {
         var cols = values[0].Count;
         if (values.Any(row => row.Count != cols))
         {
            throw new ArgumentException("Provided list of list of doubles isn't a good candidate to converse it into matrix");
         }
         
         AddRange(values);
      }

      public Matrix(Tuple<int, int> size = null)
      {
         var (cols, rows) = size?? Tuple.Create(1,1);
         _rows = rows;
         _cols = cols;
      }

      public new void Add(List<double> row)
      {
         if (row.Count != _cols)
         {
            throw new ArgumentException($"Provided row can't match its size with number of columns of matrix, expected size: {_cols}, got: {row.Count}");
         }

         base.Add(row);
         if(Count > _rows)
            ++_rows;
      }

      public Matrix Transpose()
      {
         var mat = this.SelectMany(inner => inner.Select((item, index) => new { item, index }))
            .GroupBy(i => i.index, i => i.item)
            .Select(g => g.ToList())
            .ToList();
         return new Matrix(mat);
      }

      public static Matrix operator + (Matrix first, Matrix other)
      {
         if (first._cols != other._cols || first._rows != other._rows)
         {
            throw new ArgumentException("Addition cannot be performed, provided matrices don't match the rule of size matching");
         }

         return first.Select((row, rowId) => row.Zip(other[rowId], (d, d1) => d + d1).ToList()).ToList().ToMatrix();
      }

      public static Matrix operator * (Matrix first, Matrix other)
      {
         if (first._cols != other._rows)
         {
            throw new ArgumentException("Multiplication cannot be performed, provided matrices don't match the rule: A.cols == B.rows, where A, B are matrices, cols is count of columns and rows is count of rows");
         }

         var mat = first.Select(
            (row, rowId) => other.Transpose()
                                 .Select((col, colId) => col.Zip(row, (colCell, rowCell) => colCell*rowCell).Sum()
                                 ).ToList()
            ).ToList();

         var matrix = new Matrix(mat);

         return matrix;
      }

      public static Matrix operator * (Matrix first, double alpha)
      {
        var mat = first.Select(
            (row, rowId) => row.Select(elem => elem * alpha).ToList()
         ).ToList();

         var matrix = new Matrix(mat);

         return matrix;
      }

   }
}
