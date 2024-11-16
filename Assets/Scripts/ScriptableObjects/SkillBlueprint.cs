using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SkillBlueprint", menuName = "Blueprints/SkillBlueprint")]
public class SkillBlueprint : ScriptableObject
{
    [Header("Skill Info")]
    [SerializeField] private Sprite skillSprite;
    [SerializeField] private string skillName;
    [SerializeField] private SkillType skillType;
    [SerializeField] private string description;
    [SerializeField] private List<SkillLevelData> skillLevelData;
    [SerializeField] private GameObject skillObject;


    public Sprite SkillSprite => skillSprite;
    public string SkillName => skillName;
    public SkillType SkillType => skillType;
    public string Description => description;
    public List<SkillLevelData> SkillLevelData => skillLevelData;
    public GameObject SkillObject => skillObject;
}
