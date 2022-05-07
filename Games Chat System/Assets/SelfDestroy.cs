using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField] private float destroyTime = 1.5f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
