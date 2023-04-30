using UnityEngine;

public class ChangeMusicInMenu : MonoBehaviour
{
    [SerializeField] private AudioClip[] _sounds;
    [SerializeField] private AudioSource _audio;

    public void ChangeMusic(int value)
    {
        switch (value)
        {
            case 0:
                _audio.clip = _sounds[0];
                PlayerPrefsSafe.SetInt("musicInMenuId", 0);
                break;

            case 1:
                _audio.clip = _sounds[1];
                PlayerPrefsSafe.SetInt("musicInMenuId", 1);
                break;
        }
        if (_audio.isActiveAndEnabled)
        {
            _audio.Play();
        }
    }

    private void Start()
    {
        ChangeMusic(PlayerPrefsSafe.GetInt("musicInMenuId"));
    }
}