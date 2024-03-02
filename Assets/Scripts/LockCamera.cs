using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")]
public class LockCamera : CinemachineExtension
{
    public bool lockEnable = false;
    public Vector3 cameraOffset = Vector3.zero;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (lockEnable && stage == CinemachineCore.Stage.Body)
            state.RawPosition += cameraOffset;
    }
}