using Vorcyc.Mathematics;
using Vorcyc.Mathematics.Numerics;
using System.Numerics;

namespace FFTWSharp;

public static class PinnableArrayExtension
{

    extension(PinnableArray<float> array)
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

    extension(PinnableArray<double> array)
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

    extension(PinnableArray<ComplexFp32> array)
    {
        /// <summary>
        /// 此时的 <see cref="array"/>为 idft后的结果。
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



        /// <summary>
        /// 此时的 <see cref="array"/>为 离散傅立叶变换（不是快速傅立叶变换） 后的结果。
        /// 返回单边频谱：一个 (double frequency, double amplitude)[] 数组
        /// 频率从 fs/N 开始，最高 ≤ fs/2，自动兼容奇偶长度
        /// </summary>
        public (float Frequency, float Amplitude)[] GetSingleSidedSpectrum(
            float sampleRate,
            bool inDb = false)
        {
            int N = array.Length;
            if (N < 2) throw new ArgumentException("FFT length too small");

            int len = N / 2 + (N % 2);               // 奇数时 (N+1)/2，偶数时 N/2
            var spectrum = new (float Frequency, float Amplitude)[len];

            float df = sampleRate / N;            // 频率分辨率
            float scale = 2.0f / N;                   // 单边幅度校准（DC 除外）

            for (int k = 1; k <= len; k++)
            {
                float mag = array[k].Magnitude * scale;

                float amplitude = inDb
                    ? 20.0f * MathF.Log10(mag + 1e-20f)
                    : mag;

                spectrum[k - 1] = (k * df, amplitude);   // 直接赋值 ValueTuple
            }

            return spectrum;
        }


        /// <summary>
        ///  从完整的 FFT 结果中提取单边谱（Complex[]）。此时的 array 是完整的傅立叶变换结果。
        /// - 自动跳过 DC（索引0）
        /// - 包含 Nyquist 频率（如果存在）
        /// - 奇偶长度完全兼容，和 MATLAB / NumPy 行为 100% 一致
        /// </summary>
        /// <returns>单边 Complex[]，长度 = N/2 (偶数) 或 (N+1)//2 (奇数)</returns>
        /// <remarks>
        /// 后面要做滤波、频域相乘、再 IFFT；需要相位信息（相位谱、群延迟等）用这个。
        /// </remarks>
        public ReadOnlySpan<ComplexFp32> GetSingleSided()
        {
            int N = array.Length;
            if (N < 2) throw new ArgumentException("FFT length too small");

            // 关键：这一行同时完美处理奇数和偶数！
            int len = N / 2 + (N % 2);        // 偶数 → N/2，奇数 → (N+1)/2

            return array.AsSpan(1, len);   // 跳过 DC 分量
        }


        /// <summary>
        /// 返回能量正确的单边 Complex[]（除 DC 外已 ×2/N）。此时的 array 是完整的傅立叶变换结果。
        /// 适合直接取幅度画图或功率计算
        /// </summary>
        /// <remarks>
        /// 画频谱图（线性或 dB）；找峰值频率、计算 SNR、显示 dBFS；画功率谱密度 PSD（单位 W/Hz），用这个。
        /// </remarks>
        public PinnableArray<ComplexFp32> GetSingleSidedComplexScaled()
        {
            int N = array.Length;
            int len = N / 2 + (N % 2);

            var singleSided = new PinnableArray<ComplexFp32>(len);
            float scale = 2.0f / N;                     // 经典单边校准

            for (int k = 1; k <= len; k++)
            {
                singleSided[k - 1] = array[k] * scale;
            }

            return singleSided;
        }

    }

    extension(PinnableArray<Complex> array)
    {
        /// <summary>
        /// 此时的 <see cref="array"/>为 idft后的结果。
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

        /// <summary>
        /// 此时的 <see cref="array"/>为 离散傅立叶变换（不是快速傅立叶变换） 后的结果。
        /// 返回单边频谱：一个 (double frequency, double amplitude)[] 数组
        /// 频率从 fs/N 开始，最高 ≤ fs/2，自动兼容奇偶长度
        /// </summary>
        public (double Frequency, double Amplitude)[] GetSingleSidedSpectrum(
            double sampleRate,
            bool inDb = false)
        {
            int N = array.Length;
            if (N < 2) throw new ArgumentException("FFT length too small");

            int len = N / 2 + (N % 2);               // 奇数时 (N+1)/2，偶数时 N/2
            var spectrum = new (double Frequency, double Amplitude)[len];

            double df = sampleRate / N;            // 频率分辨率
            double scale = 2.0 / N;                   // 单边幅度校准（DC 除外）

            for (int k = 1; k <= len; k++)
            {
                double mag = array[k].Magnitude * scale;

                double amplitude = inDb
                    ? 20.0 * Math.Log10(mag + 1e-20)
                    : mag;

                spectrum[k - 1] = (k * df, amplitude);   // 直接赋值 ValueTuple
            }

            return spectrum;
        }


        /// <summary>
        ///  从完整的 FFT 结果中提取单边谱（Complex[]）。此时的 array 是完整的傅立叶变换结果。
        /// - 自动跳过 DC（索引0）
        /// - 包含 Nyquist 频率（如果存在）
        /// - 奇偶长度完全兼容，和 MATLAB / NumPy 行为 100% 一致
        /// </summary>
        /// <returns>单边 Complex[]，长度 = N/2 (偶数) 或 (N+1)//2 (奇数)</returns>
        /// <remarks>
        /// 后面要做滤波、频域相乘、再 IFFT；需要相位信息（相位谱、群延迟等）用这个。
        /// </remarks>
        public ReadOnlySpan<Complex> GetSingleSided()
        {
            int N = array.Length;
            if (N < 2) throw new ArgumentException("FFT length too small");

            // 关键：这一行同时完美处理奇数和偶数！
            int len = N / 2 + (N % 2);        // 偶数 → N/2，奇数 → (N+1)/2

            return array.AsSpan(1, len);   // 跳过 DC 分量
        }


        /// <summary>
        /// 返回能量正确的单边 Complex[]（除 DC 外已 ×2/N）。此时的 array 是完整的傅立叶变换结果。
        /// 适合直接取幅度画图或功率计算
        /// </summary>
        /// <remarks>
        /// 画频谱图（线性或 dB）；找峰值频率、计算 SNR、显示 dBFS；画功率谱密度 PSD（单位 W/Hz），用这个。
        /// </remarks>
        public PinnableArray<Complex> GetSingleSidedComplexScaled()
        {
            int N = array.Length;
            int len = N / 2 + (N % 2);

            var singleSided = new PinnableArray<Complex>(len);
            double scale = 2.0 / N;                     // 经典单边校准

            for (int k = 1; k <= len; k++)
            {
                singleSided[k - 1] = array[k] * scale;
            }

            return singleSided;
        }
    }
}
