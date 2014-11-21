NETAESBenchmark
===============
This is a small (and not so clean) program I wrote quickly to test the performance of Managed vs Native AES implementations available on the framework.
Please take a look at the "Settings" region to fine tune the benchmark parameters to emulate your application behavior/needs.
```C# 
        #region "Settings"
        private const double _margin = 0.001;
        const int _keysize = 32;
        const int _iterations = 0x186A0 / 10;
        const int _startSize = 1;
        const int _maxSize = 2000;
        const int _step = 1;
        const CipherMode _cipherMode = CipherMode.CBC;
        const PaddingMode _padding = PaddingMode.PKCS7;
        private const int _mOE = 10;
        #endregion
```
You'll want to change the following at the very least:
* _margin: is the time difference in milliseconds that you'd consider a significant gain for using Native AES implementation over the Managed counterpart.
* _iterations: is the number of times you'd want to run a Encrypt, Decrypt, Dispose cycle for each data size (Bear in mind that setting the _iterations to something lower than 1000 won't give you consistent results, however at higher iterations the overall margin of error associated with the measurements (mostly timing) is minimized and you'll get more consistent results)
* _startSize and _maxSize specify the minimum and maximum data array sizes (useful if you'd want to cap the benchmark or skip lower array sizes)
* _step: the current data array size is incremented by _step each iteration (I'd suggest you keep this at 1 for the most granular result, however this can be used in conjunction with _startSize and _maxSize to first find the approximate size you want so you can investigate further in a later run)
* set the _cipherMode and the _padding to the ones you'd be using, as well as the _keysize
* _mOE tells the program how many times to run the iteration past the point it finds Managed AES to be slower (10 is a good number, this will directly affect the number of times you'd have to press <enter> to get the final results. The intermediate results could also be interesting)

TODO: complete this !
