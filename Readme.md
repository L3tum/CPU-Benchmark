# CPU Benchmark

[![Build](https://action-badges.now.sh/L3tum/CPU-Benchmark?action=.NET%20Core%20CI)](https://action-badges.now.sh/L3tum/CPU-Benchmark?action=.NET%20Core%20CI)

## Usage

Download the tool on the Releases tab.

You can download a pre-packaged version for your system or choose the "general" system. You'll need to install .NET Core 3 (or compatible) yourself in that case.

Execute it :)

## Available Options

Choose the benchmark to execute (replace BENCHMARK with the name of one of the benchmarks down below):

`.\benchmarker.exe --benchmark={BENCHMARK}`


Automatically detect the number of logical processors and execute the benchmark multithreaded (number of threads = number of logical processors)

`.\benchmarker.exe --multithreaded`


Manually set the amount of threads to use. Overwrites `--multithreaded`

`.\benchmarker.exe --threads=4`


Run the benchmark `X` times and average out the results. By default the benchmark will be run `3` times

`\.benchmarker.exe --runs=5`

List all available benchmarks

`\.benchmarker.exe --list-benchmrks`

List all past results

`\.benchmarker.exe --list-results`

Make a quick run (disables some functionality and exempts the benchmark from being verified)

`\.benchmarker.exe --quick`

Run the benchmarks in a memory efficient manner (on low-memory devices for example)

`\.benchmarker.exe --memory-efficient`


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

### Instruction extensions

* AVX
* SSE

### Cryptography

* Encryption
* Decryption
* CSPRNG (cryptographically secure pseudo-random number generator)

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

