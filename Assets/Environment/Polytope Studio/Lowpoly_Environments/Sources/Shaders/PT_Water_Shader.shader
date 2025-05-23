

Shader "Polytope Studio/PT_Water_Shader"
{
	Properties
	{
		_DeepColor("Deep Color", Color) = (0.3114988,0.5266015,0.5283019,0)
		_ShallowColor("Shallow Color", Color) = (0.5238074,0.7314408,0.745283,0)
		_Depth("Depth", Range( 0 , 1)) = 0.3
		_DepthStrength("Depth Strength", Range( 0 , 1)) = 0.3
		_Smootness("Smootness", Range( 0 , 1)) = 1
		_Mettalic("Mettalic", Range( 0 , 1)) = 1
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 5
		_WaveSpeed("Wave Speed", Range( 0 , 1)) = 0.5
		_WaveTile("Wave Tile", Range( 0 , 0.9)) = 0.5
		_WaveAmplitude("Wave Amplitude", Range( 0 , 1)) = 0.2
		[NoScaleOffset][Normal]_NormalMapTexture("Normal Map Texture ", 2D) = "bump" {}
		_NormalMapWavesSpeed("Normal Map Waves Speed", Range( 0 , 1)) = 0.1
		_NormalMapsWavesSize("Normal Maps Waves Size", Range( 0 , 10)) = 5
		_FoamColor("Foam Color", Color) = (0.3066038,1,0.9145772,0)
		_FoamAmount("Foam Amount", Range( 0 , 10)) = 1.5
		_FoamPower("Foam Power", Range( 0.1 , 5)) = 0.5
		_FoamNoiseScale("Foam Noise Scale", Range( 0 , 1000)) = 150
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Off
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 5.0
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Standard alpha:fade keepalpha noshadow vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform float _WaveAmplitude;
		uniform float _WaveSpeed;
		uniform float _WaveTile;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _FoamAmount;
		uniform float _FoamPower;
		uniform float _FoamNoiseScale;
		uniform sampler2D _NormalMapTexture;
		uniform float _NormalMapsWavesSize;
		uniform float _NormalMapWavesSpeed;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float4 _ShallowColor;
		uniform float4 _DeepColor;
		uniform float _DepthStrength;
		uniform float _Depth;
		uniform float4 _FoamColor;
		uniform float _Mettalic;
		uniform float _Smootness;
		uniform float _TessValue;


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


		float2 UnityGradientNoiseDir( float2 p )
		{
			p = fmod(p , 289);
			float x = fmod((34 * p.x + 1) * p.x , 289) + p.y;
			x = fmod( (34 * x + 1) * x , 289);
			x = frac( x / 41 ) * 2 - 1;
			return normalize( float2(x - floor(x + 0.5 ), abs( x ) - 0.5 ) );
		}
		
		float UnityGradientNoise( float2 UV, float Scale )
		{
			float2 p = UV * Scale;
			float2 ip = floor( p );
			float2 fp = frac( p );
			float d00 = dot( UnityGradientNoiseDir( ip ), fp );
			float d01 = dot( UnityGradientNoiseDir( ip + float2( 0, 1 ) ), fp - float2( 0, 1 ) );
			float d10 = dot( UnityGradientNoiseDir( ip + float2( 1, 0 ) ), fp - float2( 1, 0 ) );
			float d11 = dot( UnityGradientNoiseDir( ip + float2( 1, 1 ) ), fp - float2( 1, 1 ) );
			fp = fp * fp * fp * ( fp * ( fp * 6 - 15 ) + 10 );
			return lerp( lerp( d00, d01, fp.y ), lerp( d10, d11, fp.y ), fp.x ) + 0.5;
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		float4 tessFunction( )
		{
			return _TessValue;
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float4 appendResult153 = (float4(0.23 , -0.8 , 0.0 , 0.0));
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float4 appendResult156 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float2 panner145 = ( ( _Time.y * _WaveSpeed ) * appendResult153.xy + ( ( appendResult156 * float4( float2( 6.5,0.9 ), 0.0 , 0.0 ) ) * _WaveTile ).xy);
			float simplePerlin2D143 = snoise( panner145 );
			simplePerlin2D143 = simplePerlin2D143*0.5 + 0.5;
			float WAVESDISPLACEMENT245 = ( ( float3(0,0.05,0).y * _WaveAmplitude ) * simplePerlin2D143 );
			float3 temp_cast_3 = (WAVESDISPLACEMENT245).xxx;
			v.vertex.xyz += temp_cast_3;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth434 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth434 = abs( ( screenDepth434 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _FoamAmount ) );
			float saferPower436 = abs( distanceDepth434 );
			float temp_output_436_0 = pow( saferPower436 , _FoamPower );
			float2 temp_cast_0 = (_FoamNoiseScale).xx;
			float2 temp_cast_1 = (( _Time.y * 0.2 )).xx;
			float2 uv_TexCoord433 = i.uv_texcoord * temp_cast_0 + temp_cast_1;
			float gradientNoise437 = UnityGradientNoise(uv_TexCoord433,1.0);
			gradientNoise437 = gradientNoise437*0.5 + 0.5;
			float temp_output_471_0 = step( temp_output_436_0 , gradientNoise437 );
			float FoamMask439 = temp_output_471_0;
			float4 appendResult405 = (float4(_NormalMapsWavesSize , _NormalMapsWavesSize , 0.0 , 0.0));
			float mulTime251 = _Time.y * 0.1;
			float2 temp_cast_3 = (( mulTime251 * _NormalMapWavesSpeed )).xx;
			float2 uv_TexCoord254 = i.uv_texcoord * appendResult405.xy + temp_cast_3;
			float2 temp_output_2_0_g9 = uv_TexCoord254;
			float2 break6_g9 = temp_output_2_0_g9;
			float temp_output_25_0_g9 = ( pow( 0.5 , 3.0 ) * 0.1 );
			float2 appendResult8_g9 = (float2(( break6_g9.x + temp_output_25_0_g9 ) , break6_g9.y));
			float4 tex2DNode14_g9 = tex2D( _NormalMapTexture, temp_output_2_0_g9 );
			float temp_output_4_0_g9 = 1.0;
			float3 appendResult13_g9 = (float3(1.0 , 0.0 , ( ( tex2D( _NormalMapTexture, appendResult8_g9 ).g - tex2DNode14_g9.g ) * temp_output_4_0_g9 )));
			float2 appendResult9_g9 = (float2(break6_g9.x , ( break6_g9.y + temp_output_25_0_g9 )));
			float3 appendResult16_g9 = (float3(0.0 , 1.0 , ( ( tex2D( _NormalMapTexture, appendResult9_g9 ).g - tex2DNode14_g9.g ) * temp_output_4_0_g9 )));
			float3 normalizeResult22_g9 = normalize( cross( appendResult13_g9 , appendResult16_g9 ) );
			float3 NORMALMAPWAVES243 = normalizeResult22_g9;
			float4 color478 = IsGammaSpace() ? float4(0.4980392,0.4980392,1,0) : float4(0.2122307,0.2122307,1,0);
			float layeredBlendVar477 = FoamMask439;
			float4 layeredBlend477 = ( lerp( float4( NORMALMAPWAVES243 , 0.0 ),color478 , layeredBlendVar477 ) );
			float4 normalizeResult474 = normalize( layeredBlend477 );
			o.Normal = normalizeResult474.rgb;
			float screenDepth350 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth350 = abs( ( screenDepth350 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 100.0 ) );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor314 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( (ase_grabScreenPosNorm).xyzw + float4( ( NORMALMAPWAVES243 * 1.0 ) , 0.0 ) ).xy);
			float4 FAKEREFRACTIONS415 = ( ( 1.0 - distanceDepth350 ) * screenColor314 );
			float eyeDepth64 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float clampResult81 = clamp( ( _DepthStrength * ( eyeDepth64 - ( ase_screenPos.w + _Depth ) ) ) , 0.0 , 1.0 );
			float4 lerpResult86 = lerp( _ShallowColor , _DeepColor , clampResult81);
			float4 DeepShallowColor196 = lerpResult86;
			float4 lerpResult470 = lerp( FAKEREFRACTIONS415 , DeepShallowColor196 , float4( 0.6132076,0.6132076,0.6132076,0 ));
			float4 FoamColor442 = ( temp_output_471_0 * _FoamColor );
			o.Albedo = ( lerpResult470 + FoamColor442 ).rgb;
			o.Metallic = _Mettalic;
			float4 temp_cast_9 = (_Smootness).xxxx;
			float4 color484 = IsGammaSpace() ? float4(0.2264151,0.2264151,0.2264151,0) : float4(0.04193995,0.04193995,0.04193995,0);
			float layeredBlendVar485 = FoamMask439;
			float4 layeredBlend485 = ( lerp( temp_cast_9,color484 , layeredBlendVar485 ) );
			o.Smoothness = layeredBlend485.r;
			float DeepShallowMask197 = clampResult81;
			float smoothstepResult400 = smoothstep( 0.2 , 1.2 , FoamMask439);
			float clampResult401 = clamp( ( smoothstepResult400 * 0.05 ) , 0.0 , 1.0 );
			float TRANSPARENCYFINAL267 = ( DeepShallowMask197 + (1.0 + (0.95 - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) + clampResult401 );
			o.Alpha = TRANSPARENCYFINAL267;
		}

		ENDCG
	}
}

