﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//----------------------------------------------------
//--                                                --
//--             www.riemers.net                 --
//--         Series 4: Advanced terrain             --
//--                 Shader code                    --
//--                                                --
//----------------------------------------------------

//------- Constants --------
float4x4 View;
float4x4 Projection;
float4x4 World;
float3 LightDirection;
float4 CameraPosition;
float Ambient;
bool EnableLighting;

//-----------------------------------------
//------- Technique: Multitextured --------
//-----------------------------------------

Texture xTexture;
Texture xTexture0;
Texture xTexture1;
Texture xTexture2;
Texture xTexture3;

sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = mirror; AddressV = mirror; }; 
sampler TextureSampler0 = sampler_state { texture = <xTexture0>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; }; 
sampler TextureSampler1 = sampler_state { texture = <xTexture1>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; }; 
sampler TextureSampler2 = sampler_state { texture = <xTexture2>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = mirror; AddressV = mirror; }; 
sampler TextureSampler3 = sampler_state { texture = <xTexture3>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = mirror; AddressV = mirror; };


struct MTVertexToPixel
{
	float4 Position         : POSITION0;
	float4 Color            : COLOR0;
	float3 Normal            : TEXCOORD0;
	float2 TextureCoords    : TEXCOORD1;
	float4 LightDirection    : TEXCOORD2;
	float4 TextureWeights    : TEXCOORD3;
};

struct MTPixelToFrame
{
	float4 Color : COLOR0;
};

MTVertexToPixel MultiTexturedVS(float4 inPos : POSITION0, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD0, float4 inTexWeights : TEXCOORD1)
{
	MTVertexToPixel Output = (MTVertexToPixel)0;
	float4x4 preViewProjection = mul(View, Projection);
	float4x4 preWorldViewProjection = mul(World, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Normal = (float3)mul(float4(normalize(inNormal), 1), World);
	Output.TextureCoords = inTexCoords;
	Output.LightDirection.xyz = -LightDirection;
	Output.LightDirection.w = 1;
	Output.TextureWeights = inTexWeights;

	return Output;
}

MTPixelToFrame MultiTexturedPS(MTVertexToPixel PSIn)
{
	MTPixelToFrame Output = (MTPixelToFrame)0;

	float lightingFactor = 1;
	if (EnableLighting)
		lightingFactor = saturate(saturate(dot(float4(PSIn.Normal, 1), PSIn.LightDirection)) + Ambient);

	Output.Color = tex2D(TextureSampler0, PSIn.TextureCoords)*PSIn.TextureWeights.x;
	Output.Color += tex2D(TextureSampler1, PSIn.TextureCoords)*PSIn.TextureWeights.y;
	Output.Color += tex2D(TextureSampler2, PSIn.TextureCoords)*PSIn.TextureWeights.z;
	Output.Color += tex2D(TextureSampler3, PSIn.TextureCoords)*PSIn.TextureWeights.w;

	Output.Color *= lightingFactor;

	return Output;
}

technique MultiTextured
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MultiTexturedVS();
		PixelShader = compile PS_SHADERMODEL MultiTexturedPS();
	}
}


//--------------Technique:Skybox----------------
Texture SkyBoxTexture; 
samplerCUBE SkyBoxSampler = sampler_state 
{ 
   texture = <SkyBoxTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Mirror; 
   AddressV = Mirror; 
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 TextureCoordinate : TEXCOORD0;
};
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    output.TextureCoordinate = (worldPosition - CameraPosition).xyz;
 
    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return texCUBE(SkyBoxSampler, input.TextureCoordinate);
}
 
technique Skybox
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}

//-------------------------------------------------------------------
//-----------------Technique: day&night circle skybox ---------------
//-------------------------------------------------------------------

Texture afternoonSkyTexture;
Texture morningSkyTexture;
Texture sunsetSkyTexture;
Texture nightSkyTexture;

float4 timeWeight;


