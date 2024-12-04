using Safari;
using TMPro;
using UnityEngine;

public class AnimalProgressController : MonoBehaviour
{
    public TextMeshProUGUI butterflyCountText;
    public TextMeshProUGUI capybaraCountText;
    public TextMeshProUGUI frogCountText;
    public TextMeshProUGUI jaguarCountText;

    void Update()
    {
        butterflyCountText.text = $"x{GameManager.instance.numButterfliesRequired}";
        capybaraCountText.text = $"x{GameManager.instance.numCapybarasRequired}";
        frogCountText.text = $"x{GameManager.instance.numFrogsRequired}";
        jaguarCountText.text = $"x{GameManager.instance.numJaguarsRequired}";
    }
}
