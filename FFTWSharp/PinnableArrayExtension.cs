using Vorcyc.Mathematics;
using Vorcyc.Mathematics.Numerics;
using System.Numerics;

namespace FFTWSharp;

public static class PinnableArrayExtension
{
   
    extension (PinnableArray<float> array)
    {
        /// <summary>
        /// Scales all elements of the array in place so that their sum equals 1.
        /// </summary>
        /// <remarks>This method normalizes the array by dividing each element by the total number of
        /// elements. After calling this method, the sum of all elements in the array will be 1, provided the array
        /// contains numeric values. If the array is empty, no scaling is performed.</remarks>
        public void ScaleInPlace()
        {
            var factor = 1.0f / array.Length;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] *= factor;
            }
        }
    }

    extension (PinnableArray<double> array)
    {
        /// <summary>
        /// Scales the elements of the array in place so that their sum equals 1.
        /// </summary>
        /// <remarks>This method normalizes the array by dividing each element by the total number of
        /// elements. After calling this method, the sum of all elements in the array will be 1, provided the array
        /// contains numeric values. If the array is empty, no scaling is performed.</remarks>
        public void ScaleInPlace()
        {
            var factor = 1.0 / array.Length;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] *= factor;
            }
        }
    }

    extension (PinnableArray<ComplexFp32> array)
    {
        /// <summary>
        /// Scales the elements of the array in place so that their sum equals 1.
        /// </summary>
        /// <remarks>This method normalizes the array by dividing each element by the total number of
        /// elements. After calling this method, the sum of all elements in the array will be 1, unless the array is
        /// empty or contains non-numeric values such as NaN or Infinity.</remarks>
        public void ScaleInPlace()
        {
            var factor = 1.0f / array.Length;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] *= factor;
            }
        }
    }

    extension (PinnableArray<Complex> array)
    {
        /// <summary>
        /// Scales the elements of the underlying array in place so that their sum equals 1.
        /// </summary>
        /// <remarks>This method modifies the array directly by multiplying each element by a
        /// normalization factor. If the array is empty, no changes are made. Use this method when you need to normalize
        /// the array values without creating a new array.</remarks>
        public void ScaleInPlace()
        {
            var factor = 1.0 / array.Length;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] *= factor;
            }
        }
    }
}