samplerCUBE morningSkySampler = sampler_state { texture = <morningSkyTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = Mirror; AddressV = Mirror; };
samplerCUBE afternnonSkySampler = sampler_state { texture = <afternoonSkyTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = Mirror; AddressV = Mirror; };
samplerCUBE sunsetSkySampler = sampler_state { texture = <sunsetSkyTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = Mirror; AddressV = Mirror; };
samplerCUBE nightSkySampler = sampler_state { texture = <nightSkyTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = Mirror; AddressV = Mirror; };

void DayNightSkyboxVS(float4 inPos : POSITION0, out float4 outPos : POSITION0, out float3 texCoord : TEXCOORD0) {
	float4 worldPosition = mul(inPos, World);
	float4 viewPosition = mul(worldPosition, View);
	outPos = mul(viewPosition, Projection);
	
	texCoord = (worldPosition - CameraPosition).xyz;
}

float4 DayNightSkyBoxPS(float4 inPos: POSITION0, float3 texCoord : TEXCOORD0) : COLOR0 {
	return texCUBE(morningSkySampler, texCoord) * timeWeight.x 
		+ texCUBE(afternnonSkySampler, texCoord) * timeWeight.y
		+ texCUBE(sunsetSkySampler, texCoord) * timeWeight.z
		+ texCUBE(nightSkySampler, texCoord) * timeWeight.w;
;
}

technique DayNightSkybox{
	pass Pass0{
		VertexShader = compile VS_SHADERMODEL DayNightSkyboxVS();
        PixelShader = compile PS_SHADERMODEL DayNightSkyBoxPS();
	}
}


//-------------------------------------------------------------------
//-----------------Technique: multi textured map refraction----------
//-------------------------------------------------------------------

float4 ClipPlane;

void RefractionVS(float4 inPos: POSITION0, out float outClipDistances: TEXCOORD0,
				out float4 outPos: POSITION0){
	float4 worldPosition = mul(inPos, World);
	float4 viewPosition = mul(worldPosition, View);
	outPos = mul(viewPosition, Projection);
	
	float4 worldClipPlane = mul(ClipPlane, World);
	float4 worldViewClipPlane = mul(worldClipPlane, View);
	float4 worldViewProjClipPlane = mul(worldViewClipPlane, Projection);
	
	outClipDistances = outPos.x * worldViewProjClipPlane.x + outPos.y * worldViewProjClipPlane.y + outPos.z * worldViewProjClipPlane.z + worldViewProjClipPlane.w;
}

float4 RefractionPS(float4 pos: POSITION0, float clipDistances: TEXCOORD0) : COLOR0{
	clip(clipDistances);
	
	return float4(0, 0, 0, 0);
}

technique MultiTexturedMapRefraction { 
	
	/*pass Pass0 { 
        VertexShader = compile VS_SHADERMODEL RefractionVS(); 
        PixelShader = compile PS_SHADERMODEL RefractionPS(); 
    } */
	
	pass Pass1 {
		VertexShader = compile VS_SHADERMODEL MultiTexturedVS();
		PixelShader = compile PS_SHADERMODEL MultiTexturedPS(); 
	}
	
} 

//-------------------------------------------------------------------
//-----------------Technique: multi textured sky reflection----------
//-------------------------------------------------------------------

void DayNightSkyboxReflectionVS(float4 inPos : POSITION0, out float4 outPos : POSITION0, out float3 texCoord : TEXCOORD0) {
	float4 worldPosition = mul(inPos, World);
	float4 viewPosition = mul(worldPosition, View);
	outPos = mul(viewPosition, Projection);
	
	texCoord = (worldPosition - CameraPosition).xyz;
	
	//float clipPlanePos = worldPosition.x * ClipPlane.x + worldPosition.y * ClipPlane.y + worldPosition.z * ClipPlane.z + ClipPlane.w;
	//if (clipPlanePos < 0)
		texCoord.y = -texCoord.y;
}

