// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/Environment/Water"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin]_MainColor("MainColor", Color) = (0,0,0,0)
		[Header(Foam)]_FoamScale("FoamScale", Float) = 0
		_FoamColor("FoamColor", Color) = (0,0,0,0)
		_TillingFoam("TillingFoam", Vector) = (0,0,0,0)
		_SpeedFoam("SpeedFoam", Vector) = (0,0,0,0)
		_Distance("Distance", Float) = 0
		[Header(Noise)]_DetailNoise("DetailNoise", Float) = 0
		[SingleLineTexture]_NoiseDetailTexture("NoiseDetailTexture", 2D) = "white" {}
		_SpeedDetailNoise("SpeedDetailNoise", Vector) = (0,0,0,0)
		_NoiseTilling("NoiseTilling", Vector) = (0,0,0,0)
		[Header(Collision)]_CollisionDistance("CollisionDistance", Float) = 1
		_CollisionFallOff("CollisionFallOff", Float) = 1
		[Header(Dots)]_DotsScale("DotsScale", Float) = 0
		_DotsAmmount("DotsAmmount", Range( 0 , 1)) = 0
		_FallOffDotsInteract("FallOffDotsInteract", Float) = 0
		_FlowDotsMask("FlowDotsMask", Range( 0 , 1)) = 0
		_FlowMapMask("FlowMapMask", Range( 0 , 1)) = 0
		_SpeedDots("SpeedDots", Vector) = (0,0,0,0)
		_TillingDots("TillingDots", Vector) = (0,0,0,0)
		[Toggle]_MoveInDirection("MoveInDirection", Range( 0 , 1)) = 0
		[Header(Refraction)][NoScaleOffset][SingleLineTexture]_RefractionTexture("RefractionTexture", 2D) = "bump" {}
		_RefractionStrength("RefractionStrength", Range( 0 , 1)) = 0
		[Header(FlowMapRefraction)][SingleLineTexture]_FlowTexture("FlowTexture", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[Header(Normal)][SingleLineTexture]_Normal("Normal", 2D) = "bump" {}
		_NormalScale("NormalScale", Range( 0 , 1)) = 0
		[Header(VertexOffset)][SingleLineTexture]_DisplacementMap("DisplacementMap", 2D) = "white" {}
		_SpeedOffset("SpeedOffset", Vector) = (0,0,0,0)
		_OffsetIntensity("OffsetIntensity", Float) = 0
		_Dir("Dir", Vector) = (0,0,0,0)
		[Header(InteractObjs)]_DotsInteract("DotsInteract", Range( 0 , 1)) = 0
		_InteractDotsScale("InteractDotsScale", Float) = 0
		_IntensityDotsInteract("IntensityDotsInteract", Float) = 0
		[Toggle(_VERTEXCOLOR_ON)] _VertexColor("VertexColor", Float) = 0
		[Header(Clouds)][Toggle]_AffectedByClouds("AffectedByClouds", Range( 0 , 1)) = 0
		[SingleLineTexture]_CloudsTexture("CloudsTexture", 2D) = "white" {}
		_IntensityClouds("IntensityClouds", Float) = 0
		_FallOffClouds("FallOffClouds", Float) = 0
		_TillingClouds("TillingClouds", Vector) = (0,0,0,0)
		[HDR]_CloudsLightColor1("CloudsLightColor", Color) = (0,0,0,0)
		[ASEEnd]_CloudsDarkColor1("CloudsDarkColor", Color) = (0.3773585,0.3773585,0.3773585,0)

		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		Cull Back
		AlphaToMask Off
		HLSLINCLUDE
		#pragma target 3.5

		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS

		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_FIXED_TESSELLATION
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1
			#define REQUIRE_OPAQUE_TEXTURE 1

			
			#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_SCREEN_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _VERTEXCOLOR_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				float4 ase_color : COLOR;
				float3 ase_normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _CloudsDarkColor1;
			float4 _FoamColor;
			float4 _MainColor;
			float4 _CloudsLightColor1;
			float3 _Dir;
			float2 _SpeedOffset;
			float2 _SpeedDots;
			float2 _SpeedDetailNoise;
			float2 _NoiseTilling;
			float2 _TillingDots;
			float2 _TillingFoam;
			float2 _TillingClouds;
			float2 _SpeedFoam;
			float _DotsInteract;
			float _InteractDotsScale;
			float _FallOffClouds;
			float _AffectedByClouds;
			float _NormalScale;
			float _FlowMapMask;
			float _RefractionStrength;
			float _IntensityClouds;
			float _MoveInDirection;
			float _DotsAmmount;
			float _DotsScale;
			float _Metallic;
			float _FoamScale;
			float _Distance;
			float _CollisionFallOff;
			float _CollisionDistance;
			float _DetailNoise;
			float _IntensityDotsInteract;
			float _FallOffDotsInteract;
			float _OffsetIntensity;
			float _FlowDotsMask;
			float _Smoothness;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _DisplacementMap;
			float4 positionsArray[40];
			sampler2D _NoiseDetailTexture;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _FlowTexture;
			sampler2D _CloudsTexture;
			sampler2D _Normal;
			sampler2D _RefractionTexture;


			float MyCustomExpression272( float3 WorldPos, float3 objectPosition )
			{
				float closest=10000;
				float now=0;
				for(int i=0; i<positionsArray.Length;i++){
					now = distance(WorldPos,positionsArray[i]);
					if(now < closest){
					closest = now;
					}
				}
				return closest;
			}
			
					float2 voronoihash34( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi34( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash34( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return (F2 + F1) * 0.5;
					}
			
					float2 voronoihash57( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi57( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash57( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash74( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi74( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash74( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash91( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi91( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash91( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash251( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi251( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash251( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 texCoord147 = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner145 = ( 1.0 * _Time.y * _SpeedOffset + texCoord147);
				float2 panner146 = ( 1.0 * _Time.y * ( _SpeedOffset * float2( -1,-1 ) ) + texCoord147);
				float3 temp_output_152_0 = ( ( tex2Dlod( _DisplacementMap, float4( panner145, 0, 0.0) ).r * tex2Dlod( _DisplacementMap, float4( panner146, 0, 0.0) ).r ) * v.ase_normal * _OffsetIntensity );
				float3 VertexOffset142 = temp_output_152_0;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 WorldPos272 = ase_worldPos;
				float3 objectPosition272 = positionsArray[0].xyz;
				float localMyCustomExpression272 = MyCustomExpression272( WorldPos272 , objectPosition272 );
				float temp_output_273_0 = saturate( ( pow( localMyCustomExpression272 , _FallOffDotsInteract ) * _IntensityDotsInteract ) );
				float CollisionRocks231 = temp_output_273_0;
				
				float3 vertexPos8 = v.vertex.xyz;
				float4 ase_clipPos8 = TransformObjectToHClip((vertexPos8).xyz);
				float4 screenPos8 = ComputeScreenPos(ase_clipPos8);
				o.ase_texcoord8 = screenPos8;
				
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));
				float eyeDepth = -objectToViewPos.z;
				o.ase_texcoord7.z = eyeDepth;
				
				o.ase_texcoord7.xy = v.texcoord.xy;
				o.ase_color = v.ase_color;
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord7.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( VertexOffset142 + ( CollisionRocks231 * _Dir ) );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag ( VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 texCoord37 = IN.ase_texcoord7.xy * _NoiseTilling + float2( 0,0 );
				float2 panner117 = ( 1.0 * _Time.y * _SpeedDetailNoise + texCoord37);
				float4 tex2DNode116 = tex2D( _NoiseDetailTexture, panner117 );
				float Noise49 = saturate( ( (0.0 + (tex2DNode116.r - 0.0) * (0.35 - 0.0) / (1.0 - 0.0)) + _DetailNoise ) );
				float4 screenPos8 = IN.ase_texcoord8;
				float4 ase_screenPosNorm8 = screenPos8 / screenPos8.w;
				ase_screenPosNorm8.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm8.z : ase_screenPosNorm8.z * 0.5 + 0.5;
				float screenDepth8 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm8.xy ),_ZBufferParams);
				float distanceDepth8 = saturate( abs( ( screenDepth8 - LinearEyeDepth( ase_screenPosNorm8.z,_ZBufferParams ) ) / ( _CollisionDistance ) ) );
				float saferPower11 = max( distanceDepth8 , 0.0001 );
				float Collision51 = saturate( pow( saferPower11 , _CollisionFallOff ) );
				float4 temp_output_30_0 = ( _MainColor * Collision51 );
				float4 ase_screenPosNorm = ScreenPos / ScreenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth32 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth32 = saturate( abs( ( screenDepth32 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Distance ) ) );
				float time34 = 0.0;
				float2 voronoiSmoothId0 = 0;
				float2 texCoord22 = IN.ase_texcoord7.xy * _TillingFoam + float2( 0,0 );
				float Depth82 = distanceDepth8;
				float2 appendResult81 = (float2(texCoord22.x , Depth82));
				float2 panner80 = ( 1.0 * _Time.y * _SpeedFoam + appendResult81);
				float2 coords34 = panner80 * _FoamScale;
				float2 id34 = 0;
				float2 uv34 = 0;
				float voroi34 = voronoi34( coords34, time34, id34, uv34, 0, voronoiSmoothId0 );
				float4 Foam54 = ( step( distanceDepth32 , voroi34 ) * _FoamColor );
				float time57 = 0.0;
				float2 texCoord58 = IN.ase_texcoord7.xy * _TillingDots + float2( 0,0 );
				float2 panner72 = ( 1.0 * _Time.y * _SpeedDots + texCoord58);
				float2 temp_output_12_0_g25 = panner72;
				float2 panner7_g25 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g25);
				float2 lerpResult1_g25 = lerp( (tex2D( _FlowTexture, panner7_g25 )).rg , temp_output_12_0_g25 , _FlowDotsMask);
				float2 coords57 = lerpResult1_g25 * _DotsScale;
				float2 id57 = 0;
				float2 uv57 = 0;
				float voroi57 = voronoi57( coords57, time57, id57, uv57, 0, voronoiSmoothId0 );
				float time74 = 0.0;
				float2 texCoord75 = IN.ase_texcoord7.xy * _TillingDots + float2( 0,0 );
				float2 panner76 = ( 1.0 * _Time.y * ( _SpeedDots * float2( -1,-1 ) ) + texCoord75);
				float2 temp_output_12_0_g24 = panner76;
				float2 panner7_g24 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g24);
				float2 lerpResult1_g24 = lerp( (tex2D( _FlowTexture, panner7_g24 )).rg , temp_output_12_0_g24 , _FlowDotsMask);
				float2 coords74 = lerpResult1_g24 * _DotsScale;
				float2 id74 = 0;
				float2 uv74 = 0;
				float voroi74 = voronoi74( coords74, time74, id74, uv74, 0, voronoiSmoothId0 );
				float time91 = 0.0;
				float2 panner92 = ( 1.0 * _Time.y * _SpeedDots + texCoord75);
				float2 temp_output_12_0_g26 = panner92;
				float2 panner7_g26 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g26);
				float2 lerpResult1_g26 = lerp( (tex2D( _FlowTexture, panner7_g26 )).rg , temp_output_12_0_g26 , _FlowDotsMask);
				float2 coords91 = lerpResult1_g26 * _DotsScale;
				float2 id91 = 0;
				float2 uv91 = 0;
				float voroi91 = voronoi91( coords91, time91, id91, uv91, 0, voronoiSmoothId0 );
				float lerpResult89 = lerp( ( voroi57 * voroi74 ) , voroi91 , _MoveInDirection);
				float time251 = 0.0;
				float2 DotsSpeed303 = _SpeedDots;
				float2 DotsTilling305 = _TillingDots;
				float2 texCoord252 = IN.ase_texcoord7.xy * DotsTilling305 + float2( 0,0 );
				float2 panner254 = ( 1.0 * _Time.y * DotsSpeed303 + texCoord252);
				float2 coords251 = panner254 * _InteractDotsScale;
				float2 id251 = 0;
				float2 uv251 = 0;
				float voroi251 = voronoi251( coords251, time251, id251, uv251, 0, voronoiSmoothId0 );
				float3 WorldPos272 = WorldPosition;
				float3 objectPosition272 = positionsArray[0].xyz;
				float localMyCustomExpression272 = MyCustomExpression272( WorldPos272 , objectPosition272 );
				float temp_output_273_0 = saturate( ( pow( localMyCustomExpression272 , _FallOffDotsInteract ) * _IntensityDotsInteract ) );
				float temp_output_315_0 = ( 1.0 - IN.ase_color.g );
				#ifdef _VERTEXCOLOR_ON
				float staticSwitch307 = temp_output_315_0;
				#else
				float staticSwitch307 = temp_output_273_0;
				#endif
				float FoamCollision259 = saturate( step( voroi251 , staticSwitch307 ) );
				float lerpResult278 = lerp( step( _DotsAmmount , lerpResult89 ) , step( _DotsInteract , lerpResult89 ) , FoamCollision259);
				float Dots56 = lerpResult278;
				float4 temp_output_71_0 = ( ( ( Noise49 * temp_output_30_0 ) + Foam54 ) + Dots56 );
				float4 Albedo244 = temp_output_71_0;
				float2 appendResult30_g29 = (float2(WorldPosition.x , WorldPosition.z));
				float2 temp_output_27_0_g29 = _TillingClouds;
				float2 temp_output_31_0_g29 = ( appendResult30_g29 * temp_output_27_0_g29 );
				float2 panner20_g29 = ( 1.0 * _Time.y * float2( -0.03,0 ) + temp_output_31_0_g29);
				float2 panner21_g29 = ( 1.0 * _Time.y * float2( 0.03,0 ) + temp_output_31_0_g29);
				float Clouds317 = saturate( pow( ( tex2D( _CloudsTexture, panner20_g29 ).r * tex2D( _CloudsTexture, panner21_g29 ).r * _IntensityClouds ) , _FallOffClouds ) );
				float4 temp_cast_1 = (Clouds317).xxxx;
				float4 lerpResult325 = lerp( Albedo244 , temp_cast_1 , _AffectedByClouds);
				float4 lerpResult324 = lerp( ( _CloudsDarkColor1 * Albedo244 ) , ( _CloudsLightColor1 * Albedo244 ) , lerpResult325);
				
				float2 texCoord136 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner137 = ( 1.0 * _Time.y * float2( -0.2,0 ) + texCoord136);
				float3 unpack134 = UnpackNormalScale( tex2D( _Normal, panner137 ), _NormalScale );
				unpack134.z = lerp( 1, unpack134.z, saturate(_NormalScale) );
				float3 Normal133 = ( unpack134 + IN.ase_normal );
				
				float2 temp_output_12_0_g27 = IN.ase_texcoord7.xy;
				float2 panner7_g27 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g27);
				float2 lerpResult1_g27 = lerp( (tex2D( _FlowTexture, panner7_g27 )).rg , temp_output_12_0_g27 , _FlowMapMask);
				float2 panner109 = ( 1.0 * _Time.y * float2( 0,0 ) + lerpResult1_g27);
				float eyeDepth = IN.ase_texcoord7.z;
				float eyeDepth28_g28 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float2 temp_output_20_0_g28 = ( (UnpackNormalScale( tex2D( _RefractionTexture, panner109 ), 1.0f )).xy * ( _RefractionStrength / max( eyeDepth , 0.1 ) ) * saturate( ( eyeDepth28_g28 - eyeDepth ) ) );
				float eyeDepth2_g28 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( float4( temp_output_20_0_g28, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ),_ZBufferParams);
				float2 temp_output_32_0_g28 = (( float4( ( temp_output_20_0_g28 * saturate( ( eyeDepth2_g28 - eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
				float4 fetchOpaqueVal106 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( temp_output_32_0_g28 ), 1.0 );
				float4 Refraction107 = ( fetchOpaqueVal106 * ( 1.0 - Depth82 ) );
				
				float3 Albedo = lerpResult324.rgb;
				float3 Normal = Normal133;
				float3 Emission = Refraction107.rgb;
				float3 Specular = 0.5;
				float Metallic = _Metallic;
				float Smoothness = _Smoothness;
				float Occlusion = 1;
				float Alpha = Collision51;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				
				inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.clipPos);
				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);

				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif

				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif

				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal,0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_FIXED_TESSELLATION
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _CloudsDarkColor1;
			float4 _FoamColor;
			float4 _MainColor;
			float4 _CloudsLightColor1;
			float3 _Dir;
			float2 _SpeedOffset;
			float2 _SpeedDots;
			float2 _SpeedDetailNoise;
			float2 _NoiseTilling;
			float2 _TillingDots;
			float2 _TillingFoam;
			float2 _TillingClouds;
			float2 _SpeedFoam;
			float _DotsInteract;
			float _InteractDotsScale;
			float _FallOffClouds;
			float _AffectedByClouds;
			float _NormalScale;
			float _FlowMapMask;
			float _RefractionStrength;
			float _IntensityClouds;
			float _MoveInDirection;
			float _DotsAmmount;
			float _DotsScale;
			float _Metallic;
			float _FoamScale;
			float _Distance;
			float _CollisionFallOff;
			float _CollisionDistance;
			float _DetailNoise;
			float _IntensityDotsInteract;
			float _FallOffDotsInteract;
			float _OffsetIntensity;
			float _FlowDotsMask;
			float _Smoothness;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _DisplacementMap;
			float4 positionsArray[40];
			uniform float4 _CameraDepthTexture_TexelSize;


			float MyCustomExpression272( float3 WorldPos, float3 objectPosition )
			{
				float closest=10000;
				float now=0;
				for(int i=0; i<positionsArray.Length;i++){
					now = distance(WorldPos,positionsArray[i]);
					if(now < closest){
					closest = now;
					}
				}
				return closest;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 texCoord147 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner145 = ( 1.0 * _Time.y * _SpeedOffset + texCoord147);
				float2 panner146 = ( 1.0 * _Time.y * ( _SpeedOffset * float2( -1,-1 ) ) + texCoord147);
				float3 temp_output_152_0 = ( ( tex2Dlod( _DisplacementMap, float4( panner145, 0, 0.0) ).r * tex2Dlod( _DisplacementMap, float4( panner146, 0, 0.0) ).r ) * v.ase_normal * _OffsetIntensity );
				float3 VertexOffset142 = temp_output_152_0;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 WorldPos272 = ase_worldPos;
				float3 objectPosition272 = positionsArray[0].xyz;
				float localMyCustomExpression272 = MyCustomExpression272( WorldPos272 , objectPosition272 );
				float temp_output_273_0 = saturate( ( pow( localMyCustomExpression272 , _FallOffDotsInteract ) * _IntensityDotsInteract ) );
				float CollisionRocks231 = temp_output_273_0;
				
				float3 vertexPos8 = v.vertex.xyz;
				float4 ase_clipPos8 = TransformObjectToHClip((vertexPos8).xyz);
				float4 screenPos8 = ComputeScreenPos(ase_clipPos8);
				o.ase_texcoord2 = screenPos8;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( VertexOffset142 + ( CollisionRocks231 * _Dir ) );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 screenPos8 = IN.ase_texcoord2;
				float4 ase_screenPosNorm8 = screenPos8 / screenPos8.w;
				ase_screenPosNorm8.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm8.z : ase_screenPosNorm8.z * 0.5 + 0.5;
				float screenDepth8 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm8.xy ),_ZBufferParams);
				float distanceDepth8 = saturate( abs( ( screenDepth8 - LinearEyeDepth( ase_screenPosNorm8.z,_ZBufferParams ) ) / ( _CollisionDistance ) ) );
				float saferPower11 = max( distanceDepth8 , 0.0001 );
				float Collision51 = saturate( pow( saferPower11 , _CollisionFallOff ) );
				
				float Alpha = Collision51;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}
		
		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_FIXED_TESSELLATION
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1
			#define REQUIRE_OPAQUE_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _VERTEXCOLOR_ON


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _CloudsDarkColor1;
			float4 _FoamColor;
			float4 _MainColor;
			float4 _CloudsLightColor1;
			float3 _Dir;
			float2 _SpeedOffset;
			float2 _SpeedDots;
			float2 _SpeedDetailNoise;
			float2 _NoiseTilling;
			float2 _TillingDots;
			float2 _TillingFoam;
			float2 _TillingClouds;
			float2 _SpeedFoam;
			float _DotsInteract;
			float _InteractDotsScale;
			float _FallOffClouds;
			float _AffectedByClouds;
			float _NormalScale;
			float _FlowMapMask;
			float _RefractionStrength;
			float _IntensityClouds;
			float _MoveInDirection;
			float _DotsAmmount;
			float _DotsScale;
			float _Metallic;
			float _FoamScale;
			float _Distance;
			float _CollisionFallOff;
			float _CollisionDistance;
			float _DetailNoise;
			float _IntensityDotsInteract;
			float _FallOffDotsInteract;
			float _OffsetIntensity;
			float _FlowDotsMask;
			float _Smoothness;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _DisplacementMap;
			float4 positionsArray[40];
			sampler2D _NoiseDetailTexture;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _FlowTexture;
			sampler2D _CloudsTexture;
			sampler2D _RefractionTexture;


			float MyCustomExpression272( float3 WorldPos, float3 objectPosition )
			{
				float closest=10000;
				float now=0;
				for(int i=0; i<positionsArray.Length;i++){
					now = distance(WorldPos,positionsArray[i]);
					if(now < closest){
					closest = now;
					}
				}
				return closest;
			}
			
					float2 voronoihash34( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi34( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash34( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return (F2 + F1) * 0.5;
					}
			
					float2 voronoihash57( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi57( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash57( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash74( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi74( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash74( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash91( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi91( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash91( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash251( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi251( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash251( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 texCoord147 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner145 = ( 1.0 * _Time.y * _SpeedOffset + texCoord147);
				float2 panner146 = ( 1.0 * _Time.y * ( _SpeedOffset * float2( -1,-1 ) ) + texCoord147);
				float3 temp_output_152_0 = ( ( tex2Dlod( _DisplacementMap, float4( panner145, 0, 0.0) ).r * tex2Dlod( _DisplacementMap, float4( panner146, 0, 0.0) ).r ) * v.ase_normal * _OffsetIntensity );
				float3 VertexOffset142 = temp_output_152_0;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 WorldPos272 = ase_worldPos;
				float3 objectPosition272 = positionsArray[0].xyz;
				float localMyCustomExpression272 = MyCustomExpression272( WorldPos272 , objectPosition272 );
				float temp_output_273_0 = saturate( ( pow( localMyCustomExpression272 , _FallOffDotsInteract ) * _IntensityDotsInteract ) );
				float CollisionRocks231 = temp_output_273_0;
				
				float3 vertexPos8 = v.vertex.xyz;
				float4 ase_clipPos8 = TransformObjectToHClip((vertexPos8).xyz);
				float4 screenPos8 = ComputeScreenPos(ase_clipPos8);
				o.ase_texcoord3 = screenPos8;
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord4 = screenPos;
				
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));
				float eyeDepth = -objectToViewPos.z;
				o.ase_texcoord2.z = eyeDepth;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( VertexOffset142 + ( CollisionRocks231 * _Dir ) );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 texCoord37 = IN.ase_texcoord2.xy * _NoiseTilling + float2( 0,0 );
				float2 panner117 = ( 1.0 * _Time.y * _SpeedDetailNoise + texCoord37);
				float4 tex2DNode116 = tex2D( _NoiseDetailTexture, panner117 );
				float Noise49 = saturate( ( (0.0 + (tex2DNode116.r - 0.0) * (0.35 - 0.0) / (1.0 - 0.0)) + _DetailNoise ) );
				float4 screenPos8 = IN.ase_texcoord3;
				float4 ase_screenPosNorm8 = screenPos8 / screenPos8.w;
				ase_screenPosNorm8.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm8.z : ase_screenPosNorm8.z * 0.5 + 0.5;
				float screenDepth8 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm8.xy ),_ZBufferParams);
				float distanceDepth8 = saturate( abs( ( screenDepth8 - LinearEyeDepth( ase_screenPosNorm8.z,_ZBufferParams ) ) / ( _CollisionDistance ) ) );
				float saferPower11 = max( distanceDepth8 , 0.0001 );
				float Collision51 = saturate( pow( saferPower11 , _CollisionFallOff ) );
				float4 temp_output_30_0 = ( _MainColor * Collision51 );
				float4 screenPos = IN.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth32 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth32 = saturate( abs( ( screenDepth32 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Distance ) ) );
				float time34 = 0.0;
				float2 voronoiSmoothId0 = 0;
				float2 texCoord22 = IN.ase_texcoord2.xy * _TillingFoam + float2( 0,0 );
				float Depth82 = distanceDepth8;
				float2 appendResult81 = (float2(texCoord22.x , Depth82));
				float2 panner80 = ( 1.0 * _Time.y * _SpeedFoam + appendResult81);
				float2 coords34 = panner80 * _FoamScale;
				float2 id34 = 0;
				float2 uv34 = 0;
				float voroi34 = voronoi34( coords34, time34, id34, uv34, 0, voronoiSmoothId0 );
				float4 Foam54 = ( step( distanceDepth32 , voroi34 ) * _FoamColor );
				float time57 = 0.0;
				float2 texCoord58 = IN.ase_texcoord2.xy * _TillingDots + float2( 0,0 );
				float2 panner72 = ( 1.0 * _Time.y * _SpeedDots + texCoord58);
				float2 temp_output_12_0_g25 = panner72;
				float2 panner7_g25 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g25);
				float2 lerpResult1_g25 = lerp( (tex2D( _FlowTexture, panner7_g25 )).rg , temp_output_12_0_g25 , _FlowDotsMask);
				float2 coords57 = lerpResult1_g25 * _DotsScale;
				float2 id57 = 0;
				float2 uv57 = 0;
				float voroi57 = voronoi57( coords57, time57, id57, uv57, 0, voronoiSmoothId0 );
				float time74 = 0.0;
				float2 texCoord75 = IN.ase_texcoord2.xy * _TillingDots + float2( 0,0 );
				float2 panner76 = ( 1.0 * _Time.y * ( _SpeedDots * float2( -1,-1 ) ) + texCoord75);
				float2 temp_output_12_0_g24 = panner76;
				float2 panner7_g24 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g24);
				float2 lerpResult1_g24 = lerp( (tex2D( _FlowTexture, panner7_g24 )).rg , temp_output_12_0_g24 , _FlowDotsMask);
				float2 coords74 = lerpResult1_g24 * _DotsScale;
				float2 id74 = 0;
				float2 uv74 = 0;
				float voroi74 = voronoi74( coords74, time74, id74, uv74, 0, voronoiSmoothId0 );
				float time91 = 0.0;
				float2 panner92 = ( 1.0 * _Time.y * _SpeedDots + texCoord75);
				float2 temp_output_12_0_g26 = panner92;
				float2 panner7_g26 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g26);
				float2 lerpResult1_g26 = lerp( (tex2D( _FlowTexture, panner7_g26 )).rg , temp_output_12_0_g26 , _FlowDotsMask);
				float2 coords91 = lerpResult1_g26 * _DotsScale;
				float2 id91 = 0;
				float2 uv91 = 0;
				float voroi91 = voronoi91( coords91, time91, id91, uv91, 0, voronoiSmoothId0 );
				float lerpResult89 = lerp( ( voroi57 * voroi74 ) , voroi91 , _MoveInDirection);
				float time251 = 0.0;
				float2 DotsSpeed303 = _SpeedDots;
				float2 DotsTilling305 = _TillingDots;
				float2 texCoord252 = IN.ase_texcoord2.xy * DotsTilling305 + float2( 0,0 );
				float2 panner254 = ( 1.0 * _Time.y * DotsSpeed303 + texCoord252);
				float2 coords251 = panner254 * _InteractDotsScale;
				float2 id251 = 0;
				float2 uv251 = 0;
				float voroi251 = voronoi251( coords251, time251, id251, uv251, 0, voronoiSmoothId0 );
				float3 WorldPos272 = WorldPosition;
				float3 objectPosition272 = positionsArray[0].xyz;
				float localMyCustomExpression272 = MyCustomExpression272( WorldPos272 , objectPosition272 );
				float temp_output_273_0 = saturate( ( pow( localMyCustomExpression272 , _FallOffDotsInteract ) * _IntensityDotsInteract ) );
				float temp_output_315_0 = ( 1.0 - IN.ase_color.g );
				#ifdef _VERTEXCOLOR_ON
				float staticSwitch307 = temp_output_315_0;
				#else
				float staticSwitch307 = temp_output_273_0;
				#endif
				float FoamCollision259 = saturate( step( voroi251 , staticSwitch307 ) );
				float lerpResult278 = lerp( step( _DotsAmmount , lerpResult89 ) , step( _DotsInteract , lerpResult89 ) , FoamCollision259);
				float Dots56 = lerpResult278;
				float4 temp_output_71_0 = ( ( ( Noise49 * temp_output_30_0 ) + Foam54 ) + Dots56 );
				float4 Albedo244 = temp_output_71_0;
				float2 appendResult30_g29 = (float2(WorldPosition.x , WorldPosition.z));
				float2 temp_output_27_0_g29 = _TillingClouds;
				float2 temp_output_31_0_g29 = ( appendResult30_g29 * temp_output_27_0_g29 );
				float2 panner20_g29 = ( 1.0 * _Time.y * float2( -0.03,0 ) + temp_output_31_0_g29);
				float2 panner21_g29 = ( 1.0 * _Time.y * float2( 0.03,0 ) + temp_output_31_0_g29);
				float Clouds317 = saturate( pow( ( tex2D( _CloudsTexture, panner20_g29 ).r * tex2D( _CloudsTexture, panner21_g29 ).r * _IntensityClouds ) , _FallOffClouds ) );
				float4 temp_cast_1 = (Clouds317).xxxx;
				float4 lerpResult325 = lerp( Albedo244 , temp_cast_1 , _AffectedByClouds);
				float4 lerpResult324 = lerp( ( _CloudsDarkColor1 * Albedo244 ) , ( _CloudsLightColor1 * Albedo244 ) , lerpResult325);
				
				float2 temp_output_12_0_g27 = IN.ase_texcoord2.xy;
				float2 panner7_g27 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g27);
				float2 lerpResult1_g27 = lerp( (tex2D( _FlowTexture, panner7_g27 )).rg , temp_output_12_0_g27 , _FlowMapMask);
				float2 panner109 = ( 1.0 * _Time.y * float2( 0,0 ) + lerpResult1_g27);
				float eyeDepth = IN.ase_texcoord2.z;
				float eyeDepth28_g28 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float2 temp_output_20_0_g28 = ( (UnpackNormalScale( tex2D( _RefractionTexture, panner109 ), 1.0f )).xy * ( _RefractionStrength / max( eyeDepth , 0.1 ) ) * saturate( ( eyeDepth28_g28 - eyeDepth ) ) );
				float eyeDepth2_g28 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( float4( temp_output_20_0_g28, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ),_ZBufferParams);
				float2 temp_output_32_0_g28 = (( float4( ( temp_output_20_0_g28 * saturate( ( eyeDepth2_g28 - eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
				float4 fetchOpaqueVal106 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( temp_output_32_0_g28 ), 1.0 );
				float4 Refraction107 = ( fetchOpaqueVal106 * ( 1.0 - Depth82 ) );
				
				
				float3 Albedo = lerpResult324.rgb;
				float3 Emission = Refraction107.rgb;
				float Alpha = Collision51;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_FIXED_TESSELLATION
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _VERTEXCOLOR_ON


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _CloudsDarkColor1;
			float4 _FoamColor;
			float4 _MainColor;
			float4 _CloudsLightColor1;
			float3 _Dir;
			float2 _SpeedOffset;
			float2 _SpeedDots;
			float2 _SpeedDetailNoise;
			float2 _NoiseTilling;
			float2 _TillingDots;
			float2 _TillingFoam;
			float2 _TillingClouds;
			float2 _SpeedFoam;
			float _DotsInteract;
			float _InteractDotsScale;
			float _FallOffClouds;
			float _AffectedByClouds;
			float _NormalScale;
			float _FlowMapMask;
			float _RefractionStrength;
			float _IntensityClouds;
			float _MoveInDirection;
			float _DotsAmmount;
			float _DotsScale;
			float _Metallic;
			float _FoamScale;
			float _Distance;
			float _CollisionFallOff;
			float _CollisionDistance;
			float _DetailNoise;
			float _IntensityDotsInteract;
			float _FallOffDotsInteract;
			float _OffsetIntensity;
			float _FlowDotsMask;
			float _Smoothness;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _DisplacementMap;
			float4 positionsArray[40];
			sampler2D _NoiseDetailTexture;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _FlowTexture;
			sampler2D _CloudsTexture;


			float MyCustomExpression272( float3 WorldPos, float3 objectPosition )
			{
				float closest=10000;
				float now=0;
				for(int i=0; i<positionsArray.Length;i++){
					now = distance(WorldPos,positionsArray[i]);
					if(now < closest){
					closest = now;
					}
				}
				return closest;
			}
			
					float2 voronoihash34( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi34( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash34( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return (F2 + F1) * 0.5;
					}
			
					float2 voronoihash57( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi57( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash57( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash74( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi74( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash74( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash91( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi91( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash91( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash251( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi251( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash251( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float2 texCoord147 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner145 = ( 1.0 * _Time.y * _SpeedOffset + texCoord147);
				float2 panner146 = ( 1.0 * _Time.y * ( _SpeedOffset * float2( -1,-1 ) ) + texCoord147);
				float3 temp_output_152_0 = ( ( tex2Dlod( _DisplacementMap, float4( panner145, 0, 0.0) ).r * tex2Dlod( _DisplacementMap, float4( panner146, 0, 0.0) ).r ) * v.ase_normal * _OffsetIntensity );
				float3 VertexOffset142 = temp_output_152_0;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 WorldPos272 = ase_worldPos;
				float3 objectPosition272 = positionsArray[0].xyz;
				float localMyCustomExpression272 = MyCustomExpression272( WorldPos272 , objectPosition272 );
				float temp_output_273_0 = saturate( ( pow( localMyCustomExpression272 , _FallOffDotsInteract ) * _IntensityDotsInteract ) );
				float CollisionRocks231 = temp_output_273_0;
				
				float3 vertexPos8 = v.vertex.xyz;
				float4 ase_clipPos8 = TransformObjectToHClip((vertexPos8).xyz);
				float4 screenPos8 = ComputeScreenPos(ase_clipPos8);
				o.ase_texcoord3 = screenPos8;
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord4 = screenPos;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( VertexOffset142 + ( CollisionRocks231 * _Dir ) );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 texCoord37 = IN.ase_texcoord2.xy * _NoiseTilling + float2( 0,0 );
				float2 panner117 = ( 1.0 * _Time.y * _SpeedDetailNoise + texCoord37);
				float4 tex2DNode116 = tex2D( _NoiseDetailTexture, panner117 );
				float Noise49 = saturate( ( (0.0 + (tex2DNode116.r - 0.0) * (0.35 - 0.0) / (1.0 - 0.0)) + _DetailNoise ) );
				float4 screenPos8 = IN.ase_texcoord3;
				float4 ase_screenPosNorm8 = screenPos8 / screenPos8.w;
				ase_screenPosNorm8.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm8.z : ase_screenPosNorm8.z * 0.5 + 0.5;
				float screenDepth8 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm8.xy ),_ZBufferParams);
				float distanceDepth8 = saturate( abs( ( screenDepth8 - LinearEyeDepth( ase_screenPosNorm8.z,_ZBufferParams ) ) / ( _CollisionDistance ) ) );
				float saferPower11 = max( distanceDepth8 , 0.0001 );
				float Collision51 = saturate( pow( saferPower11 , _CollisionFallOff ) );
				float4 temp_output_30_0 = ( _MainColor * Collision51 );
				float4 screenPos = IN.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth32 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth32 = saturate( abs( ( screenDepth32 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Distance ) ) );
				float time34 = 0.0;
				float2 voronoiSmoothId0 = 0;
				float2 texCoord22 = IN.ase_texcoord2.xy * _TillingFoam + float2( 0,0 );
				float Depth82 = distanceDepth8;
				float2 appendResult81 = (float2(texCoord22.x , Depth82));
				float2 panner80 = ( 1.0 * _Time.y * _SpeedFoam + appendResult81);
				float2 coords34 = panner80 * _FoamScale;
				float2 id34 = 0;
				float2 uv34 = 0;
				float voroi34 = voronoi34( coords34, time34, id34, uv34, 0, voronoiSmoothId0 );
				float4 Foam54 = ( step( distanceDepth32 , voroi34 ) * _FoamColor );
				float time57 = 0.0;
				float2 texCoord58 = IN.ase_texcoord2.xy * _TillingDots + float2( 0,0 );
				float2 panner72 = ( 1.0 * _Time.y * _SpeedDots + texCoord58);
				float2 temp_output_12_0_g25 = panner72;
				float2 panner7_g25 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g25);
				float2 lerpResult1_g25 = lerp( (tex2D( _FlowTexture, panner7_g25 )).rg , temp_output_12_0_g25 , _FlowDotsMask);
				float2 coords57 = lerpResult1_g25 * _DotsScale;
				float2 id57 = 0;
				float2 uv57 = 0;
				float voroi57 = voronoi57( coords57, time57, id57, uv57, 0, voronoiSmoothId0 );
				float time74 = 0.0;
				float2 texCoord75 = IN.ase_texcoord2.xy * _TillingDots + float2( 0,0 );
				float2 panner76 = ( 1.0 * _Time.y * ( _SpeedDots * float2( -1,-1 ) ) + texCoord75);
				float2 temp_output_12_0_g24 = panner76;
				float2 panner7_g24 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g24);
				float2 lerpResult1_g24 = lerp( (tex2D( _FlowTexture, panner7_g24 )).rg , temp_output_12_0_g24 , _FlowDotsMask);
				float2 coords74 = lerpResult1_g24 * _DotsScale;
				float2 id74 = 0;
				float2 uv74 = 0;
				float voroi74 = voronoi74( coords74, time74, id74, uv74, 0, voronoiSmoothId0 );
				float time91 = 0.0;
				float2 panner92 = ( 1.0 * _Time.y * _SpeedDots + texCoord75);
				float2 temp_output_12_0_g26 = panner92;
				float2 panner7_g26 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g26);
				float2 lerpResult1_g26 = lerp( (tex2D( _FlowTexture, panner7_g26 )).rg , temp_output_12_0_g26 , _FlowDotsMask);
				float2 coords91 = lerpResult1_g26 * _DotsScale;
				float2 id91 = 0;
				float2 uv91 = 0;
				float voroi91 = voronoi91( coords91, time91, id91, uv91, 0, voronoiSmoothId0 );
				float lerpResult89 = lerp( ( voroi57 * voroi74 ) , voroi91 , _MoveInDirection);
				float time251 = 0.0;
				float2 DotsSpeed303 = _SpeedDots;
				float2 DotsTilling305 = _TillingDots;
				float2 texCoord252 = IN.ase_texcoord2.xy * DotsTilling305 + float2( 0,0 );
				float2 panner254 = ( 1.0 * _Time.y * DotsSpeed303 + texCoord252);
				float2 coords251 = panner254 * _InteractDotsScale;
				float2 id251 = 0;
				float2 uv251 = 0;
				float voroi251 = voronoi251( coords251, time251, id251, uv251, 0, voronoiSmoothId0 );
				float3 WorldPos272 = WorldPosition;
				float3 objectPosition272 = positionsArray[0].xyz;
				float localMyCustomExpression272 = MyCustomExpression272( WorldPos272 , objectPosition272 );
				float temp_output_273_0 = saturate( ( pow( localMyCustomExpression272 , _FallOffDotsInteract ) * _IntensityDotsInteract ) );
				float temp_output_315_0 = ( 1.0 - IN.ase_color.g );
				#ifdef _VERTEXCOLOR_ON
				float staticSwitch307 = temp_output_315_0;
				#else
				float staticSwitch307 = temp_output_273_0;
				#endif
				float FoamCollision259 = saturate( step( voroi251 , staticSwitch307 ) );
				float lerpResult278 = lerp( step( _DotsAmmount , lerpResult89 ) , step( _DotsInteract , lerpResult89 ) , FoamCollision259);
				float Dots56 = lerpResult278;
				float4 temp_output_71_0 = ( ( ( Noise49 * temp_output_30_0 ) + Foam54 ) + Dots56 );
				float4 Albedo244 = temp_output_71_0;
				float2 appendResult30_g29 = (float2(WorldPosition.x , WorldPosition.z));
				float2 temp_output_27_0_g29 = _TillingClouds;
				float2 temp_output_31_0_g29 = ( appendResult30_g29 * temp_output_27_0_g29 );
				float2 panner20_g29 = ( 1.0 * _Time.y * float2( -0.03,0 ) + temp_output_31_0_g29);
				float2 panner21_g29 = ( 1.0 * _Time.y * float2( 0.03,0 ) + temp_output_31_0_g29);
				float Clouds317 = saturate( pow( ( tex2D( _CloudsTexture, panner20_g29 ).r * tex2D( _CloudsTexture, panner21_g29 ).r * _IntensityClouds ) , _FallOffClouds ) );
				float4 temp_cast_1 = (Clouds317).xxxx;
				float4 lerpResult325 = lerp( Albedo244 , temp_cast_1 , _AffectedByClouds);
				float4 lerpResult324 = lerp( ( _CloudsDarkColor1 * Albedo244 ) , ( _CloudsLightColor1 * Albedo244 ) , lerpResult325);
				
				
				float3 Albedo = lerpResult324.rgb;
				float Alpha = Collision51;
				float AlphaClipThreshold = 0.5;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
            ZTest LEqual
            ZWrite On

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_FIXED_TESSELLATION
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float3 worldNormal : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _CloudsDarkColor1;
			float4 _FoamColor;
			float4 _MainColor;
			float4 _CloudsLightColor1;
			float3 _Dir;
			float2 _SpeedOffset;
			float2 _SpeedDots;
			float2 _SpeedDetailNoise;
			float2 _NoiseTilling;
			float2 _TillingDots;
			float2 _TillingFoam;
			float2 _TillingClouds;
			float2 _SpeedFoam;
			float _DotsInteract;
			float _InteractDotsScale;
			float _FallOffClouds;
			float _AffectedByClouds;
			float _NormalScale;
			float _FlowMapMask;
			float _RefractionStrength;
			float _IntensityClouds;
			float _MoveInDirection;
			float _DotsAmmount;
			float _DotsScale;
			float _Metallic;
			float _FoamScale;
			float _Distance;
			float _CollisionFallOff;
			float _CollisionDistance;
			float _DetailNoise;
			float _IntensityDotsInteract;
			float _FallOffDotsInteract;
			float _OffsetIntensity;
			float _FlowDotsMask;
			float _Smoothness;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _DisplacementMap;
			float4 positionsArray[40];
			uniform float4 _CameraDepthTexture_TexelSize;


			float MyCustomExpression272( float3 WorldPos, float3 objectPosition )
			{
				float closest=10000;
				float now=0;
				for(int i=0; i<positionsArray.Length;i++){
					now = distance(WorldPos,positionsArray[i]);
					if(now < closest){
					closest = now;
					}
				}
				return closest;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 texCoord147 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner145 = ( 1.0 * _Time.y * _SpeedOffset + texCoord147);
				float2 panner146 = ( 1.0 * _Time.y * ( _SpeedOffset * float2( -1,-1 ) ) + texCoord147);
				float3 temp_output_152_0 = ( ( tex2Dlod( _DisplacementMap, float4( panner145, 0, 0.0) ).r * tex2Dlod( _DisplacementMap, float4( panner146, 0, 0.0) ).r ) * v.ase_normal * _OffsetIntensity );
				float3 VertexOffset142 = temp_output_152_0;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 WorldPos272 = ase_worldPos;
				float3 objectPosition272 = positionsArray[0].xyz;
				float localMyCustomExpression272 = MyCustomExpression272( WorldPos272 , objectPosition272 );
				float temp_output_273_0 = saturate( ( pow( localMyCustomExpression272 , _FallOffDotsInteract ) * _IntensityDotsInteract ) );
				float CollisionRocks231 = temp_output_273_0;
				
				float3 vertexPos8 = v.vertex.xyz;
				float4 ase_clipPos8 = TransformObjectToHClip((vertexPos8).xyz);
				float4 screenPos8 = ComputeScreenPos(ase_clipPos8);
				o.ase_texcoord3 = screenPos8;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( VertexOffset142 + ( CollisionRocks231 * _Dir ) );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal( v.ase_normal );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.worldNormal = normalWS;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 screenPos8 = IN.ase_texcoord3;
				float4 ase_screenPosNorm8 = screenPos8 / screenPos8.w;
				ase_screenPosNorm8.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm8.z : ase_screenPosNorm8.z * 0.5 + 0.5;
				float screenDepth8 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm8.xy ),_ZBufferParams);
				float distanceDepth8 = saturate( abs( ( screenDepth8 - LinearEyeDepth( ase_screenPosNorm8.z,_ZBufferParams ) ) / ( _CollisionDistance ) ) );
				float saferPower11 = max( distanceDepth8 , 0.0001 );
				float Collision51 = saturate( pow( saferPower11 , _CollisionFallOff ) );
				
				float Alpha = Collision51;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif
				
				return float4(PackNormalOctRectEncode(TransformWorldToViewDir(IN.worldNormal, true)), 0.0, 0.0);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_FIXED_TESSELLATION
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1
			#define REQUIRE_OPAQUE_TEXTURE 1

			
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			#pragma multi_compile _ _GBUFFER_NORMALS_OCT
			
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_GBUFFER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_SCREEN_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _VERTEXCOLOR_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				float4 ase_color : COLOR;
				float3 ase_normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _CloudsDarkColor1;
			float4 _FoamColor;
			float4 _MainColor;
			float4 _CloudsLightColor1;
			float3 _Dir;
			float2 _SpeedOffset;
			float2 _SpeedDots;
			float2 _SpeedDetailNoise;
			float2 _NoiseTilling;
			float2 _TillingDots;
			float2 _TillingFoam;
			float2 _TillingClouds;
			float2 _SpeedFoam;
			float _DotsInteract;
			float _InteractDotsScale;
			float _FallOffClouds;
			float _AffectedByClouds;
			float _NormalScale;
			float _FlowMapMask;
			float _RefractionStrength;
			float _IntensityClouds;
			float _MoveInDirection;
			float _DotsAmmount;
			float _DotsScale;
			float _Metallic;
			float _FoamScale;
			float _Distance;
			float _CollisionFallOff;
			float _CollisionDistance;
			float _DetailNoise;
			float _IntensityDotsInteract;
			float _FallOffDotsInteract;
			float _OffsetIntensity;
			float _FlowDotsMask;
			float _Smoothness;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _DisplacementMap;
			float4 positionsArray[40];
			sampler2D _NoiseDetailTexture;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _FlowTexture;
			sampler2D _CloudsTexture;
			sampler2D _Normal;
			sampler2D _RefractionTexture;


			float MyCustomExpression272( float3 WorldPos, float3 objectPosition )
			{
				float closest=10000;
				float now=0;
				for(int i=0; i<positionsArray.Length;i++){
					now = distance(WorldPos,positionsArray[i]);
					if(now < closest){
					closest = now;
					}
				}
				return closest;
			}
			
					float2 voronoihash34( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi34( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash34( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return (F2 + F1) * 0.5;
					}
			
					float2 voronoihash57( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi57( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash57( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash74( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi74( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash74( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash91( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi91( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash91( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash251( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi251( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash251( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 texCoord147 = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner145 = ( 1.0 * _Time.y * _SpeedOffset + texCoord147);
				float2 panner146 = ( 1.0 * _Time.y * ( _SpeedOffset * float2( -1,-1 ) ) + texCoord147);
				float3 temp_output_152_0 = ( ( tex2Dlod( _DisplacementMap, float4( panner145, 0, 0.0) ).r * tex2Dlod( _DisplacementMap, float4( panner146, 0, 0.0) ).r ) * v.ase_normal * _OffsetIntensity );
				float3 VertexOffset142 = temp_output_152_0;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 WorldPos272 = ase_worldPos;
				float3 objectPosition272 = positionsArray[0].xyz;
				float localMyCustomExpression272 = MyCustomExpression272( WorldPos272 , objectPosition272 );
				float temp_output_273_0 = saturate( ( pow( localMyCustomExpression272 , _FallOffDotsInteract ) * _IntensityDotsInteract ) );
				float CollisionRocks231 = temp_output_273_0;
				
				float3 vertexPos8 = v.vertex.xyz;
				float4 ase_clipPos8 = TransformObjectToHClip((vertexPos8).xyz);
				float4 screenPos8 = ComputeScreenPos(ase_clipPos8);
				o.ase_texcoord8 = screenPos8;
				
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));
				float eyeDepth = -objectToViewPos.z;
				o.ase_texcoord7.z = eyeDepth;
				
				o.ase_texcoord7.xy = v.texcoord.xy;
				o.ase_color = v.ase_color;
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord7.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( VertexOffset142 + ( CollisionRocks231 * _Dir ) );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			FragmentOutput frag ( VertexOutput IN 
								#ifdef ASE_DEPTH_WRITE_ON
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								 )
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 texCoord37 = IN.ase_texcoord7.xy * _NoiseTilling + float2( 0,0 );
				float2 panner117 = ( 1.0 * _Time.y * _SpeedDetailNoise + texCoord37);
				float4 tex2DNode116 = tex2D( _NoiseDetailTexture, panner117 );
				float Noise49 = saturate( ( (0.0 + (tex2DNode116.r - 0.0) * (0.35 - 0.0) / (1.0 - 0.0)) + _DetailNoise ) );
				float4 screenPos8 = IN.ase_texcoord8;
				float4 ase_screenPosNorm8 = screenPos8 / screenPos8.w;
				ase_screenPosNorm8.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm8.z : ase_screenPosNorm8.z * 0.5 + 0.5;
				float screenDepth8 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm8.xy ),_ZBufferParams);
				float distanceDepth8 = saturate( abs( ( screenDepth8 - LinearEyeDepth( ase_screenPosNorm8.z,_ZBufferParams ) ) / ( _CollisionDistance ) ) );
				float saferPower11 = max( distanceDepth8 , 0.0001 );
				float Collision51 = saturate( pow( saferPower11 , _CollisionFallOff ) );
				float4 temp_output_30_0 = ( _MainColor * Collision51 );
				float4 ase_screenPosNorm = ScreenPos / ScreenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth32 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth32 = saturate( abs( ( screenDepth32 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Distance ) ) );
				float time34 = 0.0;
				float2 voronoiSmoothId0 = 0;
				float2 texCoord22 = IN.ase_texcoord7.xy * _TillingFoam + float2( 0,0 );
				float Depth82 = distanceDepth8;
				float2 appendResult81 = (float2(texCoord22.x , Depth82));
				float2 panner80 = ( 1.0 * _Time.y * _SpeedFoam + appendResult81);
				float2 coords34 = panner80 * _FoamScale;
				float2 id34 = 0;
				float2 uv34 = 0;
				float voroi34 = voronoi34( coords34, time34, id34, uv34, 0, voronoiSmoothId0 );
				float4 Foam54 = ( step( distanceDepth32 , voroi34 ) * _FoamColor );
				float time57 = 0.0;
				float2 texCoord58 = IN.ase_texcoord7.xy * _TillingDots + float2( 0,0 );
				float2 panner72 = ( 1.0 * _Time.y * _SpeedDots + texCoord58);
				float2 temp_output_12_0_g25 = panner72;
				float2 panner7_g25 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g25);
				float2 lerpResult1_g25 = lerp( (tex2D( _FlowTexture, panner7_g25 )).rg , temp_output_12_0_g25 , _FlowDotsMask);
				float2 coords57 = lerpResult1_g25 * _DotsScale;
				float2 id57 = 0;
				float2 uv57 = 0;
				float voroi57 = voronoi57( coords57, time57, id57, uv57, 0, voronoiSmoothId0 );
				float time74 = 0.0;
				float2 texCoord75 = IN.ase_texcoord7.xy * _TillingDots + float2( 0,0 );
				float2 panner76 = ( 1.0 * _Time.y * ( _SpeedDots * float2( -1,-1 ) ) + texCoord75);
				float2 temp_output_12_0_g24 = panner76;
				float2 panner7_g24 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g24);
				float2 lerpResult1_g24 = lerp( (tex2D( _FlowTexture, panner7_g24 )).rg , temp_output_12_0_g24 , _FlowDotsMask);
				float2 coords74 = lerpResult1_g24 * _DotsScale;
				float2 id74 = 0;
				float2 uv74 = 0;
				float voroi74 = voronoi74( coords74, time74, id74, uv74, 0, voronoiSmoothId0 );
				float time91 = 0.0;
				float2 panner92 = ( 1.0 * _Time.y * _SpeedDots + texCoord75);
				float2 temp_output_12_0_g26 = panner92;
				float2 panner7_g26 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g26);
				float2 lerpResult1_g26 = lerp( (tex2D( _FlowTexture, panner7_g26 )).rg , temp_output_12_0_g26 , _FlowDotsMask);
				float2 coords91 = lerpResult1_g26 * _DotsScale;
				float2 id91 = 0;
				float2 uv91 = 0;
				float voroi91 = voronoi91( coords91, time91, id91, uv91, 0, voronoiSmoothId0 );
				float lerpResult89 = lerp( ( voroi57 * voroi74 ) , voroi91 , _MoveInDirection);
				float time251 = 0.0;
				float2 DotsSpeed303 = _SpeedDots;
				float2 DotsTilling305 = _TillingDots;
				float2 texCoord252 = IN.ase_texcoord7.xy * DotsTilling305 + float2( 0,0 );
				float2 panner254 = ( 1.0 * _Time.y * DotsSpeed303 + texCoord252);
				float2 coords251 = panner254 * _InteractDotsScale;
				float2 id251 = 0;
				float2 uv251 = 0;
				float voroi251 = voronoi251( coords251, time251, id251, uv251, 0, voronoiSmoothId0 );
				float3 WorldPos272 = WorldPosition;
				float3 objectPosition272 = positionsArray[0].xyz;
				float localMyCustomExpression272 = MyCustomExpression272( WorldPos272 , objectPosition272 );
				float temp_output_273_0 = saturate( ( pow( localMyCustomExpression272 , _FallOffDotsInteract ) * _IntensityDotsInteract ) );
				float temp_output_315_0 = ( 1.0 - IN.ase_color.g );
				#ifdef _VERTEXCOLOR_ON
				float staticSwitch307 = temp_output_315_0;
				#else
				float staticSwitch307 = temp_output_273_0;
				#endif
				float FoamCollision259 = saturate( step( voroi251 , staticSwitch307 ) );
				float lerpResult278 = lerp( step( _DotsAmmount , lerpResult89 ) , step( _DotsInteract , lerpResult89 ) , FoamCollision259);
				float Dots56 = lerpResult278;
				float4 temp_output_71_0 = ( ( ( Noise49 * temp_output_30_0 ) + Foam54 ) + Dots56 );
				float4 Albedo244 = temp_output_71_0;
				float2 appendResult30_g29 = (float2(WorldPosition.x , WorldPosition.z));
				float2 temp_output_27_0_g29 = _TillingClouds;
				float2 temp_output_31_0_g29 = ( appendResult30_g29 * temp_output_27_0_g29 );
				float2 panner20_g29 = ( 1.0 * _Time.y * float2( -0.03,0 ) + temp_output_31_0_g29);
				float2 panner21_g29 = ( 1.0 * _Time.y * float2( 0.03,0 ) + temp_output_31_0_g29);
				float Clouds317 = saturate( pow( ( tex2D( _CloudsTexture, panner20_g29 ).r * tex2D( _CloudsTexture, panner21_g29 ).r * _IntensityClouds ) , _FallOffClouds ) );
				float4 temp_cast_1 = (Clouds317).xxxx;
				float4 lerpResult325 = lerp( Albedo244 , temp_cast_1 , _AffectedByClouds);
				float4 lerpResult324 = lerp( ( _CloudsDarkColor1 * Albedo244 ) , ( _CloudsLightColor1 * Albedo244 ) , lerpResult325);
				
				float2 texCoord136 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner137 = ( 1.0 * _Time.y * float2( -0.2,0 ) + texCoord136);
				float3 unpack134 = UnpackNormalScale( tex2D( _Normal, panner137 ), _NormalScale );
				unpack134.z = lerp( 1, unpack134.z, saturate(_NormalScale) );
				float3 Normal133 = ( unpack134 + IN.ase_normal );
				
				float2 temp_output_12_0_g27 = IN.ase_texcoord7.xy;
				float2 panner7_g27 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g27);
				float2 lerpResult1_g27 = lerp( (tex2D( _FlowTexture, panner7_g27 )).rg , temp_output_12_0_g27 , _FlowMapMask);
				float2 panner109 = ( 1.0 * _Time.y * float2( 0,0 ) + lerpResult1_g27);
				float eyeDepth = IN.ase_texcoord7.z;
				float eyeDepth28_g28 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float2 temp_output_20_0_g28 = ( (UnpackNormalScale( tex2D( _RefractionTexture, panner109 ), 1.0f )).xy * ( _RefractionStrength / max( eyeDepth , 0.1 ) ) * saturate( ( eyeDepth28_g28 - eyeDepth ) ) );
				float eyeDepth2_g28 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( float4( temp_output_20_0_g28, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ),_ZBufferParams);
				float2 temp_output_32_0_g28 = (( float4( ( temp_output_20_0_g28 * saturate( ( eyeDepth2_g28 - eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
				float4 fetchOpaqueVal106 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( temp_output_32_0_g28 ), 1.0 );
				float4 Refraction107 = ( fetchOpaqueVal106 * ( 1.0 - Depth82 ) );
				
				float3 Albedo = lerpResult324.rgb;
				float3 Normal = Normal133;
				float3 Emission = Refraction107.rgb;
				float3 Specular = 0.5;
				float Metallic = _Metallic;
				float Smoothness = _Smoothness;
				float Occlusion = 1;
				float Alpha = Collision51;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				BRDFData brdfData;
				InitializeBRDFData( Albedo, Metallic, Specular, Smoothness, Alpha, brdfData);
				half4 color;
				color.rgb = GlobalIllumination( brdfData, inputData.bakedGI, Occlusion, inputData.normalWS, inputData.viewDirectionWS);
				color.a = Alpha;

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;
				
					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;
				
					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );
				
							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif
				
				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;
				
					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
				
					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;
				
					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );
				
							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif
				
				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal, 0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif
				
				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif
				
				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif
				
				return BRDFDataToGbuffer(brdfData, inputData, Smoothness, Emission + color.rgb);
			}

			ENDHLSL
		}
		
	}
	/*ase_lod*/
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18912
0;501;1012;318;550.8284;-4.701279;1.288555;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;267;-411.8572,1716.725;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GlobalArrayNode;268;-449.4925,1899.15;Inherit;False;positionsArray;0;40;2;False;False;0;1;False;Object;-1;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;141;-4257.521,1219.799;Inherit;False;Property;_TillingDots;TillingDots;20;0;Create;True;0;0;0;False;0;False;0,0;0.05,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;223;-2516.466,-488.0229;Inherit;False;Property;_NoiseTilling;NoiseTilling;12;0;Create;True;0;0;0;False;0;False;0,0;0.01,0.19;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;37;-2332.387,-515.927;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;77;-4040.632,1280.37;Inherit;False;Property;_SpeedDots;SpeedDots;19;0;Create;True;0;0;0;False;0;False;0,0;-0.25,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CustomExpressionNode;272;-209.0781,1800.695;Inherit;False;float closest=10000@$float now=0@$for(int i=0@ i<positionsArray.Length@i++){$	now = distance(WorldPos,positionsArray[i])@$	if(now < closest){$	closest = now@$	}$}$return closest@;1;Create;2;True;WorldPos;FLOAT3;0,0,0;In;;Inherit;False;True;objectPosition;FLOAT3;0,0,0;In;;Inherit;False;My Custom Expression;True;False;0;;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-3884.535,686.6921;Inherit;False;Property;_CollisionDistance;CollisionDistance;13;1;[Header];Create;True;1;Collision;0;0;False;0;False;1;2.09;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;10;-3922.936,490.3722;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;305;-4131.16,1082.89;Inherit;False;DotsTilling;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;269;-149.0407,1930.187;Inherit;False;Property;_FallOffDotsInteract;FallOffDotsInteract;17;0;Create;True;0;0;0;False;0;False;0;-3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;8;-3716.214,539.7253;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;303;-3858.386,1069.948;Inherit;False;DotsSpeed;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;270;32.12014,1841.228;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;275;68.15456,1956.371;Inherit;False;Property;_IntensityDotsInteract;IntensityDotsInteract;43;0;Create;True;0;0;0;False;0;False;0;19.71;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;306;-245.4114,1526.729;Inherit;False;305;DotsTilling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;112;-2175.464,2362.089;Inherit;True;Property;_FlowTexture;FlowTexture;24;2;[Header];[SingleLineTexture];Create;True;1;FlowMapRefraction;0;0;False;0;False;None;c1209b845ce87ee42b41b444fc2233c1;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.WireNode;119;-2142.363,-643.6833;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;82;-3478.398,513.1608;Inherit;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;35;-4731.982,2570.523;Inherit;False;Property;_TillingFoam;TillingFoam;5;0;Create;True;0;0;0;False;0;False;0,0;0.05,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;118;-2072.358,-520.2509;Inherit;False;Property;_SpeedDetailNoise;SpeedDetailNoise;11;0;Create;True;0;0;0;False;0;False;0,0;-0.23,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;212;-1932.061,2432.99;Inherit;False;FlowTexture;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;252;-89.52203,1499.735;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.1,0.1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;274;209.0966,1824.595;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;304;180.9128,1578.571;Inherit;False;303;DotsSpeed;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;58;-4040.16,1126.246;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;75;-4062.564,1440.734;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;308;-304.8865,2065.062;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-3829.348,1337.638;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;-1,-1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;120;-1872.363,-668.6833;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;72;-3767.245,1141.193;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.24,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;22;-4591.177,2550.702;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;-0.4,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;83;-4558.316,2739.544;Inherit;False;82;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;273;347.8038,1803.427;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;76;-3720.793,1367.84;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.24,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;215;-3963.699,1666.085;Inherit;False;212;FlowTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;254;385.478,1427.735;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.14,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;216;-3779.054,1264.103;Inherit;False;Property;_FlowDotsMask;FlowDotsMask;18;0;Create;True;0;0;0;False;0;False;0;0.727;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;117;-1839.525,-518.199;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.22,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;266;368.1365,1595.667;Inherit;False;Property;_InteractDotsScale;InteractDotsScale;42;0;Create;True;0;0;0;False;0;False;0;-2.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;315;-73.76221,2063.491;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-3729.846,1335.187;Inherit;False;Property;_DotsScale;DotsScale;15;1;[Header];Create;True;1;Dots;0;0;False;0;False;0;5.15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;96;-4294.326,2773.378;Inherit;False;Property;_SpeedFoam;SpeedFoam;6;0;Create;True;0;0;0;False;0;False;0,0;0,-0.39;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;211;-3433.486,1182.706;Inherit;False;FlowMapMask;3;;25;a576844f19963ec46bc1db551bb181c0;0;4;12;FLOAT2;0,0;False;2;FLOAT;0;False;10;FLOAT2;0,0;False;11;SAMPLER2D;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;92;-3685.847,1569.547;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;213;-3415.67,1347.673;Inherit;False;FlowMapMask;3;;24;a576844f19963ec46bc1db551bb181c0;0;4;12;FLOAT2;0,0;False;2;FLOAT;0;False;10;FLOAT2;0,0;False;11;SAMPLER2D;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VoronoiNode;251;550.843,1444.938;Inherit;False;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;10;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.StaticSwitch;307;542.1581,1706.06;Inherit;False;Property;_VertexColor;VertexColor;44;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-3632.275,673.2659;Inherit;False;Property;_CollisionFallOff;CollisionFallOff;14;0;Create;True;0;0;0;False;0;False;1;0.17;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;116;-1635.158,-550.304;Inherit;True;Property;_NoiseDetailTexture;NoiseDetailTexture;10;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;621e2eb3c61948a4287bbda2bfd20ffa;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;81;-4271.468,2610.538;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-4059.637,2516.645;Inherit;False;Property;_Distance;Distance;7;0;Create;True;0;0;0;False;0;False;0;1.45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;11;-3425.672,630.9208;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-4081.973,2814.204;Inherit;False;Property;_FoamScale;FoamScale;1;1;[Header];Create;True;1;Foam;0;0;False;0;False;0;12.83;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-1301.533,-162.8518;Inherit;False;Property;_DetailNoise;DetailNoise;8;1;[Header];Create;True;1;Noise;0;0;False;0;False;0;0.93;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;74;-3075.094,1385.797;Inherit;False;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.TFHCRemapNode;43;-1367.976,-457.8336;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.35;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;214;-3394.235,1589.543;Inherit;False;FlowMapMask;3;;26;a576844f19963ec46bc1db551bb181c0;0;4;12;FLOAT2;0,0;False;2;FLOAT;0;False;10;FLOAT2;0,0;False;11;SAMPLER2D;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;80;-4099.468,2646.538;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.06;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StepOpNode;264;891.8498,1338.536;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;57;-3072.287,1197.527;Inherit;False;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.VoronoiNode;34;-3892.96,2653.476;Inherit;False;0;0;1;3;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;90;-2834.389,1556.367;Inherit;False;Property;_MoveInDirection;MoveInDirection;21;1;[Toggle];Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;91;-3063.278,1592.074;Inherit;False;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-2730.323,1139.355;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;10.08;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;262;1033.87,1398.932;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;32;-3912.145,2528.565;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;16;-3274.37,633.2496;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-1125.008,-293.4747;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;259;1232.096,1371.639;Inherit;False;FoamCollision;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-2575.986,1144.794;Inherit;False;Property;_DotsAmmount;DotsAmmount;16;0;Create;True;1;Dots;0;0;False;0;False;0;0.3438677;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;29;-3629.787,2718.123;Inherit;False;Property;_FoamColor;FoamColor;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;41;-954.3729,-290.5872;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;51;-2925.1,591.2125;Inherit;False;Collision;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;89;-2576.774,1341.631;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;280;-2599.935,1261.272;Inherit;False;Property;_DotsInteract;DotsInteract;41;1;[Header];Create;True;1;InteractObjs;0;0;False;0;False;0;0.185;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;20;-3571.842,2594.417;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-2113.613,2250.511;Inherit;False;Property;_FlowMapMask;FlowMapMask;18;0;Create;True;0;0;0;False;0;False;0;0.787;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;79;-2296.999,1184.106;Inherit;False;2;0;FLOAT;0.17;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;148;-1062.39,3843.765;Inherit;False;Property;_SpeedOffset;SpeedOffset;30;0;Create;True;0;0;0;False;0;False;0,0;0.09,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;276;-2480.905,1457.388;Inherit;False;259;FoamCollision;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-3431.287,2629.246;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;19;-2434.472,144.432;Inherit;False;Property;_MainColor;MainColor;0;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.3160361,0.6658803,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;279;-2231.635,1285.184;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-641.1189,-277.4255;Inherit;False;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-2306.659,322.7727;Inherit;False;51;Collision;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-3272.752,2627.444;Inherit;False;Foam;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;-2278.017,-93.05204;Inherit;False;49;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;147;-1099.735,3653.715;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;278;-2076.107,1236.321;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;110;-1693.613,2298.511;Inherit;False;FlowMapMask;3;;27;a576844f19963ec46bc1db551bb181c0;0;4;12;FLOAT2;0,0;False;2;FLOAT;0;False;10;FLOAT2;0,0;False;11;SAMPLER2D;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;149;-853.3901,3896.765;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;-1,-1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-2144.664,217.9239;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;146;-712.7351,3858.715;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;109;-1402.613,2313.511;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-1827.717,1226.933;Inherit;False;Dots;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-1913.104,349.0571;Inherit;False;54;Foam;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;145;-722.7351,3655.715;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-1875.31,44.80994;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;319;485.5754,2545.338;Inherit;False;Property;_IntensityClouds;IntensityClouds;47;0;Create;True;0;0;0;False;0;False;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;321;507.5754,2715.338;Inherit;False;Property;_TillingClouds;TillingClouds;49;0;Create;True;0;0;0;False;0;False;0,0;0.03,0.03;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-1695.153,198.9867;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;318;445.5754,2357.338;Inherit;True;Property;_CloudsTexture;CloudsTexture;46;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;None;a0de09b32cc537c4fbe57a14b801d0b9;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;70;-1697.887,333.6287;Inherit;False;56;Dots;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;136;-924.1643,3220.749;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;103;-1156.215,2498.203;Inherit;False;Property;_RefractionStrength;RefractionStrength;23;0;Create;True;0;0;0;False;0;False;0;0.14;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;143;-475.048,3667.67;Inherit;True;Property;_DisplacementMap;DisplacementMap;29;2;[Header];[SingleLineTexture];Create;True;1;VertexOffset;0;0;False;0;False;-1;None;a86ee5521eedff648a37bee20fecd437;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;144;-466.7351,3867.715;Inherit;True;Property;_TextureSample1;Texture Sample 1;29;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;143;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;102;-1212.215,2299.203;Inherit;True;Property;_RefractionTexture;RefractionTexture;22;3;[Header];[NoScaleOffset];[SingleLineTexture];Create;True;1;Refraction;0;0;False;0;False;-1;None;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;320;450.5754,2638.338;Inherit;False;Property;_FallOffClouds;FallOffClouds;48;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-461.4778,2594.566;Inherit;False;82;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;153;-109.5886,3834.08;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;135;-799.1643,3387.749;Inherit;False;Property;_NormalScale;NormalScale;28;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;101;-811.6152,2417.103;Inherit;False;DepthMaskedRefraction;-1;;28;c805f061214177c42bca056464193f81;2,40,0,103,0;2;35;FLOAT3;0,0,0;False;37;FLOAT;0.02;False;1;FLOAT2;38
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-160.5404,3743.701;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;316;672.9629,2454.459;Inherit;False;Clouds;-1;;29;a77d607fe972ab341b81c77308ba74a1;0;4;28;SAMPLER2D;0;False;25;FLOAT;0;False;26;FLOAT;0;False;27;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;137;-700.1643,3227.749;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.2,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;-1494.582,258.7311;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;154;-84.32178,4010.947;Inherit;False;Property;_OffsetIntensity;OffsetIntensity;31;0;Create;True;0;0;0;False;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;106;-479.0154,2402.803;Inherit;False;Global;_GrabScreen0;Grab Screen 0;16;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;244;-619.1735,147.7569;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;134;-470.1644,3259.749;Inherit;True;Property;_Normal;Normal;27;2;[Header];[SingleLineTexture];Create;True;1;Normal;0;0;False;0;False;-1;None;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;317;1084.575,2498.338;Inherit;False;Clouds;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;115;-308.5427,2597.322;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;111.443,3758.279;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;231;932.22,1685.959;Inherit;False;CollisionRocks;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;194;-328.1969,3476.147;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;232;-242.2507,151.5329;Inherit;False;244;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;193;-87.1969,3280.147;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;326;-236.2495,358.5516;Inherit;False;Property;_AffectedByClouds;AffectedByClouds;45;2;[Header];[Toggle];Create;True;1;Clouds;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;323;-190.2362,279.7901;Inherit;False;317;Clouds;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;239;300.5032,885.2292;Inherit;False;231;CollisionRocks;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;142;263.55,3725.802;Inherit;False;VertexOffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;-193.5777,2415.367;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;241;412.8325,985.9951;Inherit;False;Property;_Dir;Dir;39;0;Create;True;0;0;0;False;0;False;0,0,0;0,1.5,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;328;114.8647,39.18066;Inherit;False;Property;_CloudsLightColor1;CloudsLightColor;50;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;1.322892,1.829851,1.147973,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;327;109.2037,-128.0103;Inherit;False;Property;_CloudsDarkColor1;CloudsDarkColor;51;0;Create;True;0;0;0;False;0;False;0.3773585,0.3773585,0.3773585,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;607.2546,863.5755;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;330;360.1801,218.6015;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;329;415.588,58.82062;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;107;-61.87871,2402.51;Inherit;False;Refraction;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;151;418.3932,797.2282;Inherit;False;142;VertexOffset;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;133;55.1265,3253.562;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;325;127.7505,272.7515;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;226;-825.8201,1417.855;Inherit;False;Property;_Pos;Pos;36;1;[Header];Create;True;1;CollisionObj;0;0;False;0;False;0,0,0;-112.2,6.98,187.86;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;186;-1501.6,-1582.359;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;222;-901.2715,-116.8564;Inherit;False;Property;_Float5;Float 5;35;0;Create;True;0;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;249;-450.6094,235.5955;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;171;-1484.318,-1427.636;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;196;445.2007,3623.032;Inherit;False;myVarName;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;187;-592.5706,-796.7571;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DdyOpNode;166;-1268.041,-1339.325;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;209;-1341.674,530.0437;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;237;1.925659,1377.297;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;210;-1522.674,650.0437;Inherit;False;Property;_Float4;Float 4;34;0;Create;True;0;0;0;False;0;False;0;0.97;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;197;-557.1492,-696.4245;Inherit;False;196;myVarName;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;128;248.5438,654.3201;Inherit;False;Property;_Smoothness;Smoothness;26;0;Create;True;0;0;0;False;0;False;0;0.55;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;327.6113,490.9643;Inherit;False;107;Refraction;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.DistanceOpNode;224;-638.2159,1322.709;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;310;298.9471,2123.051;Inherit;False;sss;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;36;-2152.281,-390.7783;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;5.33;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;243;-159.6323,1374.913;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;242;780.7526,761.1003;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;431.1271,710.3491;Inherit;False;51;Collision;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;192;282.542,378.8417;Inherit;False;133;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;202;-1111.047,407.7885;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;208;-1543.546,497.4774;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;229;-294.8027,1365.947;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;188;-378.4679,-819.1179;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;127;236.5438,562.3201;Inherit;False;Property;_Metallic;Metallic;25;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;189;272.6879,3849.765;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DdxOpNode;167;-1254.322,-1509.511;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StepOpNode;221;-790.2715,-237.8564;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;190;471.6879,3858.765;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;181;-1040.944,344.3588;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;228;-583.8027,1409.947;Inherit;False;Property;_Radius;Radius;37;0;Create;True;0;0;0;False;0;False;0;12.97;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;248;-1003.22,452.1787;Inherit;False;Property;_Color2;Color 2;40;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0.4301988,0.4301988,0.4301988,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;174;-128.1417,-1026.384;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;230;-421.8153,1445.947;Inherit;False;Property;_FallOff;FallOff;38;0;Create;True;0;0;0;False;0;False;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;172;112.6554,-1019.397;Inherit;False;ToonWaveEffect;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;250;-806.8218,368.5753;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;175;-313.1417,-927.3838;Inherit;False;Property;_ToonWave;ToonWave;32;0;Create;True;0;0;0;False;0;False;0;0.16;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;220;-1952.055,195.2477;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;169;-900.7122,-1348.402;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-2376.027,-301.1866;Inherit;False;Property;_NoiseScale;NoiseScale;9;0;Create;True;0;0;0;False;0;False;0;7.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;253;158.3397,1362.314;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;203;-1218.047,490.7885;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;246;-879.6031,613.0285;Inherit;False;259;FoamCollision;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;198;-1666.632,-1443.7;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;195;214.2007,3600.032;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;227;-449.8027,1329.947;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CrossProductOpNode;168;-1077.426,-1380.402;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;225;-849.8203,1205.855;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;201;-1400.233,693.9696;Inherit;False;Property;_Float3;Float 3;33;0;Create;True;0;0;0;False;0;False;0;3.09;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;170;-759.5646,-1353.226;Inherit;False;World;Tangent;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;324;522.8417,226.2183;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCGrayscale;199;-395.3533,-1283.182;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;245;-647.6094,335.5955;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;6;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthNormals;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;7;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalGBuffer;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;911.2651,516.7381;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;Custom/Environment/Water;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;18;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;3;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;38;Workflow;1;Surface;1;  Refraction Model;0;  Blend;0;Two Sided;1;Fragment Normal Space,InvertActionOnDeselection;0;Transmission;0;  Transmission Shadow;0.5,False,-1;Translucency;0;  Translucency Strength;1,False,-1;  Normal Distortion;0.5,False,-1;  Scattering;2,False,-1;  Direct;0.9,False,-1;  Ambient;0.1,False,-1;  Shadow;0.5,False,-1;Cast Shadows;0;  Use Shadow Threshold;0;Receive Shadows;1;GPU Instancing;1;LOD CrossFade;1;Built-in Fog;1;_FinalColorxAlpha;0;Meta Pass;1;Override Baked GI;0;Extra Pre Pass;0;DOTS Instancing;0;Tessellation;1;  Phong;0;  Strength;0.5,False,-1;  Type;0;  Tess;16,False,-1;  Min;10,False,-1;  Max;25,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Write Depth;0;  Early Z;1;Vertex Position,InvertActionOnDeselection;1;0;8;False;True;False;True;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;5;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;37;0;223;0
WireConnection;272;0;267;0
WireConnection;272;1;268;0
WireConnection;305;0;141;0
WireConnection;8;1;10;0
WireConnection;8;0;9;0
WireConnection;303;0;77;0
WireConnection;270;0;272;0
WireConnection;270;1;269;0
WireConnection;119;0;37;0
WireConnection;82;0;8;0
WireConnection;212;0;112;0
WireConnection;252;0;306;0
WireConnection;274;0;270;0
WireConnection;274;1;275;0
WireConnection;58;0;141;0
WireConnection;75;0;141;0
WireConnection;78;0;77;0
WireConnection;120;0;119;0
WireConnection;72;0;58;0
WireConnection;72;2;77;0
WireConnection;22;0;35;0
WireConnection;273;0;274;0
WireConnection;76;0;75;0
WireConnection;76;2;78;0
WireConnection;254;0;252;0
WireConnection;254;2;304;0
WireConnection;117;0;120;0
WireConnection;117;2;118;0
WireConnection;315;0;308;2
WireConnection;211;12;72;0
WireConnection;211;2;216;0
WireConnection;211;11;215;0
WireConnection;92;0;75;0
WireConnection;92;2;77;0
WireConnection;213;12;76;0
WireConnection;213;2;216;0
WireConnection;213;11;215;0
WireConnection;251;0;254;0
WireConnection;251;2;266;0
WireConnection;307;1;273;0
WireConnection;307;0;315;0
WireConnection;116;1;117;0
WireConnection;81;0;22;1
WireConnection;81;1;83;0
WireConnection;11;0;8;0
WireConnection;11;1;12;0
WireConnection;74;0;213;0
WireConnection;74;2;59;0
WireConnection;43;0;116;1
WireConnection;214;12;92;0
WireConnection;214;2;216;0
WireConnection;214;11;215;0
WireConnection;80;0;81;0
WireConnection;80;2;96;0
WireConnection;264;0;251;0
WireConnection;264;1;307;0
WireConnection;57;0;211;0
WireConnection;57;2;59;0
WireConnection;34;0;80;0
WireConnection;34;2;23;0
WireConnection;91;0;214;0
WireConnection;91;2;59;0
WireConnection;69;0;57;0
WireConnection;69;1;74;0
WireConnection;262;0;264;0
WireConnection;32;0;84;0
WireConnection;16;0;11;0
WireConnection;42;0;43;0
WireConnection;42;1;40;0
WireConnection;259;0;262;0
WireConnection;41;0;42;0
WireConnection;51;0;16;0
WireConnection;89;0;69;0
WireConnection;89;1;91;0
WireConnection;89;2;90;0
WireConnection;20;0;32;0
WireConnection;20;1;34;0
WireConnection;79;0;85;0
WireConnection;79;1;89;0
WireConnection;28;0;20;0
WireConnection;28;1;29;0
WireConnection;279;0;280;0
WireConnection;279;1;89;0
WireConnection;49;0;41;0
WireConnection;54;0;28;0
WireConnection;278;0;79;0
WireConnection;278;1;279;0
WireConnection;278;2;276;0
WireConnection;110;2;111;0
WireConnection;110;11;212;0
WireConnection;149;0;148;0
WireConnection;30;0;19;0
WireConnection;30;1;52;0
WireConnection;146;0;147;0
WireConnection;146;2;149;0
WireConnection;109;0;110;0
WireConnection;56;0;278;0
WireConnection;145;0;147;0
WireConnection;145;2;148;0
WireConnection;39;0;50;0
WireConnection;39;1;30;0
WireConnection;31;0;39;0
WireConnection;31;1;55;0
WireConnection;143;1;145;0
WireConnection;144;1;146;0
WireConnection;102;1;109;0
WireConnection;101;35;102;0
WireConnection;101;37;103;0
WireConnection;150;0;143;1
WireConnection;150;1;144;1
WireConnection;316;28;318;0
WireConnection;316;25;319;0
WireConnection;316;26;320;0
WireConnection;316;27;321;0
WireConnection;137;0;136;0
WireConnection;71;0;31;0
WireConnection;71;1;70;0
WireConnection;106;0;101;38
WireConnection;244;0;71;0
WireConnection;134;1;137;0
WireConnection;134;5;135;0
WireConnection;317;0;316;0
WireConnection;115;0;114;0
WireConnection;152;0;150;0
WireConnection;152;1;153;0
WireConnection;152;2;154;0
WireConnection;231;0;273;0
WireConnection;193;0;134;0
WireConnection;193;1;194;0
WireConnection;142;0;152;0
WireConnection;113;0;106;0
WireConnection;113;1;115;0
WireConnection;240;0;239;0
WireConnection;240;1;241;0
WireConnection;330;0;328;0
WireConnection;330;1;232;0
WireConnection;329;0;327;0
WireConnection;329;1;232;0
WireConnection;107;0;113;0
WireConnection;133;0;193;0
WireConnection;325;0;232;0
WireConnection;325;1;323;0
WireConnection;325;2;326;0
WireConnection;249;0;181;0
WireConnection;249;1;245;0
WireConnection;196;0;195;0
WireConnection;166;0;198;0
WireConnection;209;0;208;2
WireConnection;209;1;210;0
WireConnection;237;0;243;0
WireConnection;224;0;225;0
WireConnection;224;1;226;0
WireConnection;310;0;315;0
WireConnection;36;0;37;0
WireConnection;36;1;44;0
WireConnection;243;0;229;0
WireConnection;242;0;151;0
WireConnection;242;1;240;0
WireConnection;202;0;203;0
WireConnection;229;0;227;0
WireConnection;229;1;230;0
WireConnection;188;0;197;0
WireConnection;189;0;152;0
WireConnection;189;1;153;0
WireConnection;167;0;198;0
WireConnection;221;0;116;1
WireConnection;221;1;222;0
WireConnection;190;0;189;0
WireConnection;181;0;71;0
WireConnection;181;2;202;0
WireConnection;174;0;199;0
WireConnection;174;1;175;0
WireConnection;172;0;174;0
WireConnection;250;0;181;0
WireConnection;250;1;248;0
WireConnection;220;1;30;0
WireConnection;169;0;168;0
WireConnection;253;0;237;0
WireConnection;253;1;252;2
WireConnection;203;0;209;0
WireConnection;203;1;201;0
WireConnection;195;0;152;0
WireConnection;227;0;224;0
WireConnection;227;1;228;0
WireConnection;168;0;167;0
WireConnection;168;1;166;0
WireConnection;170;0;169;0
WireConnection;324;0;329;0
WireConnection;324;1;330;0
WireConnection;324;2;325;0
WireConnection;199;0;168;0
WireConnection;245;1;248;0
WireConnection;245;2;246;0
WireConnection;1;0;324;0
WireConnection;1;1;192;0
WireConnection;1;2;108;0
WireConnection;1;3;127;0
WireConnection;1;4;128;0
WireConnection;1;6;53;0
WireConnection;1;8;242;0
ASEEND*/
//CHKSM=E85ABFF4DF83F80AF883E1403984296E025738D4