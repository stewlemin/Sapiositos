using UnityEngine;

public class ArtifactsDetector : MonoBehaviour
{
    public float maxrange = 50f;
    public float distance;

    public Artifact FindNearest()
    {
        Artifact nearest = null;
        distance = maxrange;
        foreach (Artifact artifact in Artifact.all)//goes through all artifacts in list
        {
            float distancetoartifacts = Vector3.Distance(transform.position, artifact.transform.position);
            if (distancetoartifacts < distance)
            {
                distance = distancetoartifacts;
                nearest = artifact;
            }
        }
        return nearest;
    }
}