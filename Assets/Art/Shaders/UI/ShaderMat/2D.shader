// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/UIEffect"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin][NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		[Header(Edge)][NoScaleOffset]_EdgeTexture("EdgeTexture", 2D) = "white" {}
		_BaseColor("BaseColor", Color) = (0,0,0,0)
		[HDR]_EdgeColor("EdgeColor", Color) = (0,0,0,0)
		_EdgeStrenght("EdgeStrenght", Float) = 0
		_EdgeSpeedNoise("EdgeSpeedNoise", Vector) = (0,0,0,0)
		_Length("Length", Range( 0 , 1)) = 0
		[Header(Opacity)]_Opacity("Opacity", Range( 0 , 1)) = 0
		[Header(Pattern)][NoScaleOffset]_PatternTexture("Pattern Texture", 2D) = "white" {}
		[NoScaleOffset]_NoiseMask("NoiseMask", 2D) = "white" {}
		_NoiseIntensity("NoiseIntensity", Float) = 0
		[HDR]_PatternColor("PatternColor", Color) = (1,0,0,0)
		[Header(This two parameters controls the size of the texutre)]_MinSize("MinSize", Float) = 1
		_MaxSize("MaxSize", Float) = 0
		[Header(Background)][NoScaleOffset]_BackgroundTexture("BackgroundTexture", 2D) = "white" {}
		[NoScaleOffset]_FlowTexture("FlowTexture", 2D) = "white" {}
		_BackgroundSpeed("BackgroundSpeed", Vector) = (0,0,0,0)
		[HDR]_BackgroundColor("BackgroundColor", Color) = (0,0,0,0)
		_BackgroundIntensity("BackgroundIntensity", Range( 0 , 1)) = 0
		[ASEEnd]_FlowStrength("FlowStrength", Range( 0 , 1)) = 0

	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		
		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 

		ENDHLSL

		
		Pass
		{
			Name "Unlit"
			

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#define ASE_SRP_VERSION 100302

			
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS_SPRITEUNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			

			sampler2D _MainTex;
			sampler2D _EdgeTexture;
			sampler2D _BackgroundTexture;
			sampler2D _FlowTexture;
			sampler2D _PatternTexture;
			sampler2D _NoiseMask;
			CBUFFER_START( UnityPerMaterial )
			float4 _BaseColor;
			float4 _EdgeColor;
			float4 _BackgroundColor;
			float4 _PatternColor;
			float2 _EdgeSpeedNoise;
			float2 _BackgroundSpeed;
			float _Length;
			float _EdgeStrenght;
			float _FlowStrength;
			float _BackgroundIntensity;
			float _MinSize;
			float _MaxSize;
			float _NoiseIntensity;
			float _Opacity;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D( _AlphaTex ); SAMPLER( sampler_AlphaTex );
				float _EnableAlphaTexture;
			#endif

			float4 _RendererColor;

					float2 voronoihash183( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi183( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash183( n + g );
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
			

			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.vertex.xyz );

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.clipPos = vertexInput.positionCS;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 texCoord3 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float4 MainText137 = tex2D( _MainTex, texCoord3 );
				float2 texCoord45 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float BaseMask134 = ( 1.0 - ( ( texCoord45.x - (-0.35 + (_Length - 0.0) * (0.9 - -0.35) / (1.0 - 0.0)) ) * 3.0 ) );
				float2 texCoord9 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner7 = ( 1.0 * _Time.y * _EdgeSpeedNoise + texCoord9);
				float Noise126 = ( tex2D( _EdgeTexture, panner7 ).r * _EdgeStrenght );
				float temp_output_58_0 = saturate( ( 1.0 - ( BaseMask134 + Noise126 ) ) );
				float4 lerpResult31 = lerp( ( _BaseColor * MainText137 ) , _EdgeColor , temp_output_58_0);
				float2 texCoord146 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner112 = ( 1.0 * _Time.y * float2( 0.75,0 ) + texCoord146);
				float2 texCoord111 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner231 = ( 1.0 * _Time.y * _BackgroundSpeed + texCoord111);
				float2 lerpResult140 = lerp( (tex2D( _FlowTexture, panner112 )).rg , panner231 , _FlowStrength);
				float CloudsBackground196 = pow( tex2D( _BackgroundTexture, lerpResult140 ).r , (0.0 + (( 1.0 - _BackgroundIntensity ) - 0.0) * (35.0 - 0.0) / (1.0 - 0.0)) );
				float4 lerpResult197 = lerp( lerpResult31 , _BackgroundColor , saturate( ( CloudsBackground196 - temp_output_58_0 ) ));
				float U130 = texCoord45.x;
				float2 temp_cast_0 = (_MinSize).xx;
				float2 temp_cast_1 = (_MaxSize).xx;
				float time183 = _TimeParameters.x;
				float2 voronoiSmoothId2 = 0;
				float2 texCoord153 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner191 = ( 1.0 * _Time.y * float2( 0.33,0 ) + texCoord153);
				float2 coords183 = panner191 * 3.96;
				float2 id183 = 0;
				float2 uv183 = 0;
				float voroi183 = voronoi183( coords183, time183, id183, uv183, 0, voronoiSmoothId2 );
				float2 texCoord190 = IN.texCoord0.xy * float2( 1,1 ) + uv183;
				float2 smoothstepResult192 = smoothstep( temp_cast_0 , temp_cast_1 , texCoord190);
				float2 panner156 = ( 1.0 * _Time.y * float2( -0.22,0 ) + texCoord153);
				float4 Pattern132 = ( _PatternColor * ( tex2D( _PatternTexture, smoothstepResult192 ).a * ( tex2D( _NoiseMask, panner156 ).r * _NoiseIntensity ) ) );
				float4 temp_cast_2 = (pow( temp_output_58_0 , 0.18 )).xxxx;
				float4 appendResult17 = (float4(( lerpResult197 + saturate( ( ( pow( U130 , 2.09 ) * Pattern132 ) - temp_cast_2 ) ) ).rgb , ( saturate( BaseMask134 ) * _Opacity * (MainText137).a )));
				
				float4 Color = appendResult17;

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D( _AlphaTex, sampler_AlphaTex, IN.texCoord0.xy );
					Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture );
				#endif

				Color *= IN.color;

				return Color;
			}

			ENDHLSL
		}
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18912
0;452;1162;367;416.7501;542.2639;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;48;-2172.348,236.0231;Inherit;False;Property;_Length;Length;6;0;Create;True;0;0;0;False;0;False;0;0.969;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;153;-936.9296,1694.286;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;45;-1974.01,74.884;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;184;-765.8304,1822.103;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;191;-737.6816,1705.081;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.33,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;89;-4030.083,402.813;Inherit;False;Property;_EdgeSpeedNoise;EdgeSpeedNoise;5;0;Create;True;0;0;0;False;0;False;0,0;0.1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;146;-4899.771,826.256;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-4048.307,281.0772;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;220;-1908.614,235.5336;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.35;False;4;FLOAT;0.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-1673.471,298.7691;Inherit;False;Constant;_Power;Power;6;0;Create;True;0;0;0;False;0;False;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;112;-4698.562,823.1762;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.75,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;47;-1667.984,184.7262;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;183;-567.5088,1710.461;Inherit;False;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;3.96;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.PannerNode;7;-3838.533,289.2701;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.06,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;190;-330.5399,1377.606;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;111;-4290.885,891.5933;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;193;-315.6916,1507.92;Inherit;False;Property;_MinSize;MinSize;12;1;[Header];Create;True;1;This two parameters controls the size of the texutre;0;0;False;0;False;1;0.12;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-3189.912,343.0245;Inherit;False;Property;_EdgeStrenght;EdgeStrenght;4;0;Create;True;0;0;0;False;0;False;0;0.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;219;-3404.829,121.0205;Inherit;True;Property;_EdgeTexture;EdgeTexture;1;2;[Header];[NoScaleOffset];Create;True;1;Edge;0;0;False;0;False;-1;None;954273eaa13e9c74a849033e34125bb2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;195;-313.8722,1579.146;Inherit;False;Property;_MaxSize;MaxSize;13;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-1496.085,192.7605;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;91;-4823.06,656.9687;Inherit;False;Property;_BackgroundSpeed;BackgroundSpeed;16;0;Create;True;0;0;0;False;0;False;0,0;0.2,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;144;-4535.235,788.2141;Inherit;True;Property;_FlowTexture;FlowTexture;15;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;ce53970b67ecaa248b97bfd5f81086e6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;156;-782.4373,2013.013;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.22,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;54;-1268.324,181.8194;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;155;-539.8863,2239.799;Inherit;False;Property;_NoiseIntensity;NoiseIntensity;10;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;231;-4047.992,880.7474;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-4208.61,1034.914;Inherit;False;Property;_FlowStrength;FlowStrength;19;0;Create;True;0;0;0;False;0;False;0;0.957;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;216;-625.7119,2029.167;Inherit;True;Property;_NoiseMask;NoiseMask;9;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;954273eaa13e9c74a849033e34125bb2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-2983.265,222.5224;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;145;-4270.125,792.29;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;233;-3876.407,1009.033;Inherit;False;Property;_BackgroundIntensity;BackgroundIntensity;18;0;Create;True;0;0;0;False;0;False;0;0.835;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;192;-96.49812,1449.297;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;235;-3621.407,1011.033;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;154;-276.1378,2039.773;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;-1095.032,224.9663;Inherit;False;BaseMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;140;-3786.271,819.8921;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;177;366.7127,1237.879;Inherit;True;Property;_PatternTexture;Pattern Texture;8;2;[Header];[NoScaleOffset];Create;True;1;Pattern;0;0;False;0;False;-1;None;919ed6f34c093ec43a66629f160615b8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;126;-2735.477,245.8854;Inherit;False;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;799.0211,1317.061;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;124;626.7925,944.2328;Inherit;False;Property;_PatternColor;PatternColor;11;1;[HDR];Create;True;0;0;0;False;0;False;1,0,0,0;21.18857,20.52226,18.68992,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;217;-3622.928,791.1494;Inherit;True;Property;_BackgroundTexture;BackgroundTexture;14;2;[Header];[NoScaleOffset];Create;True;1;Background;0;0;False;0;False;-1;None;eeede91d8501e9544bdfe0ae1c627277;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;127;-706.7657,-60.03622;Inherit;False;126;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;135;-708.8798,-125.4581;Inherit;False;134;BaseMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;234;-3498.407,1012.033;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;35;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;1001.011,1088.343;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.05660379;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;-526.3167,-108.8564;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-846.3686,-563.6975;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;98;-3301.76,830.4825;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;4.68;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-1622.205,72.11735;Inherit;False;U;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;196;-3014.567,858.0843;Inherit;False;CloudsBackground;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;57;-411.3353,-100.6022;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;132;1259.171,1136.499;Inherit;False;Pattern;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;0.7155008,27.3052;Inherit;False;130;U;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-640.5931,-578.903;Inherit;True;Property;_MainTex;MainTex;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;44efd0011d6a9bc4fb0b3a82753dac4e;44efd0011d6a9bc4fb0b3a82753dac4e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;133;102.7155,138.3052;Inherit;False;132;Pattern;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;58;-279.9741,-106.9379;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;-192.2398,-713.809;Inherit;False;Property;_BaseColor;BaseColor;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.1101114,0.5960784,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;99;151.7156,28.3052;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2.09;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;137;-302.1566,-513.5803;Inherit;False;MainText;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;198;286.5497,-236.816;Inherit;False;196;CloudsBackground;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;204;559.9449,-251.4523;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;88;189.7156,235.3052;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0.18;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;236;170.2076,-471.603;Inherit;False;Property;_EdgeColor;EdgeColor;3;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0.4078431,2.996078,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;25.24973,-549.3746;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;290.7155,34.3052;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;138;487.3287,369.067;Inherit;False;137;MainText;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;136;575.7913,193.542;Inherit;False;134;BaseMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;199;390.2082,-412.2876;Inherit;False;Property;_BackgroundColor;BackgroundColor;17;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;2.828427,2.828427,2.828427,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;31;648.5237,-507.5851;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;78;534.7722,3.993949;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;205;777.2679,-355.3118;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;197;896.2074,-491.5649;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;70;693.9522,-9.931677;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;139;651.8633,373.9371;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;51;726.6312,180.4317;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;484.0113,311.1647;Inherit;False;Property;_Opacity;Opacity;7;1;[Header];Create;True;1;Opacity;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;897.9838,267.9116;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;157;1256.751,-276.3282;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;17;1485.612,-225.4442;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;214;1680.744,-234.0203;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;13;Custom/UIEffect;cf964e524c8e69742b1d21fbe2ebcc4a;True;Unlit;0;0;Unlit;3;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;2;5;False;-1;10;False;-1;3;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;1;Vertex Position;1;0;1;True;False;;False;0
WireConnection;191;0;153;0
WireConnection;220;0;48;0
WireConnection;112;0;146;0
WireConnection;47;0;45;1
WireConnection;47;1;220;0
WireConnection;183;0;191;0
WireConnection;183;1;184;0
WireConnection;7;0;9;0
WireConnection;7;2;89;0
WireConnection;190;1;183;2
WireConnection;219;1;7;0
WireConnection;49;0;47;0
WireConnection;49;1;50;0
WireConnection;144;1;112;0
WireConnection;156;0;153;0
WireConnection;54;0;49;0
WireConnection;231;0;111;0
WireConnection;231;2;91;0
WireConnection;216;1;156;0
WireConnection;75;0;219;1
WireConnection;75;1;74;0
WireConnection;145;0;144;0
WireConnection;192;0;190;0
WireConnection;192;1;193;0
WireConnection;192;2;195;0
WireConnection;235;0;233;0
WireConnection;154;0;216;1
WireConnection;154;1;155;0
WireConnection;134;0;54;0
WireConnection;140;0;145;0
WireConnection;140;1;231;0
WireConnection;140;2;143;0
WireConnection;177;1;192;0
WireConnection;126;0;75;0
WireConnection;150;0;177;4
WireConnection;150;1;154;0
WireConnection;217;1;140;0
WireConnection;234;0;235;0
WireConnection;122;0;124;0
WireConnection;122;1;150;0
WireConnection;44;0;135;0
WireConnection;44;1;127;0
WireConnection;98;0;217;1
WireConnection;98;1;234;0
WireConnection;130;0;45;1
WireConnection;196;0;98;0
WireConnection;57;0;44;0
WireConnection;132;0;122;0
WireConnection;6;1;3;0
WireConnection;58;0;57;0
WireConnection;99;0;131;0
WireConnection;137;0;6;0
WireConnection;204;0;198;0
WireConnection;204;1;58;0
WireConnection;88;0;58;0
WireConnection;16;0;13;0
WireConnection;16;1;137;0
WireConnection;67;0;99;0
WireConnection;67;1;133;0
WireConnection;31;0;16;0
WireConnection;31;1;236;0
WireConnection;31;2;58;0
WireConnection;78;0;67;0
WireConnection;78;1;88;0
WireConnection;205;0;204;0
WireConnection;197;0;31;0
WireConnection;197;1;199;0
WireConnection;197;2;205;0
WireConnection;70;0;78;0
WireConnection;139;0;138;0
WireConnection;51;0;136;0
WireConnection;52;0;51;0
WireConnection;52;1;53;0
WireConnection;52;2;139;0
WireConnection;157;0;197;0
WireConnection;157;1;70;0
WireConnection;17;0;157;0
WireConnection;17;3;52;0
WireConnection;214;1;17;0
ASEEND*/
//CHKSM=56E31E5EA33892798A92C0C3D7D96B62BEEE7E76