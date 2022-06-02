// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/VFX/BloodSplatter"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[ASEBegin][NoScaleOffset]_Splat("Splat", 2D) = "white" {}
		[NoScaleOffset]_Sampler("Sampler", 2D) = "white" {}
		_Dissolve("Dissolve", Float) = 0
		_DissolveStrenght("DissolveStrenght", Float) = 0
		_Color0("Color 0", Color) = (0,0,0,0)
		_VertexOffset("Vertex Offset", Float) = 0
		[ASEEnd]_Float0("Float 0", Range( 0 , 1)) = 0

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
			
			#define ASE_SRP_VERSION 100400

			
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS_SPRITEUNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_COLOR


			sampler2D _Splat;
			sampler2D _Sampler;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color0;
			float _Float0;
			float _DissolveStrenght;
			float _Dissolve;
			float _VertexOffset;
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

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float2 texCoord15 = v.uv0.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_12_0_g2 = texCoord15;
				float2 panner7_g2 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g2);
				float2 lerpResult1_g2 = lerp( (tex2Dlod( _Sampler, float4( panner7_g2, 0, 0.0) )).rg , temp_output_12_0_g2 , _Float0);
				float2 temp_output_38_0 = lerpResult1_g2;
				float temp_output_16_0 = step( tex2Dlod( _Splat, float4( temp_output_38_0, 0, 0.0) ).g , ( ( tex2Dlod( _Splat, float4( temp_output_38_0, 0, 0.0) ).r * _DissolveStrenght ) + _Dissolve ) );
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = ( temp_output_16_0 * _VertexOffset * v.normal );
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

				float2 texCoord15 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_12_0_g2 = texCoord15;
				float2 panner7_g2 = ( 1.0 * _Time.y * float2( 0.12,0 ) + temp_output_12_0_g2);
				float2 lerpResult1_g2 = lerp( (tex2D( _Sampler, panner7_g2 )).rg , temp_output_12_0_g2 , _Float0);
				float2 temp_output_38_0 = lerpResult1_g2;
				float temp_output_16_0 = step( tex2D( _Splat, temp_output_38_0 ).g , ( ( tex2D( _Splat, temp_output_38_0 ).r * _DissolveStrenght ) + _Dissolve ) );
				float4 appendResult28 = (float4(( ( temp_output_16_0 * _Color0 ) * IN.color ).rgb , saturate( ( temp_output_16_0 * IN.color.a ) )));
				
				float4 Color = appendResult28;

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
0;431;1173;388;1855.008;279.4818;1.27787;True;False
Node;AmplifyShaderEditor.RangedFloatNode;37;-1464.714,98.97603;Inherit;False;Property;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;0;0.783;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;-1484.882,-7.768372;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;38;-1187.027,21.75279;Inherit;False;FlowMapMask;1;;2;a576844f19963ec46bc1db551bb181c0;0;4;12;FLOAT2;0,0;False;2;FLOAT;0;False;10;FLOAT2;0,0;False;11;SAMPLER2D;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;6;-972.95,-164.5166;Inherit;True;Property;_Splat;Splat;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;1431c215dd8a64249a07c75796f32842;1431c215dd8a64249a07c75796f32842;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;20;-809.6753,22.92671;Inherit;False;Property;_DissolveStrenght;DissolveStrenght;4;0;Create;True;0;0;0;False;0;False;0;2.99;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-633.9501,-178.1573;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.13;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-612.2052,-41.87214;Inherit;False;Property;_Dissolve;Dissolve;3;0;Create;True;0;0;0;False;0;False;0;-0.42;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;-476.1461,-147.5941;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-809.198,99.32169;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;6;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;16;-345.6572,-36.00048;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;23;-242.9674,-219.1271;Inherit;False;Property;_Color0;Color 0;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;24;-122.6892,-42.15005;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;13.07265,-149.4274;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;57.21495,11.39925;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;131.7476,192.158;Inherit;False;Property;_VertexOffset;Vertex Offset;6;0;Create;True;0;0;0;False;0;False;0;0.002;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;129.9349,-74.49812;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;9;156.9918,111.3879;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;35;143.8057,274.7335;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;28;345.7297,-48.21289;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;351.2213,187.8202;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;27;630.4078,-18.20555;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;13;Custom/VFX/BloodSplatter;cf964e524c8e69742b1d21fbe2ebcc4a;True;Unlit;0;0;Unlit;3;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;2;5;False;-1;10;False;-1;3;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;1;Vertex Position;1;0;1;True;False;;False;0
WireConnection;38;12;15;0
WireConnection;38;2;37;0
WireConnection;6;1;38;0
WireConnection;19;0;6;1
WireConnection;19;1;20;0
WireConnection;17;0;19;0
WireConnection;17;1;13;0
WireConnection;14;1;38;0
WireConnection;16;0;14;2
WireConnection;16;1;17;0
WireConnection;22;0;16;0
WireConnection;22;1;23;0
WireConnection;26;0;16;0
WireConnection;26;1;24;4
WireConnection;25;0;22;0
WireConnection;25;1;24;0
WireConnection;9;0;26;0
WireConnection;28;0;25;0
WireConnection;28;3;9;0
WireConnection;30;0;16;0
WireConnection;30;1;31;0
WireConnection;30;2;35;0
WireConnection;27;1;28;0
WireConnection;27;3;30;0
ASEEND*/
//CHKSM=20F3E76043913CA586F38158C719D01FF8C27FB7