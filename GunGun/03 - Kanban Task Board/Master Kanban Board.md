---

kanban-plugin: board

---

## Backlog

- [ ] - [ ] Refactor: Extract Gun System from [[PlayerController]] #code #refactor
		- [ ] Create new script (GunController.cs / GunSystem.gd)
		- [ ] Move shooting logic (Raycast/Projectiles) out of PlayerController
		- [ ] Move ammo, reload, and fire-rate variables to new script
		- [ ] Set up reference: PlayerController calls GunController.Shoot() on input
		- [ ] Test shooting, reloading, and weapon switching in-engine


## Mehrdad feedback

- [ ] [[02 - Asset/Hostage|Hostage]] change the prefab #art


## Bugs



## To Do

- [ ] - [ ] Shop 10/8 #art #code #ui
		- [ ] weapons
		- [ ] enemy Prefab
		- [ ] enemy Warning Sign
		- [ ] hit sprites
- [ ] - [ ] Achivements 15/8 #art #code #ui 
		- [ ] kills
		- [ ] powerup
		- [ ] combo
- [ ] - [ ] Daily and weekly Tasks #art #code #ui 
		- [ ] In Game user Interface shows
- [ ] - [ ] ObjectPool #code
- [ ] - [ ] Lightning managment #art #render
- [ ] - [ ] Add dynamic [[01 - GDD/ RespawnBox| Respawn Box]]  #art #code 
		- [ ] Handle the target by sequences
		- [ ] comes to the scene and get added the [[01 - GDD/ BoxManager|Box Manager]]
- [ ] - [ ] Create [[02 - Asset/Level1|level one asset]] #art #blender


## In Progress



## Done

- [ ] - [ ] Create warning when a target want to shoot #code #art
		- [x] Get and design [[WarningSign|warning signs]]  for
			- [x] Target At Position
			- [x] Target leaving the scene
			- [x] Target shooting player
		- [x] Code [[TargetEntity| Target Script]] to Trigger signs
- [ ] [[01 - GDD/ BoxManager|Box Manager]]s [[01 - GDD/ RespawnBox|respawn box]]s [[01 - GDD/ activator|activator]] should not colider with targets while getting active
- [ ] On  empty gun shot combo get reset


## fixed but not sure

- [ ] respawn manager don't get scene clear i think the problem should be on target feed back it do not trigger to check
	not happening again just adding a check for it at end of wawe
	added in [[WaveManager|wave Manager]] at end of last wave it get a(( trigger after two seconds ( magical number) that trigger end of level))




%% kanban:settings
```
{"kanban-plugin":"board","list-collapse":[false,false,true,false,true,true,true],"show-checkboxes":false,"tag-colors":[]}
```
%%