using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class CarManager : Singleton<CarManager>
{
    private const string PlayerPrefsGearbox = "isPlayerCarManual";
    private const string PlayerPrefsAgentLevel = "AgentLevel";
    [SerializeField] private Track track;
    [field: SerializeField] public Car PlayerCar { get; private set; }
    [field: SerializeField] public CarAgent Npc { get; private set; }
    [SerializeField] private Transform dummyCar;
    [SerializeField] private List<NNModel> models = new List<NNModel>();
    private Car mainCar;
    public Car Car
    {
        get
        {
            return mainCar;
        }
        private set
        {
            speedometer.BeforeCarChange();
            mainCar = value;
            speedometer.AfterCarChange();
        }
    }
    public bool BlockInput { get; private set; }
    public event Action CarInputBlocked;
    public event Action CarInputUnblocked;
    [SerializeField] private Speedometer speedometer;
    [SerializeField] private LapTracker lapTracker;
    private void OnEnable()
    {
        track.TrackComputed += ResetDummyCar;
        GameManager.Instance.EnterEditState += HandleEnterEdit;
        GameManager.Instance.ExitEditState += HandleExitEdit;
        GameManager.Instance.EnterPlayState += HandleEnterPlay;
        GameManager.Instance.ExitPlayState += HandleExitPlay;
        GameManager.Instance.EnterPausedState += BlockCarInput;
        GameManager.Instance.ExitPausedState += UnblockCarInput;
        GameManager.Instance.RaceStart += HandleRaceStart;
    }
    private void OnDisable()
    {
        track.TrackComputed -= ResetDummyCar;
        GameManager.Instance.EnterEditState -= HandleEnterEdit;
        GameManager.Instance.ExitEditState -= HandleExitEdit;
        GameManager.Instance.EnterPlayState -= HandleEnterPlay;
        GameManager.Instance.ExitPlayState -= HandleExitPlay;
        GameManager.Instance.EnterPausedState -= BlockCarInput;
        GameManager.Instance.ExitPausedState -= UnblockCarInput;
        GameManager.Instance.RaceStart -= HandleRaceStart;
    }
    protected override void Awake()
    {
        base.Awake();
        Car = Npc.GetComponent<Car>();
    }
    private void Start()
    {
        LoadPlayerCarGearbox();
        dummyCar.gameObject.SetActive(false);
        PlayerCar.gameObject.SetActive(false);
    }

    private void Update()
    {
        // car.DebugInfo();
    }

    private void HandleEnterEdit()
    {
        dummyCar.gameObject.SetActive(true);
        ResetDummyCar();
    }
    private void HandleExitEdit()
    {
        dummyCar.gameObject.SetActive(false);
    }
    private void HandleEnterPlay()
    {
        PlayerCar.gameObject.SetActive(true);
        Car = PlayerCar;
        Npc.Show();
    }
    private void HandleRaceStart()
    {
        ResetCar(PlayerCar, false, false);
        PlayerCar.transform.position += PlayerCar.transform.right * 4f;
        Npc.EndEpisode();
        Npc.transform.position += Npc.transform.right * -4f;
    }
    private void HandleExitPlay()
    {
        PlayerCar.gameObject.SetActive(false);
        Car = Npc.Car;
        UnblockCarInput();
    }
    private void ResetDummyCar()
    {
        dummyCar.gameObject.SetActive(true);
        Transform spawnPoint = GetSpawnPoint(track.StartPiece);
        ResetCarPosition(dummyCar, spawnPoint);
        ResetCarDirection(dummyCar, spawnPoint);
    }
    public void ResetCar(Car car, bool hasRandomPosition, bool hasRandomDirection)
    {
        TrackPiece startPiece = hasRandomPosition ? track.GetRandomTrackPiece() : track.GetLastPiece();
        // TrackPiece startPiece = hasRandomPosition ? track.GetRandomTrackPiece() : track.StartPiece;
        if (startPiece == null)
        {
            car.gameObject.SetActive(false);
        }
        car.IdleStop();
        CheckpointManager.Instance.ResetProgress(car.transform);
        if (car == Car)
        {
            speedometer.ResetSpeed();
            lapTracker.ResetLapText();
            SaveDataManager.Instance.LoadObstaclesData();
        }
        Transform spawnPoint = GetSpawnPoint(startPiece);
        ResetCarPosition(car.transform, spawnPoint);
        if (hasRandomDirection)
        {
            RandomResetCarDirection(car.transform);
        }
        else
        {
            ResetCarDirection(car.transform, spawnPoint);
        }
        if (hasRandomPosition)
        {
            SetCarStartingCheckpointIndex(car.transform, startPiece);

        }
    }
    private void ResetCarPosition(Transform carTransform, Transform spawnPoint)
    {
        if (spawnPoint == null)
        {
            carTransform.gameObject.SetActive(false);
            return;
        }
        carTransform.position = spawnPoint.position;
    }
    private void ResetCarDirection(Transform carTransform, Transform spawnPoint)
    {
        carTransform.forward = spawnPoint ? spawnPoint.forward : carTransform.forward;
    }

    private void SetCarStartingCheckpointIndex(Transform carTransform, TrackPiece startPiece)
    {

        int nextCheckpointIndex = startPiece != null ? CheckpointManager.Instance.GetCheckpointIndex(startPiece.GetFirstCheckpoint()) : 0;
        CheckpointManager.Instance.Tracker[carTransform].NextCheckpointIndex = nextCheckpointIndex;
    }

    private void RandomResetCarDirection(Transform carTransform)
    {
        Vector3 randomDirection = UnityEngine.Random.onUnitSphere;
        carTransform.forward = new Vector3(randomDirection.x, carTransform.forward.y, randomDirection.z);
    }
    private Transform GetSpawnPoint(TrackPiece piece)
    {
        return piece ? piece.GetSpawnPoint() : null;
    }
    public void BlockCarInput()
    {
        if (BlockInput || GameManager.Instance.IsPreviousGameState(GameState.EDIT))
        {
            return;
        }
        BlockInput = true;
        CarInputBlocked?.Invoke();
    }
    public void UnblockCarInput()
    {
        if (GameManager.Instance.IsPreviousGameState(GameState.PLAY) || GameManager.Instance.IsPreviousGameState(GameState.EDIT))
        {
            return;
        }
        BlockInput = false;
        CarInputUnblocked?.Invoke();
    }
    public bool GetPlayerPrefsGearbox()
    {
        return PlayerPrefs.GetInt(PlayerPrefsGearbox, 0) != 0;
    }
    private void LoadPlayerCarGearbox()
    {
        PlayerCar.HasManualGearBox = GetPlayerPrefsGearbox();
    }
    public void ChangePlayerCarGearbox(bool isManual)
    {
        PlayerPrefs.SetInt(PlayerPrefsGearbox, isManual ? 1 : 0);
        PlayerCar.HasManualGearBox = isManual;
    }
    public void ChangeAgentTrainingLevel(int level)
    {
        PlayerPrefs.SetInt(PlayerPrefsAgentLevel, level);
        SetModel(level);
        Npc.EndEpisode();
    }
    public int GetPlayerPrefsAgentLevel()
    {
        return PlayerPrefs.GetInt(PlayerPrefsAgentLevel, 0);
    }
    public void LoadNpcLevel()
    {
        int level = GetPlayerPrefsAgentLevel();
        SetModel(level);
    }
    private void SetModel(int level)
    {
        if (level < 0)
        {
            return;
        }
        Npc.SetModel("Drive", models[level > models.Count - 1 ? 0 : level]);
    }
}
