using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public class MathUtility
    {
        public static Vector3 GetMouseToTerrainPosition(Camera camera, string layer)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000, LayerMask.GetMask(layer)))
            {
                Debug.Log("<color=cyan>" + "GetMouseToTerrainPosition: " + hitInfo.point + "</color>");
                return hitInfo.point;
            }

            return Vector3.zero;
        }

        public static RaycastHit CameraRay(Camera camera)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000))
            {
                return hitInfo;
            }
            return new RaycastHit();
        }

        public static Vector2 CalculatePolarCoordinates(float angle, float radius, Vector3 center)
        {
            float angleRad = Mathf.Deg2Rad * angle;
            float xPostion = center.x + (radius * Mathf.Cos(angleRad));
            float yPostion = center.y + (radius * Mathf.Sin(angleRad));

            return new Vector2(xPostion, yPostion);
        }

        public static Vector3 GetRandomPosition(float minRadius, float maxRadius)
        {
            float randomAngle = Random.Range(0.0f, 360.0f);
            Vector2 spawnPoint = CalculatePolarCoordinates(randomAngle, Random.Range(minRadius, maxRadius), Vector3.zero);

            return new Vector3(spawnPoint.x, 0.0f, spawnPoint.y);
        }
    }
}
