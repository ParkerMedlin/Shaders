SSF Audio Uniforms
Synesthesia analyzes whatever music is playing in real-time, providing a large set of audio reactive uniforms. These uniforms are automatically updated every frame, making it easy to create reactive visuals without performing any of your own audio analysis.

Many of the uniforms rely on FFT analysis, splitting the audio spectrum into 4 main bands: Bass, Mid, MidHigh, and High.

Data Textures
There are two textures which store different audio data in each channel. Here is a visualization of these 1DSamplers from the built-in Audio Indicators scene:

Audio Samplers

syn_Spectrum stores FFT and audio waveform data. This is useful for creating audio reactive shapes and direct music visualizers.
r channel - Raw FFT. Unprocessed audio frequency data (first in visualization from bottom to top)
g channel - Juiced FFT. This data is processed to look much better than the Raw FFT -- this should be your default when working with FFT sampling. It incorporates bin smoothing, temporal smoothing, logarithmic frequency scaling, and high-end boosting to make the FFT look consistent and smooth across all frequencies (second in visualization)
b channel - Smooth FFT. This channel represents the FFT with much less frequency resolution, creating a smoother shape. It incoporates the same smoothing and scaling as the Juiced FFT (third in visualization)
a channel - Waveform. The raw audio waveform data, which changes every frame. (fourth in visualization)
syn_LevelTrail stores the level of frequency bands over time. This is useful for creating trails that react to music (top in visualization)
r channel - Whole Spectrum Level
g channel - Bass Level
b channel - Mid Level
a channel - High Level
Level
These uniforms track the level (loudness) of specific frequency bands, with some added smoothing to reduce jitter. They are normalized to the range 0.0 to 1.0.

syn_Level (whole spectrum)
syn_BassLevel
syn_MidLevel
syn_MidHighLevel
syn_HighLevel
Hits
These uniforms detect 'hits' within specific frequency bands, spiking in value during the isolated transients. Great for tracking drum hits or percussive action. They are normalized to the range 0.0 to 1.0.

syn_Hits (whole spectrum)
syn_BassHits
syn_MidHits
syn_MidHighHits
syn_HighHits
Time
These uniforms are clocks that move forward when the volume of a specific frequency band is high. Use these uniforms to make a scene move more when the music picks up, or to create pulsing movement (like a camera that moves forward on each kick). Note that these do not normalize between 0 and 1 and are similar scale to the TIME uniform.

syn_Time (whole spectrum)
syn_BassTime
syn_MidTime
syn_MidHighTime
syn_HighTime
SPECIAL: syn_CurvedTime (similar to syn_Time, but responds to volume increases with much higher acceleration)
Presence
These uniforms track the 'presence' of specific frequency bands, detecting rising or falling action without reacting to each individual sound. For example, syn_BassPresence will be high when "there is a significant bass presence in the song right now."

syn_Presence (whole spectrum)
syn_BassPresence
syn_MidPresence
syn_MidHighPresence
syn_HighPresence
Beat
These uniforms use beat detection to change a variety of variables on beat.

Uniform	Description
syn_OnBeat	Returns 1.0 immediately when a beat is detected, then quickly falls back to zero.
syn_ToggleOnBeat	Returns 0.0 until a beat is detected, then stays at 1.0 until another beat is detected, then toggles back to 0.0, etc. Smoothly transitions between values using a logistic curve.
syn_RandomOnBeat	Returns a new random float in the range 0.0 to 1.0 whenever a beat is detected. Smoothly transitions using a logistic curve.
syn_BeatTime	A clock that increments by 1.0 whenever a beat is detected.
BPM
These uniforms are separate from the preceding beat detection uniforms and give different results, as they are based on a continuous analysis of the song's BPM. Good for detecting a groove and moving with it consistently.

Uniform	Description
syn_BPM	The actual BPM, ranging from 50 to 220.
syn_BPMConfidence	Estimates the stability of the BPM in the current song. If the song is very consistent, this will be close to 1.0. If the BPM is shifting frequently, it will stay close to 0.0.
syn_BPMTwitcher	A clock that increments every beat. In contrast to syn_BeatTime, it exponentially "jumps" in value to the next beat, allowing smoother action in the scene. To get just a clock like syn_BeatTime, use floor(syn_BPMTwitcher). To get just the jittery action on each beat, use fract(syn_BPMTwitcher).
syn_BPMSin	A sine wave that oscillates between 0.0 and 1.0, with a frequency equal to the current BPM and a phase that is usually synced to the beat.
syn_BPMSin2	A sine wave that oscillates between 0.0 and 1.0, with a frequency equal to HALF of the current BPM for slower action.
syn_BPMSin4	A sine wave that oscillates between 0.0 and 1.0, with a frequency equal to ONE QUARTER of the current BPM for slower action.
syn_BPMTri	A triangle wave that oscillates between 0.0 and 1.0, with a frequency equal to the current BPM and a phase that is usually synced to the beat.
syn_BPMTri2	A triangle wave that oscillates between 0.0 and 1.0, with a frequency equal to HALF of the current BPM for slower action.
syn_BPMTri4	A triangle wave that oscillates between 0.0 and 1.0, with a frequency equal to ONE QUARTER of the current BPM for even slower action.
Large Features
Uniform	Description
syn_FadeInOut	Slowly rises to 1.0 as music starts, then slowly falls to 0.0 as the music ends. Great for fading in and out of a scene like a movie would.
syn_Intensity	Similar to syn_FadeInOut, but slowly accumulates to 1.0 depending on the intensity of the song.