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
- [ ] [[WarningSign]] change enemies sign
- [ ] SFX of gun shot is feel off check how can fix it on rapid fire
- [ ] add combo bonus something like call of


## Mehrdad feedback

- [ ] [[02 - Asset/Hostage|Hostage]] change the prefab #art
- [ ] Leader board can insert name and be smaller
- [ ] add reward on combo


## Bugs

- [ ] [[01 - GDD/ RespawnBox| View box]] miss for target as edge cases should get handled


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
- [ ] - [ ] Add more [[01 - GDD/ Enemy|enemy]]
		- [x] [[01 - GDD/ Enemy/EnemyBasic|basic enemy]]
		- [x] [[EnemyHostageTaker|Hostage Taker]]
		- [ ] [[01 - GDD/ Enemy/ShieldedEnemy|Shielded enemy]]
		- [ ] [[01 - GDD/ Enemy/LuncherEnemy|Luncher enemy]]
		- [ ]
- [ ] - [ ] Add dynamic [[01 - GDD/ RespawnBox| Respawn Box]]  #art #code 
		- [ ] Handle the target by sequences
		- [ ] comes to the scene and get added the [[01 - GDD/ BoxManager|Box Manager]]
- [ ] - [ ] Create [[02 - Asset/Level1|level one asset]] #art #blender


## Done Before Comit

- [ ] add [[EnemyBasic|Shielded enemy]]


## Done

- [ ] Relocate the damage taking effects from [[01 - Gdd/UI/InGame|In Game]] to [[01 - GDD/UI/PlayerCanvas|player canvas]] and fix the not showing the sprite #code #art
	- [x] move elements
	- [x] fix the sprite
- [ ] Change [[EnemyHostageTaker| enemy hostage taker]] look from [[01 - GDD/ Enemy/EnemyBasic|enemy basic]]
- [ ] Big places should have more than one [[01 - GDD/ RespawnBox|respawn point]]
- [ ] - [ ] Create warning when a target want to shoot #code #art
		- [x] Get and design [[WarningSign|warning signs]]  for
			- [x] Target At Position
			- [x] Target leaving the scene
			- [x] Target shooting player
		- [x] Code [[TargetEntity| Target Script]] to Trigger signs
- [ ] [[01 - GDD/ BoxManager|Box Manager]]s [[01 - GDD/ RespawnBox|respawn box]]s [[01 - GDD/ activator|activator]] should not colider with targets while getting active
- [ ] On  empty gun shot combo get reset
- [ ] Remove bullet point on game ends ( resets)
- [ ] death effect is too messy make it simpler #art
- [ ] activator of box the logic is wrong
- [ ] after fight don't remove death effect
- [ ] on wave change close and open activators more phase change feel #code 
	- [x] when wave executed should wait until the scene is clear
	- [x] when scene is clear call for new waweRespawn
	- [x] chose new set of respawn boxes from all available boxes
	- [x] active ones get active or stay active and de active ones get de active or stay de active
	- [x] at end of activation respawns should get start
- [ ] Add path line to [[01 - GDD/ RespawnBox| Respawn Points]] 
	- [x] Gizmos Draw from the point to target point
	- [x] on editor can see
- [ ] enemy and hostage should not snap on each other 
	enemy and hostage now get stuck together while moving and never reach the distance


## In Progress



## fixed but not sure

- [ ] respawn manager don't get scene clear i think the problem should be on target feed back it do not trigger to check
	not happening again just adding a check for it at end of wawe
	added in [[WaveManager|wave Manager]] at end of last wave it get a(( trigger after two seconds ( magical number) that trigger end of level))




%% kanban:settings
```
{"kanban-plugin":"board","list-collapse":[false,false,false,false,false,true,true,false],"show-checkboxes":false,"tag-colors":[]}
```
%%