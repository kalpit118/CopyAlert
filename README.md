<div align="center">

<br/>

<!-- Logo / Banner -->
<img src="https://img.shields.io/badge/ClipAlert-7C3AED?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0id2hpdGUiPjxwYXRoIGQ9Ik0xOCAyaDItMnpNNiAyaDJ2MkgzYTEgMSAwIDAgMC0xIDF2MTZhMSAxIDAgMCAwIDEgMWgxNmExIDEgMCAwIDAgMS0xdi01aC0ydjRINFY2aDJ2Mkg2VjJ6bTEwIDB2Mmg0VjZoMlY0YTIgMiAwIDAgMC0yLTJoLTR6TTggOGg4djJIOFY4em0wIDRoOHYySDh2LTJ6bTAgNGg1djJIOHYtMnoiLz48L3N2Zz4=&logoColor=white" alt="ClipAlert" height="40"/>

<h1>ClipAlert</h1>

<p><strong>Lightweight Material UI copy notifications · Built for fun and to avoid pressing CTRL + C multiple times</strong></p>

<br/>

[![GitHub stars](https://img.shields.io/github/stars/kalpit118/ClipAlert?color=7C3AED&style=flat-square&logo=github)](https://github.com/kalpit118/ClipAlert/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/kalpit118/ClipAlert?color=9333EA&style=flat-square&logo=github)](https://github.com/kalpit118/ClipAlert/network)
[![GitHub issues](https://img.shields.io/github/issues/kalpit118/ClipAlert?color=A855F7&style=flat-square)](https://github.com/kalpit118/ClipAlert/issues)
[![License](https://img.shields.io/badge/license-GPL--3.0-7C3AED?style=flat-square)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-7C3AED.svg?style=flat-square)](https://github.com/kalpit118/ClipAlert/pulls)

<br/>

</div>

---

## ✦ Overview

**ClipAlert** is a lightweight, modern clipboard monitoring utility for Windows 10/11. Built for fun and productivity, it provides beautiful Material UI-styled notifications whenever you perform a **Copy** action. 

Ever found yourself pressing `CTRL + C` three times just to be sure? ClipAlert gives you the visual confirmation you need so you can copy once and move on.

Designed for developers and power users, ClipAlert stays out of your way in the system tray while providing real-time feedback for your copy interactions.

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

1. **Download**: Grab the latest release from the [Releases](https://github.com/kalpit118/ClipAlert/releases) page.
2. **Launch**: Run `ClipAlert.exe`.
3. **Configure**: Right-click the system tray icon and select **Settings** to customize the appearance and behavior.

### Build from Source

```powershell
# Clone the repository
git clone https://github.com/kalpit118/ClipAlert.git
cd ClipAlert

# Build using Visual Studio or dotnet CLI
dotnet build --configuration Release
```

---

## ✦ Settings Guide

ClipAlert is highly customizable through its dedicated settings panel:

- **Location**: Choose from **Bottom Right**, **Bottom Left**, **Top Right**, **Top Left**, or **Custom** (draggable).
- **Style**: Toggle between **Compact** for minimal distraction or **Full Card** for detailed info.
- **Animations**: Select between **Fade**, **Slide**, or **Smart** transition modes.
- **Auto-Start**: Keep ClipAlert active every time you boot your PC.
- **Visual Confirmation**: Never wonder "did it copy?" again.

---

## ✦ How It Works

```
System Clipboard Event
         │
         ▼
  ClipAlert Monitor
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
ClipAlert/
├── ClipAlert.csproj      # Project configuration
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

[![GitHub](https://img.shields.io/badge/GitHub-kalpit118-7C3AED?style=flat-square&logo=github)](https://github.com/kalpit118/ClipAlert)

<br/>

*If ClipAlert improved your workflow, give it a ⭐*

</div>
