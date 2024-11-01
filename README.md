
# Unity 2D Scripts Collection

## Overview
This repository contains a set of custom scripts designed for Unity Engine's 2D projects. These scripts have been tested and optimized for Unity version 2022.3.51f and Unity 6 engine, ensuring compatibility with all LTS versions. This collection includes scripts for background music management, environmental audio control, parallax effects, and player audio handling to enhance 2D game development.

## Scripts Included

### 1. BackgroundMusicController.cs
Manages background music playback with capabilities for playing, stopping, and fading out music over a specified duration. This script ensures a seamless audio experience by handling audio sources effectively.
- **Features**:
  - Play and loop background music.
  - Stop currently playing music.
  - Fade out music with a customizable duration.

### 2. EnvironmentAudioMixer.cs
A crossfading audio controller that blends environmental audio between two sources, creating smooth transitions for dynamic soundscapes.
- **Features**:
  - Crossfade between audio sources.
  - Customizable fade duration.
  - Supports seamless switching of environmental tracks.

### 3. EnvironmentalSoundController.cs
Controls various environmental sound effects, including atmospheric sounds, wind, and animal calls. This script manages audio clips with randomized intervals and looping capabilities.
- **Features**:
  - Play looping and interval-based environmental sounds.
  - Customizable volume and play intervals.
  - Multi-source audio management.

### 4. PlayerAudioController.cs
Handles audio effects associated with player actions, such as jumping, landing, taking damage, and attacking. The script can be integrated with `PlayerController2D` for event-driven audio playback.
- **Features**:
  - Custom sound settings for different actions.
  - Volume control and mixer group assignment for audio sources.
  - Footstep handling based on player velocity and state.
  - Randomized sound playback to prevent repetition.

### 5. FlexibleParallax.cs
Implements a parallax effect for background and foreground layers with customizable multipliers for the X and Y axes. The script ensures a smooth parallax experience that adjusts based on camera movement.
- **Features**:
  - Layer grouping for independent background and foreground parallax.
  - Customizable depth and vertical shifts.
  - Option to control Z-axis movement.
  - Damping control for smoother transitions.

### 6. GlobalParallax.cs
Similar to `FlexibleParallax.cs`, this script provides parallax effects for multiple layers, with the added option to keep a main background layer static.
- **Features**:
  - Advanced control over background and foreground layers.
  - Main background static option for better scene control.
  - Customizable parallax multipliers, depth steps, and vertical shifts.

## Installation
1. Clone the repository to your Unity project:
   ```bash
   git clone https://github.com/yourusername/Unity2D-Scripts.git
   ```
2. Copy the desired scripts into your project’s `Assets/Scripts` directory.
3. Assign and configure the scripts in your Unity scenes as needed.

## Usage

### BackgroundMusicController
Attach `BackgroundMusicController.cs` to a GameObject that holds an `AudioSource`. Set the `AudioSource` in the Inspector, and use the `PlayMusic()`, `StopMusic()`, and `FadeOutMusic()` methods to control the background music.

### EnvironmentAudioMixer
Attach `EnvironmentAudioMixer.cs` to a GameObject and set two `AudioSource` components in the Inspector. Use `PlayAndCrossfade()` to switch and blend between audio clips smoothly.

### EnvironmentalSoundController
Attach `EnvironmentalSoundController.cs` to a GameObject. Configure `AudioSource` components for atmosphere, wind, and animal sounds, and define clip settings through the Inspector to manage playback intervals and volume.

### PlayerAudioController
Attach `PlayerAudioController.cs` to the player GameObject and link it to `PlayerController2D`. Assign audio clips and `AudioSource` components for each action, and set up audio mixer groups as needed.

### FlexibleParallax & GlobalParallax
Add `FlexibleParallax.cs` or `GlobalParallax.cs` to a camera GameObject. Group your background and foreground layers, set parallax multipliers, and adjust the `depthStep` for Z-axis distribution.

## Contributions
Contributions are welcome! Feel free to submit pull requests, report issues, or suggest new features to improve the functionality of these scripts.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements
Special thanks to the Unity developer community for inspiration and support in creating these tools.

### 7. GroundTagAssigner.cs
This script is responsible for dynamically assigning tags to different ground objects at runtime.
- **Features**:
  - Assigns tags to ground objects based on type (e.g., Hard, Soft, Slippery).
  - Supports adding multiple tags to a single object.

