using UnityEngine;

public class TurnObject : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 720f;

    private void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}
