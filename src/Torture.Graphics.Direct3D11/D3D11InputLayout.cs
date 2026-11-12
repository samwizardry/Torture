using System.Runtime.InteropServices;

using Torture.Core.Graphics;

using Vortice.Direct3D11;

namespace Torture.Graphics.Direct3D11;

public sealed class D3D11InputLayout : InputLayout
{
    public D3D11InputLayout(ID3D11Device5 device, in VertexLayout description, D3D11Shader shader)
    {
        InputElementDescription[] nativeDescription = new InputElementDescription[description.Elements.Length];

        for (int i = 0; i < description.Elements.Length; ++i)
        {
            VertexElement element = description.Elements[i];

            nativeDescription[i].SemanticName = element.SemanticName.ToD3D11SemanticName();
            nativeDescription[i].SemanticIndex = element.SemanticIndex;
            nativeDescription[i].Format = element.Format.ToDXGIFormat();
            nativeDescription[i].Slot = 0;
            nativeDescription[i].AlignedByteOffset = element.Offset;
            nativeDescription[i].Classification = InputClassification.PerVertexData;
            nativeDescription[i].InstanceDataStepRate = 0;
        }

        Native = device.CreateInputLayout(nativeDescription, shader.Bytecode);
    }

    public ID3D11InputLayout Native { get; private set; }

    public override void Dispose()
    {
        if (_disposed)
            return;

        Native?.Dispose();
        Native = null!;

        _disposed = true;
    }
}
