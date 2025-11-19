using UnityEngine;
using Unity.Cinemachine;
public class CameraSpeedOffSet : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] PlayerMovement PlayerMovement;
    public CinemachineCamera vm;
    public float offsetStrenght=0.1f;
    private Vector3 newOffset;
    private CinemachinePositionComposer positionComposer;
    private void Start()
    {
        vm = GetComponent<CinemachineCamera>();
        positionComposer = vm.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;

    }
    private void Update()
    {

        if (playerRb != null)
        {
          newOffset = new Vector3(playerRb.linearVelocity.x * offsetStrenght, 
              playerRb.linearVelocity.y * offsetStrenght, 
              positionComposer.TargetOffset.z);
            positionComposer.TargetOffset = newOffset; 

        }
      
     //   Debug.Log($"Velocity: {playerRb.linearVelocity}, New Offset: {newOffset}, Camera Pos: {vm.transform.position}");
    }
}




