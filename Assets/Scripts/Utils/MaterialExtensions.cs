using System;

using UnityEngine;

namespace Utils
{
    public static class MaterialExtensions
    {
        private static readonly int Surface = Shader.PropertyToID("_Surface");
        private static readonly int AlphaClip = Shader.PropertyToID("_AlphaClip");
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        private static readonly int Blend = Shader.PropertyToID("_Blend");

        public enum SurfaceType
        {
            Opaque,
            Transparent
        }

        public enum BlendMode
        {
            Alpha,
            Premultiply,
            Additive,
            Multiply
        }

        public static void SetSurfaceType(this Material standardShaderMaterial, SurfaceType surfaceType)
        {
            standardShaderMaterial.SetFloat(Surface, (float) surfaceType);
            if (surfaceType == SurfaceType.Transparent)
                standardShaderMaterial.SetFloat(Blend, (float) BlendMode.Alpha);

            standardShaderMaterial.SetupMaterialBlendMode();
        }

        private static void SetupMaterialBlendMode(this Material material)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));
            
            bool alphaClip = material.GetFloat(AlphaClip) == 1;
            if (alphaClip)
                material.EnableKeyword("_ALPHATEST_ON");
            else
                material.DisableKeyword("_ALPHATEST_ON");
            SurfaceType surfaceType = (SurfaceType) material.GetFloat(Surface);
            if (surfaceType == 0)
            {
                material.SetOverrideTag("RenderType", "");
                material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.One);
                material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt(ZWrite, 1);
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                material.SetShaderPassEnabled("ShadowCaster", true);
            }
            else
            {
                BlendMode blendMode = (BlendMode) material.GetFloat(Blend);
                switch (blendMode)
                {
                    case BlendMode.Alpha:
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt(ZWrite, 0);
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
                        material.SetShaderPassEnabled("ShadowCaster", false);
                        break;
                    case BlendMode.Premultiply:
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.One);
                        material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt(ZWrite, 0);
                        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
                        material.SetShaderPassEnabled("ShadowCaster", false);
                        break;
                    case BlendMode.Additive:
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.One);
                        material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.One);
                        material.SetInt(ZWrite, 0);
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
                        material.SetShaderPassEnabled("ShadowCaster", false);
                        break;
                    case BlendMode.Multiply:
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.DstColor);
                        material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt(ZWrite, 0);
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
                        material.SetShaderPassEnabled("ShadowCaster", false);
                        break;
                }
            }
        }
    }
}