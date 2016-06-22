using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class MetalMirror : MonoBehaviour
{
    private static bool s_InsideRendering;

    private readonly Hashtable m_ReflectionCameras = new Hashtable();
    public float m_ClipPlaneOffset = 0.07f;
    public bool m_DisablePixelLights = true;
    public bool m_IsFlatMirror = true;
    private int m_OldReflectionTextureSize;

    private RenderTexture m_ReflectionTexture;

    public LayerMask m_ReflectLayers = -1;
    public int m_TextureSize = 256;

    public void OnWillRenderObject()
    {
        if (!enabled || !GetComponent<Renderer>() || !GetComponent<Renderer>().sharedMaterial ||
            !GetComponent<Renderer>().enabled)
            return;

        var cam = Camera.current;
        if (!cam)
            return;

        if (s_InsideRendering)
            return;
        s_InsideRendering = true;

        Camera reflectionCamera;
        CreateMirrorObjects(cam, out reflectionCamera);

        var pos = transform.position;
        Vector3 normal;
        if (m_IsFlatMirror)
        {
            normal = transform.up;
        }
        else
        {
            normal = transform.position - cam.transform.position;
            normal.Normalize();
        }
        var oldPixelLightCount = QualitySettings.pixelLightCount;
        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = 0;

        UpdateCameraModes(cam, reflectionCamera);

        var d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
        var reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

        var reflection = Matrix4x4.zero;
        CalculateReflectionMatrix(ref reflection, reflectionPlane);
        var oldpos = cam.transform.position;
        var newpos = reflection.MultiplyPoint(oldpos);
        reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix*reflection;


        var clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
        var projection = cam.projectionMatrix;
        CalculateObliqueMatrix(ref projection, clipPlane);
        reflectionCamera.projectionMatrix = projection;

        reflectionCamera.cullingMask = ~(1 << 4) & m_ReflectLayers.value;
        reflectionCamera.targetTexture = m_ReflectionTexture;
        GL.SetRevertBackfacing(true);
        reflectionCamera.transform.position = newpos;
        var euler = cam.transform.eulerAngles;
        reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
        reflectionCamera.Render();
        reflectionCamera.transform.position = oldpos;
        GL.SetRevertBackfacing(false);
        var materials = GetComponent<Renderer>().sharedMaterials;
        foreach (var mat in materials)
        {
            if (mat.HasProperty("_Ref"))
                mat.SetTexture("_Ref", m_ReflectionTexture);
        }
        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = oldPixelLightCount;

        s_InsideRendering = false;
    }

    private void OnDisable()
    {
        if (m_ReflectionTexture)
        {
            DestroyImmediate(m_ReflectionTexture);
            m_ReflectionTexture = null;
        }
        foreach (DictionaryEntry kvp in m_ReflectionCameras)
            DestroyImmediate(((Camera) kvp.Value).gameObject);
        m_ReflectionCameras.Clear();
    }


   /// <summary>
   /// 更新摄像机模式
   /// </summary>
   /// <param name="src"></param>
   /// <param name="dest"></param>
    private void UpdateCameraModes(Camera src, Camera dest)
    {
        if (dest == null)
            return;

        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;
        if (src.clearFlags == CameraClearFlags.Skybox)
        {
            var sky = src.GetComponent(typeof (Skybox)) as Skybox;
            var mysky = dest.GetComponent(typeof (Skybox)) as Skybox;
            if (!sky || !sky.material)
            {
                mysky.enabled = false;
            }
            else
            {
                mysky.enabled = true;
                mysky.material = sky.material;
            }
        }

        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
        dest.renderingPath = src.renderingPath;
    }


    /// <summary>
    /// 创建一个镜像对象
    /// </summary>
    /// <param name="currentCamera"></param>
    /// <param name="reflectionCamera"></param>
    private void CreateMirrorObjects(Camera currentCamera, out Camera reflectionCamera)
    {
        reflectionCamera = null;


        if (!m_ReflectionTexture || m_OldReflectionTextureSize != m_TextureSize)
        {
            if (m_ReflectionTexture)
                DestroyImmediate(m_ReflectionTexture);
            m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
            m_ReflectionTexture.name = "__MirrorReflection" + GetInstanceID();
            m_ReflectionTexture.isPowerOfTwo = true;
            m_ReflectionTexture.hideFlags = HideFlags.DontSave;
            m_OldReflectionTextureSize = m_TextureSize;
        }


        reflectionCamera = m_ReflectionCameras[currentCamera] as Camera;
        if (!reflectionCamera)
        {
            var go = new GameObject(
                "Mirror Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof (Camera),
                typeof (Skybox));
            reflectionCamera = go.GetComponent<Camera>();
            reflectionCamera.enabled = false;
            reflectionCamera.transform.position = transform.position;
            reflectionCamera.transform.rotation = transform.rotation;
            reflectionCamera.gameObject.AddComponent<FlareLayer>();
            go.hideFlags = HideFlags.HideAndDontSave;
            m_ReflectionCameras[currentCamera] = reflectionCamera;
        }
    }

    private static float sgn(float a)
    {
        if (a > 0.0f) return 1.0f;
        if (a < 0.0f) return -1.0f;
        return 0.0f;
    }

    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        var offsetPos = pos + normal*m_ClipPlaneOffset;
        var m = cam.worldToCameraMatrix;
        var cpos = m.MultiplyPoint(offsetPos);
        var cnormal = m.MultiplyVector(normal).normalized*sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
    {
        var q = projection.inverse*new Vector4(
            sgn(clipPlane.x),
            sgn(clipPlane.y),
            1.0f,
            1.0f
            );
        var c = clipPlane*(2.0F/Vector4.Dot(clipPlane, q));

        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];
    }

    private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = 1F - 2F*plane[0]*plane[0];
        reflectionMat.m01 = -2F*plane[0]*plane[1];
        reflectionMat.m02 = -2F*plane[0]*plane[2];
        reflectionMat.m03 = -2F*plane[3]*plane[0];

        reflectionMat.m10 = -2F*plane[1]*plane[0];
        reflectionMat.m11 = 1F - 2F*plane[1]*plane[1];
        reflectionMat.m12 = -2F*plane[1]*plane[2];
        reflectionMat.m13 = -2F*plane[3]*plane[1];

        reflectionMat.m20 = -2F*plane[2]*plane[0];
        reflectionMat.m21 = -2F*plane[2]*plane[1];
        reflectionMat.m22 = 1F - 2F*plane[2]*plane[2];
        reflectionMat.m23 = -2F*plane[3]*plane[2];

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }
}