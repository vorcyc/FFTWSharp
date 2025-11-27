



using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FFTWSharp;
using FFTWSharp.FftwSingle;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Vorcyc.Mathematics;
using Vorcyc.Mathematics.Framework.Utilities;
using Vorcyc.Mathematics.Numerics;
using Vorcyc.Mathematics.SignalProcessing.Fourier;
using Vorcyc.Mathematics.SignalProcessing.Transforms;

PinnableArray<float> r = new(1024, true);
r.Fill(0, 1);
PinnableArray<ComplexFp32> c = new(1024, true);
c.Fill(0, new ComplexFp32(1, 0));

r.PrintLine(ConsoleColor.Yellow);
c.PrintLine(ConsoleColor.Cyan);

FFTWSharp.FftwSingle.Dft1D.Dft1DComplexInPlace(c, FFTWSharp.Interop.fftw_direction.Forward);
c.PrintLine(ConsoleColor.Green);

FFTWSharp.FftwSingle.Dft1D.Dft1DComplexInPlace(c, FFTWSharp.Interop.fftw_direction.Backward);

c.ScaleInPlace();
c.PrintLine(ConsoleColor.Magenta);



FFTWSharp.FftwSingle.Dft1D.Dft1DR2C(r, c);
c.PrintLine();

FFTWSharp.FftwSingle.Dft1D.Dft1DC2R(c, r);
r.PrintLine(ConsoleColor.Green);






//BenchmarkRunner.Run<FFT_benchmark>();


public class FFT_benchmark
{


    [Params(16384, 32768, 65536, 131072, 262144, 524288, 4194304, 16777216)]
    public int N;


    //public float[] _array;
    private PinnableArray<float>? _array;
    private PinnableArray<ComplexFp32>? _complex;

    [GlobalSetup]
    public void Setup()
    {
        //_array = new float[N];
        //for (int i = 0; i < _array.Length; i++)
        //{
        //    _array[i] = Random.Shared.NextSingle();
        //}
        _array?.Dispose();
        _array = null;
        _array = new(N, true);

        _complex?.Dispose();
        _complex = null;
        _complex = new(N, true);

        FastFourierTransform.Version = FftVersion.SIMD;
    }


    [Benchmark]
    public bool my_forward() => FastFourierTransform.Forward(_array, _complex);


    [Benchmark]
    public bool my_backward() => FastFourierTransform.Inverse(_complex, _complex);


    [Benchmark]
    public void fftw_forward() => FFTWSharp.FftwSingle.Dft1D.Dft1DR2C(_array, _complex);


    [Benchmark]
    public void fftw_backward() => FFTWSharp.FftwSingle.Dft1D.Dft1DC2R(_complex, _array);







}