technique MultiTexturedSkyReflection { 
	
	/*pass Pass0 { 
        VertexShader = compile VS_SHADERMODEL RefractionVS(); 
        PixelShader = compile PS_SHADERMODEL RefractionPS(); 
    } */
	
	pass Pass1 {
		VertexShader = compile VS_SHADERMODEL DayNightSkyboxReflectionVS();
		PixelShader = compile PS_SHADERMODEL DayNightSkyBoxPS();
	}
} 

//-------------------------------------------------------------------
//-----------------Technique: water ---------------------------------
//-------------------------------------------------------------------

float4x4 ReflectionView;
Texture ReflectionMap;
Texture WaterBumpMap;
//Texture RefractionMap;
float WaveLength;
float WaveHeight;

sampler ReflectionSampler = sampler_state { texture = <ReflectionMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};
sampler WaterBumpMapSampler = sampler_state { texture = <WaterBumpMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};
sampler RefractionSampler = sampler_state { texture = <RefractionMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

void WaterVS(float4 inPos: POSITION0, float2 inTex: TEXCOORD0,
			out float4 outPos: POSITION0, out float4 outReflectionMapSamplingPos: TEXCOORD1, out float2 outBumpMapSamplingPos: TEXCOORD2,
			out float4 outRefractionMapSamplingPos: TEXCOORD3, out float4 outPosition3D: TEXCOORD4){
	
	float4x4 preViewProjection = mul (View, Projection);
	float4x4 preWorldViewProjection = mul (World, preViewProjection);
	float4x4 preReflectionViewProjection = mul (ReflectionView, Projection);
	float4x4 preWorldReflectionViewProjection = mul (World, preReflectionViewProjection);
	
	outPos = mul(inPos, preWorldViewProjection);
	outReflectionMapSamplingPos = mul(inPos, preWorldReflectionViewProjection);
	outBumpMapSamplingPos = inTex / WaveLength;
	outRefractionMapSamplingPos = mul(inPos, preWorldViewProjection);
	outPosition3D = mul(inPos, World);
}

float4 WaterPS(float4 pos: POSITION0, float4 reflectionMapSamplingPos: TEXCOORD1, float2 bumpMapSamplingPos: TEXCOORD2,
				float4 refractionMapSamplingPos: TEXCOORD3, float4 position3D: TEXCOORD4) : COLOR0{
	float2 ProjectedTexCoords;
	ProjectedTexCoords.x = reflectionMapSamplingPos.x/reflectionMapSamplingPos.w/2.0f + 0.5f;
	ProjectedTexCoords.y = -reflectionMapSamplingPos.y/reflectionMapSamplingPos.w/2.0f + 0.5f;
	float4 bumpColor = tex2D(WaterBumpMapSampler, bumpMapSamplingPos);
	float2 perturbation = WaveHeight*(bumpColor.rg - 0.5f)*2.0f;
	float2 perturbatedTexCoords = ProjectedTexCoords + perturbation;
	float4 reflectiveColor = tex2D(ReflectionSampler, perturbatedTexCoords);
	
	float2 ProjectedRefrTexCoords;
	ProjectedRefrTexCoords.x = refractionMapSamplingPos.x/refractionMapSamplingPos.w/2.0f + 0.5f;
	ProjectedRefrTexCoords.y = -refractionMapSamplingPos.y/refractionMapSamplingPos.w/2.0f + 0.5f;
	float2 perturbatedRefrTexCoords = ProjectedRefrTexCoords + perturbation;
	float4 refractiveColor = tex2D(RefractionSampler, perturbatedRefrTexCoords);
	
	float3 eyeVector = normalize(CameraPosition - position3D);
	float3 normalVector = float3(0,1,0);
	float fresnelTerm = dot(eyeVector, normalVector);    
	float4 combinedColor = lerp(reflectiveColor, refractiveColor, fresnelTerm);
	
	float4 dullColor = float4(0.3f, 0.3f, 0.5f, 1.0f);
	
	return lerp(combinedColor, dullColor, 0.2f);
}

technique Water {
	pass Pass0 {
		VertexShader = compile VS_SHADERMODEL WaterVS();
		PixelShader = compile PS_SHADERMODEL WaterPS();
	}
}