using UnityEngine;
using System.Collections;

public struct PatchConfig
{
    public string name;
    public Vector3 uAxis;
    public Vector3 vAxis;
    public Vector3 height;
    public PatchConfig(string aName, Vector3 aUAxis, Vector3 aVAxis)
    {
        name = aName;
        uAxis = aUAxis;
        vAxis = aVAxis;
        height = Vector3.Cross(vAxis, uAxis);
    }
}



public class Sphere : MonoBehaviour
{
    private static PatchConfig[] patches = new PatchConfig[]
    {
         new PatchConfig("top", Vector3.right, Vector3.forward),
         new PatchConfig("bottom", Vector3.left, Vector3.forward),
         new PatchConfig("left", Vector3.up, Vector3.forward),
         new PatchConfig("right", Vector3.down, Vector3.forward),
         new PatchConfig("front", Vector3.right, Vector3.down),
         new PatchConfig("back", Vector3.right, Vector3.up)
    };

    public int uPatchCount = 2;
    public int vPatchCount = 2;
    public int xVertCount = 250;
    public int yVertCount = 250;
    public float radius = 5f;
    public float seed;
    public float scale;
    public int octaves;
    public float persistance;
    public float lacunarity;

    public Material patchMaterial;

    public Color[] regions;
    public float[] heights;
    public AnimationCurve heightCurve;

    public GameObject ore;
    public float oreSeed;
    public float oreScale;

    void Start()
    {
         //GeneratePatches();
    }
    public static float Perlin3d(float x, float y, float z)
    {
        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);

        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        float ABC = AB + BC + AC + BA + CB + CA;

