using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
  [SerializeField]
  private Vector2 platformSize;
  [SerializeField]
  private UIHandler uiHandler;
  [SerializeField]
  private List<Transform> cylinders;
  [SerializeField]
  private Transform platform;
  [SerializeField]
  private MeshFilter inversePlatformMeshFilter;
  [SerializeField]
  [Range(-45, 45f)]
  private float pitch, roll;
  [SerializeField]
  private float cylinderBaseY;

  private MeshFilter platformMeshFilter;
  private float platformPitch, platformRoll;
  private float xCenter, yCenter;

  private void Start()
  {
    platformMeshFilter = platform.GetComponent<MeshFilter>();
    xCenter = platformSize.x / 2;
    yCenter = platformSize.y / 2;
  }

  private void Update()
  {
    HandlePlatform();
    HandleCylinders();
    UpdateUI();
  }

  public void SetPitch(float pitch)
  {
    this.pitch = pitch;
  }

  public void SetRoll(float roll)
  {
    this.roll = roll;
  }

  private void HandlePlatform()
  {
    platform.position = GetAveragePosition();
    Vector3[] vertices = GetVertices();
    platformMeshFilter.mesh = CalculateMesh(vertices);
    inversePlatformMeshFilter.mesh = platformMeshFilter.mesh;
    CalculateAngles(vertices);
  }

  private void HandleCylinders()
  {
    for (int i = 0; i < 4; i++) {
      float yPos = cylinderBaseY;
      yPos += roll / 45 * xCenter * ((i % 2 == 0) ? -1 : 1);
      yPos += pitch / 45 * yCenter * ((i < 2) ? -1 : 1);
      float xPos = (i % 2 == 0) ? xCenter : -xCenter;
      float zPos = (i < 2) ? yCenter : -yCenter;
      cylinders[i].position = new Vector3(xPos, yPos, zPos);
    }
  }

  private void UpdateUI()
  {
    uiHandler.UpdatePitch(platformPitch);
    uiHandler.UpdateRoll(platformRoll);
  }

  private Vector3 GetAveragePosition()
  {
    Vector3 averagePosition = Vector3.zero;
    foreach (Transform cylinder in cylinders)
    {
      averagePosition += cylinder.position;
    }
    averagePosition /= cylinders.Count;
    return averagePosition;
  }

  private Mesh CalculateMesh(Vector3[] vertices)
  {
    Mesh mesh = new Mesh();
    mesh.vertices = vertices;
    mesh.triangles = new int[] { 0, 1, 2, 3, 4, 5 };
    return mesh;
  }

  private void CalculateAngles(Vector3[] vertices)
  {
    Vector3 normal1 = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]);
    Vector3 normal2 = Vector3.Cross(vertices[3] - vertices[0], vertices[5] - vertices[0]);

    Vector3 combinedNormal = normal1 + normal2;
    combinedNormal.Normalize();

    float dot = Vector3.Dot(normal1, normal2);
    if (dot > 0)
    {
      combinedNormal *= -1;
    }

    platformPitch = 90 - Vector3.Angle(combinedNormal, Vector3.forward);
    platformRoll = 90 - Vector3.Angle(combinedNormal, Vector3.right);
  }

  private Vector3[] GetVertices()
  {
    return new Vector3[] { cylinders[1].position, cylinders[0].position, cylinders[2].position, cylinders[1].position, cylinders[2].position, cylinders[3].position };
  }
}
