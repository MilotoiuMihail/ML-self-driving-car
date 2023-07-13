using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : Singleton<CarManager>
{
    private const string PlayerPrefsGearbox = "isPlayerCarManual";
    [SerializeField] private Track track;
    [SerializeField] private Car playerCar;
    [SerializeField] private CarAgent npc;
    [SerializeField] private Transform dummyCar;
    public Car Car { get; private set; }
    public bool BlockInput { get; private set; }
    public event Action CarInputBlocked;
    public event Action CarInputUnblocked;
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
        GameManager.Instance.ExitViewState += HandleExitView;
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
        GameManager.Instance.ExitPlayState -= UnblockCarInput;
        GameManager.Instance.RaceStart -= HandleRaceStart;
        GameManager.Instance.ExitViewState -= HandleExitView;
    }
    protected override void Awake()
    {
        base.Awake();
        Car = npc.GetComponent<Car>();
    }
    private void Start()
    {
        LoadPlayerCarGearbox();
        dummyCar.gameObject.SetActive(false);
        playerCar.gameObject.SetActive(false);
        npc.gameObject.SetActive(true);
        // ResetCar(npc.transform);
        // speedometer.ChangeCar(npc.Car);
    }

    private void Update()
    {
        // car.DebugInfo();
    }

    private void HandleEnterEdit()
    {
        npc.gameObject.SetActive(false);
        dummyCar.gameObject.SetActive(true);
        ResetDummyCar();
    }
    private void HandleExitEdit()
    {
        dummyCar.gameObject.SetActive(false);
        npc.gameObject.SetActive(true);
        ResetCar(npc.Car);
    }
    private void HandleEnterPlay()
    {
        playerCar.gameObject.SetActive(true);
        Car = playerCar;
    }
    private void HandleRaceStart()
    {
        ResetCar(playerCar);
        playerCar.transform.position += playerCar.transform.right * 4f;
        ResetCar(npc.Car);
        npc.transform.position += npc.transform.right * -4f;
    }
    private void HandleExitView()
    {
        ResetCar(npc.Car);
        npc.transform.position += npc.transform.right * -4f;
    }
    private void HandleExitPlay()
    {
        playerCar.gameObject.SetActive(false);
        Car = npc.Car;
        ResetCar(npc.Car);
        UnblockCarInput();
    }
    public void ResetDummyCar()
    {
        dummyCar.gameObject.SetActive(true);
        Transform spawnPoint = GetSpawnPoint(track.StartPiece);
        ResetCarPosition(dummyCar, spawnPoint);
        ResetCarDirection(dummyCar, spawnPoint);
    }
    public void ResetCar(Car car)
    {
        car.StopCompletely();
        Transform spawnPoint = GetSpawnPoint(track.StartPiece);
        ResetCarPosition(car.transform, spawnPoint);
        ResetCarDirection(car.transform, spawnPoint);
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
    public void RandomResetCar(Car car)
    {
        TrackPiece startPiece = track.GetRandomTrackPiece();
        if (startPiece == null)
        {
            car.gameObject.SetActive(false);
            return;
        }
        car.StopCompletely();
        ResetCarPosition(car.transform, GetSpawnPoint(startPiece));
        ResetCarDirection(car.transform, GetSpawnPoint(startPiece));
        // RandomResetCarDirection(carTransform);
        SetCarStartingCheckpointIndex(car.transform, startPiece);
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
        if (BlockInput)
        {
            return;
        }
        BlockInput = true;
        CarInputBlocked?.Invoke();
    }
    public void UnblockCarInput()
    {
        if (GameManager.Instance.IsPreviousGameState(GameState.PLAY))
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
        playerCar.HasManualGearBox = GetPlayerPrefsGearbox();
    }
    public void ChangePlayerCarGearbox(bool isManual)
    {
        PlayerPrefs.SetInt(PlayerPrefsGearbox, isManual ? 1 : 0);
        playerCar.HasManualGearBox = isManual;
    }
}
