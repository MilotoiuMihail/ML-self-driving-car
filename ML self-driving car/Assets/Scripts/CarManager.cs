using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : Singleton<CarManager>
{
    [SerializeField] private Track track;
    [SerializeField] private Car car;
    private CarInput carInput;
    [SerializeField] private TopDownCameraRig cameraRig;
    private void OnEnable()
    {
        track.StartPieceChanged += HandleStartPieceChanged;
        track.HasTrackDirectionChanged += HandleTrackDirectionChanged;
    }
    private void OnDisable()
    {
        track.StartPieceChanged -= HandleStartPieceChanged;
        track.HasTrackDirectionChanged -= HandleTrackDirectionChanged;
    }
    protected override void Awake()
    {
        base.Awake();
        carInput = car.GetComponent<CarInput>();
    }

    private void Update()
    {
        car.DebugInfo();
    }
    public void ResetCar(Transform carTransform)
    {
        Transform spawnPoint = GetSpawnPoint(track.StartPiece);
        ResetCarPosition(carTransform, spawnPoint);
        ResetCarDirection(carTransform, spawnPoint);
    }
    private void ResetCarPosition(Transform carTransform, Transform spawnPoint)
    {
        carTransform.position = spawnPoint ? spawnPoint.position : cameraRig.transform.position;
    }
    private void ResetCarDirection(Transform carTransform, Transform spawnPoint)
    {
        carTransform.forward = spawnPoint ? spawnPoint.forward : carTransform.forward;
    }
    public void RandomResetCar(Transform carTransform)
    {
        TrackPiece startPiece = track.GetRandomTrackPiece();
        ResetCarPosition(carTransform, GetSpawnPoint(startPiece));
        RandomResetCarDirection(carTransform);
        SetCarCurrentCheckpointIndex(carTransform, startPiece);
    }

    private void SetCarCurrentCheckpointIndex(Transform carTransform, TrackPiece startPiece)
    {

        int currentCheckpointIndex = startPiece.GetFirstCheckpointIndex();
        if (currentCheckpointIndex == CheckpointManager.Instance.GetCheckpointsCount() - 1)
        {
            currentCheckpointIndex = 0;
        }
        CheckpointManager.Instance.CheckpointTracker[carTransform] = currentCheckpointIndex;
    }

    private void RandomResetCarDirection(Transform carTransform)
    {
        Vector3 randomDirection = UnityEngine.Random.onUnitSphere;
        carTransform.forward = new Vector3(randomDirection.x, carTransform.forward.y, randomDirection.z);
    }
    private Transform GetSpawnPoint(TrackPiece piece)
    {
        return piece ? piece.GetSpawnPoint(track.IsTrackDirectionClockwise) : null;
    }
    private void HandleStartPieceChanged()
    {
        ResetCar(car.transform);
    }
    private void HandleTrackDirectionChanged(bool value)
    {
        ResetCar(car.transform);
    }
}
