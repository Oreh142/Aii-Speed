using UnityEngine;

public class PlayerAngles : MonoBehaviour
{
    private float _angle;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        PlaneAngles();
    }

    private void PlaneAngles() //Многострадальные наклоны самолёта
    {
        if (Time.timeScale != 0)
        {
            if (!_gameManager.IsTheEnd())
            {
                if (Input.touchCount == 0 ^ transform.parent.position.y == 4.3f) //Если самолёт снижается
                {
                    if (_angle > 0)
                    {
                        ChangeAngles(-0.2f);
                    }
                }
                else if (Input.touchCount > 0) //Если самолёт набирает высоту
                {
                    if (_angle < 7)
                    {
                        ChangeAngles(0.2f);
                    }
                }
            }
            else
            {
                if (_angle > -10)
                {
                    ChangeAngles(-0.17f);
                } 
            }
            transform.eulerAngles = new Vector3(0, 0, _angle);
        }
    }

    private void ChangeAngles(float changedAngle)
    {
        _angle += changedAngle;
    }
}