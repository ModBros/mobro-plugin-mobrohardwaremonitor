# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 0.4.0 - 2024-11-11

### Added

- Included LibreHardwareMonitor Lib
- Return metrics for each individual CPU
- New metrics: GPU temperature, GPU core, GPU power, GPU memory load, CPU temperature, CPU power

### Changed

- Removed metrics: CPU clock

## 0.3.0 - 2024-03-15

### Changed

- Sanitize manufacturer name for CPUs and GPUs
- Sanitize CPU name
- Return metric value of -1 if CPU performance counters are not available instead of failing

### Fixed

- Clock speed metric values were not correctly converted to base unit

## 0.2.2 - 2023-08-21

### Added

- Metric for platform architecture

### Fixed

- OS name showing as Win10 on Win11

## 0.2.1 - 2023-07-27

### Changed

- Updated SDK

## 0.2.0 - 2023-07-03

### Added

- Added Program.cs to run and test the plugin locally
- Expose update frequency as settings
- Added top processes (by cpu or memory usage) as metrics

### Changed

- Updated SDK
- Do not publish .dll of SDK
- Set 'InvariantGlobalization' to true

## 0.1.0 - 2023-01-23

### Added

- Settings to enable/disable metrics of certain components

### Changed

- General optimizations, reduced memory allocations
- Updated to new SDK
- Updated to .NET 7

## 0.0.1 - 2022-10-17

### Added

- Initial release
