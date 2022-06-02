using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerThrowProyection : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] PlayerThrow thrower;
    [SerializeField] int num_of_segments;
    [SerializeField] float time_between_points;
    [SerializeField] LayerMask layer_to_collide;

    Vector3[] positions;

    bool isRender;

    public void BeginRender() { isRender = true; lineRenderer.positionCount = 1; lineRenderer.SetPosition(0, new Vector3(0, int.MinValue, 0)); lineRenderer.enabled = true; }
    public void StopRender() { isRender = false; lineRenderer.positionCount = 1; lineRenderer.SetPosition(0, new Vector3(0, int.MinValue, 0)); lineRenderer.enabled = false; }

    private void Update()
    {
        if (!isRender) return;
        lineRenderer.positionCount = num_of_segments; //ver si esto se puede hacer afuera// no se puede porque el overlapsphere lo puede cortar
        List<Vector3> points = new List<Vector3>();
        Vector3 P0 = thrower.shootPoint.position; //pos inicial
        Vector3 V0 = thrower.shoot_velocity * thrower.Force; //velocidad inicial * fuerza
        float g = Physics.gravity.y;

        positions = new Vector3[num_of_segments];

        for (float t = 0; t < num_of_segments; t += time_between_points)
        {
            Vector3 newPoint = P0 + t * V0;
            //formula de proyeccion
            // posicion inicial en Y
            // +
            // velocidad inicial en Y por el tiempo
            // + 
            // gravedad / 2 * tiempo ^ 2
            // 

            newPoint.y = P0.y + V0.y * t + g /2 * t * t;
            points.Add(newPoint);

            //var colliders = Physics.OverlapSphere(newPoint, 2, layer_to_collide);
            //colliders.Where(x => { var obj = x.GetComponent<Character>(); return obj != null; });

            //for (int i = 0; i < colliders.Length; i++)
            //{
            //    Debug.Log(colliders[i].gameObject.name);
            //}

            //if (colliders.Length > 0)
            //{
            //    lineRenderer.positionCount = points.Count;
            //    break;
            //}

            lineRenderer.SetPositions(points.ToArray());
        }

        positions = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            positions[i] = points[i];
        }
    }
}
