using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] Slider hpBar;
    public void SetGage(float gage)
    {
        hpBar.value = gage;
    }
}
