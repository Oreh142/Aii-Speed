using UnityEngine;

public class Opponents : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public float speed;

    private void Update()
    {
        speed = FormatNumsHelper.FormatToUnitySpeed(gameManager.currentSpeed);
    }

    public void DestroyOpponents()
    {
        Destroy(gameObject);
    }
}