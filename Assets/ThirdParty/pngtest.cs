using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using PNGNet;
using SS13;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class pngtest : MonoBehaviour
{
    public string DmiFolder = Application.streamingAssetsPath + "/turf";
    private PNGBitmap _img;
    public MeshRenderer renderer;
    public MeshFilter mesh;
    private DmiFile dmiFile;

    private float timer = 0.5f;
    private int dir = 2;
    private int frame = 0;

    // Start is called before the first frame update
    void Start()
    {
        string[] files = Directory.GetFiles(DmiFolder, "*.dmi", SearchOption.AllDirectories);
        Stopwatch stopwatch = new Stopwatch();
        List<Tuple<string, double>> times = new List<Tuple<string, double>>(400);

        foreach (var file in files)
        {
            stopwatch.Reset();
            stopwatch.Start();
            dmiFile = new DmiFile(file);
            renderer.material = new Material(renderer.material);
            renderer.material.mainTexture = dmiFile.Texture;
            renderer.material.mainTexture.filterMode = FilterMode.Point;
            renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            renderer.material.SetInt("_ZWrite", 1);
            renderer.material.EnableKeyword("_ALPHATEST_ON");
            renderer.material.DisableKeyword("_ALPHABLEND_ON");
            renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            renderer.material.renderQueue = 2450;
            stopwatch.Stop();
            times.Add(new Tuple<string, double>(file, stopwatch.Elapsed.TotalSeconds));
        }

        foreach (var tuple in times)
        {
            Debug.Log(tuple.Item1 + " => " +tuple.Item2);
        }

    }

    //// Update is called once per frame
    //void Update()
    //{
    //    timer -= Time.deltaTime;
    //    if (timer <= 0)
    //    {
    //        timer += 0.5f;
    //        foreach (var dmiState in dmiFile.States)
    //        {
    //            if (dmiState.StateName != "iron0")
    //            {
    //                continue;
    //            }
    //            
    //            
    //            var uv = dmiState.GetUV(dir, frame);
    //            var uvStateSize = dmiState.GetUVStateSize();
    //
    //            var uvBottomLeft = uv;
    //            var uvBottomRight = new Vector2(uvBottomLeft.x + uvStateSize.x, uvBottomLeft.y);
    //            var uvTopLeft = new Vector2(uvBottomLeft.x, uvBottomLeft.y + uvStateSize.y);
    //            var uvTopRight = new Vector2(uvBottomLeft.x + uvStateSize.x, uvBottomLeft.y + uvStateSize.y);
    //
    //            mesh.mesh.SetUVs(0, new List<Vector2>(){uvBottomLeft, uvBottomRight, uvTopLeft, uvTopRight});
    //            frame++;
    //            frame %= dmiState.FramesCount;
    //        }
    //
    //
    //    }
    //
    //}
}