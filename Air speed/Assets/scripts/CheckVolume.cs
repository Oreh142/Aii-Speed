using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckVolume : MonoBehaviour
{
    private AudioSource _audioSrc;
    private float _volume;
    [SerializeField] private string _saveVolumeKey;
    [SerializeField] private bool _inMenu;
    public void Awake()
    {
        _audioSrc = GetComponent<AudioSource>();
        if (_audioSrc != null)
        {
            if (PlayerPrefs.HasKey(_saveVolumeKey))
            {
                _volume = PlayerPrefs.GetFloat(_saveVolumeKey);
            }
            else
            {
                _volume = 1;
                PlayerPrefs.SetFloat(_saveVolumeKey, _volume);
            }
            _audioSrc.volume = _volume;
            if (_volume == 0 && !_inMenu)
            {
                Destroy(gameObject);
            }
        }
    }
}
