using Safari.Player;
using UnityEngine;
using UnityEngine.UI;

public class FillHealthBar : MonoBehaviour
{
    public Image fillImage;
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value <= slider.minValue)
        {
            fillImage.enabled = false;
        }
        else if (!fillImage.enabled)
        {
            fillImage.enabled = true;
        }

        float fillValue = (float)PlayerController.instance.CurrentHealth / (float)PlayerController.instance.maxHealth;
        slider.value = fillValue;
    }
}
