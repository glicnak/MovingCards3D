Shader "Card/Border"
{
  // Properties are options set per material,
  // exposed by the material inspector.
  Properties
  {
    // [MainColor] allow Material.color to use the correct properties.
    [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
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

      // This attributes struct receives data about the mesh we're
      // currently rendering. Data is automatically placed in
      // fields according to their semantic.
      struct Attributes
      {
        float4 vertex : POSITION;  // Position in object space.
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
      };

      half4 _BaseColor;

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
        
        return output;
      }

      // The fragment function. This runs once per fragment, which
      // you can think of as a pixel on the screen. It must output
      // the final color of this pixel.
      half4 frag(const Varyings input) : SV_Target
      {
        return _BaseColor;
      }
      ENDHLSL
    }
  }
}