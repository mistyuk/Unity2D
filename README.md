Unity 2D Scripts Collection
Overview
This repository contains a set of custom scripts designed for Unity Engine's 2D projects. These scripts have been tested and optimized for Unity version 2022.3.51f and Unity 6 engine, ensuring compatibility with all LTS versions. This collection includes scripts for background music management, environmental audio control, parallax effects, and player audio handling to enhance 2D game development.

Scripts Included
1. BackgroundMusicController.cs
Manages background music playback with capabilities for playing, stopping, and fading out music over a specified duration. This script ensures a seamless audio experience by handling audio sources effectively.

Features:
Play and loop background music.
Stop currently playing music.
Fade out music with a customizable duration.
2. EnvironmentAudioMixer.cs
A crossfading audio controller that blends environmental audio between two sources, creating smooth transitions for dynamic soundscapes.

Features:
Crossfade between audio sources.
Customizable fade duration.
Supports seamless switching of environmental tracks.
3. EnvironmentalSoundController.cs
Controls various environmental sound effects, including atmospheric sounds, wind, and animal calls. This script manages audio clips with randomized intervals and looping capabilities.

Features:
Play looping and interval-based environmental sounds.
Customizable volume and play intervals.
Multi-source audio management.
4. PlayerAudioController.cs
Handles audio effects associated with player actions, such as jumping, landing, taking damage, and attacking. The script can be integrated with PlayerController2D for event-driven audio playback.

Features:
Custom sound settings for different actions.
Volume control and mixer group assignment for audio sources.
Footstep handling based on player velocity and state.
Randomized sound playback to prevent repetition.
5. FlexibleParallax.cs
Implements a parallax effect for background and foreground layers with customizable multipliers for the X and Y axes. The script ensures a smooth parallax experience that adjusts based on camera movement.

Features:
Layer grouping for independent background and foreground parallax.
Customizable depth and vertical shifts.
Option to control Z-axis movement.
Damping control for smoother transitions.
6. GlobalParallax.cs
Similar to FlexibleParallax.cs, this script provides parallax effects for multiple layers, with the added option to keep a main background layer static.

Features:
Advanced control over background and foreground layers.
Main background static option for better scene control.
Customizable parallax multipliers, depth steps, and vertical shifts.
Installation
Clone the repository to your Unity project:
bash
Copy code
git clone https://github.com/yourusername/Unity2D-Scripts.git
Copy the desired scripts into your project’s Assets/Scripts directory.
Assign and configure the scripts in your Unity scenes as needed.
Usage
BackgroundMusicController
Attach BackgroundMusicController.cs to a GameObject that holds an AudioSource. Set the AudioSource in the Inspector, and use the PlayMusic(), StopMusic(), and FadeOutMusic() methods to control the background music.

EnvironmentAudioMixer
Attach EnvironmentAudioMixer.cs to a GameObject and set two AudioSource components in the Inspector. Use PlayAndCrossfade() to switch and blend between audio clips smoothly.

EnvironmentalSoundController
Attach EnvironmentalSoundController.cs to a GameObject. Configure AudioSource components for atmosphere, wind, and animal sounds, and define clip settings through the Inspector to manage playback intervals and volume.

PlayerAudioController
Attach PlayerAudioController.cs to the player GameObject and link it to PlayerController2D. Assign audio clips and AudioSource components for each action, and set up audio mixer groups as needed.

FlexibleParallax & GlobalParallax
Add FlexibleParallax.cs or GlobalParallax.cs to a camera GameObject. Group your background and foreground layers, set parallax multipliers, and adjust the depthStep for Z-axis distribution.

Contributions
Contributions are welcome! Feel free to submit pull requests, report issues, or suggest new features to improve the functionality of these scripts.

License
This project is licensed under the MIT License. See the LICENSE file for details.

Acknowledgements
Special thanks to the Unity developer community for inspiration and support in creating these tools.
