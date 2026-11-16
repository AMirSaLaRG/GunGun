# Mehrdad_Gun_project


````````
## Rewarded Ads Setup Note

### Goal

Add a rewarded video advertisement that gives the player `1` `soldirCoin` after the video is watched until completion.

The reward can be claimed once every three hours.

### Unity setup

1. Install the Unity `Advertisement Legacy` package from `Window > Package Manager`.
2. Create a persistent `AdManager` GameObject in the first scene.
3. Configure the Android and iOS Unity Ads Game IDs.
4. Configure the rewarded ad unit IDs:
   - `Rewarded_Android`
   - `Rewarded_iOS`
5. Connect the panel button to:

   `AdManager.ShowRewardedAd()`

6. Keep Test Mode enabled during development.

### Reward behavior

- Grant the reward only when Unity reports:
  `UnityAdsShowCompletionState.Completed`
- Closing or skipping the advertisement gives no reward.
- A completed advertisement adds `1` to `PlayerData.soldirCoins`.
- Save the next available time in:
  `PlayerData.rewardedAdNextAvailableUtcTicks`
- Use a three-hour cooldown.
- Start the cooldown only after successful completion.
- Save the reward and cooldown through `SaveManager`.
- Allow the player to watch another rewarded advertisement after the cooldown expires.

### Google Play requirements

A Google Play Developer account is required to publish the Android game on Google Play. It is not required for local development or testing.

Before publishing:

- Create and configure the Android app in the Unity Ads Dashboard.
- Configure the Android package name.
- Use the production Unity Ads Game ID and rewarded ad unit ID.
- Disable Test Mode in the production build.
- Complete Google Play's Data Safety form.
- Declare advertising and collected device identifiers accurately.
- Add a privacy policy if required.
- Add consent handling for regions that require advertising consent.
- Test using Unity test mode or official test ad units.
- Never click production advertisements during testing.

Unity Ads configuration and Google Play publishing are separate:

- Unity Ads provides the advertisement service.
- Google Play distributes the Android application.

