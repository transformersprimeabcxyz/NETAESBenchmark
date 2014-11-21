NETAESBenchmark
===============
This is a small program I wrote to test the performance of Managed vs Native AES implementations available on the framework.
Please take a look at the "Settings" region to fine tune the benchmark parameters to emulate your application behavior/needs.

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

Bear in mind that setting the _iterations to something lower than 1000 won't give you consistent results, however at higher iterations the overall margin of error associated with the measurements (mostly timing) is minimized and you'll get more consistent results.

TODO: complete this !
