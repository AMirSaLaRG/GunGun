---

kanban-plugin: board

---

## Backlog

- [ ] Add power up and bonus targets
	- [x] power ups the items that shooting them will activate ability
	- [ ] bonus item 
		- [ ] items with points
		- [ ] secret items 
		- [ ] achive items
	- [x] handle respawn and movement
	- [x] create visuals for it
- [ ] add some animation on building transform
- [ ] add bgm
	fix the setting sound section
- [ ] - [ ] Add dynamic [[RespawnBox| Respawn Box]]  #art #code 
		- [ ] Handle the target by sequences
		- [ ] comes to the scene and get added the [[01 - GDD/ BoxManager|Box Manager]]
- [ ] - [ ] Achivements 15/8 #art #code #ui 
		- [ ] kills
		- [ ] powerup
		- [ ] combo
- [ ] - [ ] Daily and weekly Tasks #art #code #ui 
		- [ ] In Game user Interface shows
- [ ] rework leader board
- [ ] make survival mode for first level
	- [ ] Create survival manager
	- [ ] Create survival difficulty
- [ ] write some history for game #writing
- [ ] need to put some reward on middle of level with time or wave
	updating reward manager #code
- [ ] Create Levels
	- [ ] Create Level 1
	- [ ] Create Level 2
	- [ ] Create Level 3
	- [ ] Create Level 4
	- [ ] Create Level 5
- [ ] - [ ] ObjectPool #code
- [ ] document the game
- [ ] - [ ] Shop 10/8 #art #code #ui
		- [x] weapons
		- [ ] enemy Prefab
		- [ ] enemy Warning Sign
		- [ ] hit sprites
- [ ] Add earning of honor


## Mehrdad feedback

- [ ] guns better to get upgrade over bought like 1 2 3
- [ ] add reward on combo
- [ ] go next kar nemikone chon yedoonas
- [ ] enemy shot fire and get some dramatic movie like 007 movies
- [ ] Leader board can insert name and be smaller
- [ ] add another currency that earned by different path like shooting a clow or box
- [ ] there may be a bug with  summon looks like caused by hostage
- [ ] add a roll shop to the game that get trigger by new currency
- [ ] currencies should be manage by how much play or chance and the item rewards
- [ ] tashkhis hostage sakhte
- [ ] hostage rahate yoho biad ke darsade eshtebah bere balatar
- [ ] too menu ye axe background bashe ghashang mishe
- [ ] doshmani ke maro koshte 007 moshakhas she
- [ ] dara ke baz mishe (bahal) vali az lahaze graphicy irad dare rang bandia doross nisst


## mehrdads done

- [ ] close the boxes and open boxes is better to be seperated
- [ ] zoom out kuchike
- [ ] pause menu
- [ ] retry menu
- [ ] resum
- [ ] spawn az kenar bashe !?
- [ ] need some threshold around to fucose in midle
- [ ] bad feeling it is on the air
- [ ] the question mark on the enemy is nice can add some noise to it


## To Do



## In Progress



## Done Before Comit

- [ ] Add Ability System to the Player.
	- [x] Adrenaline rush : time get slow 
	- [ ] Supper gun : Get a supper gun
	- [ ] Sniper Squad : helps to kill enemies.
	- [x] General watch : Get Double points
	- [x] extra shield
- [ ] when on phone it get minimize or out of main app should get pause
- [ ] - [x] Add more [[Enemy|enemy]]
		- [x] [[01 - GDD/ Enemy/EnemyBasic|basic enemy]]
		- [x] [[EnemyHostageTaker|Hostage Taker]]
		- [x] [[01 - GDD/ Enemy/ShieldedEnemy|Shielded enemy]]
		- [x] [[01 - GDD/ Enemy/LuncherEnemy|Luncher enemy]]


## Done

- [ ] remove horizental view it should has same ratio
- [ ] generate voices for game
	- [x] voice for combo
	- [x] voice for rewards
	- [x] voice for game start
	- [x] voice for wave change
	- [x] voice for combo breaks
- [ ] changed ui background added leader board to dynamic ui s
- [ ] rework ui compeletely
- [ ] guns should have upgrade like when they're bullet numbers goes up this ones will given to player as reward so they should not has reload time and switch time
- [ ] Loading building and wire up building manager
- [ ] Rework reward system
	- [x] it should be instance with out claim request
	- [x] add switch gun system to use burst fire should be instance as well
	- [x] penalty should be on drop combo
	- [x] Add some vfx for reward
	- [x] Add some sfx for reward
