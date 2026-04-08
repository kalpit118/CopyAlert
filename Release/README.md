<div align="center">

<br/>

<!-- Logo / Banner -->
<img src="CopyAlert_Logo.png" alt="CopyAlert" height="120"/>

<h1>CopyAlert</h1>

<p><strong>Lightweight Material UI copy notifications · Built for fun and to avoid pressing CTRL + C multiple times</strong></p>

<br/>

[![GitHub stars](https://img.shields.io/github/stars/kalpit118/CopyAlert?color=7C3AED&style=flat-square&logo=github)](https://github.com/kalpit118/CopyAlert/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/kalpit118/CopyAlert?color=9333EA&style=flat-square&logo=github)](https://github.com/kalpit118/CopyAlert/network)
[![GitHub issues](https://img.shields.io/github/issues/kalpit118/CopyAlert?color=A855F7&style=flat-square)](https://github.com/kalpit118/CopyAlert/issues)
[![License](https://img.shields.io/badge/license-GPL--3.0-7C3AED?style=flat-square)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-7C3AED.svg?style=flat-square)](https://github.com/kalpit118/CopyAlert/pulls)

<br/>

</div>

---

## ✦ Overview

**CopyAlert** is a lightweight, modern clipboard monitoring utility for Windows. Built for fun and productivity, it provides beautiful Material UI-styled notifications whenever you perform a **Copy** action. 

Ever found yourself pressing `CTRL + C` three times just to be sure? CopyAlert gives you the visual confirmation you need so you can copy once and move on.

Designed for developers and power users, CopyAlert stays out of your way in the system tray while providing real-time feedback for your copy interactions.

---

## ✦ Features

| Feature | Description |
|---|---|
| 🎨 **Material UI** | Modern, clean notification cards with smooth animations |
| 📋 **Real-time Monitoring** | Instant alerts for every copy action |
| 📍 **Custom Locations** | Preset positions (Top/Bottom, Left/Right) or manual dragging |
| 🚀 **Auto Start** | Option to launch automatically with Windows |
| 🛸 **System Tray** | Runs silently in the background with a minimized footprint |
| ⚙️ **Smart Settings** | Toggle detailed info, switch styles, and choose animations |

---

## ✦ Getting Started

### Prerequisites

- **Windows 10** or **Windows 11**
- **.NET Desktop Runtime** (latest version recommended)

### Installation

<<<<<<< HEAD
1. **Install**: Right-click **`Setup.ps1`** and select **Run with PowerShell**. This will:
   - Install CopyAlert to your local apps folder.
   - Create a Desktop shortcut.
   - Create a Start Menu shortcut.
2. **Alternative**: You can also just run **`CopyAlert.exe`** directly if you prefer not to install it.
3. **Configure**: Right-click the system tray icon and select **Settings** to customize appearance and behavior.
=======
1. **Download**: Grab the latest release from the [Releases](https://github.com/kalpit118/ClipAlert/releases) page.
2. **Launch**: Run `CopyAlert.exe`.
3. **Configure**: Right-click the system tray icon and select **Settings** to customize the appearance and behavior.
>>>>>>> 884aef575463ac44f8d686edc5d74bcba80be308

### Build from Source

```powershell
# Clone the repository
<<<<<<< HEAD
git clone https://github.com/kalpit118/CopyAlert.git
=======
git clone https://github.com/kalpit118/ClipAlert.git
>>>>>>> 884aef575463ac44f8d686edc5d74bcba80be308
cd CopyAlert

# Build using Visual Studio or dotnet CLI
dotnet build --configuration Release
```

---

## ✦ Settings Guide

CopyAlert is highly customizable through its dedicated settings panel:

- **Location**: Choose from **Bottom Right**, **Bottom Left**, **Top Right**, **Top Left**, or **Custom** (draggable).
- **Style**: Toggle between **Compact** for minimal distraction or **Full Card** for detailed info.
- **Animations**: Select between **Fade**, **Slide**, or **Smart** transition modes.
- **Auto-Start**: Keep CopyAlert active every time you boot your PC.
- **Visual Confirmation**: Never wonder "did it copy?" again.

---

## ✦ How It Works

```
System Clipboard Event
         │
         ▼
  CopyAlert Monitor
         │
    ┌────┴────┐
    │ Action? │
    └────┬────┘
         │ Copy
         ▼
  Material UI Popup
  (Animated & Positioned)
```

---

## ✦ Project Structure

```
CopyAlert/
├── CopyAlert.csproj      # Project configuration
├── App.xaml              # Application entry point
├── MainWindow.xaml       # Main Notification UI
├── SettingsWindow.xaml   # Configuration UI
├── Settings.cs           # User preferences logic
├── ClipboardMonitor.cs   # Windows API hooks for clipboard
└── Release/              # Binary distribution
```

---

## ✦ Roadmap

- [x] Material UI Notification Cards
- [x] Custom Popup Positioning
- [x] System Tray Integration
- [x] Auto-start implementation
- [ ] Multiple Clipboard History
- [ ] Cloud Sync for Clipboard
- [ ] Dark/Light Theme Switching

---

## ✦ Contributing

Contributions are warmly welcome!

1. **Fork** the repository.
2. **Create** a feature branch (`git checkout -b feature/CoolFeature`).
3. **Commit** your changes (`git commit -m 'Add some CoolFeature'`).
4. **Push** to the branch (`git push origin feature/CoolFeature`).
5. **Open** a Pull Request.

---

## ✦ License

Distributed under the **GNU General Public License v3.0**. See [`LICENSE`](LICENSE) for more information.

---

<div align="center">

<br/>

Made with ♥ by [**Kalpit Jare**](https://github.com/kalpit118)

[![GitHub](https://img.shields.io/badge/GitHub-kalpit118-7C3AED?style=flat-square&logo=github)](https://github.com/kalpit118/CopyAlert)

<br/>

*If CopyAlert improved your workflow, give it a ⭐*

</div>
