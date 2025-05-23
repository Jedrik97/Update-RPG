

Shader "Polytope Studio/PT_Vegetation_Opaque_Shader"
{
	Properties
	{
		[NoScaleOffset]_BaseTexture("Base Texture", 2D) = "white" {}
		[Toggle]_CUSTOMCOLORSTINTING("CUSTOM COLORS  TINTING", Float) = 0
		[HDR]_GroundColor("Ground Color", Color) = (0.08490568,0.05234205,0.04846032,1)
		[HDR]_TopColor("Top Color", Color) = (0.4811321,0.4036026,0.2382966,1)
		[HDR]_Gradient("Gradient ", Range( 0 , 1)) = 1
		_GradientPower("Gradient Power", Range( 0 , 10)) = 1
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.7748996
		[Toggle(_SNOWONOFF_ON)] _SNOWONOFF("SNOW ON/OFF", Float) = 0
		_SnowAmount("Snow Amount", Range( 0 , 1)) = 1
		_SnowCoverage("Snow Coverage", Range( 0 , 1)) = 0.45
		_SnowFade("Snow Fade", Range( 0 , 1)) = 0.83
		[Toggle(_CUSTOMWIND_ON)] _CUSTOMWIND("CUSTOM WIND", Float) = 1
		[Toggle(_WINDMASKONOFF_ON)] _WINDMASKONOFF("WIND MASK ON/OFF", Float) = 0
		_WindMovement("Wind Movement", Range( 0 , 10)) = 0.5
		_WindDensity("Wind Density", Range( 0 , 5)) = 3.3
		_WindStrength("Wind Strength", Range( 0 , 1)) = 0.3
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.5
		#pragma multi_compile_instancing
		#pragma shader_feature _CUSTOMWIND_ON
		#pragma shader_feature_local _WINDMASKONOFF_ON
		#pragma shader_feature_local _SNOWONOFF_ON
		#pragma multi_compile __ LOD_FADE_CROSSFADE
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float _WindMovement;
		uniform float _WindDensity;
		uniform float _WindStrength;
		uniform float _CUSTOMCOLORSTINTING;
		uniform sampler2D _BaseTexture;
		uniform float4 _GroundColor;
		uniform float4 _TopColor;
		uniform float _Gradient;
		uniform float _GradientPower;
		uniform float _SnowAmount;
		uniform float _SnowFade;
		uniform float _SnowCoverage;
		uniform float _Smoothness;


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


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_vertex4Pos = v.vertex;
			float simplePerlin2D622 = snoise( (ase_vertex4Pos*1.0 + ( _Time.y * _WindMovement )).xy*_WindDensity );
			simplePerlin2D622 = simplePerlin2D622*0.5 + 0.5;
			float4 appendResult632 = (float4(( ( ( ( simplePerlin2D622 - 0.5 ) / 10.0 ) * _WindStrength ) + ase_vertex4Pos.x ) , ase_vertex4Pos.y , ase_vertex4Pos.z , 1.0));
			#ifdef _WINDMASKONOFF_ON
				float staticSwitch633 = ( ( 1.0 - v.texcoord.xy.y ) * 5.0 );
			#else
				float staticSwitch633 = ase_vertex4Pos.y;
			#endif
			float4 lerpResult635 = lerp( ase_vertex4Pos , appendResult632 , staticSwitch633);
			float4 transform636 = mul(unity_WorldToObject,float4( _WorldSpaceCameraPos , 0.0 ));
			float4 temp_cast_2 = (transform636.w).xxxx;
			#ifdef _CUSTOMWIND_ON
				float4 staticSwitch638 = ( lerpResult635 - temp_cast_2 );
			#else
				float4 staticSwitch638 = ase_vertex4Pos;
			#endif
			float4 LOCALWIND639 = staticSwitch638;
			v.vertex.xyz = LOCALWIND639.xyz;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BaseTexture2 = i.uv_texcoord;
			float4 tex2DNode2 = tex2D( _BaseTexture, uv_BaseTexture2 );
			float4 ase_vertex4Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float clampResult555 = clamp( pow( ( (0.5 + (ase_vertex4Pos.y - 0.0) * (2.0 - 0.5) / (1.0 - 0.0)) * _Gradient ) , _GradientPower ) , 0.0 , 1.0 );
			float4 lerpResult557 = lerp( _GroundColor , _TopColor , clampResult555);
			float4 Gradient558 = lerpResult557;
			float grayscale180 = dot(tex2DNode2.rgb, float3(0.299,0.587,0.114));
			float saferPower568 = abs( grayscale180 );
			float4 temp_cast_1 = (pow( saferPower568 , 0.5 )).xxxx;
			float4 blendOpSrc18 = Gradient558;
			float4 blendOpDest18 = CalculateContrast(1.8,temp_cast_1);
			float2 temp_cast_2 = (0.5).xx;
			float2 uv_TexCoord578 = i.uv_texcoord + temp_cast_2;
			float clampResult596 = clamp( step( uv_TexCoord578.x , 1.0 ) , 0.0 , 1.0 );
			float2 temp_cast_3 = (0.0).xx;
			float2 uv_TexCoord577 = i.uv_texcoord + temp_cast_3;
			float clampResult585 = clamp( ( ( 1.0 - clampResult596 ) * step( uv_TexCoord577.y , 0.5 ) ) , 0.0 , 1.0 );
			float FRUITSMASK586 = clampResult585;
			float temp_output_594_0 = ( 1.0 - FRUITSMASK586 );
			float4 lerpBlendMode18 = lerp(blendOpDest18,( blendOpSrc18 * blendOpDest18 ),temp_output_594_0);
			float4 lerpResult595 = lerp( tex2DNode2 , ( saturate( lerpBlendMode18 )) , temp_output_594_0);
			float4 COLOR502 = (( _CUSTOMCOLORSTINTING )?( lerpResult595 ):( tex2DNode2 ));
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float4 color443 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			float fresnelNdotV454 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode454 = ( 0.11 + 1.0 * pow( 1.0 - fresnelNdotV454, color443.r ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float dotResult450 = dot( ase_normWorldNormal , float3(0,1,0) );
			float smoothstepResult531 = smoothstep( 0.0 , _SnowFade , ( dotResult450 + (-1.0 + (_SnowCoverage - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ));
			float SNOW489 = ( ( (0.0 + (_SnowAmount - 0.0) * (10.0 - 0.0) / (1.0 - 0.0)) * fresnelNode454 ) * smoothstepResult531 );
			#ifdef _SNOWONOFF_ON
				float4 staticSwitch372 = ( SNOW489 + COLOR502 );
			#else
				float4 staticSwitch372 = COLOR502;
			#endif
			o.Albedo = staticSwitch372.rgb;
			o.Smoothness = ( _Smoothness * ( 1.0 - FRUITSMASK586 ) );
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred dithercrossfade vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.5
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}

