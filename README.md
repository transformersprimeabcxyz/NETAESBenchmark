NETAESBenchmark
===============
This is a small (and not so clean) program I wrote quickly to test the performance of Managed vs Native AES implementations available on the framework.
Background *(Why did I write this)*
============
As you may know .net framework provides two classes inherited from `Aes` that implement Advanced Encryption Standard, `AesCryptoServiceProvider` which is a native (i.e. non managed code) implementation that calls the [MS CryptoAPI](http://en.wikipedia.org/wiki/Microsoft_CryptoAPI) and `AesManaged` which is a purely managed implementation of the algorithm. So the question most are asking is [which one should I use ?](https://www.google.com/search?q=aesmanaged%20vs%20aescryptoserviceprovider&rct=j)

`AesCryptoServiceProvider` calls CAPI which is FIPS compliant (wheras `AesManaged` isn't) and CAPI is managed code and it is [generally accepted that native code runs faster than managed code](https://www.google.com/webhp?ion=1&ie=UTF-8#q=managed%20code%20vs%20native%20code%20performance) due to various overheads like JIT compilation and the fact that there is an extra abstraction layer on top of the operating system when you're running managed code ... that may be true, but does that mean that it is always a good idea to call native implementation of an algorithm in a managed environment ? Can we write `AesManaged` off ?

Well, that depends on your application, loading COM objects and calling unmanaged code (e.g. using P/Invoke) is somewhat expensive in terms of memory and the initialization time when compared to instanciating a managed object. On the other hand native code usually runs faster (if implemented correctly).

This creates an interesting phenomena that managed objects perform faster when the initialization time outweighs the execution of the method(s) that we are calling ! And that's why I created this benchmark tool, to evaluate how each implementation performs with different data sizes, key sizes etc.

Configuration
============
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
        private const bool _disposeEachCycle = true;
        #endregion
```
You'll want to change the following at the very least:
* `_margin`: is the time difference in milliseconds that you'd consider a significant gain for using Native AES implementation over the Managed counterpart.
* `_iterations`: is the number of times you'd want to run a Encrypt, Decrypt, Dispose cycle for each data size (Bear in mind that setting the _iterations to something lower than 1000 won't give you consistent results, however at higher iterations the overall margin of error associated with the measurements (mostly timing) is minimized and you'll get more consistent results)
* `_startSize` and `_maxSize` specify the minimum and maximum data array sizes (useful if you'd want to cap the benchmark or skip lower array sizes)
* `_step`: the current data array size is incremented by `_step` each iteration (I'd suggest you keep this at 1 for the most granular result, however this can be used in conjunction with `_startSize` and `_maxSize` to first find the approximate size you want so you can investigate further in a later run)
* set the `_cipherMode` and the `_padding` to the ones you'd be using, as well as the `_keysize`
* `_mOE` tells the program how many times to run the iteration past the point it finds Managed AES to be slower (10 is a good number, this will directly affect the number of times you'd have to press <enter> to get the final results. The intermediate results could also be interesting)
* `_disposeEachCycle` can be set to false to emualte scenarios where the crypto transform is reused (e.g. using the same keys all the time, no salting, etc.) bear in mind that the Native Decryptor transform goes out of sync the first time you call the `TransformFinalBlock()` on it therefore the benchmark code always calls `CreateDecryptor()` for a new Decryptor (otherwise a `CryptographicException` is thrown with the message *Padding is invalid and cannot be removed* *[bug?]*)

Sample results
=============
Here's the first output from the program (which I ran just now on my laptop)
```
With the current parameters ...
Managed AES has a distinct advantage for data sizes below 75 bytes !
Performance is almost identical (with fluctuations about 0.000133 milliseconds)
for the 76 to 143 byte ranges.

Use native AES for buffers larger than 154 to save ~0.000455ms per iteration !

Press enter to run the benchmark further to find the upper limit given the curre
nt 0.001ms margin ...
```
*Note that given the input parameters you CAN get negative results for the fluctuations, which simply means Managed AES was on average that much faster.*

And when I pressed enter twice, it finally reached the _margin (which was 0.001ms/iteration)
```
With the current parameters ...
Managed AES has a distinct advantage for data sizes below 75 bytes !
Performance is almost identical (with fluctuations about 0.000896 milliseconds)
for the 76 to 159 byte ranges.

Use native AES for buffers larger than 170 to save ~0.001014ms per iteration !

Press enter to exit ...

Result: if your average data size approaches 170 bytes ManagedAes would be 0.001
014ms slower which approaches your margin !
```

TODO: complete this !
