#if OPENGL
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
float3 CameraPosition;
float Ambient;
bool EnableLighting;

//------- Texture Samplers --------
Texture xTexture;

sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = mirror; AddressV = mirror; }; Texture xTexture0;

sampler TextureSampler0 = sampler_state { texture = <xTexture0>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; }; Texture xTexture1;

sampler TextureSampler1 = sampler_state { texture = <xTexture1>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; }; Texture xTexture2;

sampler TextureSampler2 = sampler_state { texture = <xTexture2>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = mirror; AddressV = mirror; }; Texture xTexture3;

sampler TextureSampler3 = sampler_state { texture = <xTexture3>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = mirror; AddressV = mirror; };


//------- Technique: Multitextured --------
struct MTVertexToPixel
{
	float4 Position         : POSITION;
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

MTVertexToPixel MultiTexturedVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD0, float4 inTexWeights : TEXCOORD1)
{
	MTVertexToPixel Output = (MTVertexToPixel)0;
	float4x4 preViewProjection = mul(View, Projection);
	float4x4 preWorldViewProjection = mul(World, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Normal = mul(normalize(inNormal), World);
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
		lightingFactor = saturate(saturate(dot(PSIn.Normal, PSIn.LightDirection)) + Ambient);

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


// ----------Technique: Skysphere ----------


sampler SkysphereSampler = sampler_state { 
		texture = <xTexture0>; 
		magfilter = LINEAR; 
		minfilter = LINEAR; 
		mipfilter = LINEAR; 
		AddressU = Mirror; 
		AddressV = Mirror; };

void SkysphereVS(float3 pos : POSITION0, 
				out float4 outPos : POSITION0, out float3 outTexCoord : TEXCOORD0) {
								
	float3 rotatedPosition = mul(pos, View);
	
	outPos = mul(float4(rotatedPosition, 1), Projection).xyww;
	outTexCoord = pos;
	
	
	/*outPos = mul(pos, World);
	outPos = mul(outPos, View);
	outPos = mul(outPos, Projection);
	
	float4 vertexWorldPos = mul(pos, World);
	outTexCoord = vertexWorldPos - CameraPosition;*/
}

float4 SkyspherePS(float3 SkyCoord : TEXCOORD0) : COLOR0 {
	return texCUBE(SkysphereSampler, normalize(SkyCoord)) * float4(2, 2, 2, 1);
}

technique Skysphere {
	pass Pass0 {
		VertexShader = compile VS_SHADERMODEL SkysphereVS();
		PixelShader = compile PS_SHADERMODEL SkyspherePS();
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
 
    output.TextureCoordinate = worldPosition - CameraPosition;
 
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


samplerCUBE morningSkySampler = sampler_state 
{ 
   texture = <morningSkyTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Mirror; 
   AddressV = Mirror; 
};

samplerCUBE afternnonSkySampler = sampler_state 
{ 
   texture = <afternoonSkyTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Mirror; 
   AddressV = Mirror; 
};

samplerCUBE sunsetSkySampler = sampler_state 
{ 
   texture = <sunsetSkyTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Mirror; 
   AddressV = Mirror; 
};

samplerCUBE nightSkySampler = sampler_state 
{ 
   texture = <nightSkyTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Mirror; 
   AddressV = Mirror; 
};

void DayNightSkyboxVS(float4 inPos : POSITION0, out float4 outPos : POSITION0, out float3 texCoord : TEXCOORD0) {
	float4 worldPosition = mul(inPos, World);
	float4 viewPosition = mul(worldPosition, View);
	outPos = mul(viewPosition, Projection);
	
	texCoord = worldPosition - CameraPosition;
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