        return ABC / 6f;
    }
   

    public void GeneratePatch(PatchConfig aConf, int u, int v)
    {
        GameObject patch = new GameObject(aConf.name + "_" + u + v);
        MeshFilter mf = patch.AddComponent<MeshFilter>();
        MeshRenderer rend = patch.AddComponent<MeshRenderer>();

        patch.AddComponent<SectionTexture>();
        if (transform.name == "clouds2 Inversed")
        {
            patch.AddComponent<ReverseNormals>();
        }




        rend.sharedMaterial = patchMaterial;
        Mesh m = mf.sharedMesh = new Mesh();
        patch.transform.parent = transform;
        patch.transform.localEulerAngles = Vector3.zero;
        patch.transform.localPosition = Vector3.zero;
        Vector2 UVstep = new Vector2(1f / uPatchCount, 1f / vPatchCount);
        Vector2 step = new Vector2(UVstep.x / (xVertCount - 1), UVstep.y / (yVertCount - 1));
        Vector2 offset = new Vector3((-0.5f + u * UVstep.x), (-0.5f + v * UVstep.y));
        Vector3[] vertices = new Vector3[xVertCount * yVertCount];
        Vector3[] normals = new Vector3[vertices.Length];
        Vector2[] uvs = new Vector2[vertices.Length];

        Texture2D tex = new Texture2D(xVertCount,yVertCount);

        float minNoiseHeight = float.MaxValue+.1f;
        float maxNoiseHeight = float.MinValue;

        for (int y = 0; y < yVertCount; y++)
        {
            for (int x = 0; x < xVertCount; x++)
            {

                int i = x + y * xVertCount;
                Vector2 p = offset + new Vector2(x * step.x, y * step.y);
                uvs[i] = p + Vector2.one * 0.5f;
                Vector3 vec = (aConf.uAxis * p.x + aConf.vAxis * p.y + aConf.height * 0.5f);
                vec = vec.normalized;

                float noiseHeight = 0;
                float oreHeight = 0;
                float frequency = 1;
                float amplitude = 1;
                float range = 1f;
                oreSeed = seed * seed / .35f;

                for (int g = 0; g < octaves; g++)
                {
                    float nx = transform.TransformPoint(vec).x;
                    float ny = transform.TransformPoint(vec).y;
                    float nz = transform.TransformPoint(vec).z;



                    float xx = ((nx - xVertCount) / scale)*frequency;
                    float yy = ((ny - yVertCount) / scale)*frequency;
                    float zz = ((nz - xVertCount) / scale)*frequency;

                    float ox = ((nx - xVertCount) / oreScale) * frequency;
                    float oy = ((ny - yVertCount) / oreScale) * frequency;
                    float oz = ((nz - xVertCount) / oreScale) * frequency;

                    float perlinValue = Perlin3d(xx+seed, yy+seed, zz+seed);
                    float orePerlinValue = Perlin3d(ox + oreSeed, oy + oreSeed, oz + oreSeed);
                    noiseHeight += perlinValue * amplitude;
                    oreHeight += orePerlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                    range += amplitude/4;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }



                vec = vec * (1.0f + (heightCurve.Evaluate(noiseHeight/range)));
                float currentHeight = noiseHeight/range;
                float oreCurrentHeight = oreHeight / range;
                //Creating Ores//
                if (transform.name == "CubeSphere")
                {
                    if (oreCurrentHeight> .945f)
                    {
                        GameObject Iron = Instantiate(ore,transform.localPosition+( vec*radius), Quaternion.identity);
                        Iron.transform.name = "Iron";
                        Iron.GetComponent<Renderer>().material.color = Color.gray;
                    }
                    if (oreCurrentHeight < .52f)
                    {
                        GameObject Copper = Instantiate(ore, transform.localPosition + (vec * radius), Quaternion.identity);
                        Copper.transform.name = "Copper";
                        Copper.GetComponent<Renderer>().material.color = Color.red;
                    }
                    if (oreCurrentHeight < .75f && oreCurrentHeight > .7499f)
                    {
                        GameObject Aluminum = Instantiate(ore, transform.localPosition + (vec * radius), Quaternion.identity);
                        Aluminum.transform.name = "Aluminum";
                        Aluminum.GetComponent<Renderer>().material.color = Color.green;
                    }
                    if (oreCurrentHeight < .65f && oreCurrentHeight > .64999f)
                    {
                        GameObject Titanium = Instantiate(ore, transform.localPosition + (vec * radius), Quaternion.identity);
                        Titanium.transform.name = "Titanium";
                        Titanium.GetComponent<Renderer>().material.color = Color.yellow;
                    }
                }


                normals[i] = vec;
                vertices[i] = vec * radius;
                for (int r = 0; r < regions.Length; r++)
                {
                    if (r < regions.Length - 1)
                    {
                       
                            if (currentHeight >= heights[r] && currentHeight < heights[r + 1])
                            {
                                tex.SetPixel(x, y, regions[r]);
                                break;
                            }
                       
                    }
                    else {
                        tex.SetPixel(x, y, regions[r]);
                        break;
                    }

                }
            }
        }
        tex.Apply();
        //tex.alphaIsTransparency = true;
        patch.GetComponent<Renderer>().material.mainTexture = tex;
        patch.GetComponent<SectionTexture>().tex = tex;
        

        int[] indices = new int[(xVertCount - 1) * (yVertCount - 1) * 4];
        for (int y = 0; y < yVertCount - 1; y++)
        {
            for (int x = 0; x < xVertCount - 1; x++)
            {
                int i = (x + y * (xVertCount - 1)) * 4;
                indices[i] = x + y * xVertCount;
                indices[i + 1] = x + (y + 1) * xVertCount;
                indices[i + 2] = x + 1 + (y + 1) * xVertCount;
                indices[i + 3] = x + 1 + y * xVertCount;
            }
        }
        m.vertices = vertices;
        m.normals = normals;
        m.uv = uvs;
        m.SetIndices(indices, MeshTopology.Quads, 0);
        m.RecalculateBounds();

        mf.sharedMesh.SetTriangles(mf.sharedMesh.GetTriangles(0), 0);


    }

    public void GeneratePatches()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int u = 0; u < uPatchCount; u++)
            {
                for (int v = 0; v < vPatchCount; v++)
                {
                    GeneratePatch(patches[i], u, v);
                }
            }
        }
    }
}
