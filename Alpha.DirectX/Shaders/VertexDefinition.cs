using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace Alpha.DirectX.Shaders
{
    static class VertexDefinition
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PositionTextureNormal
        {
            private static readonly int AppendAlignedElement1 = 12;
            private static readonly int AppendAlignedElement2 = 20;
            public Vector3 position;
            public Vector2 texture;
            public Vector3 normal;

            public static InputLayout GetInputLayout(Device device, CompilationResult vertexShaderByteCode)
            {
                var inputElements = new[]
                {
                    new InputElement
                    {
                        SemanticName = "POSITION",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = 0,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 0,
                        Format = Format.R32G32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement1,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "NORMAL",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement2,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };
                return new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionTextureNormal4Weights
        {
            private static readonly int AppendAlignedElement1 = 12;
            private static readonly int AppendAlignedElement2 = 20;
            private static readonly int AppendAlignedElement3 = 36;
            public Vector3 position;
            public Vector2 texture;
            public Vector4 weights;
            public Vector3 normal;

            public static InputLayout GetInputLayout(Device device, CompilationResult vertexShaderByteCode)
            {
                var inputElements = new[]
                {
                    new InputElement
                    {
                        SemanticName = "POSITION",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = 0,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 0,
                        Format = Format.R32G32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement1,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 1,
                        Format = Format.R32G32B32A32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement2,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "NORMAL",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement3,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };
                return new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionColorNormal
        {
            private static readonly int AppendAlignedElement1 = 12;
            private static readonly int AppendAlignedElement2 = 28;
            public Vector3 position;
            public Vector4 color;
            public Vector3 normal;

            public static InputLayout GetInputLayout(Device device, CompilationResult vertexShaderByteCode)
            {
                var inputElements = new[]
                {
                    new InputElement
                    {
                        SemanticName = "POSITION",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = 0,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "COLOR",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32A32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement1,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "NORMAL",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement2,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };
                return new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionColor
        {
            private static readonly int AppendAlignedElement1 = 12;
            public Vector3 position;
            public Vector4 color;

            public static InputLayout GetInputLayout(Device device, CompilationResult vertexShaderByteCode)
            {
                var inputElements = new[]
                {
                    new InputElement
                    {
                        SemanticName = "POSITION",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = 0,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "COLOR",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32A32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement1,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };
                return new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionTextureColor
        {
            private static readonly int AppendAlignedElement1 = 12;
            private static readonly int AppendAlignedElement2 = 20;
            public Vector3 position;
            public Vector2 texture;
            public Vector4 color;

            public static InputLayout GetInputLayout(Device device, CompilationResult vertexShaderByteCode)
            {
                var inputElements = new[]
                {
                    new InputElement
				    {
					    SemanticName = "POSITION",
					    SemanticIndex = 0,
					    Format = Format.R32G32B32_Float,
					    Slot = 0,
					    AlignedByteOffset = 0,
					    Classification = InputClassification.PerVertexData,
					    InstanceDataStepRate = 0
				    },
				    new InputElement
				    {
					    SemanticName = "TEXCOORD",
					    SemanticIndex = 0,
					    Format = Format.R32G32_Float,
					    Slot = 0,
					    AlignedByteOffset = AppendAlignedElement1,
					    Classification = InputClassification.PerVertexData,
					    InstanceDataStepRate = 0
				    },
				    new InputElement
				    {
					    SemanticName = "COLOR",
					    SemanticIndex = 0,
					    Format = Format.R32G32B32A32_Float,
					    Slot = 0,
					    AlignedByteOffset = AppendAlignedElement2,
					    Classification = InputClassification.PerVertexData,
					    InstanceDataStepRate = 0
				    }
                };
                return new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Path
        {
            public Vector3 position;
            public Vector2 pathLength; //1 : total path length up to the current segment, 2 : current segment length
            public uint fillingIndex;

            public static InputLayout GetInputLayout(Device device, CompilationResult vertexShaderByteCode)
            {
                var inputElements = new[]
                {
                    new InputElement
                    {
                        SemanticName = "POSITION",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = 0,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 0,
                        Format = Format.R32G32_Float,
                        Slot = 0,
                        AlignedByteOffset = 12,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 1,
                        Format = Format.R32_UInt,
                        Slot = 0,
                        AlignedByteOffset = 20,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };
                return new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PositionTexture
        {
            private static readonly int AppendAlignedElement1 = 12;
            public Vector3 position;
            public Vector2 texture;

            public static InputLayout GetInputLayout(Device device, CompilationResult vertexShaderByteCode)
            {
                var inputElements = new[]
                {
                    new InputElement
                    {
                        SemanticName = "POSITION",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = 0,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 0,
                        Format = Format.R32G32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement1,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };
                return new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct WaterVertex
        {
            private static readonly int AppendAlignedElement1 = 12;
            private static readonly int AppendAlignedElement2 = 20;
            public Vector3 position;
            public Vector2 bumpTexture;
            public Vector2 borderTexture;

            public static InputLayout GetInputLayout(Device device, CompilationResult vertexShaderByteCode)
            {
                var inputElements = new[]
                {
                    new InputElement
                    {
                        SemanticName = "POSITION",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = 0,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 0,
                        Format = Format.R32G32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement1,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 1,
                        Format = Format.R32G32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement2,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };
                return new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);
            }
        }
        public struct TerrainVertex
        {
            private static readonly int AppendAlignedElement1 = 12;
            private static readonly int AppendAlignedElement2 = 20;
            public Vector3 position;
            public Vector2 borderTexture;
            public Vector2 provinceIds;

            public static InputLayout GetInputLayout(Device device, CompilationResult vertexShaderByteCode)
            {
                var inputElements = new[]
                {
                    new InputElement
                    {
                        SemanticName = "POSITION",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = 0,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 0,
                        Format = Format.R32G32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement1,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 1,
                        Format = Format.R32G32_Float,
                        Slot = 0,
                        AlignedByteOffset = AppendAlignedElement2,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };
                return new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);
            }
        }
    }
}
