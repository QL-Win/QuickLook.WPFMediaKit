# Changelog

All notable changes to QuickLook.WPFMediaKit are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added

- System Media Transport Controls integration on `MediaUriElement`. `EnableSystemMediaTransportControls` publishes playback to Windows media keys and the system media flyout. The integration does not reference WinRT SDK contract assemblies.

## [2.3.4] - 2026-05-10

### Added

- `EnableGlobalConfig`, so a graph can keep the process-wide LAV settings and skip per-graph LAV configuration.

### Changed

- LAV `SetRuntimeConfig` is applied before the other LAV setup for a graph.

## [2.3.3] - 2026-05-08

### Changed

- Default LAV hardware-acceleration settings were redesigned.

## [2.3.2] - 2026-05-08

### Added

- LAV video settings and hardware acceleration, including `EnableLAVHardwareAcceleration` and `ApplyHardwareAcceleration` for toggling acceleration on an open graph.
- PDB symbols and XML documentation in the NuGet package.

### Changed

- Solution files were replaced with `.slnx` manifests.
- Projects were converted to SDK-style, using a batch prepared from upstream for .NET 9. The package target stayed on .NET Framework 4.6.2.
- NuGet license and project links now point at this repository.
- LAV hardware acceleration is off unless the application turns it on.

### Removed

- The separate symbols NuGet package.

## [2.3.1] - 2025-06-09

### Fixed

- Generic theme resources failed to load.

## [2.3.0] - 2024-12-16

First package published as `QuickLook.WPFMediaKit`. It targets .NET Framework 4.6.2 and ships the EVR presenter binaries through a NuGet `.targets` file.

### Added

- DirectShow filters can be loaded from a local directory without COM registration, and a local copy is preferred over a registered filter.
- x64 build of the EVR presenter.
- `PlayerState` and `PlayerStateChanged` on the media player.
- `FrameStep` on `MediaUriElement`.
- `VideoFrameRate` on `MediaDetector`.
- `WPFMediaKitException`, used when DirectShow setup fails, including VMR9 creation and the case where D3D is unavailable.
- Logging around media setup (upstream #51).

### Changed

- Target framework is .NET Framework 4.6.2. An upstream move to 4.8 was merged and then rolled back to match QuickLook's dependencies.
- Volume is mapped on a logarithmic scale between silence and full volume.
- The EVR presenter links the C runtime statically.
- Minimum framework for the inherited upstream work in this range moved from .NET Framework 4.0 to 4.5.

### Fixed

- Opening a media file no longer keeps the file locked after playback (QuickLook #603).
- COM exceptions when closing a player or replacing its source.
- `HasVideo` was left unset on `MediaUriPlayer`.
- `MediaOpened` was raised when opening the file had failed.
- Playback position is reset to 0 when media opens (upstream #39).
- `VideoCaptureElement` stayed black when the D3D front buffer was unavailable.
- `WorkDispatcher` could run after shutdown, and its thread did not create a message queue before waiting.
- `DeeperColor` effect was not compiled as a resource (upstream #40).

## [2.2.0] - 2017-01-19

### Added

- Thread-safe copy of the current `D3DRenderer` video frame.
- NuGet package metadata for WPF MediaKit 2.2.0.

### Changed

- `MediaSeekingPlayer.SpeedRatio` is virtual.
- `D3DRenderer` frame cloning takes fewer locks. Image invalidation is posted with `BeginInvoke` and skipped when the render thread is still busy.

### Fixed

- `MediaUriPlayer` converted a file `Uri` to a string the file source could not open.
- Handle leak from creating the flush-done event twice.
- Compiler warnings.

## [2.1.0] - 2015-09-15

### Changed

- EVR is the default video renderer.
- The LAV Splitter Source is no longer hardcoded.
- DirectShowLib was updated.

### Fixed

- Memory leak in graph teardown.
- Video playback failed when no audio device was present.
- If the manually built DirectShow graph fails, playback falls back to letting DirectShow build the graph.

## [2.0.0] - 2015-06-19

This repository is a fork of [WPF MediaKit](https://github.com/Sascha-L/WPF-MediaKit). Version 2.0.0 is the first version in this history and is inherited from that project, not written for QuickLook. WPF MediaKit was originally created by Jeremiah Morrill and was previously hosted at `wpfmediakit.codeplex.com` before it moved to GitHub. Versions 2.0.0 through 2.2.0 record that upstream line. QuickLook-specific work is included starting with 2.3.0.

### Added

- The EVR presenter uses DXVA2 hardware acceleration (thanks to Siegfried Krüger).
- The DirectShow graph is built manually, with options to choose the video, audio, and splitter codecs.

### Changed

- The EVR presenter no longer requires the DirectX March 2009 runtimes (thanks to Siegfried Krüger).

[Unreleased]: https://github.com/QL-Win/QuickLook.WPFMediaKit/compare/2.3.4...HEAD
[2.3.4]: https://github.com/QL-Win/QuickLook.WPFMediaKit/compare/2.3.3...2.3.4
[2.3.3]: https://github.com/QL-Win/QuickLook.WPFMediaKit/compare/2.3.2...2.3.3
[2.3.2]: https://github.com/QL-Win/QuickLook.WPFMediaKit/compare/2.3.1...2.3.2
[2.3.1]: https://github.com/QL-Win/QuickLook.WPFMediaKit/compare/2.3.0...2.3.1
[2.3.0]: https://github.com/QL-Win/QuickLook.WPFMediaKit/compare/2.2.0...2.3.0
[2.2.0]: https://github.com/QL-Win/QuickLook.WPFMediaKit/compare/2.1.0...2.2.0
[2.1.0]: https://github.com/QL-Win/QuickLook.WPFMediaKit/compare/2.0.0...2.1.0
[2.0.0]: https://github.com/QL-Win/QuickLook.WPFMediaKit/releases/tag/2.0.0
