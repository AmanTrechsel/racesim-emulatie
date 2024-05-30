using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    // Public variables
    public float cFactor;
    public bool cylinderOverride;

    // Serialized fields
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
    [Range(-90, 90)]
    private float pitch, roll;
    [SerializeField]
    private float cylinderBaseY;

    // Private variables
    private MeshFilter platformMeshFilter;
    private float platformPitch, platformRoll;
    private float xCenter, yCenter;
    private Vector3 normal;

    private enum VectorAxis
    {
      X,
      Y,
      Z
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Get the mesh filter of the platform
        platformMeshFilter = platform.GetComponent<MeshFilter>();

        // Calculate the center of the platform
        xCenter = platformSize.x / 2;
        yCenter = platformSize.y / 2;
    }

    // Update is called once per frame
    private void Update()
    {
        // Handle the platform and cylinders
        HandlePlatform();

        if (!cylinderOverride)
        {
          HandleCylinders();
        }

        // Update the UI
        UpdateUI();
    }

    // This method is responsible for drawing the gizmos
    private void OnDrawGizmos()
    {
        // Draw the normal of the platform
        Gizmos.color = Color.green;
        Gizmos.DrawRay(platform.position, normal * 10);
    }

    // This method is responsible for setting the pitch of the platform
    public void SetPitch(float pitch)
    {
        this.pitch = pitch;
    }

    // This method is responsible for setting the roll of the platform
    public void SetRoll(float roll)
    {
        this.roll = roll;
    }

    // This method is responsible for positioning the platform
    private void HandlePlatform()
    {
        // Set the position of the platform
        platform.position = GetAveragePosition();

        // Set the mesh of the platform
        Vector3[] vertices = GetVertices();
        platformMeshFilter.mesh = CalculateMesh(vertices);
        inversePlatformMeshFilter.mesh = platformMeshFilter.mesh;

        // Calculate the angles of the platform
        CalculateAngles(vertices);
    }

    // This method is responsible for positioning the cylinders
    private void HandleCylinders()
    {
        // Iterate through each cylinder
        for (int i = 0; i < 4; i++)
        {
            // Base position of the cylinder
            Vector3 basePosition = new Vector3((i % 2 == 0) ? xCenter : -xCenter, cylinderBaseY, (i < 2) ? yCenter : -yCenter);

            // Calculate the offset due to pitch and roll
            Vector3 offset = RotateVector(basePosition, pitch, VectorAxis.X) - basePosition;
            offset += RotateVector(basePosition, -roll, VectorAxis.Z) - basePosition;

            // Set the position of the cylinder with the offset
            cylinders[i].position = basePosition + new Vector3(0, offset.y, 0);
        }
    }

    // This method is responsible for updating the UI
    private void UpdateUI()
    {
        uiHandler.UpdatePitch(platformPitch);
        uiHandler.UpdateRoll(platformRoll);
    }

    // Applies a rotation on a vector on the given axis
    private Vector3 RotateVector(Vector3 baseVector, float degrees, VectorAxis axis)
    {
        // Convert to Radians
        float radians = degrees * Mathf.Deg2Rad;

        // Get Cos and Sin
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        
        // Get the rotation matrix based on input
        float[,] rotationMatrix = new float[0,0];

        switch (axis)
        {
            case (VectorAxis.X):
                rotationMatrix = new float[,]
                {
                    { 1, 0, 0 },
                    { 0, cos, -sin },
                    { 0, sin, cos }
                };
                break;
            case (VectorAxis.Y):
                rotationMatrix = new float[,]
                {
                    { cos, 0, sin },
                    { 0, 1, 0 },
                    { -sin, 0, cos }
                };
                break;
            case (VectorAxis.Z):
                rotationMatrix = new float[,]
                {
                    { cos, -sin, 0 },
                    { sin, cos, 0 },
                    { 0, 0, 1 }
                };
                break;
        }
        
        // Apply the rotation matrix to the base vector and return
        return new Vector3(
            rotationMatrix[0, 0] * baseVector.x + rotationMatrix[0, 1] * baseVector.y + rotationMatrix[0, 2] * baseVector.z,
            rotationMatrix[1, 0] * baseVector.x + rotationMatrix[1, 1] * baseVector.y + rotationMatrix[1, 2] * baseVector.z,
            rotationMatrix[2, 0] * baseVector.x + rotationMatrix[2, 1] * baseVector.y + rotationMatrix[2, 2] * baseVector.z
        );
    }

    // This method calculates the average position of the cylinders
    private Vector3 GetAveragePosition()
    {
        // Initialize the average position
        Vector3 averagePosition = Vector3.zero;

        // Iterate through each cylinder
        foreach (Transform cylinder in cylinders)
        {
            averagePosition += cylinder.position;
        }

        // Calculate the average position
        averagePosition /= cylinders.Count;

        // Return the average position
        return averagePosition;
    }

    // This method calculates the mesh of the platform
    private Mesh CalculateMesh(Vector3[] vertices)
    {
        // Create a new mesh
        Mesh mesh = new Mesh();

        // Set the vertices and triangles of the mesh
        mesh.vertices = vertices;
        mesh.triangles = new int[] { 0, 1, 2, 3, 4, 5 };

        // Calculate the normals of the mesh
        mesh.RecalculateNormals();

        // Return the mesh
        return mesh;
    }

    // This method calculates the angles of the platform
    private void CalculateAngles(Vector3[] vertices)
    {
        // Calculate the normals of the platform
        Vector3 normal1 = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]);
        Vector3 normal2 = Vector3.Cross(vertices[3] - vertices[0], vertices[5] - vertices[0]);

        // Combine the normals and normalize
        normal = (normal1 + normal2).normalized;

        // Calculate the platform pitch and roll
        platformPitch = Mathf.Atan2(normal.z, normal.y) * Mathf.Rad2Deg;
        platformRoll = Mathf.Atan2(normal.x, normal.y) * Mathf.Rad2Deg;
    }

    // This method returns the vertices of the platform
    private Vector3[] GetVertices()
    {
        return new Vector3[] { cylinders[1].position, cylinders[0].position, cylinders[2].position, cylinders[1].position, cylinders[2].position, cylinders[3].position };
    }
}