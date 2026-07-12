# Project Overview
- Game Title: Kedainesia
- High-Level Concept: 2D restaurant management and cooking simulation game focused on traditional Indonesian cuisine.
- Players: Single player
- Inspiration / Reference Games: Diner Dash, Cooking Mama
- Tone / Art Direction: 2D, Cultural, Traditional Indonesian
- Target Platform: Android
- Screen Orientation / Resolution: Landscape (likely 1920x1080)
- Render Pipeline: URP

# Game Mechanics
## Core Gameplay Loop
Players receive orders from customers, prepare traditional Indonesian dishes by selecting the correct ingredients in the stove UI, and serve the food before the customer's patience runs out.
## Controls and Input Methods
Touch-based drag and drop for ingredients and serving food. UI interactions for recipe selection and management.

# UI
Standard Unity UI (uGUI) with Canvas, Images, and TextMeshPro. Uses a drag-and-drop system for cooking and serving.

# Key Asset & Context
- **Fonts**: Excessive font files (112 total, 66 in `Resources/`) including web formats (`.eot`, `.woff`).
- **Textures**: Large 2048px backgrounds (`background full.png`) without Android-specific compression.
- **Audio**: Stereo BGM and SFX in WAV/Vorbis format.
- **Packages**: Unused packages like `com.unity.ai.inference` (Sentis) and `com.unity.visualscripting`.
- **Project Settings**: Managed Stripping set to `Minimal`, Minification disabled.

# Implementation Steps
## 1. Clean Up Unused Packages
- **Description**: Uninstall packages that are not used in the project to reduce build size and overhead.
- **Assigned role**: developer
- **Files**: `Packages/manifest.json`
- **Dependencies**: None
- **Parallelizable**: Yes

## 2. Optimize Font Assets & Resources Folder
- **Description**: Remove web-specific font formats (`.eot`, `.woff`) and unused TTF/OTF files. Move necessary fonts out of `Resources/` to prevent them from being forced into the APK regardless of scene usage. Use TMP SDF assets for text.
- **Assigned role**: developer
- **Files**: `Assets/TextMesh Pro/Resources/`, `Assets/Resources/`
- **Dependencies**: None
- **Parallelizable**: Yes

## 3. Adjust Player & Build Settings
- **Description**: Increase `Managed Stripping Level` to `Low` or `Medium`. Enable `Minify` for Release builds (R8). Ensure IL2CPP is targeting the required architectures (ARM64).
- **Assigned role**: developer
- **Files**: `ProjectSettings/ProjectSettings.asset`
- **Dependencies**: Step 1 & 2
- **Parallelizable**: No

## 4. Texture Compression & Overrides
- **Description**: Set Android-specific overrides for large textures. Use `ASTC` compression. Downscale textures that do not require high resolution (e.g., 2048px to 1024px or 512px where appropriate).
- **Assigned role**: developer
- **Files**: `Assets/ASSET FANTEAM KEDAINESIA/`, `Assets/Sprites/`
- **Dependencies**: None
- **Parallelizable**: Yes

## 5. Audio Optimization
- **Description**: Convert stereo audio clips to Mono using `Force To Mono`. Adjust Vorbis quality to ~70%. Ensure SFX are set to `Decompress On Load` or `Compressed In Memory` based on length.
- **Assigned role**: developer
- **Files**: `Assets/Audio/`, `Assets/Zacky/Assets/BGM & SFX/`
- **Dependencies**: None
- **Parallelizable**: Yes

## 6. Implement Sprite Atlasing
- **Description**: Create `SpriteAtlas` assets to group sprites together. This reduces draw calls and minimizes the impact of transparent padding in individual textures.
- **Assigned role**: developer
- **Files**: Various sprite assets
- **Dependencies**: Step 4
- **Parallelizable**: No

# Verification & Testing
- **Build Report**: Use the Unity Build Report (found in the Console after a build) to verify size reduction in each category (Textures, Scripts, Resources, etc.).
- **Visual Check**: Inspect UI and backgrounds in-game to ensure compression hasn't introduced noticeable artifacts.
- **Functional Check**: Ensure all text renders correctly after font cleanup and TMP SDF migration.
- **Stability Check**: Verify that `Managed Stripping` hasn't removed necessary code (check for runtime errors related to reflection or missing types).