### 8. TagHolder.cs
Holds a list of custom tags for a GameObject and registers them with a tag manager at runtime.
- **Features**:
  - Stores custom tags in a list.
  - Integrates with a `TagManager` to register tags during the start phase.

### 9. EnemyAttackHitbox.cs
Handles the enemy attack mechanics, including hit detection and applying damage to the player.
- **Features**:
  - Triggers damage and knockback on player contact.
  - Supports critical hit mechanics based on a damage dealer's stats.

### 10. EnemySpawner.cs
A simple script for spawning enemy prefabs, such as skeletons, at specified points.
- **Features**:
  - Spawns enemies at the assigned spawn point or at the object's position.
  - Supports manual spawning triggered by a key press.

### 11. ObjectMover2D.cs
Controls object movement along predefined waypoints with support for looping and ping-pong effects.
- **Features**:
  - Configurable speed and pause duration between waypoints.
  - Supports linear and easing movement types.
  - Includes collision handling to parent/unparent the player for platforming mechanics.

### 12. PlayerAttackHitbox.cs
Manages the player's attack hitbox activation, damage application, and critical hit calculations.
- **Features**:
  - Configurable activation delay and active duration for the hitbox.
  - Detects and applies damage to enemies within the hitbox.

### 13. PlayerController2D.cs
Comprehensive 2D player controller script that handles movement, jumping, dashing, and attacks.
- **Features**:
  - Supports coyote time, jump buffering, and dash cooldown.
  - Integrates with various animation clips for character actions.
  - Includes a health system, experience tracking, and damage handling.
  - Provides crouch, fall, and landing mechanics with related animations.

### 14. SkeletonAI.cs
Implements basic AI behavior for a skeleton enemy, including patrolling, chasing, and attacking the player.
- **Features**:
  - State-based AI with support for idle, patrol, chase, attack, and hurt states.
  - Handles player detection, health management, and invincibility after being hit.
  - Supports animation control and event invocation for enemy death.

## Managers (Separate Folder)

### 1. EventManager.cs
A central script for managing and invoking game-wide events to ensure decoupled communication between different game systems.
- **Features**:
  - Provides methods to register, deregister, and trigger events.
  - Facilitates a robust event-driven architecture in Unity projects.

### 2. GameManager.cs
Manages the overall game state, including level transitions, game initialization, and game over conditions.
- **Features**:
  - Controls the flow of the game.
  - Manages game states and ensures proper initialization of game components.

### 3. LevelManager.cs
Handles level-specific functions, such as player progression, experience tracking, and level transitions.
- **Features**:
  - Tracks player experience and handles level-up logic.
  - Manages level progression and provides support for updating the HUD.

### 4. TagManager.cs
A utility script that provides centralized management for custom tags used throughout the game.
- **Features**:
  - Allows for dynamic tag assignment and checking.
  - Supports custom tag registration at runtime for various game objects.

## Damage System (Separate Folder)

### 1. DamageCalculator.cs
A static class that calculates damage and knockback force based on given `DamageStats`.
- **Features**:
  - Calculates random damage within a specified range.
  - Determines if the hit is critical and applies the appropriate multiplier.
  - Computes knockback force with randomness for varied gameplay dynamics.

### 2. DamageDealer.cs
Handles the application of damage to targets and displays damage text when a hit is registered.
- **Features**:
  - Randomized damage and critical hit checks.
  - Knockback force application on the target.
  - Instantiates damage text above targets to visualize the damage dealt.

### 3. DamageInfo.cs
A struct representing the details of damage dealt, including damage amount, critical hit status, and knockback properties.
- **Features**:
  - Bundles damage, critical multiplier, and knockback data for easy transfer and use.

### 4. DamageReceiver.cs
An interface to be implemented by any game object that should receive damage.
- **Features**:
  - Defines the `TakeDamage` method for applying `DamageInfo` to the implementing object.

### 5. DamageStats.cs
A `ScriptableObject` that holds parameters related to damage calculation, such as minimum/maximum damage, critical hit chances, and knockback forces.
- **Features**:
  - Customizable damage ranges and critical hit multipliers.
  - Configurable knockback forces for dynamic gameplay.

### 6. DamageText.cs
A component responsible for displaying and animating the damage text shown when a target takes damage.
- **Features**:
  - Displays critical hits with distinctive formatting and color.
  - Animates text to float and fade out over time.
  - Configurable movement direction and speed for visual variety.
