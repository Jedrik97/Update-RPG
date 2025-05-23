

Shader "Vefects/SH_Vefects_VFX_SRP_Fire_Flames_01"
{
	Properties
	{
		_EmissiveIntensity("Emissive Intensity", Float) = 1
		_Noise_01_Texture("Noise_01_Texture", 2D) = "white" {}
		_Noise_02_Texture("Noise_02_Texture", 2D) = "white" {}
		_ColorGradientMask("Color Gradient Mask", 2D) = "white" {}
		_DistortionMaskTexture("Distortion Mask Texture", 2D) = "white" {}
		_Mask_Texture("Mask_Texture", 2D) = "white" {}
		_TextureSample2("Texture Sample 2", 2D) = "white" {}
		_ColorBottom("Color Bottom", Color) = (1,0.3666667,0,0)
		_ColorTop("Color Top", Color) = (1,0.6666667,0,0)
		_NoiseDistortion_Texture("NoiseDistortion_Texture", 2D) = "white" {}
		_Noise_01_Scale("Noise_01_Scale", Vector) = (0.8,0.8,0,0)
		_Noise_02_Scale("Noise_02_Scale", Vector) = (1,1,0,0)
		_NoiseDistortion_Scale("NoiseDistortion_Scale", Vector) = (1,1,0,0)
		_Noise_01_Speed("Noise_01_Speed", Vector) = (0.5,0.5,0,0)
		_Noise_02_Speed("Noise_02_Speed", Vector) = (-0.2,0.4,0,0)
		_Mask_Scale("Mask_Scale", Vector) = (1,1,0,0)
		_Mask_Offset("Mask_Offset", Vector) = (0,0,0,0)
		_NoiseDistortion_Speed("NoiseDistortion_Speed", Vector) = (-0.3,-0.3,0,0)
		_Mask_Multiply("Mask_Multiply", Float) = 1
		_Noises_Multiply("Noises_Multiply", Float) = 5
		_Mask_Power("Mask_Power", Float) = 1
		_Noises_Power("Noises_Power", Float) = 1
		_DistortionAmount("Distortion Amount", Float) = 1
		_DistortionMaskIntensity("Distortion Mask Intensity", Float) = 1
		_NoisesOpacityBoost("Noises Opacity Boost", Float) = 1
		_DepthFade("Depth Fade", Float) = 1
		_WindSpeed("Wind Speed", Float) = 1
		_Dissolve("Dissolve", Float) = 0
		[Space(13)][Header(AR)][Space(13)]_Cull1("Cull", Float) = 2
		_Src1("Src", Float) = 5
		_Dst1("Dst", Float) = 10
		_ZWrite1("ZWrite", Float) = 0
		_ZTest1("ZTest", Float) = 2
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
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float _Cull1;
		uniform float _Src1;
		uniform float _Dst1;
		uniform float _ZTest1;
		uniform float _ZWrite1;
		uniform float4 _ColorBottom;
		uniform float4 _ColorTop;
		uniform sampler2D _ColorGradientMask;
		uniform float4 _ColorGradientMask_ST;
		uniform sampler2D _Mask_Texture;
		uniform sampler2D _DistortionMaskTexture;
		uniform sampler2D _NoiseDistortion_Texture;
		uniform float _WindSpeed;
		uniform float2 _NoiseDistortion_Speed;
		uniform float2 _NoiseDistortion_Scale;
		uniform float _DistortionAmount;
		uniform float _DistortionMaskIntensity;
		uniform float2 _Mask_Scale;
		uniform float2 _Mask_Offset;
		uniform float _Mask_Power;
		uniform float _Mask_Multiply;
		uniform sampler2D _Noise_01_Texture;
		uniform float2 _Noise_01_Speed;
		uniform float2 _Noise_01_Scale;
		uniform sampler2D _Noise_02_Texture;
		uniform float2 _Noise_02_Speed;
		uniform float2 _Noise_02_Scale;
		uniform float _Noises_Power;
		uniform float _Noises_Multiply;
		uniform float _NoisesOpacityBoost;
		uniform sampler2D _TextureSample2;
		uniform float _EmissiveIntensity;
		uniform float _Dissolve;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthFade;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_ColorGradientMask = i.uv_texcoord * _ColorGradientMask_ST.xy + _ColorGradientMask_ST.zw;
			float4 lerpResult152 = lerp( _ColorBottom , _ColorTop , tex2D( _ColorGradientMask, uv_ColorGradientMask ).r);
			float2 uv_TexCoord92 = i.uv_texcoord + float2( 0,0.4 );
			float windSpeed60 = ( _WindSpeed * _Time.y );
			float2 panner65 = ( windSpeed60 * _NoiseDistortion_Speed + ( i.uv_texcoord * _NoiseDistortion_Scale ));
			float Distortion81 = ( ( tex2D( _NoiseDistortion_Texture, panner65 ).r * 0.1 ) * _DistortionAmount );
			float2 uv_TexCoord106 = i.uv_texcoord * _Mask_Scale + _Mask_Offset;
			float2 panner85 = ( windSpeed60 * _Noise_01_Speed + ( i.uv_texcoord * _Noise_01_Scale ));
			float2 panner83 = ( windSpeed60 * _Noise_02_Speed + ( i.uv_texcoord * _Noise_02_Scale ));
			float noises109 = saturate( ( pow( ( tex2D( _Noise_01_Texture, ( panner85 + Distortion81 ) ).r * tex2D( _Noise_02_Texture, ( panner83 + Distortion81 ) ).r ) , _Noises_Power ) * _Noises_Multiply ) );
			float2 uv_TexCoord116 = i.uv_texcoord + float2( 0,0.4 );
			float temp_output_133_0 = ( saturate( ( pow( tex2D( _Mask_Texture, ( ( tex2D( _DistortionMaskTexture, uv_TexCoord92 ).r * ( Distortion81 * _DistortionMaskIntensity ) ) + uv_TexCoord106 ) ).r , _Mask_Power ) * _Mask_Multiply ) ) - ( ( noises109 * _NoisesOpacityBoost ) * tex2D( _TextureSample2, uv_TexCoord116 ).r ) );
			o.Emission = ( ( ( i.vertexColor * lerpResult152 ) * temp_output_133_0 ) * _EmissiveIntensity ).rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth131 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth131 = abs( ( screenDepth131 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFade ) );
			o.Alpha = saturate( ( ( ( temp_output_133_0 - _Dissolve ) * saturate( distanceDepth131 ) ) * i.vertexColor.a ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}

