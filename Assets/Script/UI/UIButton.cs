using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour
{
    [SerializeField] private UIType soundType = UIType.Button;
    [SerializeField] private bool overrideDefaultSound;

    private void Awake()
    {
        if (!overrideDefaultSound)
            soundType = UIType.Button;

        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    private void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        GameEventSystem.InvokePlaySound(soundType);
    }
}