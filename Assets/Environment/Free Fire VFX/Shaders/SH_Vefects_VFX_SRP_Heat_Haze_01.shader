

Shader "Vefects/SH_Vefects_VFX_SRP_Heat_Haze_01"
{
	Properties
	{
		_Texture("Texture", 2D) = "white" {}
		_TextureChannel("Texture Channel", Vector) = (0,1,0,0)
		_DistortionStrength("Distortion Strength", Float) = 7
		_DissolveMask("Dissolve Mask", 2D) = "white" {}
		[Space(13)][Header(AR)][Space(13)]_Cull1("Cull", Float) = 2
		_Src1("Src", Float) = 5
		_Dst1("Dst", Float) = 10
		_ZWrite1("ZWrite", Float) = 0
		_ZTest1("ZTest", Float) = 2
		_DissolveUVS("Dissolve UV S", Vector) = (0,0,0,0)
		_DissolveUVP("Dissolve UV P", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull [_Cull1]
		ZWrite [_ZWrite1]
		ZTest [_ZTest1]
		Blend [_Src1] [_Dst1]
		
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 screenPos;
			float4 vertexColor : COLOR;
			float4 uv_texcoord;
		};

		uniform float _Cull1;
		uniform float _Src1;
		uniform float _Dst1;
		uniform float _ZTest1;
		uniform float _ZWrite1;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;
		uniform float4 _TextureChannel;
		uniform sampler2D _DissolveMask;
		uniform float2 _DissolveUVP;
		uniform float2 _DissolveUVS;
		uniform float _DistortionStrength;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			float dotResult15 = dot( tex2D( _Texture, uv_Texture ) , _TextureChannel );
			float2 panner47 = ( 1.0 * _Time.y * _DissolveUVP + ( i.uv_texcoord.xy * _DissolveUVS ));
			float4 temp_output_28_0 = ( i.vertexColor.a * ( saturate( dotResult15 ) * ( saturate( tex2D( _DissolveMask, panner47 ) ) + i.uv_texcoord.z ) ) );
			float4 screenColor31 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_screenPosNorm + ( temp_output_28_0 * _DistortionStrength ) ).xy);
			o.Emission = (screenColor31).rgb;
			o.Alpha = saturate( temp_output_28_0 ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}

