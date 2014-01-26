using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace PlaneSimulator.Graphics.Shaders
{
    static class VertexDefinition
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PositionTextureNormal
        {
            public static int AppendAlignedElement1 = 12;
            public static int AppendAlignedElement2 = 20;
            public Vector3 position;
            public Vector2 texture;
            public Vector3 normal;

            public static InputLayout GetInputLayout(Device device, CompilationResult vertexShaderByteCode)
            {
                var inputElements = new InputElement[]
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
        public struct PositionColorNormal
        {
            public static int AppendAlignedElement1 = 12;
            public static int AppendAlignedElement2 = 28;
            public Vector3 position;
            public Vector4 color;
            public Vector3 normal;

            public static InputLayout GetInputLayout(Device device, CompilationResult vertexShaderByteCode)
            {
                var inputElements = new InputElement[]
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
            public static int AppendAlignedElement1 = 12;
            public Vector3 position;
            public Vector4 color;

            public static InputLayout GetInputLayout(Device device, CompilationResult vertexShaderByteCode)
            {
                var inputElements = new InputElement[]
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
                        AlignedByteOffset = LightShader.Vertex.AppendAlignedElement1,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };
                return new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);
            }
        }
    }
}
