using UnityEngine;
public class Lightmap : MonoBehaviour
{
    private LightmapData[] originalLightmaps;
    public Texture2D blackTexture;
    private void Start()
    {
        originalLightmaps = LightmapSettings.lightmaps;
        LightmapData[] turnedOffLightmaps = new LightmapData[originalLightmaps.Length];
        for (int i = 0; i < turnedOffLightmaps.Length; i++)
        {
            var thisOriginalLightmap = originalLightmaps[i];
            var thisTurnedOffLightmap = new LightmapData();
 
            thisTurnedOffLightmap.lightmapDir = thisOriginalLightmap.lightmapDir;
            thisTurnedOffLightmap.shadowMask = thisOriginalLightmap.shadowMask;
            thisTurnedOffLightmap.lightmapColor = blackTexture;
 
            turnedOffLightmaps[i] = thisTurnedOffLightmap;
        }
        LightmapSettings.lightmaps = turnedOffLightmaps;
    }
}
