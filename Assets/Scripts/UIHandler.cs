using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI pitchText, rollText;
  [SerializeField]
  private TextMeshProUGUI sliderPitchLabel, sliderRollLabel;
  [SerializeField]
  private Slider pitchSlider, rollSlider;

  private void Update()
  {
    SetPitchSlider();
    SetRollSlider();
  }

  public void UpdatePitch(float pitch)
  {
    pitchText.text = "Pitch: " + pitch.ToString("F2");
  }

  public void UpdateRoll(float roll)
  {
    rollText.text = "Roll: " + roll.ToString("F2");
  }

  public void SetPitchSlider()
  {
    sliderPitchLabel.text = "Pitch: " + pitchSlider.value.ToString("F2");
  }

  public void SetRollSlider()
  {
    sliderRollLabel.text = "Roll: " + rollSlider.value.ToString("F2");
  }
}
