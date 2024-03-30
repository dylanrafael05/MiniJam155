using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIBar : MonoBehaviour
{
    [Header("Draw Settings")]
    [SerializeField] private Color filled;
    [SerializeField] private Color unfilled;

    private Image img;

    float filledAmount;
    Material mat;

    private void Awake()
    {
        img = GetComponent<Image>();

        mat = new Material(img.material);
        img.material = mat;
    }

    public float Fill 
    {
        get => filledAmount;
        set
        {
            mat.SetFloat("_Fill", value);
            filledAmount = value;
        }
    }

    public Color FilledColor
    {
        get => filled;
        set
        {
            mat.SetColor("_FilledColor", value);
            filled = value;
        }
    }

    public Color UnfilledColor
    {
        get => unfilled;
        set
        {
            mat.SetColor("_UnfilledColor", value);
            unfilled = value;
        }
    }
}
