using UnityEngine;
using UnityEngine.UI;

public class ChangeVolume : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSrc;
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _volumeText;
    [SerializeField] private string _textValue;
    [SerializeField] private string _saveVolumeKey;
    private float volume;

    public void Awake()
    {
        if (PlayerPrefs.HasKey(_saveVolumeKey))
        {
            volume = PlayerPrefs.GetFloat(_saveVolumeKey);
            _slider.value = volume;
            if (_audioSrc != null)
            {
                _audioSrc.volume = volume;
            }
        }
        else
        {
            volume = 1;
            _slider.value = volume;
            PlayerPrefs.SetFloat(_saveVolumeKey, volume);
            if (_audioSrc != null)
            {
                _audioSrc.volume = volume;
            }
        }
    }

    private void LateUpdate()
    {
        if (_slider.value != volume)
        {
            volume = _slider.value;
            PlayerPrefs.SetFloat(_saveVolumeKey, volume);
        }
        _volumeText.text = _textValue + Mathf.Round(volume * 100) + "%";
        if (_audioSrc != null)
        {
            _audioSrc.volume = volume;
        }
    }
}