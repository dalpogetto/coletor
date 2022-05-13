﻿using System;
using Android.Util;

[assembly: Xamarin.Forms.Dependency(typeof(ILogInterface))]
namespace CollectorQualiIT.Droid
{
    public class LogImplementation : ILogInterface
    {
        public void Debug(string TAG, string message)
        {
            Log.Debug(TAG, message);
        }

        public void Error(string TAG, string message)
        {
            Log.Error(TAG, message);
        }

        public void Info(string TAG, string message)
        {
            Log.Info(TAG, message);
        }

        public void Verbose(string TAG, string message)
        {
            Log.Verbose(TAG, message);
        }

        public void Warn(string TAG, string message)
        {
            Log.Warn(TAG, message);
        }
    }
}