- [ ] create a panel line for data that get shows
- [ ] between waves for scene clear wait make combo drop fix that
- [ ] gun at start should be basic one
- [ ] when there is no more level go next get disabled
- [ ] add light to activators on top
- [ ] add reward for combo
	- [x] in game ui create as place for claim rewards #ui 
	- [x] need some reward manager #code
	- [x] i think score manager should handle rewards
- [ ] to each wave add string to display
- [ ] add penalty for hostage death to reset reward
- [ ] [[RespawnBox| View box]] miss for target as edge cases should get handled
- [ ] on try again a set of activator main door was closed but it was active
- [ ] design first level
- [ ] design second level
- [ ] added immunity icon on player canvas
- [ ] fix sprite don't get hidden when game restart or revive
- [ ] Create a standard frame for game fucose should be inside there
- [ ] added enemy world space ui
- [ ] added sprite as warning sign to get better render on phone
- [ ] add some text fwhen wave changes
- [ ] in game ui should get changes
	- [x] add some bar for data display
	- [x] make it smaller uses alot of space
	- [x] enemy image get changes
- [ ] make activator time by controlled by breath time in middle of game
- [ ] addd some colider in building for windows behind ones
- [ ] added noise to warning sign of enemy
- [ ] Refactor Game Manager
- [ ] Add shop Manager for pereches
- [ ] respawn box view tracker get buged should change with way points
	refactor respawn box
	refactor target move logic
	repair current respawn boxes
- [ ] some smoke vfx on hostage death
- [ ] restart has bug do not works on full open windows
- [ ] when a respawn box get empty it gave some time to respawn another one has a basic time or use body disapior added a fixed time and body disipier time get added to it
- [ ] when buy a weapon should get equiped
- [ ] Add pause btn ingame ui
	add pause function to game manager #code
- [ ] Hostage kill do not works
- [ ] make the magazine smaller
- [ ] when gun changes in pause it do not works on the resume
- [ ] Add Sky Box
- [ ] sat up the respawn boxes and activators for first level
- [ ] refactored the entity respawner
- [ ] Create and added meshes and texture for first level
- [ ] problem with the air hit
- [ ] Shield when get hit by multy shots get detected holder after first shot
- [ ] - [x] Refactor: Extract Gun System from [[PlayerController]] #code #refactor
		- [x] Create new script (GunController.cs / GunSystem.gd)
		- [x] Move shooting logic (Raycast/Projectiles) out of PlayerController
		- [x] Move ammo, reload, and fire-rate variables to new script
		- [x] Set up reference: PlayerController calls GunController.Shoot() on input
		- [x] Test shooting, reloading, and weapon switching in-engine
		- [x] switching function for gun 
		- [x] switching middle of game ( later can be use for power  up and etx
		- [x] switching before game starts need a user interface to use unlock weapons
		- [x] switching animation
- [ ] Gun Data and Gun manager.
	- [x] All controls of gun should be on one behavior on Player #code
	- [x] Create scriptable object or store the data of each gun on the gun manager #code
	- [x] Write data for Gun: #code
		- [x] Amo
		- [x] gun force
		- [x] shooting behavior
		- [x] reload time
		- [x] Price
		- [ ] unlock Requierments
	- [x] Create or download Asset for each gun #art
	- [x] Create user interface for gun selection #ui
- [ ] Add three shot gun to the game 
	- [x] create prefab #art
	- [x] write code for 3 shot
	- [x] handle the detection
	- [x] create guns
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


## Bugs

- [ ] fix bug of Retry i try it but do not happens again
- [ ] voice goes over each other should get fixed
- [ ] after retry
	spawn boxes get buged
	a set of respawn box of windows both items are de active the box is active the enemy spawned
- [ ] a bug of not reloading


## fixed but not sure

- [ ] respawn manager don't get scene clear i think the problem should be on target feed back it do not trigger to check
	not happening again just adding a check for it at end of wawe
	added in [[WaveManager|wave Manager]] at end of last wave it get a(( trigger after two seconds ( magical number) that trigger end of level))
- [ ] SFX of gun shot is feel off check how can fix it on rapid fire




%% kanban:settings
```
{"kanban-plugin":"board","list-collapse":[false,true,true,false,true,false,true,false,false],"show-checkboxes":false,"tag-colors":[]}
```
%%