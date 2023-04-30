using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    private Vector2 _startPos = new Vector2(28.5f, 0);
    private Vector2 _finishPos = new Vector2(-28.5f, 0);
    [SerializeField] private float _speed;

    private void Update()
    {
        Moving();
    }

    private void Moving()
    {
        transform.position = Vector2.MoveTowards(transform.position, _finishPos, _speed * Time.deltaTime);
        if (transform.position.x == _finishPos.x)
        {
            transform.position = _startPos;
        }
    }
}