# v2.0.0
## Added
- Added ToF to target indicator for missiles, which also shows the time until detonation (ToD) when viewing a nuke
- Added transparency option for the HMD missile screen
- Added support for OA-27 Cavalier, F-22E King Raptor and AH-31 Kukri
- Added option to disable automatic switching to the newest fired missile
- Added a pitch ladder to the missile screen that indicates missile pitch
    - A simple pitch indicator can be enabled that is just a label showing the pitch in degrees displayed near the velocity vector
    - This can be toggled off if you wish to do so
    - The pitch ladder can be position to your liking
    - Degree labels can be hidden and only show the horizon
    - The horizon line width can be changed in the config

## Changed
- Mod config headings changed and tidied up a bit, expect some settings to get reset (most likely your element disabling/declutter and settings under the "Misc" heading)
- Symbology, such as the velocity vector and lead indicator, can now be hidden to help declutter elements to your own preferences
- Orientation Indicator is turned off by default now due to adding the pitch ladder to help declutter due to the added elements
- Missile camera roll will now stay up right when launching instead of using aircraft roll


## Fix(es)
- Fixed a bug that would brick the panel when respawning (I didn't know that the safe call operator doesn't work on unity objects)


