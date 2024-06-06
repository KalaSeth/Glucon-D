using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController instance;

    [SerializeField] Transform targetPoint;

	[SerializeField] float moveSpeed;
	[SerializeField] float rotateSpeed;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		targetPoint = gameObject.transform.parent;
	}

	private void LateUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetPoint.rotation, rotateSpeed * Time.deltaTime);
	}
}
