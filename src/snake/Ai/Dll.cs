using Godot;
using System;
using System.Runtime.InteropServices;



namespace Snake.AI
{
    public static class Dll
    {
       
        private const string DllPath = "Training_IA.dll";

        //PMC 

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr createPMC(int[] npl_data, int npl_size);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void trainPMC(IntPtr modelPtr, double[] X_flat, double[] Y_flat, int nb_samples, int input_size, int output_size, int iteration, double learning_rate, bool is_classification);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void predictPMC(IntPtr modelPtr, double[] input_data, int input_size, bool is_classification, double[] output_data);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void deletePMC(IntPtr modelPtr);
    }
}
