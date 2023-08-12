Shader "Card/Front"
{
  // Properties are options set per material,
  // exposed by the material inspector.
  Properties
  {
    // [MainTexture] allow Material.mainTexture to use the correct properties.
    [MainTexture] _BaseMap("Image (RGBA)", 2D) = "white" {}
    
    _FrameTex("Frame (RGBA)", 2D) = "white" {}
    _FrameColor("Frame Color", Color) = (1, 1, 1, 1)

    _BackgroundTex("Background (RGB)", 2D) = "white" {}

    _FrontFXTex("Front FX (RGBA)", 2D) = "white" {}
    _FrontFXScrollVelocity("Front Scroll Velocity", Vector) = (0, 0, 0, 0)
    
    _BackFXTex("Background FX (RGBA)", 2D) = "white" {}
    _BackFXScrollVelocity("Background Scroll Velocity", Vector) = (0, 0, 0, 0)
  }

  // Subshaders allow for different behaviour and options for
  // different pipelines and platforms.
  SubShader
  {
    // These tags are shared by all passes in this sub shader.
    Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" }

    // Shaders can have several passes which are used to render
    // different data about the material. Each pass has it's own
    // vertex and fragment function and shader variant keywords.
    Pass
    {
      // Begin HLSL code
      HLSLPROGRAM
      
      // Register our programmable stage functions.
      #pragma vertex vert
      #pragma fragment frag

      // Include basics URP functions.
      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

      // This attributes struct receives data about the mesh we're
      // currently rendering. Data is automatically placed in
      // fields according to their semantic.
      struct Attributes
      {
        float4 vertex : POSITION;  // Position in object space.
        float2 uv     : TEXCOORD0; // Material texture UVs.
      };

      // This struct is output by the vertex function and input to
      // the fragment function. Note that fields will be
      // transformed by the intermediary rasterization stage.
      struct Varyings
      {
        // This value should contain the position in clip space (which
        // is similar to a position on screen) when output from the
        // vertex function. It will be transformed into pixel position
        // of the current fragment on the screen when read from
        // the fragment function.      
        float4 position : SV_POSITION;
        float2 uv       : TEXCOORD0; // Material texture UVs.
      };

      // This is automatically set by Unity.
      // Used in TRANSFORM_TEX to apply UV tiling.
      float4 _BaseMap_ST;
      
      TEXTURE2D(_FrameTex);
      SAMPLER(sampler_FrameTex);

      TEXTURE2D(_BackgroundTex);
      SAMPLER(sampler_BackgroundTex);

      TEXTURE2D(_FrontFXTex);
      SAMPLER(sampler_FrontFXTex);

      TEXTURE2D(_BackFXTex);
      SAMPLER(sampler_BackFXTex);

      half4 _FrameColor;

      float2 _FrontFXScrollVelocity;
      float2 _BackFXScrollVelocity;
      
      // The vertex function. This runs for each vertex on the mesh. It
      // must output the position on the screen each vertex should
      // appear at, as well as any data the fragment function will need.
      Varyings vert(Attributes input)
      {
        Varyings output = (Varyings)0;
        
        // These helper functions transform object space values into
        // world and clip space.        
        const VertexPositionInputs positionInputs = GetVertexPositionInputs(input.vertex.xyz);
        
        // Pass position data to the fragment function.
        output.position = positionInputs.positionCS;
        output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
        
        return output;
      }

      // The fragment function. This runs once per fragment, which
      // you can think of as a pixel on the screen. It must output
      // the final color of this pixel.
      half4 frag(const Varyings input) : SV_Target
      {
        // Sample the background.
        const half4 background = SAMPLE_TEXTURE2D(_BackgroundTex, sampler_BackgroundTex, input.uv);

        // Apply linear velocity to UV coordinates.
        half2 uvBackFX = input.uv + frac(_BackFXScrollVelocity * _Time.y);

        // Sample the background FX.
        const half4 backFX = SAMPLE_TEXTURE2D(_BackFXTex, sampler_BackFXTex, uvBackFX);

        // Blend background with the effect layer.
        half4 pixel = lerp(background, backFX, backFX.a);
        
        // Sample the image.
        const half4 image = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);

        // Add the image.
        pixel = lerp(pixel, image, image.a);

        // Apply linear velocity to UV coordinates.
        half2 uvFrontFX = input.uv + frac(_FrontFXScrollVelocity * _Time.y);

        // Sample the front FX.
        const half4 frontFX = SAMPLE_TEXTURE2D(_FrontFXTex, sampler_FrontFXTex, uvFrontFX);

        // Add the effect.
        pixel = lerp(pixel, frontFX, frontFX.a);
        
        // Sample the border.
        const half4 frame = SAMPLE_TEXTURE2D(_FrameTex, sampler_FrameTex, input.uv);
        
        // Add the border.
        pixel = lerp(pixel, frame * _FrameColor, frame.a);

        return pixel;
      }
      ENDHLSL
    }
  }
}