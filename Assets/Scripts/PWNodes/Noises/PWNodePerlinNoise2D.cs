﻿using UnityEditor;
using UnityEngine;

namespace PW
{
	public class PWNodePerlinNoise2D : PWNode {
		
		public float		persistance;
		public int			octaves;

		[PWOutput("OUT")]
		public Sampler2D	output;

		Texture2D			previewTex;

		public override void OnNodeCreate()
		{
			name = "Perlin noise 2D";
			output = new Sampler2D(chunkSize);
			previewTex = new Texture2D(chunkSize, chunkSize, TextureFormat.ARGB32, false, false);
		}

		public override void OnNodeGUI()
		{
			EditorGUIUtility.labelWidth = 70;

			EditorGUI.BeginChangeCheck();
			{
				persistance = EditorGUILayout.Slider("Persistance", persistance, 0, 1);
				octaves = EditorGUILayout.IntSlider("Octaves", octaves, 0, 32);
			}
			if (EditorGUI.EndChangeCheck())
			{
				UpdateNoise();
				notifyDataChanged = true;
			}

			//TODO: shader preview here
			
			GUILayout.Label(previewTex, GUILayout.Width(100), GUILayout.Height(100));
		}

		void UpdateNoise()
		{
			output.Foreach((x, y) => {
				float val = Mathf.PerlinNoise((float)x / 20f + seed, (float)y / 20f + seed);
				for (int i = 0; i < octaves; i++)
					val *= 1.2f;
				previewTex.SetPixel(x, y, new Color(val, val, val));
				return val;
			});
			previewTex.Apply();
		}

		public override void OnNodeProcess()
		{
			if (chunkSizeHasChanged)
			{
				output.Resize(chunkSize);
				previewTex = new Texture2D(chunkSize, chunkSize, TextureFormat.ARGB32, false, false);
			}

			//recalcul perlin noise values with new seed / position.
			if (needUpdate)
				UpdateNoise();
		}
	}
}