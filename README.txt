LTJ Whiskey manual
version 1.1 Beniz
By SzikakA#3853

Description:
	Powerful rifle, capable of shooting down killdrones in one hit, but suffering from a poor magazine capacity and clumsy ergonomics

How to install:
	Copy the "Whiskey" folder into Receiver 2's gun mod directory (<Username>\AppData\LocalLow\Wolfire Games\Receiver2\Guns). Create the Guns folder if needed
	Copy the "WhiskeyPatch" folder into BepInEx plugins folder (<Game's directory>\BepInEx\plugins) if you don't have BepInEx, you can download it from its GitHub repository https://github.com/BepInEx/BepInEx
	Start the game, it should now contain an example loadout and a gun's enty in the spawnmenu
Controls:
	Shoot - default: LMB
	Rack the slide - default: R
	Press check - default: T+R
	Safety on/off - default: V
	Detach magazine - default: E
Features:
	Fully functional fictional rifle patterned after an AK and chambered for a custom .650 LTJ cardridge
	Capable of all basic firearm controls, also supports Stovepipe, Out of Battery and Failure to Feed malfunctions
	Magazine holds 4 rounds of custom ammunition, right now only standard capacity ones are available, with an extended version planned
	Cardridge spec:
		calliber: 16.51mm (0.65 in)
		shell length: 63.23mm (2.48 in)
		projectile mass: 40g (1.410958 oz)
		projectile speed: 350m/s (1148.294 fps)
		projectile energy: 2451J (1808ft.lbf)
Changes:
	Fixed an issue where low Gun Distance values would case the gun to clip into the camera
	Tweaked the sights alligment and made front sight more visible in darker areas
	Added support for failure to feed malfunctions
	Added custom level loading sprite
	Update: fixed an issue with colliders not working properly
