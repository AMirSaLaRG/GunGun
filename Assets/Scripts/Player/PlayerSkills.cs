using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    PlayerController player;

    [Header("Skill Settings")]
    [SerializeField] private SkillsUi skillUi;
    [Space]
    [Header("AdrenalineRush")]
    [SerializeField] private float aRDuration = 10f;
    [SerializeField] private float aRTimeScale = .5f;
    [SerializeField] private GameObject adrenalineRushVfx;

    private Coroutine aRCo;
    private float reminderTimeAR;
    [Header("GeneralWatch")]
    [SerializeField] private float gWDuration = 10f;
    [SerializeField] private float additionalPointMultiplierByGw = 1f;
    [SerializeField] private GameObject generalWatchVfx;
    [Header("Shield")]
    [SerializeField] private GameObject shieldVfx;


    private Coroutine gWCo;
    private bool isGwActive;

    private List<ESkillType> activateSkills = new List<ESkillType>();
    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }
    public void ActivateSkill(ESkillType skillType)
    {
        switch (skillType)
        {
            case ESkillType.AdrenalineRush:
                ActivateAdrenalineRush(aRDuration);
                break;
            case ESkillType.GenralWatch:
                ActivateGeneralWatch(aRDuration);
                break;
            case ESkillType.Shielded:
                ActiveShield(true);
                break;

            default:
                Debug.Log("Skill Not Planted yet");
                break;
        }
    }

    [ContextMenu ("AdrenalineRushTest")]
    public void AdrenalineRushTest() => ActivateSkill(ESkillType.AdrenalineRush);
    [ContextMenu ("GeneralWatchTest")]
    public void GeneralWatchTest() => ActivateSkill(ESkillType.GenralWatch);
    private void ActivateAdrenalineRush(float duration)
    {
        if (activateSkills.Contains(ESkillType.AdrenalineRush))
        {
            StopCoroutine(aRCo);
            reminderTimeAR = duration;

            aRCo = StartCoroutine(AdrenalineRushStoperCo());
            return;
        }

        GameManager.instance.ChangeTimescale(aRTimeScale);
        player.ScaleReloadTime(aRTimeScale);
        skillUi.onAdrenalineRUsh(true);
        adrenalineRushVfx.SetActive(true);
        
        activateSkills.Add(ESkillType.AdrenalineRush);

        reminderTimeAR = duration;

        aRCo = StartCoroutine(AdrenalineRushStoperCo());


    }
    private void ActivateGeneralWatch(float duration)
    {
        if (activateSkills.Contains(ESkillType.GenralWatch))
        {
            StopCoroutine(gWCo);

            gWCo = StartCoroutine(GeneralWatchStoperCo());
            return;
        }

        player.scoreManager.AddBonusePointMuiltiplier(additionalPointMultiplierByGw);
        skillUi.onActiveGeneralWatch(true);
        isGwActive = true;
        generalWatchVfx.SetActive(true);



        activateSkills.Add(ESkillType.GenralWatch);

        gWCo = StartCoroutine(GeneralWatchStoperCo());


    }
    private void ActiveShield(bool enable)
    {
        if (activateSkills.Contains(ESkillType.GenralWatch))
            return;

        player.ActiveShield(enable);
        skillUi.onActiveShield(enable);
        shieldVfx.SetActive(enable);


        if (enable)
            activateSkills.Add(ESkillType.Shielded);
        else
            activateSkills.Remove(ESkillType.Shielded);

    }

    public void DeActiveShield() => ActiveShield(false);

    private void DeActivateAdrenalineRush()
    {
        GameManager.instance.ChangeTimescale(1);
        activateSkills.Remove(ESkillType.AdrenalineRush);
        player.ScaleReloadTime(1);
        skillUi.onAdrenalineRUsh(false);
        adrenalineRushVfx.SetActive(false);


    }
    private void DeActivateGeneralWatch()
    {
        if (isGwActive == false)
            return;
        isGwActive = false;
        player.scoreManager.AddBonusePointMuiltiplier(-additionalPointMultiplierByGw);
        activateSkills.Remove(ESkillType.GenralWatch);
        skillUi.onActiveGeneralWatch(false);
        generalWatchVfx.SetActive(false);

    }

    private IEnumerator AdrenalineRushStoperCo()
    {            
        while (reminderTimeAR > 0)
        {
            yield return new WaitForSecondsRealtime(.25f);
            reminderTimeAR -= .25f;
            skillUi.UpdateAdrenalineRushTime(reminderTimeAR, aRDuration);

        }
        DeActivateAdrenalineRush();
    }
    private IEnumerator GeneralWatchStoperCo()
    {
        float reminderTime = gWDuration;

        while (reminderTime > 0)
        {
            yield return new WaitForSeconds(.25f);
            reminderTime -= .25f;
            skillUi.UpdateGeneralWatchTime(reminderTime, aRDuration);

        }
        DeActivateGeneralWatch();
    }

    public void ResetSkills()
    {
        StopAllCoroutines();

        DeActivateAdrenalineRush();
        DeActivateGeneralWatch();
        ActiveShield(false);
    }

    public void OnResume()
    {
        if (reminderTimeAR > 0)
            ActivateAdrenalineRush(reminderTimeAR);

    }

    public void OnPause()
    {
        if (aRCo != null)
            StopCoroutine(aRCo);
        DeActivateAdrenalineRush();
    }
}
