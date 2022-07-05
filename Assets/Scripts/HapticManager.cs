using UnityEngine;
using UnityEngine.UI;

public class HapticManager : MonoBehaviour
{

    public static HapticManager Instance;

    public enum HapticType
    {
        Warning,
        Failure,
        Success,
        Light,
        Medium,
        Heavy,
        Default,
        Vibrate,
        Selection
    }
    private void Awake()
    {
        Instance = this;
    }

    public void TriggerTaptic(HapticType type)
    {
        if (type == HapticType.Warning)
        {
            Taptic.Warning();
        }
        else if (type == HapticType.Failure)
        {
            Taptic.Failure();
        }
        else if (type == HapticType.Success)
        {
            Taptic.Success();
        }
        else if (type == HapticType.Light)
        {
            Taptic.Light();
        }
        else if (type ==HapticType.Medium)
        {
            Taptic.Medium();
        }
        else if (type ==HapticType.Heavy)
        {
            Taptic.Heavy();
        }
        else if (type == HapticType.Default)
        {
            Taptic.Default();
        }
        else if (type == HapticType.Vibrate)
        {
            Taptic.Vibrate();
        }
        else if (type == HapticType.Selection)
        {
            Taptic.Selection();
        }
    }


}