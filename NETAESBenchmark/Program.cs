﻿using System;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Threading;

// ReSharper disable once RedundantAssignment

namespace EncryptionBenchmark
{
    class Program
    {
        #region "Settings"
        private const double _margin = 0.001; // time difference in milliseconds that we consider to be significant
        const int _keysize = 32;            // must be a legal AES key size
        const int _iterations = 0x186A0 / 10;    // how many times to run each encryption/decryption run ... 10K by default
        const int _startSize = 1;           // initial data size to encrypt
        const int _maxSize = 2000;           // maximum size to encrypt 
        const int _step = 1;                // increase data size by _step each run
        const CipherMode _cipherMode = CipherMode.CBC;
        const PaddingMode _padding = PaddingMode.PKCS7;
        private const int _mOE = 10;
        #endregion

        private static TimeSpan _last = TimeSpan.Zero;
        private static int _begSize = 0;
        private static int _cOcSz = 0;
        private static int _cOcc = 0;
        private static float _curVar = 0;

        private static int _sCount = 0;
        private static int _avgDev = 0;

        static void Main()
        {
            var data = new byte[0];
            var key = new byte[_keysize];

            var r = new RNGCryptoServiceProvider();

            for (var _sz = _startSize; _sz < _maxSize; _sz += _step)
            {
                data = new byte[_sz];

                r.GetBytes(data);
                r.GetBytes(key);

                Benchmark("AesCryptoServiceProvider", new AesCryptoServiceProvider { Padding = _padding, Key = key, Mode = _cipherMode }, data);
                Benchmark("AesManaged", new AesManaged { Padding = _padding, Key = key, Mode = _cipherMode }, data);
                Console.WriteLine("\n");
            }
        }

        private static void Benchmark(string _cipherName, SymmetricAlgorithm aes, byte[] data)
        {
            var sw = new Stopwatch();
            int _dataSize = data.Length;
            Console.Write("{1} {0} bytes\t\t\t", _dataSize, _cipherName);

            sw.Start();
            for (int i = 0; i < _iterations; i++)
            {
                ICryptoTransform crypto = aes.CreateEncryptor();
                ICryptoTransform decryptor = aes.CreateDecryptor();
                var encryptedData = crypto.TransformFinalBlock(data, 0, data.Length);
                decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                crypto.Dispose();
                decryptor.Dispose();
            }
            sw.Stop();

            bool _isManaged = _cipherName == "AesManaged";

            if (_begSize != 0)
                _avgDev = ((_avgDev * _sCount++) + (int)((((_isManaged) ?
                    (sw.Elapsed - _last).TotalMilliseconds : (_last - sw.Elapsed).TotalMilliseconds) / _iterations) * 0xF4240)) / _sCount;

            if (_isManaged && (sw.Elapsed > _last))
            {
                float _msVar = 0;

                _last = sw.Elapsed;

                if (_begSize == 0) _begSize = _dataSize;
                if (_cOcc == 0) { _cOcSz = _dataSize; }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\t\t" + sw.Elapsed);

                Console.WriteLine("\n\n===> Critical size chance @ " + _dataSize + " bytes");
                Console.ForegroundColor = ConsoleColor.Gray;
                if ((_cOcc >= _mOE & _curVar > 0) | (_msVar = (_avgDev / 1000000f)) > _margin)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(
                        "\n\n================\n" +
                        "With the current parameters ...\n"
                        + "Managed AES has a distinct advantage for data sizes below {0} bytes !\n" +
                        "Performance is almost identical (with fluctuations about {4} milliseconds) for the {1} to {2} byte ranges.\n" +
                        "Use native AES for buffers larger than {3} to save more than {5}ms per iteration !\n" +
                        "\nPress enter to run the benchmark further " +
                        ((_msVar <= _margin) ? "to find the upper limit given the current {5}ms margin ...\n" : " (would be an overkill tho) !!!"),
                        _begSize, _begSize + 1, _cOcSz - _mOE, _cOcSz + 1, _curVar, _margin);
                    Console.ReadLine();
                    _cOcc = 0;
                }
                if (++_cOcc == 1) { _curVar = _msVar; }
                return;
            }
            if (_isManaged) _cOcc = 0;
            Console.WriteLine((_isManaged ? "\t\t" : "") + (_last = sw.Elapsed));
            sw.Reset();
        }
    }
}