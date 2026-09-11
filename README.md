# Pac-Man Clone (Unity 2D)

A modern 2D Pac-Man recreation built in Unity, focusing on clean code architecture, scalable design patterns, and classic arcade mechanics.

---

## 📱 Download APK

* ⬇️ **[Download BoraBaydarAgaveCase.apk](https://github.com/itu-itis25-baydarb21/BoraBaydar/releases/latest/download/BoraBaydarAgaveCase.apk)**
* 📦 **[View Release Notes (Pac-man Case Android v1.0)](https://github.com/itu-itis25-baydarb21/BoraBaydar/releases/latest)**

---

## 🕹️ Overview

This project rebuilds the classic arcade experience from the ground up using modern software engineering practices. Key highlights include a decoupled State Pattern for ghost AI, centralized gameplay configurations, responsive grid-based movement, synchronized animation states, and dynamic camera framing.

---

## 🚀 Key Features

* **State Pattern Ghost AI:**
  * `InHouseState`: Ghosts oscillate vertically inside the home area while awaiting their release timers.
  * `JoiningGameState`: Paths and aligns ghosts with the gate coordinate (`JoinGameCell`) before entry into the maze.
  * `ScatterState`: Navigates grid intersections using randomized non-reversing directions while scanning line of sight (LoS).
  * `ChaseState`: Pursues Pac-Man using Manhattan-distance path selection at intersections; returns to scatter mode upon line-of-sight loss or timeout.
* **Directional Ghost Eye Rotation:** Rotates eye transforms in real-time to reflect movement vectors (`GridDirection`).
* **Precise Grid Movement & Animator Freezing:** Pac-Man stops immediately upon hitting walls; pauses the mouth animation at the exact current frame and resumes seamlessly upon movement.
* **Centralized Configuration (`GameSettings`):** Eliminates magic numbers by consolidating speeds, release delays, vision ranges, arrival tolerances, and camera padding into a single static class.
* **Dynamic Orthographic Camera:** Calculates maze bounds and aspect ratio at startup to frame any grid layout without edge clipping.
* **Optimized Multi-Source Audio:**
  * Uses 16-bit PCM uncompressed WAV assets for zero-latency playback.
  * Game siren halts until intro theme completion.
  * Independent loop channel for Pac-Man's "waka-waka" that stops instantly when stationary.
  * Contextual siren switching to alarm audio when chase mode triggers.

---

## 🛠️ Architecture & Project Structure

```text
Assets/
├── Scripts/
│   ├── AiStates/
│   │   ├── GhostState.cs           # Abstract base class for ghost states
│   │   ├── InHouseState.cs         # Home bounce and timer logic
│   │   ├── JoiningGameState.cs     # Gate exit and maze alignment
│   │   ├── ScatterState.cs         # Free roaming and LoS detection
│   │   └── ChaseState.cs           # Target pursuit and abort conditions
│   ├── AudioManager.cs             # Multi-channel audio controller
│   ├── GameSettings.cs             # Centralized gameplay constants
│   ├── GameManager.cs              # Game loop, ghost orchestration, camera setup
│   ├── Ghost.cs                    # Ghost controller and eye rotation
│   ├── GhostBlackboard.cs          # Shared state context and dependencies
│   └── Pacman.cs                   # Input, grid traversal, animator control
└── Audio/                          # Low-latency 16-bit PCM sound effects
