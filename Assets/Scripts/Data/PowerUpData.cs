using UnityEngine;

[System.Serializable]
public class PowerUpData
{
    public ESkillType powerUpSkill;
    [Range(0f, 1f)] public float powerUpChance;
    public Color myColor;
    
}
