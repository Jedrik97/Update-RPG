

Shader "Polytope Studio/PT_Vegetation_Flowers_Shader"
{
	Properties
	{
		[NoScaleOffset]_BASETEXTURE("BASE TEXTURE", 2D) = "black" {}
		[Toggle]_CUSTOMCOLORSTINTING("CUSTOM COLORS  TINTING", Float) = 1
		_TopColor("Top Color", Color) = (0.3505436,0.5754717,0.3338822,1)
		_GroundColor("Ground Color", Color) = (0.1879673,0.3113208,0.1776878,1)
		[HDR]_Gradient(" Gradient", Range( 0 , 1)) = 1
		_GradientPower1("Gradient Power", Range( 0 , 10)) = 1
		_LeavesThickness("Leaves Thickness", Range( 0.1 , 0.95)) = 0.5
		[Toggle]_CUSTOMFLOWERSCOLOR("CUSTOM FLOWERS COLOR", Float) = 0
		[HideInInspector]_MaskClipValue("Mask Clip Value", Range( 0 , 1)) = 0.5
		[HDR]_FLOWERSCOLOR("FLOWERS COLOR", Color) = (1,0,0,0)
		[Toggle(_TRANSLUCENCYONOFF_ON)] _TRANSLUCENCYONOFF("TRANSLUCENCY ON/OFF", Float) = 1
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		[Toggle(_CUSTOMWIND_ON)] _CUSTOMWIND("CUSTOM WIND", Float) = 1
		_WindMovement("Wind Movement", Range( 0 , 1)) = 0.5
		_WindDensity("Wind Density", Range( 0 , 5)) = 0.2
		_WindStrength("Wind Strength", Range( 0 , 1)) = 0.3
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature _CUSTOMWIND_ON
		#pragma shader_feature_local _TRANSLUCENCYONOFF_ON
		#pragma multi_compile __ LOD_FADE_CROSSFADE
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform float _WindMovement;
		uniform float _WindDensity;
		uniform float _WindStrength;
		uniform float _CUSTOMCOLORSTINTING;
		uniform float _CUSTOMFLOWERSCOLOR;
		uniform sampler2D _BASETEXTURE;
		uniform float4 _FLOWERSCOLOR;
		uniform float4 _GroundColor;
		uniform float4 _TopColor;
		uniform float _Gradient;
		uniform float _GradientPower1;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float _LeavesThickness;
		uniform float _MaskClipValue;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_vertex4Pos = v.vertex;
			float simplePerlin2D321 = snoise( (ase_vertex4Pos*1.0 + ( _Time.y * _WindMovement )).xy*_WindDensity );
			simplePerlin2D321 = simplePerlin2D321*0.5 + 0.5;
			float4 appendResult329 = (float4(( ( ( ( simplePerlin2D321 - 0.5 ) / 10.0 ) * _WindStrength ) + ase_vertex4Pos.x ) , ase_vertex4Pos.y , ase_vertex4Pos.z , 1.0));
			float4 lerpResult330 = lerp( ase_vertex4Pos , appendResult329 , ( ase_vertex4Pos.y * 2.0 ));
			float4 transform331 = mul(unity_WorldToObject,float4( _WorldSpaceCameraPos , 0.0 ));
			float4 temp_cast_2 = (transform331.w).xxxx;
			#ifdef _CUSTOMWIND_ON
				float4 staticSwitch333 = ( lerpResult330 - temp_cast_2 );
			#else
				float4 staticSwitch333 = ase_vertex4Pos;
			#endif
			float4 WIND393 = staticSwitch333;
			v.vertex.xyz = WIND393.xyz;
			v.vertex.w = 1;
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !defined(DIRECTIONAL)
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_BASETEXTURE2 = i.uv_texcoord;
			float4 tex2DNode2 = tex2D( _BASETEXTURE, uv_BASETEXTURE2 );
			float grayscale313 = dot(tex2DNode2.rgb, float3(0.299,0.587,0.114));
			float2 temp_cast_1 = (0.5).xx;
			float2 uv_TexCoord204 = i.uv_texcoord + temp_cast_1;
			float2 temp_cast_2 = (0.0).xx;
			float2 uv_TexCoord235 = i.uv_texcoord + temp_cast_2;
			float temp_output_238_0 = ( step( uv_TexCoord204.x , 1.0 ) + step( uv_TexCoord235.y , 0.5 ) );
			float clampResult248 = clamp( ( 1.0 - temp_output_238_0 ) , 0.0 , 1.0 );
			float FLOWERMASK395 = clampResult248;
			float4 temp_cast_3 = (( grayscale313 * FLOWERMASK395 )).xxxx;
			float4 blendOpSrc310 = _FLOWERSCOLOR;
			float4 blendOpDest310 = temp_cast_3;
			float4 lerpBlendMode310 = lerp(blendOpDest310,( blendOpSrc310 * blendOpDest310 ),FLOWERMASK395);
			float4 lerpResult341 = lerp( tex2DNode2 , ( saturate( lerpBlendMode310 )) , FLOWERMASK395);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float clampResult354 = clamp( pow( ( (0.5 + (ase_vertex3Pos.y - 0.0) * (2.0 - 0.5) / (1.0 - 0.0)) * _Gradient ) , _GradientPower1 ) , 0.0 , 1.0 );
			float4 lerpResult356 = lerp( _GroundColor , _TopColor , clampResult354);
			float4 GRADIENT380 = lerpResult356;
			float4 temp_cast_4 = (temp_output_238_0).xxxx;
			float4 lerpResult205 = lerp( GRADIENT380 , temp_cast_4 , FLOWERMASK395);
			float4 GRADIENTMASK402 = lerpResult205;
			float4 temp_cast_5 = (grayscale313).xxxx;
			float4 lerpResult362 = lerp( temp_cast_5 , (( _CUSTOMFLOWERSCOLOR )?( lerpResult341 ):( tex2DNode2 )) , FLOWERMASK395);
			float4 blendOpSrc232 = GRADIENTMASK402;
			float4 blendOpDest232 = lerpResult362;
			float clampResult281 = clamp( temp_output_238_0 , 0.0 , 1.0 );
			float FLOWERMASKINVERT398 = clampResult281;
			float4 lerpBlendMode232 = lerp(blendOpDest232,(( blendOpDest232 > 0.5 ) ? ( 1.0 - 2.0 * ( 1.0 - blendOpDest232 ) * ( 1.0 - blendOpSrc232 ) ) : ( 2.0 * blendOpDest232 * blendOpSrc232 ) ),FLOWERMASKINVERT398);
			float4 FINALCOLOR400 = (( _CUSTOMCOLORSTINTING )?( lerpBlendMode232 ):( (( _CUSTOMFLOWERSCOLOR )?( lerpResult341 ):( tex2DNode2 )) ));
			o.Albedo = FINALCOLOR400.rgb;
			float temp_output_407_0 = 0.0;
			o.Metallic = temp_output_407_0;
			o.Smoothness = temp_output_407_0;
			#ifdef _TRANSLUCENCYONOFF_ON
				float4 staticSwitch390 = ( FINALCOLOR400 * 1.0 );
			#else
				float4 staticSwitch390 = float4( 0,0,0,0 );
			#endif
			float4 TRANSLUCENCY391 = staticSwitch390;
			o.Translucency = TRANSLUCENCY391.rgb;
			o.Alpha = 1;
			float ALPHA382 = tex2DNode2.a;
			float TRANSPARENCY384 = ( 1.0 - step( ALPHA382 , ( 1.0 - _LeavesThickness ) ) );
			clip( TRANSPARENCY384 - _MaskClipValue );
		}

		ENDCG
	}
	Fallback "Diffuse"
}

