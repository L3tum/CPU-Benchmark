# Change Log

## [v0.5.0](https://github.com/L3tum/CPU-Benchmark/tree/v0.5.0) (2019-12-23)
[Full Changelog](https://github.com/L3tum/CPU-Benchmark/compare/v0.4.1...v0.5.0)

- Fixed an exception that could occur on older hardware that doesn't support one of the instruction extensions
- Added SSE2 (128-bit integer), AVX2 (256-bit integer) and FMA (fused multiply-add of 128-bit float) benchmarks
- Added arithmetic_double benchmark
- Added arithmetic_fp16 benchmark
- Added support for multiple categories per benchmark to better group them together
- Added uploaded field to save data

## [v0.4.1](https://github.com/L3tum/CPU-Benchmark/tree/v0.4.1) (2019-12-22)
[Full Changelog](https://github.com/L3tum/CPU-Benchmark/compare/v0.4.0...v0.4.1)

**Updates:**

- Fixed exception that could occur when calculating the hash of the current saved data
- Added "category aggregator" that enables users to run each benchmark separately and if they got all for one category (or "all"), then that category is added (or updated) to the results

## [v0.4.0](https://github.com/L3tum/CPU-Benchmark/tree/v0.4.0) (2019-12-21)
[Full Changelog](https://github.com/L3tum/CPU-Benchmark/compare/v0.3.0...v0.4.0)

**Updates:**

- Added clear option to clear all (locally) saved data
- Added upload option to upload last (valid) benchmark run instead of uploading regardless of situation
- Deprecated the `-q / --quick` option in favor of the upload option
- Moved save data to hidden directory "_save" to make it easier managable
- Switched to only allow uploading once "category:all" (`--benchmark all`) has been run
- Added option to view uploaded results in browser
- Simplified progress reporting
- Switched reference values to always refer to the all-core performance of the stock 3900X to simplify and unify the point system
- Reworked the categorization logic to clean up the code and fix some bugs
- Adjusted volumes of several benchmarks to make the run faster yet still comparable. A full benchmark run now takes ~60 seconds on the 3900X.
- Added comparisons to benchmarks. These serve as the new "reference" value that is only printed to the user rather than used in calculating the points. These can be easily expanded to more than SC/AC references.
- Reworked result saving logic to be more concise and easier on the user, while also being stricter and more secure against tampering done on the save
- Added better error messages
- Fixed some minor bugs that could pop up in specific situations
- Fixed memory leak that occurs when saving the results (which should only happen immediately before closing the program, but you never know)

## [v0.3.0](https://github.com/L3tum/CPU-Benchmark/tree/v0.3.0) (2019-10-26)
[Full Changelog](https://github.com/L3tum/CPU-Benchmark/compare/v0.2.0...v0.3.0)

**Updates:**

- Add decompression benchmarks [\#81](https://github.com/L3tum/CPU-Benchmark/pull/81) ([L3tum](https://github.com/L3tum))
- Update for uploading results to the benchmark database [\#78](https://github.com/L3tum/CPU-Benchmark/pull/78) ([L3tum](https://github.com/L3tum))
- ResultUploader + ResultLister + Bugfixes + Dependency Updates [\#73](https://github.com/L3tum/CPU-Benchmark/pull/73) ([L3tum](https://github.com/L3tum))
- Bump HtmlAgilityPack from 1.11.12 to 1.11.15 [\#71](https://github.com/L3tum/CPU-Benchmark/pull/71) ([dependabot-preview[bot]](https://github.com/apps/dependabot-preview))

## [v0.2.0](https://github.com/L3tum/CPU-Benchmark/tree/v0.2.0) (2019-09-14)
[Full Changelog](https://github.com/L3tum/CPU-Benchmark/compare/v0.1.1...v0.2.0)

**Updates:**

- Updated changelog [\#65](https://github.com/L3tum/CPU-Benchmark/pull/65) ([L3tum](https://github.com/L3tum))
- Feature/prerelease [\#64](https://github.com/L3tum/CPU-Benchmark/pull/64) ([L3tum](https://github.com/L3tum))
- Switch to external lib [\#63](https://github.com/L3tum/CPU-Benchmark/pull/63) ([L3tum](https://github.com/L3tum))
- Update netcore.yml [\#62](https://github.com/L3tum/CPU-Benchmark/pull/62) ([L3tum](https://github.com/L3tum))
- Feature/improvements [\#60](https://github.com/L3tum/CPU-Benchmark/pull/60) ([L3tum](https://github.com/L3tum))
- Feature/rework benchmark structure [\#59](https://github.com/L3tum/CPU-Benchmark/pull/59) ([L3tum](https://github.com/L3tum))
- Feature/rework rater [\#58](https://github.com/L3tum/CPU-Benchmark/pull/58) ([L3tum](https://github.com/L3tum))
- Update build.ps1 [\#55](https://github.com/L3tum/CPU-Benchmark/pull/55) ([L3tum](https://github.com/L3tum))
- Added option to list all benchmarks [\#54](https://github.com/L3tum/CPU-Benchmark/pull/54) ([L3tum](https://github.com/L3tum))
- Update Readme.md [\#53](https://github.com/L3tum/CPU-Benchmark/pull/53) ([L3tum](https://github.com/L3tum))
- Update label-manager.yml [\#52](https://github.com/L3tum/CPU-Benchmark/pull/52) ([L3tum](https://github.com/L3tum))
- Add more actions [\#51](https://github.com/L3tum/CPU-Benchmark/pull/51) ([L3tum](https://github.com/L3tum))
- Update netcore.yml [\#50](https://github.com/L3tum/CPU-Benchmark/pull/50) ([L3tum](https://github.com/L3tum))
- Update netcore.yml [\#49](https://github.com/L3tum/CPU-Benchmark/pull/49) ([L3tum](https://github.com/L3tum))
- Add github action for simple build on pull request [\#48](https://github.com/L3tum/CPU-Benchmark/pull/48) ([L3tum](https://github.com/L3tum))
- Switch rating algorithm to linear [\#47](https://github.com/L3tum/CPU-Benchmark/pull/47) ([L3tum](https://github.com/L3tum))
- Feature/add changelog generator [\#46](https://github.com/L3tum/CPU-Benchmark/pull/46) ([L3tum](https://github.com/L3tum))
- Add result saver and machine information [\#27](https://github.com/L3tum/CPU-Benchmark/pull/27) ([L3tum](https://github.com/L3tum))

## [v0.1.1](https://github.com/L3tum/CPU-Benchmark/tree/v0.1.1) (2019-09-03)
[Full Changelog](https://github.com/L3tum/CPU-Benchmark/compare/v0.1-alpha...v0.1.1)

**Updates:**

- Switch to AES GCM since AES anything else is insecure [\#45](https://github.com/L3tum/CPU-Benchmark/pull/45) ([L3tum](https://github.com/L3tum))
- Fix SSE only add and not multiplying [\#44](https://github.com/L3tum/CPU-Benchmark/pull/44) ([L3tum](https://github.com/L3tum))
- Fix general benchmarks progress being wrong [\#43](https://github.com/L3tum/CPU-Benchmark/pull/43) ([L3tum](https://github.com/L3tum))
- Fix bug throwing exception when no options given [\#42](https://github.com/L3tum/CPU-Benchmark/pull/42) ([L3tum](https://github.com/L3tum))
- Update issue templates [\#41](https://github.com/L3tum/CPU-Benchmark/pull/41) ([L3tum](https://github.com/L3tum))
- Update Readme.md [\#30](https://github.com/L3tum/CPU-Benchmark/pull/30) ([L3tum](https://github.com/L3tum))

## [v0.1-alpha](https://github.com/L3tum/CPU-Benchmark/tree/v0.1-alpha) (2019-09-02)
**Updates:**

- Fix docs and naming [\#29](https://github.com/L3tum/CPU-Benchmark/pull/29) ([L3tum](https://github.com/L3tum))
- Add multiplication to avx to better represent common workloads [\#28](https://github.com/L3tum/CPU-Benchmark/pull/28) ([L3tum](https://github.com/L3tum))
- Feature/bnch 21 add crypto [\#26](https://github.com/L3tum/CPU-Benchmark/pull/26) ([L3tum](https://github.com/L3tum))
- Update Readme for Brotli [\#24](https://github.com/L3tum/CPU-Benchmark/pull/24) ([L3tum](https://github.com/L3tum))
- Update build.ps1 with zipping [\#23](https://github.com/L3tum/CPU-Benchmark/pull/23) ([L3tum](https://github.com/L3tum))
- Added brotli benchmark [\#22](https://github.com/L3tum/CPU-Benchmark/pull/22) ([L3tum](https://github.com/L3tum))
- Update Readme.md [\#20](https://github.com/L3tum/CPU-Benchmark/pull/20) ([L3tum](https://github.com/L3tum))
- Add SSE [\#19](https://github.com/L3tum/CPU-Benchmark/pull/19) ([L3tum](https://github.com/L3tum))
- Feature/add build [\#18](https://github.com/L3tum/CPU-Benchmark/pull/18) ([L3tum](https://github.com/L3tum))
- Add floating point benchmark [\#17](https://github.com/L3tum/CPU-Benchmark/pull/17) ([L3tum](https://github.com/L3tum))
- Improved progress reporting and memory usage [\#16](https://github.com/L3tum/CPU-Benchmark/pull/16) ([L3tum](https://github.com/L3tum))
- Feature/add avx [\#14](https://github.com/L3tum/CPU-Benchmark/pull/14) ([L3tum](https://github.com/L3tum))
- Update Benchmark rating to support a wider range [\#13](https://github.com/L3tum/CPU-Benchmark/pull/13) ([L3tum](https://github.com/L3tum))
- Update Readme.md [\#12](https://github.com/L3tum/CPU-Benchmark/pull/12) ([L3tum](https://github.com/L3tum))
- Update runner to support multiple benchmarks [\#11](https://github.com/L3tum/CPU-Benchmark/pull/11) ([L3tum](https://github.com/L3tum))
- Add arithmetic to docs [\#9](https://github.com/L3tum/CPU-Benchmark/pull/9) ([L3tum](https://github.com/L3tum))
- Implement integer arithmetic benchmark [\#6](https://github.com/L3tum/CPU-Benchmark/pull/6) ([L3tum](https://github.com/L3tum))
- Update Readme.md [\#3](https://github.com/L3tum/CPU-Benchmark/pull/3) ([L3tum](https://github.com/L3tum))
- Update readme [\#2](https://github.com/L3tum/CPU-Benchmark/pull/2) ([L3tum](https://github.com/L3tum))



\* *This Change Log was automatically generated by [github_changelog_generator](https://github.com/skywinder/Github-Changelog-Generator)*