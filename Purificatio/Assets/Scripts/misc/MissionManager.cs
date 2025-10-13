using System;
using System.Collections.Generic;
using UnityEngine;

public enum MissionState { Inactive, Active, Completed }

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    private readonly Dictionary<string, MissionState> _missions =
        new Dictionary<string, MissionState>();

    public event Action<string> OnMissionCompleted;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartMission(string missionId)
    {
        _missions[missionId] = MissionState.Active;
        Debug.Log($"[MissionManager] Mission started: {missionId}");
    }

    public void CompleteMission(string missionId)
    {
        if (_missions.ContainsKey(missionId) &&
            _missions[missionId] == MissionState.Active)
        {
            _missions[missionId] = MissionState.Completed;
            Debug.Log($"[MissionManager] Mission completed: {missionId}");
            OnMissionCompleted?.Invoke(missionId);
        }
    }

    public bool IsCompleted(string missionId) =>
        _missions.TryGetValue(missionId, out var state) && state == MissionState.Completed;

    public bool IsActive(string missionId) =>
        _missions.TryGetValue(missionId, out var state) && state == MissionState.Active;
}