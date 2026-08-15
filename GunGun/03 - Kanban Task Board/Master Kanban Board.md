---

kanban-plugin: board

---

## Backlog

- [ ] - [ ] Refactor: Extract Gun System from [[PlayerController]] #code #refactor
		- [x] Create new script (GunController.cs / GunSystem.gd)
		- [x] Move shooting logic (Raycast/Projectiles) out of PlayerController
		- [x] Move ammo, reload, and fire-rate variables to new script
		- [x] Set up reference: PlayerController calls GunController.Shoot() on input
		- [ ] Test shooting, reloading, and weapon switching in-engine
		- [x] switching function for gun 
		- [x] switching middle of game ( later can be use for power  up and etx
		- [ ] switching before game starts need a user interface to use unlock weapons
		- [ ] switching animation
- [ ] Add Enemy projectiles to the targets
- [ ] SFX of gun shot is feel off check how can fix it on rapid fire
- [ ] add combo bonus something like call of
- [ ] Create [[Enemy|enemy launcher]]
- [ ] Add Ability System to the Player.
	- [ ] Adrenaline rush : time get slow 
	- [ ] Supper gun : Get a supper gun
	- [ ] Sniper Squad : helps to kill enemies.
	- [ ] General watch : Get Double points
- [ ] Gun Data and Gun manager.
	- [ ] All controls of gun should be on one behavior on Player #code
	- [ ] Create scriptable object or store the data of each gun on the gun manager #code
	- [ ] Write data for Gun: #code
		- [ ] Amo
		- [ ] gun force
		- [ ] shooting behavior
		- [ ] reload time
		- [ ] Price
		- [ ] unlock Requierments
	- [ ] Create or download Asset for each gun #art
	- [ ] Create user interface for gun selection #ui
- [ ] Add power up and bonus targets
	- [ ] power ups the items that shooting them will activate ability
	- [ ] bonus item 
		- [ ] items with points
		- [ ] secret items 
		- [ ] achive items
	- [ ] handle respawn and movement
	- [ ] create visuals for it
- [ ] make activator time by controlled by breath time in middle of game


## Mehrdad feedback

- [ ] Leader board can insert name and be smaller
- [ ] add reward on combo


## Bugs

- [ ] [[RespawnBox| View box]] miss for target as edge cases should get handled


## In Progress



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
- [ ] - [ ] Add more [[Enemy|enemy]]
		- [x] [[01 - GDD/ Enemy/EnemyBasic|basic enemy]]
		- [x] [[EnemyHostageTaker|Hostage Taker]]
		- [x] [[01 - GDD/ Enemy/ShieldedEnemy|Shielded enemy]]
		- [ ] [[01 - GDD/ Enemy/LuncherEnemy|Luncher enemy]]
		- [ ]
- [ ] - [ ] Add dynamic [[RespawnBox| Respawn Box]]  #art #code 
		- [ ] Handle the target by sequences
		- [ ] comes to the scene and get added the [[01 - GDD/ BoxManager|Box Manager]]
- [ ] - [ ] Create [[02 - Asset/Level1|level one asset]] #art #blender


## Done Before Comit



## Done

- [ ] [[WarningSign]] change enemies sign
- [ ] [[02 - Asset/Hostage|Hostage]] change the prefab #art
- [ ] Relocate the damage taking effects from [[01 - Gdd/UI/InGame|In Game]] to [[01 - GDD/UI/PlayerCanvas|player canvas]] and fix the not showing the sprite #code #art
	- [x] move elements
	- [x] fix the sprite
- [ ] add [[EnemyBasic|Shielded enemy]]
- [ ] Change [[EnemyHostageTaker| enemy hostage taker]] look from [[01 - GDD/ Enemy/EnemyBasic|enemy basic]]
- [ ] Big places should have more than one [[RespawnBox|respawn point]]
- [ ] Change the sfx when someone take damage will get sound of sfx on hit not from hit only miss can be there
- [ ] - [ ] Create warning when a target want to shoot #code #art
		- [x] Get and design [[WarningSign|warning signs]]  for
			- [x] Target At Position
			- [x] Target leaving the scene
			- [x] Target shooting player
		- [x] Code [[TargetEntity| Target Script]] to Trigger signs
- [ ] [[01 - GDD/ BoxManager|Box Manager]]s [[RespawnBox|respawn box]]s [[01 - GDD/ activator|activator]] should not colider with targets while getting active
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
- [ ] Add path line to [[RespawnBox| Respawn Points]] 
	- [x] Gizmos Draw from the point to target point
	- [x] on editor can see
- [ ] enemy and hostage should not snap on each other 
	enemy and hostage now get stuck together while moving and never reach the distance


## fixed but not sure

- [ ] respawn manager don't get scene clear i think the problem should be on target feed back it do not trigger to check
	not happening again just adding a check for it at end of wawe
	added in [[WaveManager|wave Manager]] at end of last wave it get a(( trigger after two seconds ( magical number) that trigger end of level))




%% kanban:settings
```
{"kanban-plugin":"board","list-collapse":[false,false,false,false,false,false,false,false],"show-checkboxes":false,"tag-colors":[]}
```
%%