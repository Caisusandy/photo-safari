using UnityEngine;
using UnityEngine.UI;

public class FillHealthBar : MonoBehaviour
{
    public PlayerHealth playerHealth;
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

        float fillValue = (float)playerHealth.currentHealth / (float)playerHealth.maxHealth;
        slider.value = fillValue;
    }
}
