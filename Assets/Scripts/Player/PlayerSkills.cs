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
    [SerializeField] private float aRDuration = 5f;
    [SerializeField] private float aRTimeScale = .5f;
    private Coroutine aRCo;
    private float reminderTime;
    

    private List<ESkillType> activateSkills = new List<ESkillType>();
    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateSkill(ESkillType skillType)
    {
        switch (skillType)
        {
            case ESkillType.AdrenalineRush:
                ActivateAdrenalineRush(aRDuration);
                break;

            default:
                Debug.Log("Skill Not Planted yet");
                break;
        }
    }

    [ContextMenu ("AdrenalineRushTest")]
    public void AdrenalineRushTest() => ActivateSkill(ESkillType.AdrenalineRush);
    private void ActivateAdrenalineRush(float duration)
    {
        if (activateSkills.Contains(ESkillType.AdrenalineRush))
        {
            StopCoroutine(aRCo);
            reminderTime = duration;

            aRCo = StartCoroutine(AdrenalineRushStoperCo());
            return;
        }

        GameManager.instance.ChangeTimescale(aRTimeScale);
        player.ScaleReloadTime(aRTimeScale);
        skillUi.onAdrenalineRUsh(true);
        
        
        activateSkills.Add(ESkillType.AdrenalineRush);

        reminderTime = duration;

        aRCo = StartCoroutine(AdrenalineRushStoperCo());


    }

    private void DeActivateAdrenalineRush()
    {
        GameManager.instance.ChangeTimescale(1);
        activateSkills.Remove(ESkillType.AdrenalineRush);
        player.ScaleReloadTime(1);
        skillUi.onAdrenalineRUsh(false);



    }

    private IEnumerator AdrenalineRushStoperCo()
    {            
        while (reminderTime > 0)
        {
            yield return new WaitForSecondsRealtime(.25f);
            reminderTime -= .25f;
            skillUi.UpdateAdrenalineRushTime(reminderTime, aRDuration);

        }
        DeActivateAdrenalineRush();
    }

    public void ResetSkills()
    {
        StopAllCoroutines();

        DeActivateAdrenalineRush();
    }

    public void OnResume()
    {
        if (reminderTime > 0)
            ActivateAdrenalineRush(reminderTime);

    }

    public void OnPause()
    {
        StopCoroutine(aRCo);
        DeActivateAdrenalineRush();
    }
}
