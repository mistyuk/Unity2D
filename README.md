
# Unity 2D Scripts Collection

## Overview
This repository contains a set of custom scripts designed for Unity Engine's 2D projects. These scripts have been tested and optimized for Unity version 2022.3.51f and Unity 6 engine, ensuring compatibility with all LTS versions. This collection includes scripts for managing audio, implementing parallax effects, handling player and enemy behavior, and more. 

Scripts are grouped into specific folders for clarity.

## Script Folders

### 1. Core Scripts
General-purpose scripts that are used throughout different parts of a 2D game.

- **BackgroundMusicController.cs**: Manages background music playback.
- **EnvironmentAudioMixer.cs**: Crossfades between two environmental audio sources.
- **EnvironmentalSoundController.cs**: Manages atmospheric sounds, wind, and animal calls.
- **FlexibleParallax.cs**: Provides parallax effects for background and foreground layers.
- **GlobalParallax.cs**: Similar to `FlexibleParallax`, but with added control for static main backgrounds.
- **PlayerAudioController.cs**: Handles audio feedback for player actions.

### 2. Player and Enemy Scripts
Scripts related to player and enemy behaviors.

- **PlayerController2D.cs**: A comprehensive player controller that handles movement, dashing, jumping, and attacking. *(Integration work may be required)*
- **SkeletonAI.cs**: Implements basic enemy AI behavior for patrolling, chasing, and attacking. *(Integration work may be required)*

### 3. Damage System (Folder: `/damagesystem/`)
Scripts that manage damage application and visualization in the game.

- **DamageCalculator.cs**: Calculates damage based on `DamageStats` and determines if it's a critical hit. *(Integration work may be required)*
- **DamageDealer.cs**: Handles applying damage to targets and visualizing it with damage text. *(Integration work may be required)*
- **DamageInfo.cs**: A struct that holds details about damage dealt.
- **DamageReceiver.cs**: An interface to be implemented by objects that receive damage.
- **DamageStats.cs**: A `ScriptableObject` containing parameters for damage calculations.
- **DamageText.cs**: Displays damage text when a target takes damage.

### 4. Managers (Folder: `/managers/`)
Scripts for managing game state, events, and custom tags.

- **EventManager.cs**: Central manager for handling and invoking game-wide events. *(Integration work may be required)*
- **GameManager.cs**: Oversees game state transitions and game initialization. *(Integration work may be required)*
- **LevelManager.cs**: Manages player progression, level-ups, and experience tracking. *(Integration work may be required)*
- **TagManager.cs**: Provides centralized management for custom tags used in the game.

### 5. Miscellaneous Utility Scripts
Additional scripts for specific gameplay functions.

- **GroundTagAssigner.cs**: Assigns tags to ground objects dynamically.
- **TagHolder.cs**: Holds custom tags for a `GameObject` and registers them at runtime.
- **EnemyAttackHitbox.cs**: Handles the enemy attack hitbox mechanics.
- **EnemySpawner.cs**: Spawns enemy prefabs at specified locations.
- **ObjectMover2D.cs**: Moves objects between waypoints with support for looping and ping-pong effects.
- **PlayerAttackHitbox.cs**: Manages the player's attack hitbox activation and damage application.

## Installation
1. Clone the repository to your Unity project:
   ```bash
   git clone https://github.com/yourusername/Unity2D-Scripts.git
   ```
2. Copy the desired scripts or folders into your project’s `Assets/Scripts` directory.
3. Configure and integrate the scripts into your scenes as needed.

## Notes on Integration
- **Managers**, **Damage System**, **PlayerController2D**, and **SkeletonAI** scripts may require integration work. This could include:
  - Assigning references in the Unity Inspector.
  - Connecting with other systems like UI, animation controllers, or custom game logic.
  - Adjusting certain properties or creating compatible `ScriptableObject` instances for use with the scripts.

## Contributions
Contributions are welcome! Feel free to submit pull requests, report issues, or suggest new features to improve the functionality of these scripts.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements
Special thanks to the Unity developer community for their support and contributions to the creation of these tools.
