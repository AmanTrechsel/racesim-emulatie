using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
  // Serialized fields
  [SerializeField]
  private TextMeshProUGUI pitchText, rollText;
  [SerializeField]
  private TextMeshProUGUI sliderPitchLabel, sliderRollLabel;
  [SerializeField]
  private Slider pitchSlider, rollSlider;

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
    sliderPitchLabel.text = "Pitch: " + pitchSlider.value.ToString("F2");
  }

  // This method is responsible for setting the label of the roll slider
  public void SetRollSlider()
  {
    sliderRollLabel.text = "Roll: " + rollSlider.value.ToString("F2");
  }
}
