using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
  // Public fields
  public Vector2 labelOffset;
  public bool hasControlOverPitchRoll;

  // Serialized fields
  [SerializeField]
  private TextMeshProUGUI pitchText, rollText;
  [SerializeField]
  private TextMeshProUGUI sliderPitchLabel, sliderRollLabel;
  [SerializeField]
  private Slider pitchSlider, rollSlider;
  [SerializeField]
  private List<RectTransform> cylinderPanels;
  [SerializeField]
  private List<TextMeshProUGUI> cylinderLabels;

  // Update is called once per frame
  private void Update()
  {
    // Update the pitch and roll slider labels
    SetPitchSlider();
    SetRollSlider();
  }

  // This method is responsible for updating the pitch label
  public void UpdatePitch(float pitch)
  {
    pitchText.text = "Pitch: " + pitch.ToString("F2");
  }

  // This method is responsible for updating the roll label
  public void UpdateRoll(float roll)
  {
    rollText.text = "Roll: " + roll.ToString("F2");
  }

  // This method is responsible for setting the label of the pitch slider
  public void SetPitchSlider()
  {
    if (hasControlOverPitchRoll)
    {
      sliderPitchLabel.text = "Pitch: " + pitchSlider.value.ToString("F2");
    }
  }

  // This method is responsible for setting the label of the roll slider
  public void SetRollSlider()
  {
    if (hasControlOverPitchRoll)
    {
      sliderRollLabel.text = "Roll: " + rollSlider.value.ToString("F2");
    }
  }

  // This method is responsible for updating the position of a cylinder of the given index
  public void SetCylinderPanelPosition(int index, Vector2 newPosition)
  {
    cylinderPanels[index].anchoredPosition = newPosition + labelOffset;
  }

  // This method is responsible for updating the label of a cylinder of the given index
  public void SetCylinderLabel(int index, float position, float speed)
  {
    cylinderLabels[index].text = $"{position.ToString("F2")} cm\n{speed.ToString("F2")} cm/s";
  }
}
