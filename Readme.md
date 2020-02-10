# CPU Benchmark

[![Build](https://action-badges.now.sh/L3tum/CPU-Benchmark?action=.NET%20Core%20CI)](https://action-badges.now.sh/L3tum/CPU-Benchmark?action=.NET%20Core%20CI)

WIP Website is here [Website](https://l3tum.github.io/CPU-Benchmark/)

### Complementary Repositories

- [Website](https://github.com/L3tum/CPU-Benchmark-Website)
- [Database](https://github.com/L3tum/CPU-Benchmark-Database)
- [Aggregator](https://github.com/L3tum/CPU-Benchmark-Database-Aggregator)
- ~~Server~~ until I'm 99% certain that opening up the source of that won't enable hundreds of exploits it's gonna stay closed-source. However, it *will* be open-sourced until or with v1.0.

## Usage

Download the tool [from here](https://github.com/L3tum/CPU-Benchmark/releases/latest).

You can download a pre-packaged version for your system or choose the "general" system. You'll need to install .NET Core 3 (or compatible) yourself in that case.

Execute it :)

## Building it yourself

The master branch always has the "bleeding edge" changes and should, but may not, be able to be compiled and run.
If you want to get the latest changes then clone the repo and open it in your favourite .NET IDE. Compile the "Benchmarker" project which contains the CLI and then run the resulting executable.

## Available Options

Choose the benchmark to execute (replace BENCHMARK with the name of one of the benchmarks down below):

`.\benchmarker.exe --benchmark={BENCHMARK}`

Automatically detect the number of logical processors and execute the benchmark multithreaded (number of threads = number of logical processors)

`.\benchmarker.exe --multithreaded`

Manually set the amount of threads to use. Overwrites `--multithreaded`

`.\benchmarker.exe --threads=4`

Run the benchmark `X` times and average out the results. By default the benchmark will be run `3` times

`\.benchmarker.exe --runs=5`

Run the benchmarks in a memory efficient manner (on low-memory devices for example)

`\.benchmarker.exe --memory-efficient`

Run a stress test for the selected benchmark (currently only available for all extension benchmarks). Stop by pressing any key.

`\.benchmarker.exe --stress`

List all available benchmarks

`\.benchmarker.exe --list-benchmrks`

List all past results

`\.benchmarker.exe --list-results`

Upload your results

`\.benchmarker.exe --upload`

Clear all saved data

`\.benchmarker.exe --clear`


To combine the above

`.\benchmarker.exe --benchmark=zip --multithreaded --runs=64`

## Available Benchmarks

### General

* ALL (runs all of the below benchmarks)
* COMPRESSION (runs all compression benchmarks)
* DECOMPRESSION (runs all decompression benchmarks)
* ARITHMETIC (runs all arithmetic benchmarks)
* EXTENSION (runs all instruction extension benchmarks)
* CRYPTOGRAPHY (runs all cryptography benchmarks)
* INT (runs all integer-related benchmarks)
* FLOAT (runs all float-related benchmarks)
* AVX (runs all AVX benchmarks)
* SSE (runs all SSE benchmarks)
* Latency (runs all latency-related benchmarks)
* Experimental (runs all experimental benchmarks)

### Compression

`Note: All compression algorithms are run with the highest compression level (if applicable).`

* ZIP
* GZip
* BZip2
* Deflate

### Decompression

`Note: All compression/decompression algorithms are run with the highest compression level (if applicable).`

* ZIP
* GZip
* BZip2
* Deflate/Inflate

### Arithmetic

* Arithmetic_Int (Simple Integer Benchmark for byte, short, int and long performance)
* Arithmetic_Float (Simple Float Benchmark)
* Arithmetic_Double (Simple Double Benchmark)
* Arithmetic_FP16 (Simple FP16 (half-precision 16-bit floating point) Benchmark)

### Instruction extensions

* AVX
* SSE
* SSE2 (128-bit integer)
* AVX2Int (256-bit integer)
* FMA (fused multiply-add of 128-bit float)
* SSE4 (SSE4.1 Benchmark)
* SSE4-CRC32C (SSE4.2 CRC32C Benchmark)

### Cryptography

* Encryption
* Decryption
* CSPRNG (cryptographically secure pseudo-random number generator)
* Hashing (Hash with SHA-256)

### Latency

* L2CacheLatency (Tests L2 Cache Latency and outputs it into the file ./nanoseconds.txt, name is currently l2cachelatency-experimental)

### Experimental

* L2CacheLatency

More will be added in the future :)

## References

The benchmark will print reference points for a 3900x (non-overclocked) system.
Detailed system specs:
- R9 3900x
- 32 GB 3200 MHz CL16 RAM
- ASUS Hero VIII

## License

This tool is licensed under the GNU GPLv3. This tool and the developers are not responsible for bricking your system (e.g. when you're running without a thermal treshold).

You may contact us if the license is not suitable for your needs.

## Acknowledgements

- [CommandLine](https://github.com/commandlineparser/commandline) for the command line options
- [ShellProgressBar](https://github.com/Mpdreamz/shellprogressbar) for the progress bar
- [HTMLAgilityPack](https://github.com/zzzprojects/html-agility-pack) used in the HTML benchmark
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) used in the JSON benchmark and for saves
- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib) used for the (de)compression benchmarks

