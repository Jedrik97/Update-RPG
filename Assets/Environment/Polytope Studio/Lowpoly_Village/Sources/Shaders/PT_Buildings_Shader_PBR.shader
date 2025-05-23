

Shader "Polytope Studio/ PT_Medieval Buildings Shader PBR"
{
	Properties
	{
		[HDR][Header(WALLS )]_Exteriorwalls1colour("Exterior walls 1 colour", Color) = (0.6792453,0.6010633,0.5863296,1)
		[HDR]_Exteriorwalls2color("Exterior walls 2 color", Color) = (0.3524386,0.6133218,0.754717,1)
		[HDR]_Exteriorwalls3color("Exterior walls 3 color", Color) = (0.8867924,0.6561894,0.23843,1)
		[HDR]_Interiorwallscolor("Interior walls color", Color) = (0.4127358,0.490063,0.5,0)
		[Header(EXTERIOR WALLS  DETAILS)][Toggle(_EXTERIORTEXTUREONOFF_ON)] _ExteriortextureOnOff("Exterior texture On/Off", Float) = 0
		[NoScaleOffset]_Exteriorwallstexture("Exterior walls texture", 2D) = "white" {}
		_Exteriorwallstiling("Exterior walls tiling", Range( 0 , 1)) = 0
		[Header(INTERIOR WALLS  DETAILS)][Toggle(_INTERIORTEXTUREONOFF_ON)] _InteriortextureOnOff("Interior texture On/Off", Float) = 0
		[NoScaleOffset]_Interiorwallstexture("Interior walls texture", 2D) = "white" {}
		_Interiorwallstiling("Interior walls tiling", Range( 0 , 1)) = 0
		[HDR][Header(WOOD)]_Wood1color("Wood 1 color", Color) = (0.4056604,0.2683544,0.135858,1)
		[HDR]_Wood2color("Wood 2 color", Color) = (0.1981132,0.103908,0.06634924,1)
		[HDR]_Wood3color("Wood 3 color", Color) = (0.5377358,0.4531547,0.377937,1)
		[HDR][Header(FABRICS)]_Fabric1color("Fabric 1 color", Color) = (0.5849056,0.5418971,0.4331613,0)
		[HDR]_Fabric2color("Fabric 2 color", Color) = (0.3649431,0.5566038,0.4386422,0)
		[HDR]_Fabric3color("Fabric 3 color", Color) = (0.5450981,0.6936808,0.6980392,0)
		[HDR][Header(ROCK )]_Rock1color("Rock 1 color", Color) = (0.4127358,0.490063,0.5,0)
		[HDR]_Rock2color("Rock 2 color", Color) = (0.3679245,0.2968027,0.1787558,0)
		[HDR][Header(CERAMIC TILES)]_Ceramictiles1color("Ceramic tiles 1 color", Color) = (0.3207547,0.04869195,0.01059096,1)
		_Ceramic1smoothness("Ceramic 1 smoothness", Range( 0 , 1)) = 0.3
		[HDR]_Ceramictiles2color("Ceramic tiles 2 color", Color) = (0.7924528,0.3776169,0.1682093,1)
		_Ceramic2smoothness("Ceramic 2 smoothness", Range( 0 , 1)) = 0.3
		[HDR]_Ceramictiles3color("Ceramic tiles 3 color ", Color) = (0.4677838,0.3813261,0.2501584,1)
		_Ceramic3smoothness("Ceramic 3 smoothness", Range( 0 , 1)) = 0.3
		[HDR][Header(METAL)]_Metal1color("Metal 1 color", Color) = (0.385947,0.5532268,0.5566038,0)
		_Metal1metallic("Metal 1 metallic", Range( 0 , 1)) = 0.65
		_Metal1smootness("Metal 1 smootness", Range( 0 , 1)) = 0.7
		[HDR]_Metal2color("Metal 2 color", Color) = (2,0.5960785,0,0)
		_Metal2metallic("Metal 2 metallic", Range( 0 , 1)) = 0.65
		_Metal2smootness("Metal 2 smootness", Range( 0 , 1)) = 0.7
		[HDR]_Metal3color("Metal 3 color", Color) = (1.584906,0.8017758,0,1)
		_Metal3metallic("Metal 3 metallic", Range( 0 , 1)) = 0.65
		_Metal3smootness("Metal 3 smootness", Range( 0 , 1)) = 0.7
		[HDR][Header(OTHER COLORS)]_Ropecolor("Rope color", Color) = (0.6037736,0.5810711,0.3389106,1)
		[HDR]_Haycolor("Hay color", Color) = (0.764151,0.517899,0.1622019,1)
		[HDR]_Mortarcolor("Mortar color", Color) = (0.6415094,0.5745595,0.4629761,0)
		[HDR]_Coatofarmscolor("Coat of arms color", Color) = (1,0,0,0)
		[NoScaleOffset]_Coarofarmstexture("Coar of arms texture", 2D) = "black" {}
		[Toggle]_MetallicOn("Metallic On", Float) = 1
		[Toggle]_SmoothnessOn("Smoothness On", Float) = 1
		[HideInInspector][Gamma]_Transparency("Transparency", Range( 0 , 1)) = 1
		[HideInInspector]_TextureSample2("Texture Sample 2", 2D) = "white" {}
		[HideInInspector][NoScaleOffset]_TextureSample9("Texture Sample 9", 2D) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		AlphaToMask On
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.5
		#pragma shader_feature_local _EXTERIORTEXTUREONOFF_ON
		#pragma shader_feature_local _INTERIORTEXTUREONOFF_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
			float3 worldPos;
			half3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _TextureSample2;
		uniform half4 _TextureSample2_ST;
		uniform half4 _Interiorwallscolor;
		uniform sampler2D _TextureSample9;
		uniform half4 _Mortarcolor;
		uniform half4 _Rock1color;
		uniform half4 _Rock2color;
		uniform half4 _Fabric1color;
		uniform half4 _Fabric2color;
		uniform half4 _Fabric3color;
		uniform half4 _Exteriorwalls1colour;
		uniform half4 _Exteriorwalls2color;
		uniform half4 _Exteriorwalls3color;
		uniform half4 _Wood1color;
		uniform half4 _Wood2color;
		uniform half4 _Wood3color;
		uniform half4 _Ceramictiles1color;
		uniform half4 _Ceramictiles2color;
		uniform half4 _Ceramictiles3color;
		uniform half4 _Ropecolor;
		uniform half4 _Haycolor;
		uniform half4 _Metal1color;
		uniform half4 _Metal2color;
		uniform half4 _Metal3color;
		uniform half4 _Coatofarmscolor;
		uniform sampler2D _Coarofarmstexture;
		uniform sampler2D _Interiorwallstexture;
		uniform half _Interiorwallstiling;
		uniform sampler2D _Exteriorwallstexture;
		uniform half _Exteriorwallstiling;
		uniform half _MetallicOn;
		uniform half _Metal1metallic;
		uniform half _Metal2metallic;
		uniform half _Metal3metallic;
		uniform half _SmoothnessOn;
		uniform half _Ceramic1smoothness;
		uniform half _Ceramic2smoothness;
		uniform half _Ceramic3smoothness;
		uniform half _Metal1smootness;
		uniform half _Metal2smootness;
		uniform half _Metal3smootness;
		uniform float _Transparency;


		inline float4 TriplanarSampling322( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			float negProjNormalY = max( 0, projNormal.y * -nsign.y );
			projNormal.y = max( 0, projNormal.y * nsign.y );
			half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
			xNorm  = tex2D( midTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm  = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			yNormN = tex2D( botTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm  = tex2D( midTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + yNormN * negProjNormalY + zNorm * projNormal.z;
		}


		inline float4 TriplanarSampling298( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			float negProjNormalY = max( 0, projNormal.y * -nsign.y );
			projNormal.y = max( 0, projNormal.y * nsign.y );
			half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
			xNorm  = tex2D( midTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm  = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			yNormN = tex2D( botTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm  = tex2D( midTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + yNormN * negProjNormalY + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_TextureSample2 = i.uv_texcoord * _TextureSample2_ST.xy + _TextureSample2_ST.zw;
			half4 BASETEXTURE243 = tex2D( _TextureSample2, uv_TextureSample2 );
			half4 color315 = IsGammaSpace() ? half4(0.1607843,1,0,1) : half4(0.02217388,1,0,1);
			float2 uv_TextureSample9120 = i.uv_texcoord;
			half4 MASKTEXTURE251 = tex2D( _TextureSample9, uv_TextureSample9120 );
			half temp_output_310_0 = saturate( ( 1.0 - ( ( distance( color315.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) );
			half4 lerpResult309 = lerp( float4( 0,0,0,0 ) , ( BASETEXTURE243 * _Interiorwallscolor ) , temp_output_310_0);
			half4 color314 = IsGammaSpace() ? half4(0.4392157,0,0.4392157,1) : half4(0.1620294,0,0.1620294,1);
			half4 lerpResult313 = lerp( lerpResult309 , ( BASETEXTURE243 * _Mortarcolor ) , saturate( ( 1.0 - ( ( distance( color314.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) ));
			half4 color130 = IsGammaSpace() ? half4(0,0.4784314,0.4784314,1) : half4(0,0.1946179,0.1946179,1);
			half4 lerpResult132 = lerp( lerpResult313 , ( BASETEXTURE243 * _Rock1color ) , saturate( ( 1.0 - ( ( distance( color130.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) ));
			half4 color134 = IsGammaSpace() ? half4(0,1,0.7294118,1) : half4(0,1,0.4910209,1);
			half4 lerpResult133 = lerp( lerpResult132 , ( BASETEXTURE243 * _Rock2color ) , saturate( ( 1.0 - ( ( distance( color134.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) ));
			half4 color121 = IsGammaSpace() ? half4(0.6196079,0.9333334,1,1) : half4(0.3419145,0.8549928,1,1);
			half4 lerpResult124 = lerp( lerpResult133 , ( BASETEXTURE243 * _Fabric1color ) , saturate( ( 1.0 - ( ( distance( color121.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) ));
			half4 color138 = IsGammaSpace() ? half4(0.9333334,1,0.6196079,1) : half4(0.8549928,1,0.3419145,1);
			half4 lerpResult126 = lerp( lerpResult124 , ( BASETEXTURE243 * _Fabric2color ) , saturate( ( 1.0 - ( ( distance( color138.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) ));
			half4 color136 = IsGammaSpace() ? half4(1,0.6196079,0.9333334,1) : half4(1,0.3419145,0.8549928,1);
			half4 lerpResult9 = lerp( lerpResult126 , ( BASETEXTURE243 * _Fabric3color ) , saturate( ( 1.0 - ( ( distance( color136.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) ));
			half4 color107 = IsGammaSpace() ? half4(1,1,0,1) : half4(1,1,0,1);
			half temp_output_7_0 = saturate( ( 1.0 - ( ( distance( color107.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) );
			half4 lerpResult10 = lerp( lerpResult9 , ( BASETEXTURE243 * _Exteriorwalls1colour ) , temp_output_7_0);
			half4 color106 = IsGammaSpace() ? half4(1,0,1,1) : half4(1,0,1,1);
			half temp_output_11_0 = saturate( ( 1.0 - ( ( distance( color106.rgb , MASKTEXTURE251.rgb ) - 0.0 ) / max( 0.0 , 1E-05 ) ) ) );
			half4 lerpResult15 = lerp( lerpResult10 , ( BASETEXTURE243 * _Exteriorwalls2color ) , temp_output_11_0);
			half4 color105 = IsGammaSpace() ? half4(0,1,1,1) : half4(0,1,1,1);
			half temp_output_13_0 = saturate( ( 1.0 - ( ( distance( color105.rgb , MASKTEXTURE251.rgb ) - 0.0 ) / max( 0.0 , 1E-05 ) ) ) );
			half4 lerpResult19 = lerp( lerpResult15 , ( BASETEXTURE243 * _Exteriorwalls3color ) , temp_output_13_0);
			half4 color103 = IsGammaSpace() ? half4(0.6862745,0.8352942,0.8352942,1) : half4(0.4286906,0.6653875,0.6653875,1);
			half4 lerpResult22 = lerp( lerpResult19 , ( BASETEXTURE243 * _Wood1color ) , saturate( ( 1.0 - ( ( distance( color103.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) ));
			half4 color104 = IsGammaSpace() ? half4(1,0.7294118,0,1) : half4(1,0.4910209,0,1);
			half4 lerpResult27 = lerp( lerpResult22 , ( BASETEXTURE243 * _Wood2color ) , saturate( ( 1.0 - ( ( distance( color104.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) ));
			half4 color102 = IsGammaSpace() ? half4(0.7294118,0,1,1) : half4(0.4910209,0,1,1);
			half4 lerpResult32 = lerp( lerpResult27 , ( BASETEXTURE243 * _Wood3color ) , saturate( ( 1.0 - ( ( distance( color102.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) ));
			half4 color17 = IsGammaSpace() ? half4(0.3960785,0.2627451,0.02352941,1) : half4(0.1301365,0.05612849,0.001821162,1);
			half temp_output_92_0 = saturate( ( 1.0 - ( ( distance( color17.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) );
			half4 lerpResult91 = lerp( lerpResult32 , ( BASETEXTURE243 * _Ceramictiles1color ) , temp_output_92_0);
			half4 color24 = IsGammaSpace() ? half4(0.5372549,0.4313726,0.2509804,1) : half4(0.2501584,0.1559265,0.05126947,1);
			half temp_output_95_0 = saturate( ( 1.0 - ( ( distance( color24.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) );
			half4 lerpResult93 = lerp( lerpResult91 , ( BASETEXTURE243 * _Ceramictiles2color ) , temp_output_95_0);
			half4 color94 = IsGammaSpace() ? half4(0.7137255,0.6509804,0.5372549,1) : half4(0.4677838,0.3813261,0.2501584,1);
			half temp_output_96_0 = saturate( ( 1.0 - ( ( distance( color94.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) );
			half4 lerpResult39 = lerp( lerpResult93 , ( BASETEXTURE243 * _Ceramictiles3color ) , temp_output_96_0);
			half4 color84 = IsGammaSpace() ? half4(0,0.1294118,0.5058824,1) : half4(0,0.01520852,0.2195262,1);
			half4 lerpResult71 = lerp( lerpResult39 , ( BASETEXTURE243 * _Ropecolor ) , saturate( ( 1.0 - ( ( distance( color84.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) ));
			half4 color85 = IsGammaSpace() ? half4(1,0.4313726,0.5254902,1) : half4(1,0.1559265,0.2383977,1);
			half4 lerpResult72 = lerp( lerpResult71 , ( BASETEXTURE243 * _Haycolor ) , saturate( ( 1.0 - ( ( distance( color85.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) ));
			half4 color38 = IsGammaSpace() ? half4(0.8274511,0.8784314,0.6980392,1) : half4(0.6514059,0.7454044,0.4452012,1);
			half temp_output_41_0 = saturate( ( 1.0 - ( ( distance( color38.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) );
			half4 lerpResult47 = lerp( lerpResult72 , ( BASETEXTURE243 * _Metal1color ) , temp_output_41_0);
			half4 color117 = IsGammaSpace() ? half4(0.6392157,0.6784314,0.5411765,1) : half4(0.3662527,0.4178852,0.2541522,1);
			half temp_output_116_0 = saturate( ( 1.0 - ( ( distance( color117.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) );
			half4 lerpResult150 = lerp( lerpResult47 , ( BASETEXTURE243 * _Metal2color ) , temp_output_116_0);
			half4 color118 = IsGammaSpace() ? half4(0.4627451,0.4901961,0.3921569,1) : half4(0.1811642,0.2050788,0.1274377,1);
			half temp_output_149_0 = saturate( ( 1.0 - ( ( distance( color118.rgb , MASKTEXTURE251.rgb ) - 0.1 ) / max( 0.0 , 1E-05 ) ) ) );
			half4 lerpResult151 = lerp( lerpResult150 , ( BASETEXTURE243 * _Metal3color ) , temp_output_149_0);
			float2 uv1_Coarofarmstexture157 = i.uv2_texcoord2;
			half temp_output_49_0 = ( 1.0 - tex2D( _Coarofarmstexture, uv1_Coarofarmstexture157 ).a );
			half4 temp_cast_42 = (temp_output_49_0).xxxx;
			half4 temp_output_1_0_g169 = temp_cast_42;
			half4 color54 = IsGammaSpace() ? half4(0,0,0,0) : half4(0,0,0,0);
			half4 temp_output_2_0_g169 = color54;
			half temp_output_11_0_g169 = distance( temp_output_1_0_g169 , temp_output_2_0_g169 );
			half2 _Vector0 = half2(1.6,1);
			half4 lerpResult21_g169 = lerp( _Coatofarmscolor , temp_output_1_0_g169 , saturate( ( ( temp_output_11_0_g169 - _Vector0.x ) / max( _Vector0.y , 1E-05 ) ) ));
			half temp_output_156_0 = ( 1.0 - temp_output_49_0 );
			half4 lerpResult165 = lerp( lerpResult151 , lerpResult21_g169 , temp_output_156_0);
			half2 temp_cast_43 = (_Interiorwallstiling).xx;
			float3 ase_worldPos = i.worldPos;
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float4 triplanar322 = TriplanarSampling322( _Interiorwallstexture, _Interiorwallstexture, _Interiorwallstexture, ase_worldPos, ase_worldNormal, 1.0, temp_cast_43, float3( 1,1,1 ), float3(0,0,0) );
			float4 INDETAILTEXTUREvar323 = triplanar322;
			half4 blendOpSrc325 = INDETAILTEXTUREvar323;
			half4 blendOpDest325 = lerpResult165;
			half INTWALLSMASK329 = temp_output_310_0;
			half4 lerpBlendMode325 = lerp(blendOpDest325,( blendOpSrc325 * blendOpDest325 ),INTWALLSMASK329);
			#ifdef _INTERIORTEXTUREONOFF_ON
				half4 staticSwitch328 = ( saturate( lerpBlendMode325 ));
			#else
				half4 staticSwitch328 = lerpResult165;
			#endif
			half2 temp_cast_45 = ((0.1 + (_Exteriorwallstiling - 0.0) * (0.4 - 0.1) / (1.0 - 0.0))).xx;
			float4 triplanar298 = TriplanarSampling298( _Exteriorwallstexture, _Exteriorwallstexture, _Exteriorwallstexture, ase_worldPos, ase_worldNormal, 10.0, temp_cast_45, float3( 1,1,1 ), float3(0,0,0) );
			half4 OUTDETAILTEXTUREvar299 = triplanar298;
			half4 blendOpSrc231 = OUTDETAILTEXTUREvar299;
			half4 blendOpDest231 = staticSwitch328;
			half WALLSMASK227 = ( temp_output_7_0 + temp_output_11_0 + temp_output_13_0 );
			half4 lerpBlendMode231 = lerp(blendOpDest231,( blendOpSrc231 * blendOpDest231 ),WALLSMASK227);
			#ifdef _EXTERIORTEXTUREONOFF_ON
				half4 staticSwitch266 = ( saturate( lerpBlendMode231 ));
			#else
				half4 staticSwitch266 = staticSwitch328;
			#endif
			o.Albedo = staticSwitch266.rgb;
			half lerpResult110 = lerp( 0.0 , _Metal1metallic , temp_output_41_0);
			half lerpResult112 = lerp( lerpResult110 , _Metal2metallic , temp_output_116_0);
			half lerpResult113 = lerp( lerpResult112 , _Metal3metallic , temp_output_149_0);
			half lerpResult55 = lerp( lerpResult113 , 0.0 , temp_output_156_0);
			o.Metallic = (( _MetallicOn )?( lerpResult55 ):( 0.0 ));
			half lerpResult26 = lerp( 0.0 , _Ceramic1smoothness , temp_output_92_0);
			half lerpResult31 = lerp( lerpResult26 , _Ceramic2smoothness , temp_output_95_0);
			half lerpResult34 = lerp( lerpResult31 , _Ceramic3smoothness , temp_output_96_0);
			half lerpResult42 = lerp( lerpResult34 , _Metal1smootness , temp_output_41_0);
			half lerpResult43 = lerp( lerpResult42 , _Metal2smootness , temp_output_116_0);
			half lerpResult46 = lerp( lerpResult43 , _Metal3smootness , 0.0);
			o.Smoothness = (( _SmoothnessOn )?( lerpResult46 ):( 0.0 ));
			o.Alpha = _Transparency;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			AlphaToMask Off
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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack1.zw = customInputData.uv2_texcoord2;
				o.customPack1.zw = v.texcoord1;
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
				surfIN.uv2_texcoord2 = IN.customPack1.zw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}

