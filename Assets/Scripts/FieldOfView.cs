using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask obstacleMask;
    public LayerMask triggerMask;

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDistanceThreshold;

    public float maskCutawayDistance = 0.1f;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    void Awake()
    {
        viewMeshFilter = GetComponent<MeshFilter>();
    }

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = -1;

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    public void SetObstacleMask(LayerMask mask)
    {
        obstacleMask = mask;
    }

    public void SetTriggerMask(LayerMask mask)
    {
        triggerMask = mask;
    }

    Vector3 DirFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    public Vector3 DrawFieldOfView()
    {
        Vector3 targetPoint = Vector3.zero;

        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i < stepCount; i++)
        {
            float angle = -transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            
            if (i > 0)
            {
                bool edgeDistThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }

                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }
            
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;

            if (targetPoint == Vector3.zero && triggerMask == (triggerMask| (1 << newViewCast.layer)))
            {
                targetPoint = newViewCast.point;
            }
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.up * maskCutawayDistance;

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();

        return targetPoint;
    }

    ViewCastInfo ViewCast(float angle)
    {
        Vector3 dir = DirFromAngle(angle);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);
        if (hit)
        {
            if (hit.transform.gameObject.name == "Robot")
            {
                hit.transform.gameObject.GetComponent<RobotController>().Show();
            }

            if (hit.transform.gameObject.name == "Box")
            {
                hit.transform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }

            return new ViewCastInfo(true, hit.transform.gameObject.layer, hit.point, hit.distance, angle);
        }

        return new ViewCastInfo(false, -1, transform.position + dir * viewRadius, viewRadius, angle);
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2f;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
            if (!newViewCast.hit == minViewCast.hit && !edgeDistThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            } else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    void OnDrawGizmos()
    {
        // Draw view radius
        // Matrix4x4 oldMatrix = Gizmos.matrix;
        // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(1, 1, 0.01f));
        // Gizmos.DrawWireSphere(Vector3.zero, viewRadius);
        // Gizmos.matrix = oldMatrix;

        // Draw view angle lines
        // float halfAngle = viewAngle / 2f;
        // Vector3 angleA = DirFromAngle(-halfAngle - transform.eulerAngles.z);
        // Vector3 angleB = DirFromAngle(halfAngle - transform.eulerAngles.z);
        // Gizmos.DrawLine(transform.position, transform.position + angleA * viewRadius);
        // Gizmos.DrawLine(transform.position, transform.position + angleB * viewRadius);
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public int layer;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, int _layer, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            layer = _layer;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
