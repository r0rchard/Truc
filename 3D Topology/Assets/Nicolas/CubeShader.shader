// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Unlit/CubeShader" {
    Properties {
        _CubeSize ("CubeSize", Float) = .01 // 0.3
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Geometry-1" }
        //Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200
        Pass {
            Cull Back ZWrite On Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
                
            //#pragma exclude_renderers flash
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            #include "UnityCG.cginc"
            
            float _CubeSize ;
                             
            struct InputData {
                float4 pos : POSITION ;
                fixed4 color : COLOR ;
            };

            
            struct GeomInputData {
                float4 pos : SV_POSITION ;
                fixed4 color : COLOR ;
            };

            struct FragInputData {
                float4 pos : SV_POSITION ;
                fixed4 color : COLOR ;
            };

            GeomInputData vert (InputData v) {
                GeomInputData o ;
                o.pos = v.pos ;
                o.color = v.color ;
                return o ;
            }

            half4 frag (FragInputData i) : COLOR0 {
                return i.color ; 
            }

            [maxvertexcount (36)]
            // ----------------------------------------------------
            // Using "point" type as input, not "triangle"
            void geom (point GeomInputData p [1], inout TriangleStream<FragInputData> triStream) {
                const float f = _CubeSize / 2 ; // half size                
                //  The 8 vertex positions of a CUBE tile
                const float4 vc [8] = { float4 ( -f, -f, -f, 0.0f ),
                                        float4 ( -f, -f, +f, 0.0f ),
                                        float4 ( -f, +f, -f, 0.0f ),
                                        float4 ( -f, +f, +f, 0.0f ),
                                        float4 ( +f, -f, -f, 0.0f ),
                                        float4 ( +f, -f, +f, 0.0f ),
                                        float4 ( +f, +f, -f, 0.0f ),
                                        float4 ( +f, +f, +f, 0.0f ) };    
                                            
                FragInputData v [8] ; //for CUBE
                
                // Assign new vertices positions (8 new tile vertices, forming CUBE)
                for (int i = 0 ; i < 8 ; i++) {
                    v [i].pos = UnityObjectToClipPos (p [0].pos + vc [i]) ;
                    v [i].color = p [0].color ;
                }
                const int TRI_CUBE [36] = { 1, 2, 0,
                                            1, 3, 2,
                                            1, 5, 7,
                                            1, 7, 3,
                                            4, 6, 7,
                                            4, 7, 5,
                                            0, 2, 6,
                                            0, 6, 4,
                                            0, 5, 1,
                                            0, 4, 5,
                                            2, 3, 7,
                                            2, 7, 6 } ;
                // les normales semblent inversées, mais c'est peut-être dû à la transparence ?
            
                // Build the CUBE tile by submitting triangle vertices
                for (int j = 0 ; j < 36 ; j++) {
                    triStream.Append (v [TRI_CUBE [j]]) ;
                }
            }
            ENDCG
        }
    }
}
