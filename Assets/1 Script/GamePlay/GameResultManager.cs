using System.Collections.Generic;
using UnityEngine;

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager Instance { get; private set; }

    private int killCount;
    private float survivalTime;
    private bool isGameOver;

    // 스킬 이름과 스킬 레벨 저장
    private readonly Dictionary<string, int> skillLevels
        = new Dictionary<string, int>();

    public int KillCount => killCount;
    public float SurvivalTime => survivalTime;
    public IReadOnlyDictionary<string, int> SkillLevels => skillLevels;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ResetResult();
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        survivalTime += Time.deltaTime;
    }

    public void AddKill()
    {
        killCount++;
    }

    public void SetSkillLevel(string skillName, int level)
    {
        if (string.IsNullOrEmpty(skillName))
        {
            return;
        }

        skillLevels[skillName] = level;
    }

    public void AddSkillLevel(string skillName)
    {
        if (string.IsNullOrEmpty(skillName))
        {
            return;
        }

        if (!skillLevels.ContainsKey(skillName))
        {
            skillLevels.Add(skillName, 1);
            return;
        }

        skillLevels[skillName]++;
    }

    public void StopRecord()
    {
        isGameOver = true;
    }

    public void ResetResult()
    {
        killCount = 0;
        survivalTime = 0f;
        isGameOver = false;
        skillLevels.Clear();
    }
}