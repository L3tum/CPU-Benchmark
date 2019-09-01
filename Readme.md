# CPU Benchmark

## Usage

Download the tool here.

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


To combine the above

`.\benchmarker.exe --benchmark=zip --multithreaded --runs=64`

## Available Benchmarks

### General

* ALL (runs all of the below benchmarks)
* COMPRESSION (runs all compression benchmarks)
* ARITHMETIC (runs all arithmetic benchmarks)
* EXTENSION (runs all instruction extension benchmarks)
* INT (runs all integer-related benchmarks)
* FLOAT (runs all float-related benchmarks)

### Compression

`Note: All compression algorithms are run with the highest compression level (if applicable).`

* ZIP
* GZip
* BZip2
* Deflate

### Arithmetic
* Arithmetic_Int (Simple Integer Benchmark for byte, short, int and long performance)
* Arithmetic_Float (Simple Float Benchmark)

### Instruction extensions

* AVX
* SSE

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

