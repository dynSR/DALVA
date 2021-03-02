using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class VisibilityRepresentation : MonoBehaviour
{
    [Header("VIEW")]
    [SerializeField] private float viewRadius = 8f;
    [Range(0, 360)] [SerializeField] private float viewAngle = 360f;
    [SerializeField] private float viewDepth = 1f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("VIEW RENDERER")]
    [SerializeField] private FogOfWarProjection fogProjector;
    [SerializeField] private int meshResolution = 16;
    [SerializeField] private int edgeResolveIterations = 1;
    [SerializeField] private float edgeDstThreshold = 0f;
    [SerializeField] private MeshFilter viewMeshFilter;
    [SerializeField] private float updateDistance = 1;

    [SerializeField] private bool debugMode;

    Mesh viewMesh;
    Vector3 lastUpdatePos;

    public float ViewAngle { get => viewAngle; }
    public float ViewRadius { get => viewRadius; }

    void OnEnable()
    {
        viewMesh = new Mesh {name = "View Mesh"};
        viewMeshFilter.mesh = viewMesh;

        fogProjector = fogProjector != null ? fogProjector : FindObjectOfType<FogOfWarProjection>();
    }

    void LateUpdate()
    {
        DrawFieldOfView();
        if (Vector3.Distance(transform.position, lastUpdatePos) > updateDistance || Time.time < .5f)
        {
            lastUpdatePos = transform.position;
            fogProjector.UpdateFog();
        }
    }

    #region Field Of View Drawer
    void DrawFieldOfView()
    {
        float stepAngleSize = ViewAngle / meshResolution;
        List<Vector3> viewPoints = new List<Vector3>();
        ObstacleInfo oldObstacle = new ObstacleInfo();
        for (int i = 0; i <= meshResolution; i++)
        {
            float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
            ObstacleInfo newObstacle = FindObstacles(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldObstacle.dst - newObstacle.dst) > edgeDstThreshold;
                if (oldObstacle.hit != newObstacle.hit ||
                    oldObstacle.hit && edgeDstThresholdExceeded)
                {
                    EdgeInfo edge = FindEdge(oldObstacle, newObstacle);
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


            viewPoints.Add(newObstacle.point);
            oldObstacle = newObstacle;
        }

        int vertexCount = viewPoints.Count + 1;
        var vertices = new Vector3[vertexCount];
        var triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

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
    }

    EdgeInfo FindEdge(ObstacleInfo minObstacle, ObstacleInfo maxObstacle)
    {
        float minAngle = minObstacle.angle;
        float maxAngle = maxObstacle.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ObstacleInfo newObstacle = FindObstacles(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minObstacle.dst - newObstacle.dst) > edgeDstThreshold;
            if (newObstacle.hit == minObstacle.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newObstacle.point;
            } else
            {
                maxAngle = angle;
                maxPoint = newObstacle.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ObstacleInfo FindObstacles(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (DebugRayCast(transform.position, dir, out hit, ViewRadius, obstacleMask))
        {
            return new ObstacleInfo(true, hit.point + hit.normal * -viewDepth, hit.distance, globalAngle);
        }
        return new ObstacleInfo(false, transform.position + dir * (ViewRadius - viewDepth), ViewRadius, globalAngle);
    }

    bool DebugRayCast(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int mask)
    {
        if (Physics.Raycast(origin, direction, out hit, maxDistance, mask))
        {
            if (debugMode)
                Debug.DrawLine(origin, hit.point);
            return true;
        }
        if (debugMode)
            Debug.DrawLine(origin, origin + direction * maxDistance);
        return false;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool isGlobal)
    {
        if (!isGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ObstacleInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ObstacleInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
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
    #endregion
}